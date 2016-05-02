using System;
using System.IO;
using System.Security.Cryptography;

namespace sha256rename
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files;

            if (args.Length >= 2 && args[0] == "-dir" && !string.IsNullOrWhiteSpace(args[1]))
            {
                var dir = args[1];
                if (!Directory.Exists(dir))
                {
                    Console.WriteLine("Directory not found: " + dir);
                    Console.ReadKey(true);
                    return;
                }
                files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            }
            else if (args.Length >= 2 && args[0] == "-file" && !string.IsNullOrWhiteSpace(args[1]))
            {
                var file = args[1];
                if (!File.Exists(file))
                {
                    Console.WriteLine("File not found: " + file);
                    Console.ReadKey(true);
                    return;
                }
                files = new string[] { file };
            }
            else
            {
                Console.WriteLine("Rename a file or directory to SHA256 hash.");
                Console.WriteLine("Usage 1: sha256rename.exe -dir directory");
                Console.WriteLine("Usage 2: sha256rename.exe -file file");
                Console.ReadKey();
                return;
            }

            for (var index = 0; index < files.Length; index++)
            {
                var file = files[index];
                Console.WriteLine("<-- " + file + " (" + index + " / " + files.Length + ")");
                RenameToSha256Hash(file);
            }

            Console.WriteLine("Finished.");
            Console.ReadKey(true);
        }

        private static void RenameToSha256Hash(string filename)
        {
            var checksum = GetSha256Hash(filename);
            var checksumFilename = Path.GetDirectoryName(filename) + @"\" + checksum + Path.GetExtension(filename);

            if (checksumFilename == filename) return;

            var n = 0;
            while (File.Exists(checksumFilename))
            {
                n++;
                checksumFilename = Path.GetDirectoryName(filename) + @"\_" + n.ToString().PadLeft(5,'0') + "_" + checksum + Path.GetExtension(filename);
            }

            File.Move(filename, checksumFilename);

            
            Console.WriteLine("--> " + checksumFilename);
            Console.WriteLine();
        }

        private static string GetSha256Hash(string filename)
        {
            using (var stream = new BufferedStream(File.OpenRead(filename), 4194304 /* 4 MB */))
            {
                var sha256 = new SHA256Managed();
                var hashBytes = sha256.ComputeHash(stream);
                var hashString = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hashString;
            }
        }
    }
}
