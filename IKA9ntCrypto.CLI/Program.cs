using System.Security.Cryptography;
using System.Text;

namespace IKA9ntCrypto.CLI;
public class Program
{
    public static void Main(string[] args) => CommandLine.Init(args);
    public static void Run(Options o)
    {
        foreach(var file in o.Input?.GetFiles("*.*", SearchOption.AllDirectories) ?? [])
        {
            Console.WriteLine($"Processing {file.Name}...");

            o.Output?.Create();

            var outputPath = Path.Combine(o.Output!.FullName, Path.GetRelativePath(o.Input!.FullName, file.FullName));

            using FileStream inputFS = file.OpenRead();
            using FileStream outputFS = File.Open(outputPath, FileMode.Create);

            var salt = Encoding.UTF8.GetBytes(o.Extension ? Path.GetFileName(file.FullName) : Path.GetFileNameWithoutExtension(file.FullName));

            DeriveBytes deriveBytes = o.Derive switch
            {
                DeriveType.PBKDF1 => new PasswordDeriveBytes(o.Password!, salt, "SHA1", 100),
                DeriveType.PBKDF2 => new Rfc2898DeriveBytes(o.Password!, salt, 1000, HashAlgorithmName.SHA1),
                _ => throw new NotSupportedException()
            };

            using Aes aes = Aes.Create();

            aes.KeySize = o.KeySize;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            aes.Key = deriveBytes.GetBytes(o.KeySize / 8);
            aes.IV = new byte[0x10];

            using SeekableAesStream seekableAesStream = new(inputFS, aes.CreateEncryptor());

            seekableAesStream.CopyTo(outputFS);

            Console.WriteLine($"{file.Name} processed !!");
        }
    }
}
