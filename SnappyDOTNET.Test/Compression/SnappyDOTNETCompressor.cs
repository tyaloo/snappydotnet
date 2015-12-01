using SnappyDOTNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SnappyDOTNET.Test
{
    public class SnappyDOTNETCompressor : AbstractCompressor
    {
        public override Stream CreateCompressionStream(Stream stream)
        {
            return new SnappyDOTNETStream(stream, CompressionMode.Compress);
        }

        public override Stream CreateDecompressionStream(Stream stream)
        {
            return new SnappyDOTNETStream(stream, CompressionMode.Decompress);
        }


    }
}
