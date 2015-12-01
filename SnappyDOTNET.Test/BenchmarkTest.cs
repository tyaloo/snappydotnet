using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;
using SnappyDOTNET;
using System.Threading;
using System.Threading.Tasks;

namespace SnappyDOTNET.Test
{
    /// <summary>
    /// BenchmarkTest 的摘要说明
    /// </summary>
    [TestClass]
    public class BenchmarkTest
    {
        [TestMethod]
        public void CompressionTest()
        {

            DoCompressionBenchmark(1024 * 256);
            DoCompressionBenchmark(1024 * 512);
            DoCompressionBenchmark(1024 * 1024);
            DoCompressionBenchmark(1024 * 1024 * 5);
            DoCompressionBenchmark(1024 * 1024 * 10);

        }

        [TestMethod]
        public void Uncompression()
        {
            DoUncompressionBenchmark(1024 * 256);
            DoUncompressionBenchmark(1024 * 512);
            DoUncompressionBenchmark(1024 * 1024);
            DoUncompressionBenchmark(1024 * 1024 * 5);
            DoUncompressionBenchmark(1024 * 1024 * 10);

        }

        [TestMethod]
        public void AsyncCompressionTest()
        {
            Task.WaitAll(
             DoAsyncCompressionBenchmark(1024 * 256),
             DoAsyncCompressionBenchmark(1024 * 512),
             DoAsyncCompressionBenchmark(1024 * 1024),
            DoAsyncCompressionBenchmark(1024 * 1024 * 5),
            DoAsyncCompressionBenchmark(1024 * 1024 * 10)
            );

        }

        [TestMethod]
        public void AsyncUncompression()
        {
            Task.WaitAll(
            DoAsyncUncompressionBenchmark(1024 * 256),
            DoAsyncUncompressionBenchmark(1024 * 512),
            DoAsyncUncompressionBenchmark(1024 * 1024),
            DoAsyncUncompressionBenchmark(1024 * 1024 * 5),
            DoAsyncUncompressionBenchmark(1024 * 1024 * 10)
            );

        }



        [TestMethod]
        public void OneParaCompressionTest()
        {

            DoCompressionOneParaBenchmark(1024 * 256);
            DoCompressionOneParaBenchmark(1024 * 512);
            DoCompressionOneParaBenchmark(1024 * 1024);
            DoCompressionOneParaBenchmark(1024 * 1024 * 5);
            DoCompressionOneParaBenchmark(1024 * 1024 * 10);

        }

        [TestMethod]
        public void OneParaUncompression()
        {
            DoUncompressionOneParaBenchmark(1024 * 256);
            DoUncompressionOneParaBenchmark(1024 * 512);
            DoUncompressionOneParaBenchmark(1024 * 1024);
            DoUncompressionOneParaBenchmark(1024 * 1024 * 5);
            DoUncompressionOneParaBenchmark(1024 * 1024 * 10);

        }


        [TestMethod]
        public void OneParaAsyncCompressionTest()
        {
            Task.WaitAll(
             DoAsyncCompressionOneParaBenchmark(1024 * 256),
             DoAsyncCompressionOneParaBenchmark(1024 * 512),
             DoAsyncCompressionOneParaBenchmark(1024 * 1024),
             DoAsyncCompressionOneParaBenchmark(1024 * 1024 * 5),
             DoAsyncCompressionOneParaBenchmark(1024 * 1024 * 10)
            );

        }

        [TestMethod]
        public void OneParaAsyncUncompression()
        {
            Task.WaitAll(
            DoAsyncUncompressionOneParaBenchmark(1024 * 256),
            DoAsyncUncompressionOneParaBenchmark(1024 * 512),
            DoAsyncUncompressionOneParaBenchmark(1024 * 1024),
            DoAsyncUncompressionOneParaBenchmark(1024 * 1024 * 5),
            DoAsyncUncompressionOneParaBenchmark(1024 * 1024 * 10)
            );

        }


        private void DoCompressionBenchmark(int size)
        {
            Benchmark.Run("Compressing", size, benchmark =>
            {
                var output = new byte[CompressTool.GetMaxCompressedLength(benchmark.Input.Length)];
                int length = 0;
                benchmark.Stopwatch.Start();
                for (int i = 0; i < benchmark.Iterations; ++i)
                    length = CompressTool.Compress(benchmark.Input, 0, benchmark.Input.Length, output, 0);
                benchmark.Stopwatch.Stop();
                var roundtrip = new byte[benchmark.Input.Length];
                var roundtripLength = CompressTool.Uncompress(output, 0, length, roundtrip, 0);
                Assert.IsTrue(benchmark.Input.SequenceEqual(roundtrip.Take(roundtripLength)));
                benchmark.Note = String.Format(" ({0:0.00 %})", length / (double)benchmark.Input.Length);
            });
        }

