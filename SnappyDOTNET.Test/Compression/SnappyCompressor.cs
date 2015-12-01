using System.IO;
using System.IO.Compression;
using Snappy;

namespace SnappyDOTNET.Test
{
    public class SnappyCompressor : AbstractCompressor
    {
        #region Implementation of ICompressor
        public override Stream CreateCompressionStream(Stream stream)
        {
            return new SnappyStream(stream, CompressionMode.Compress);
        }

        public override Stream CreateDecompressionStream(Stream stream)
        {
            return new SnappyStream(stream, CompressionMode.Decompress);
        }
        #endregion
    }
}