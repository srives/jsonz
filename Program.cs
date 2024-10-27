using System.IO.Compression;

namespace jsonz
{
    internal class Program
    {
        static async Task DecompressFile(string filename, bool writeOutputToConsole)
        {
            var decompressedStream = new MemoryStream();
            using var compressedStream = File.Open(filename, FileMode.Open);
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress, true))
            {
                await gzipStream.CopyToAsync(decompressedStream);
            }

            decompressedStream.Position = 0;
            using var sr = new StreamReader(decompressedStream);
            string line;

            if (!writeOutputToConsole)
            {
                var outputFilename = Path.GetFileNameWithoutExtension(filename) + ".json";

                Console.WriteLine("Processing:    " + filename);
                Console.WriteLine("Creating file: " + outputFilename);
                if (File.Exists(outputFilename))
                {
                    File.Delete(outputFilename);
                }
                using var sw = new StreamWriter(outputFilename);
                while ((line = sr.ReadLine()) != null)
                {
                    sw.WriteLine(line);
                }
            }
            else
            {
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: jzon <filename.jsonz | *.jsonz> [--w|-w|w]");
                Console.WriteLine("       This program will decompress the given file(s). If the filename specified has a wildcard in it, all matching files are processed.");
                Console.WriteLine("       The output is written to a filename with the same name, but .json extension (overwriting any file of the same name if it exists).");
                Console.WriteLine("       If the optional '--w', or '-w', or 'w' is given, the output will be written to the console.");
                return;
            }

            var writeOutputToConsole = (args.Length > 1 && (args[1].ToLower() == "w" || args[1].ToLower().Contains("-w")));            

            var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), args[0], SearchOption.AllDirectories).ToList();
            if (files.Count == 0)
            {
                Console.WriteLine("No files found matching the given filename.");
                return;
            }
            files.ForEach(async file =>
            {
                await DecompressFile(file, writeOutputToConsole);
            });
        }
    }
}
