using System.IO.Compression;

namespace jsonz
{
    internal class Program
    {
        static async Task<bool> DecompressFile(string filename, bool writeOutputToConsole)
        {
            var success = true;
            try
            {
                var dir = Directory.GetCurrentDirectory();
                var fname = Path.GetFileName(filename);

                var decompressedStream = new MemoryStream();
                if (!writeOutputToConsole)
                {
                    Console.WriteLine("Decompressing: " + filename);
                }
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
                    var outputFilename = dir + "\\" + Path.GetFileNameWithoutExtension(fname) + ".json";

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
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                success = false;
            }
            return success;
        }

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: jsonz <filename.jsonz | *.jsonz> [--w|-w|w]");
                Console.WriteLine();
                Console.WriteLine("       Decompress JSONZ files into JSON files.");
                Console.WriteLine();
                Console.WriteLine("       This program will decompress the given filename.jsonz. If the filename specified has a wildcard in it, all matching files are processed.");
                Console.WriteLine("       The output is written to a filename with the same name, but .json extension (overwriting any file of the same name if it exists).");
                Console.WriteLine("       The output is written to the current directory.");
                Console.WriteLine("       If the optional '--w', or '-w', or 'w' is given, the output will be written to the console.");
                Console.WriteLine();
                Console.WriteLine("Examples:");
                Console.WriteLine();
                Console.WriteLine("       To decompress all the Jsonz file in Downloads:");
                Console.WriteLine("                jsonz C:\\Users\\SteveRives\\Downloads\\*.jsonz");
                Console.WriteLine("       In this case, the output is written to whatever directory you are in when you run the program.");
                Console.WriteLine();
                Console.WriteLine("       To decompress a single JSONZ file to the console:");
                Console.WriteLine("                jsonz myData.jsonz --w");
                Console.WriteLine();
                Console.WriteLine("       To decompress a single JSONZ file to the console and pretty-print it:");
                Console.WriteLine("                jsonz myData.jsonz --w | jq");
                Console.WriteLine("       Where jq is a third-party app, last available here: https://jqlang.github.io/jq/download/");
                Console.WriteLine("       To install jq, run: choco install jq");
                Console.WriteLine();
                Console.WriteLine("(c) CodeKill: v. 10/27/2024");
                return;
            }

            var writeOutputToConsole = (args.Length > 1 && (args[1].ToLower() == "w" || args[1].ToLower().Contains("-w")));            

            var dir = Path.GetDirectoryName(args[0]);
            var fileSpec = Path.GetFileName(args[0]);
            if (string.IsNullOrEmpty(dir))
            {
                dir = Directory.GetCurrentDirectory();
            }

            if (!Directory.Exists(dir))
            {
                Console.WriteLine($"The directory, {dir}, does not exist.");
                return;
            }

            var files = Directory.EnumerateFiles(dir, fileSpec, SearchOption.TopDirectoryOnly).ToList();
            if (files.Count == 0)
            {
                Console.WriteLine("No files found matching the given filename.");
                return;
            }

            var taskList = new List<Task<bool>>();
            files.ForEach(file =>
            {
                var t = DecompressFile(file, writeOutputToConsole);
                taskList.Add(t);
            });

            if (taskList.Count > 0)
            {
                Task.WaitAll([.. taskList]);
            }
        }
    }
}