        private void DoUncompressionBenchmark(int size)
        {
            Benchmark.Run("Uncompressing", size, benchmark =>
            {
                var compressed = CompressTool.Compress(benchmark.Input);
                var roundtrip = new byte[benchmark.Input.Length];
                int length = 0;
                benchmark.Stopwatch.Start();
                for (int i = 0; i < benchmark.Iterations; ++i)
                    length = CompressTool.Uncompress(compressed, 0, compressed.Length, roundtrip, 0);
                benchmark.Stopwatch.Stop();
                CollectionAssert.AreEqual(benchmark.Input, roundtrip);
            });

        }

        private Task DoAsyncCompressionBenchmark(int size)
        {
            return Task.Factory.StartNew(() =>
             {
                 Benchmark.Run("Compressing", size, benchmark =>
                 {
                     var output = new byte[CompressTool.GetMaxCompressedLength(benchmark.Input.Length)];
                     int length = 0;
                     benchmark.Stopwatch.Start();
                     for (int i = 0; i < benchmark.Iterations; ++i)
                         length = CompressTool.Compress(benchmark.Input, 0, benchmark.Input.Length, output, 0);
                     benchmark.Stopwatch.Stop();
                     var roundtrip = new byte[benchmark.Input.Length];
                     var roundtripLength = CompressTool.Uncompress(output, 0, length, roundtrip, 0);
                     Assert.IsTrue(benchmark.Input.SequenceEqual(roundtrip.Take(roundtripLength)));
                     benchmark.Note = String.Format(" ({0:0.00 %})", length / (double)benchmark.Input.Length);
                 });
             });
        }

        private Task DoAsyncUncompressionBenchmark(int size)
        {

            return Task.Factory.StartNew(() =>
            {
                Benchmark.Run("Uncompressing", size, benchmark =>
                {
                    var compressed = CompressTool.Compress(benchmark.Input);
                    var roundtrip = new byte[benchmark.Input.Length];
                    int length = 0;
                    benchmark.Stopwatch.Start();
                    for (int i = 0; i < benchmark.Iterations; ++i)
                        length = CompressTool.Uncompress(compressed, 0, compressed.Length, roundtrip, 0);
                    benchmark.Stopwatch.Stop();
                    CollectionAssert.AreEqual(benchmark.Input, roundtrip);
                });
            });

        }





        private void DoCompressionOneParaBenchmark(int size)
        {
            Benchmark.Run("Compressing", size, benchmark =>
            {
                int length = 0;
                benchmark.Stopwatch.Start();
                for (int i = 0; i < benchmark.Iterations; ++i)
                {
                    var output = CompressTool.Compress(benchmark.Input);
                    length = output.Length;
                }
                benchmark.Stopwatch.Stop();

               
                benchmark.Note = String.Format(" ({0:0.00 %})", length / (double)benchmark.Input.Length);
            });
        }

        private void DoUncompressionOneParaBenchmark(int size)
        {
            Benchmark.Run("Uncompressing", size, benchmark =>
            {
                var compressed = CompressTool.Compress(benchmark.Input);
                var roundtrip = new byte[benchmark.Input.Length];
                int length = 0;
                benchmark.Stopwatch.Start();
                for (int i = 0; i < benchmark.Iterations; ++i)
                {
                    roundtrip = CompressTool.Uncompress(compressed);
                    length = roundtrip.Length;
                }
                benchmark.Stopwatch.Stop();
                CollectionAssert.AreEqual(benchmark.Input, roundtrip);
            });

        }

        private Task DoAsyncCompressionOneParaBenchmark(int size)
        {
            return Task.Factory.StartNew(() =>
            {
                Benchmark.Run("Compressing", size, benchmark =>
                {
                    var output = new byte[CompressTool.GetMaxCompressedLength(benchmark.Input.Length)];
                    int length = 0;
                    benchmark.Stopwatch.Start();
                    for (int i = 0; i < benchmark.Iterations; ++i)
                    {
                        output = CompressTool.Compress(benchmark.Input);
                        length = output.Length;
                    }
                    benchmark.Stopwatch.Stop();
                    var roundtrip = CompressTool.Uncompress(output);
                    Assert.IsTrue(benchmark.Input.SequenceEqual(roundtrip));
                    benchmark.Note = String.Format(" ({0:0.00 %})", length / (double)benchmark.Input.Length);
                });
            });
        }

        private Task DoAsyncUncompressionOneParaBenchmark(int size)
        {

            return Task.Factory.StartNew(() =>
            {
                Benchmark.Run("Uncompressing", size, benchmark =>
                {
                    var compressed = CompressTool.Compress(benchmark.Input);
                    var roundtrip = new byte[benchmark.Input.Length];
                    int length = 0;
                    benchmark.Stopwatch.Start();
                    for (int i = 0; i < benchmark.Iterations; ++i)
                    {
                        roundtrip = CompressTool.Uncompress(compressed);
                        length = roundtrip.Length;
                    }
                    benchmark.Stopwatch.Stop();
                    CollectionAssert.AreEqual(benchmark.Input, roundtrip);
                });
            });

        }

    }
}
