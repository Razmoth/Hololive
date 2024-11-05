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

            var salt = Encoding.UTF8.GetBytes(Path.GetFileNameWithoutExtension(file.FullName));
            using SeekableAesStream seekableAesStream = new(inputFS, o.Password!, salt, o.Derive, o.KeySize);

            seekableAesStream.CopyTo(outputFS);

            Console.WriteLine($"{file.Name} processed !!");
        }
    }
}
