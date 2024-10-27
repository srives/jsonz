using System.IO.Compression;
using System.IO.Compression;

namespace jsonz
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: jzon <filename.jsonz>");
                Console.WriteLine("       This program will decompress the given file.");
                return;
            }
            var decompressedStream = new MemoryStream();
            using var compressedStream = File.Open(args[0], FileMode.Open);
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress, true))
            {
                 await gzipStream.CopyToAsync(decompressedStream);
            }
            decompressedStream.Position = 0;
            using var sr = new StreamReader(decompressedStream);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Replace("HorizontalGridLineId", "hid");
                line = line.Replace("HorizontalAnnotation", "ha");
                line = line.Replace("HorizontalDirection", "hd");
                line = line.Replace("VerticalAnnotation", "va");
                line = line.Replace("VerticalDirection", "vd");
                line = line.Replace("VerticalGridLineId", "vid");
                Console.WriteLine(line);
            }
        }
    }
}
