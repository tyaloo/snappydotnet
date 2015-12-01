using System.IO;
using System.IO.Compression;

namespace SnappyDOTNET.Test
{
    public class DeflateCompressor : AbstractCompressor
    {
        #region Implementation of ICompressor
        public override Stream CreateCompressionStream(Stream stream)
        {
            return new DeflateStream(stream, CompressionMode.Compress);
        }

        public override Stream CreateDecompressionStream(Stream stream)
        {
            return new DeflateStream(stream, CompressionMode.Decompress);
        }
        #endregion
    }
}