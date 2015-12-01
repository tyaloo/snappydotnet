using System.IO;
using SnappyDOTNET.Test.Util;

namespace SnappyDOTNET.Test
{
    public abstract class AbstractCompressor : ICompressor
    {
        public abstract Stream CreateCompressionStream(Stream stream);
        public abstract Stream CreateDecompressionStream(Stream stream);

        public byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                Stream compStream = null;
                try
                {
                    compStream = CreateCompressionStream(ms);
                    compStream.Write(data, 0, data.Length);
                    compStream.Flush();
                }
                finally
                {
                    if (compStream != null)
                    {
                        try
                        {
                            compStream.Dispose();
                        }
                        catch
                        {
                        }
                    }
                }
                return ms.ToArray();
            }
        }

        public byte[] Decompress(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                Stream decompStream = null;
                try
                {
                    decompStream = CreateDecompressionStream(ms);
                    return decompStream.ReadAllBytes();
                }
                finally
                {
                    if (decompStream != null)
                    {
                        try
                        {
                            decompStream.Dispose();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}