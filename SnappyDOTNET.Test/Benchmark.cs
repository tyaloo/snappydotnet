using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SnappyDOTNET.Test
{
    public class Benchmark
    {
        //public static readonly string DataPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\testdata");
        public int Iterations { get; private set; }
        public byte[] Input { get; private set; }
        public string Note { get; set; }
        public readonly Stopwatch Stopwatch = new Stopwatch();

        static TestDataCreator _dataCreator = new TestDataCreator(0.5f);
        private const int KB = 1024;
        private const int MB = 1024 * KB;

        //public static string[] GetTestFiles()
        //{
        //    return Directory.GetFiles(DataPath).Select(Path.GetFileNameWithoutExtension).ToArray();
        //}

        static string GetSizeText(int size)
        {
            if (size < KB)
            {
                return size + "bytes";
            }
            if (size < MB)
            {
                return size / KB + "K";
            }
            return size / MB + "M";
        }

        public static void Run(string type, int length, Action<Benchmark> action)
        {
            var data = _dataCreator.GenerateTestData(length);
            Benchmark last = null;
            for (int iterations = 1; last == null || last.Iterations < 1000000 && last.Stopwatch.Elapsed < TimeSpan.FromMilliseconds(500); iterations *= 2)
            {
                var benchmark = new Benchmark();
                benchmark.Iterations = iterations;
                benchmark.Input = data;
                benchmark.Note = "";
                action(benchmark);
                last = benchmark;
            }
            var speed = last.Iterations * (double)data.Length / 1024 / 1024;
            if (speed >= 1000)
                Console.WriteLine("{0} {1}: {2:0.0} GB/s{3}", type, GetSizeText(length), speed / 1024, last.Note);
            else
                Console.WriteLine("{0} {1}: {2:0} MB/s{3}", type, GetSizeText(length), speed, last.Note);
        }

    }

}
