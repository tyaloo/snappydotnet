using System.IO;
using System.IO.Compression;

namespace SnappyDOTNET.Test
{
    public class GzipCompressor : AbstractCompressor
    {
        #region Implementation of ICompressor
        public override Stream CreateCompressionStream(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Compress);
        }

        public override Stream CreateDecompressionStream(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Decompress);
        }
        #endregion
    }
}