using System.Buffers.Binary;
using System.Security.Cryptography;

namespace IKA9ntCrypto;
public class SeekableAesStream : Stream
{
    private readonly ICryptoTransform _cryptoTransform;
    private readonly Stream _baseStream;

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => _baseStream.CanSeek;
    public override bool CanWrite => _baseStream.CanWrite;
    public override long Length => _baseStream.Length;
    public override long Position
    {
        get => _baseStream.Position;
        set => _baseStream.Position = value;
    }

    public SeekableAesStream(Stream stream, ICryptoTransform cryptoTransform)
    {
        _baseStream = stream;
        _cryptoTransform = cryptoTransform;
    }

    public override void Flush() => _baseStream.Flush();
    public override long Seek(long offset, SeekOrigin origin) => _baseStream.Seek(offset, origin);
    public override void SetLength(long value) => _baseStream.SetLength(value);

    public override int Read(byte[] buffer, int offset, int count)
    {
        long position = _baseStream.Position;
        int read = _baseStream.Read(buffer, offset, count);
        Transform(buffer, offset, count, position);

        return read;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        Transform(buffer, offset, count, _baseStream.Position);
        _baseStream.Write(buffer, offset, count);
    }

    private void Transform(byte[] buffer, int offset, int count, long position)
    {
        var index = position % _cryptoTransform.InputBlockSize;
        var counter = ((int)position / _cryptoTransform.InputBlockSize) + 1;

        byte[] seed = new byte[_cryptoTransform.InputBlockSize];
        byte[] temp = new byte[_cryptoTransform.InputBlockSize];
        while (offset < count)
        {
            if (index % _cryptoTransform.InputBlockSize == 0)
            {
                BinaryPrimitives.WriteInt32LittleEndian(seed, counter++);
                _cryptoTransform.TransformBlock(seed, 0, seed.Length, temp, 0);
                index = 0;
            }

            buffer[offset++] ^= temp[index++];
        }
    } 
}