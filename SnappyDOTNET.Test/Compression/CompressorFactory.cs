using System.Collections;
using System.Collections.Generic;

namespace SnappyDOTNET.Test
{
    public static class CompressorFactory
    {

        private static readonly IDictionary<string, ICompressor> compressorCache = new Dictionary<string, ICompressor>();

        static CompressorFactory()
        {
            compressorCache["snappy"] = new SnappyDOTNETCompressor();
            compressorCache["gzip"] = new GzipCompressor();
            compressorCache["deflate"] = new DeflateCompressor();
        }

        public static ICompressor Create(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return null;
            }
            ICompressor compressor;
            if (!compressorCache.TryGetValue(type.ToLower(), out compressor))
            {
                System.Console.WriteLine("Unknown session data compressor: " + type);
            }
            return compressor;
        }
    }
}