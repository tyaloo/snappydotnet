using System.IO;

namespace SnappyDOTNET.Test
{
    public interface ICompressor
    {
        Stream CreateCompressionStream(Stream stream);

        Stream CreateDecompressionStream(Stream stream);

        byte[] Compress(byte[] data);

        byte[] Decompress(byte[] data);
    }
}