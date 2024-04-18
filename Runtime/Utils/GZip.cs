using System;
using System.IO;
using System.IO.Compression;

namespace FPS
{
    public static class GZip
    {
        public static string Compress(string input)
        {
            using MemoryStream memoryStream = new();
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            using (StreamWriter streamWriter = new StreamWriter(gzipStream))
            {
                streamWriter.Write(input);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }
        
        public static string Decompress(string encodedValue)
        {
            byte[] compressedBytes = Convert.FromBase64String(encodedValue);
            using MemoryStream memoryStream = new(compressedBytes);
            using GZipStream gzipStream = new(memoryStream, CompressionMode.Decompress);
            using StreamReader streamReader = new(gzipStream);
            return streamReader.ReadToEnd();
        }
    }
}