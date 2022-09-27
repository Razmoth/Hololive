using System.Security.Cryptography;
using System.Text;

namespace Hololive
{
    public class Decryptor
    {
        public static readonly Aes Aes = Aes.Create();
        public static void DecryptBundle(string input, string output, string key)
        {
            var fileName = Path.GetFileNameWithoutExtension(input);
            var bytes = File.ReadAllBytes(input);

            var rfc = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(fileName));

            Aes.Mode = CipherMode.ECB;
            Aes.Padding = PaddingMode.None;
            Aes.Key = rfc.GetBytes(Aes.KeySize / 8);
            Aes.IV = new byte[0x10];

            Console.WriteLine("Decrypting...");
            Decrypt(bytes, 0, bytes.Length);

            Console.WriteLine("Writing output...");
            var outPath = Path.Combine(output, fileName);
            Directory.CreateDirectory(output);
            File.WriteAllBytes(outPath, bytes);
        }
        public static void Decrypt(byte[] buffer, int offset, int count)
        {
            var encryptor = Aes.CreateEncryptor();
            var blockSize = Aes.BlockSize / 8;
            var remainder = offset % blockSize;
            var quotient = (offset / blockSize) + 1;
            var transformBuffer = new byte[blockSize];
            var quotientBuffer = new byte[blockSize];
            while (offset < count)
            {
                if (remainder % blockSize == 0)
                {
                    var bytes = BitConverter.GetBytes(quotient++);
                    bytes.CopyTo(quotientBuffer, 0);
                    encryptor.TransformBlock(quotientBuffer, 0, quotientBuffer.Length, transformBuffer, 0);
                    remainder = 0;
                }
                buffer[offset++] ^= transformBuffer[remainder++];
            }
        }
    }
}
