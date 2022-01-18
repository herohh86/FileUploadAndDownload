using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace Server
{
    public class GZip
    {
        public static string GZipCompress(string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
            {
                return string.Empty;
            }
            else
            {
                byte[] rawBytes = System.Text.Encoding.UTF8.GetBytes(rawString);
                byte[] zippedData = Compress(rawBytes);
                return Convert.ToBase64String(zippedData);
            }
        }

        public static string GZipDecompress(string zipString)
        {
            if (string.IsNullOrEmpty(zipString))
            {
                return string.Empty;
            }

            byte[] zipBytes = Convert.FromBase64String(zipString);
            return System.Text.Encoding.UTF8.GetString(Decompress(zipBytes));
        }

        private static byte[] Compress(byte[] rawData)
        {
            System.IO.MemoryStream m = new System.IO.MemoryStream();
            GZipStream zipStream = new GZipStream(m, CompressionMode.Compress, true);
            zipStream.Write(rawData, 0, rawData.Length);
            zipStream.Close();
            return m.ToArray();
        }

        private static byte[] Decompress(byte[] zipData)
        {
            MemoryStream m = new MemoryStream(zipData);
            GZipStream zipStream = new GZipStream(m, CompressionMode.Decompress);
            MemoryStream outStream = new MemoryStream();
            byte[] buffer = new byte[1024];
            while (true)
            {
                var readCount = zipStream.Read(buffer, 0, buffer.Length);
                if (readCount <= 0)
                {
                    break;
                }
                outStream.Write(buffer, 0, readCount);
            }
            zipStream.Close();
            return outStream.ToArray();
        }
    }
}