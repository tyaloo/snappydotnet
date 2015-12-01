using System;
using System.Diagnostics;
using System.Linq;
using SnappyDOTNET.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using SnappyDOTNET.Test.Util;

namespace SnappyDOTNET.Test
{
    [TestClass]
    public class CompressorTest
    {
        TestDataCreator _dataCreator = new TestDataCreator(0.5f);
        [TestMethod]
        public void SnappyCompressorTest()
        {
            var compressor = new SnappyCompressor();
            TestCompressor(compressor);
        }

        [TestMethod]
        public void GzipCompressorTest()
        {
            var compressor = new GzipCompressor();
            TestCompressor(compressor);
        }

        [TestMethod]
        public void DeflateCompressorTest()
        {
            var compressor = new DeflateCompressor();
            TestCompressor(compressor);
        }

        [TestMethod]
        public void CSnappyCompressorTest()
        {
            var compressor = new SnappyDOTNETCompressor();
            TestCompressor(compressor);
        }

        [TestMethod]
        public void CSnappySnappyInteractionTest()
        {
            int loop = 50;
            for (int i = 0; i < loop; i++)
            {

                var data = _dataCreator.GenerateTestData(50 * i + 500);
                var csnappy = new SnappyDOTNETCompressor();
                var snappy = new SnappyCompressor();


                var ms = new MemoryStream();

                ms.Write(data, 0, data.Length);

                var compressedData = csnappy.Compress(ms.ToArray());
                var compressedData1 = snappy.Compress(data);

                Assert.IsTrue(compressedData1.SequenceEqual(compressedData));

                var decompressedData = snappy.Decompress(compressedData);

                Assert.IsTrue(data.SequenceEqual(decompressedData));

                compressedData = snappy.Compress(data);

                decompressedData = csnappy.Decompress(compressedData);

                Assert.IsTrue(data.SequenceEqual(decompressedData));
            }



        }

        private void TestCompressor(ICompressor compressor)
        {
            var data = _dataCreator.GenerateTestData(637);
            var compressedData = compressor.Compress(data);
            var decompressedData = compressor.Decompress(compressedData);
            Assert.IsTrue(data.SequenceEqual(decompressedData));
        }

        [TestMethod]
        public void Benchmark()
        {
            Benchmark(new SnappyCompressor());
            Benchmark(new GzipCompressor());
            Benchmark(new DeflateCompressor());
            Benchmark(new SnappyDOTNETCompressor());

            DoPureBenchmark();
        }

        private void Benchmark(ICompressor compressor)
        {
            Console.WriteLine("Target compression ratio: {0:P1}", _dataCreator.CompressionRatio);
            Console.WriteLine();

            const int batch = 10;
            Console.WriteLine("Benchmark of {0}:", compressor.GetType().Name);
            WarmUpCompressor(compressor);
            DoBenchmark(compressor, 4 * 1024, batch);
            DoBenchmark(compressor, 32 * 1024, batch);
            DoBenchmark(compressor, 64 * 1024, batch);
            DoBenchmark(compressor, 128 * 1024, batch);
            DoBenchmark(compressor, 256 * 1024, batch);
            DoBenchmark(compressor, 512 * 1024, batch);
            DoBenchmark(compressor, 1024 * 1024, batch);
            DoBenchmark(compressor, 2048 * 1024, batch);
            DoBenchmark(compressor, 5120 * 1024, batch);
            DoBenchmark(compressor, 10240 * 1024, batch);
            DoBenchmark(compressor, 20480 * 1024, batch);
            Console.WriteLine("---------------------------------------------");
        }

        private void WarmUpCompressor(ICompressor compressor)
        {
            var warmUpData = _dataCreator.GenerateTestData(256);

            for (var i = 0; i < 64; ++i)
            {
                var compressedData = compressor.Compress(warmUpData);
                var decompressedData = compressor.Decompress(compressedData);
                Assert.IsTrue(warmUpData.SequenceEqual(decompressedData));
            }
        }

        private void DoPureBenchmark()
        {

            Console.WriteLine("Target compression ratio: {0:P1}", _dataCreator.CompressionRatio);
            Console.WriteLine();

            Console.WriteLine("Benchmark of {0}:", "pure snappydotnet invoke");

            int batchCount = 10;
            foreach(int dataSize in new int[]{
                1024*4,1024*32,1024*64,1024*128,1024*256,1024*512,1024*1024,
                1024*1024*2,1024*1024*5,1024*1024*10,1024*1024*20
            }){
            var data = _dataCreator.GenerateTestData(dataSize);
            byte[] compressedData = null, decompressedData = null;
            var stopWatch = Stopwatch.StartNew();
            for (var i = 0; i < batchCount; ++i)
            {
                compressedData = SnappyDOTNET.CompressTool.Compress(data);
                decompressedData = SnappyDOTNET.CompressTool.Uncompress(compressedData);
            }
            stopWatch.Stop();
            var msPerCycle = (double)stopWatch.ElapsedMilliseconds / batchCount;
            var compressionRate = (double)compressedData.Length / data.Length;
            Console.WriteLine("\t{0} data {1} compressed {2:P1}: {3}ms per cycle",
                TextUtils.GetSizeText(data.Length), TextUtils.GetSizeText(compressedData.Length),
                compressionRate, msPerCycle);
            }
            Console.WriteLine("---------------------------------------------");
        }

        private void DoBenchmark(ICompressor compressor, int dataSize, int batchCount)
        {
            var data = _dataCreator.GenerateTestData(dataSize);

            byte[] compressedData = null, decompressedData = null;
            var stopWatch = Stopwatch.StartNew();
            for (var i = 0; i < batchCount; ++i)
            {
                compressedData = compressor.Compress(data);
                decompressedData = compressor.Decompress(compressedData);
            }
            stopWatch.Stop();
            var msPerCycle = (double)stopWatch.ElapsedMilliseconds / batchCount;
            var compressionRate = (double)compressedData.Length / data.Length;
            Console.WriteLine("\t{0} data {1} compressed {2:P1}: {3}ms per cycle",
                TextUtils.GetSizeText(data.Length), TextUtils.GetSizeText(compressedData.Length),
                compressionRate, msPerCycle);
        }


    }
}