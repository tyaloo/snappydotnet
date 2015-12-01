using SnappyDOTNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace SnappyDOTNET.Test
{
    [TestClass]
    public class CSnappyStreamThroughputTest
    {
        [TestMethod]
        public void Compression()
        {
            Benchmark.Run("Compressing", 1024*1024*2, benchmark =>
            {
                var stream = new NullStream();
                var compressor = new SnappyDOTNETStream(stream, CompressionMode.Compress);
                benchmark.Stopwatch.Start();
                for (int i = 0; i < benchmark.Iterations; ++i)
                {
                    compressor.Write(benchmark.Input, 0, benchmark.Input.Length);
                    compressor.Flush();
                }
                benchmark.Stopwatch.Stop();
                benchmark.Note = String.Format(" ({0:0.00 %})", stream.Written / (double)benchmark.Input.Length / benchmark.Iterations);
            });
        }


        [TestMethod]
        public void Decompression()
        {
            Benchmark.Run("Decompressing", 1024 * 1024 * 2, benchmark =>
            {
                var stream = new RepeaterStream(GetCompressedFile(benchmark.Input));
                var decompressor = new SnappyDOTNETStream(stream, CompressionMode.Decompress);
                var decompressed = new byte[benchmark.Input.Length];
                benchmark.Stopwatch.Start();
                for (int i = 0; i < benchmark.Iterations; ++i)
                    ReadFully(decompressor, decompressed, 0, decompressed.Length);
                benchmark.Stopwatch.Stop();
            });
        }



        void ReadFully(Stream stream, byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                int read = stream.Read(buffer, offset, count);
                if (read <= 0)
                    throw new EndOfStreamException();
                offset += read;
                count -= read;
            }
        }


        byte[] GetCompressedFile(byte[] uncompressed)
        {
            var compressed = new MemoryStream();
            using (var compressor = new SnappyDOTNETStream(compressed, CompressionMode.Compress, true))
                compressor.Write(uncompressed, 0, uncompressed.Length);
            compressed.Close();
            return compressed.ToArray();
        }
    }


}
