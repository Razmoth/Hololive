using System.Text;

namespace Hololive
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Help();
                return;
            }
            var input = new FileInfo(args[0]);
            var output = new FileInfo(args[1]);
            var key = args[2];
            Decryptor.DecryptBundle(input.FullName, output.FullName, key);
        }

        public static void Help()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Usage: Hololive <input> <output> <key>\n");
            Console.WriteLine("<input>\tInput file.");
            Console.WriteLine("<output>\tOutput Folder.");
            Console.WriteLine("<key>\tAES ECB 256 decrytion key.\n");
            Console.WriteLine("Known Keys:");
            Console.WriteLine("hololive ERROR Demo: マgとNにAｾAｼcnヒﾙチねﾓヨモしjゆムアAIiソみiコ");
            Console.WriteLine("hololive ERROR the Game: イくZuwツらﾊなよ7ｹKJむやQHなヌさK4");
        }
    }
}
