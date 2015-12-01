using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using SnappyDOTNET;
using System.Collections.Generic;
using System.Linq;

namespace SnappyDOTNET.Test
{
    [TestClass]
    public class CompressToolTest
    {
        TestDataCreator _dataCreator = new TestDataCreator(0.7f);
        [TestMethod]
        public void CompressRange()
        {
            var input = Encoding.ASCII.GetBytes("ByeHelloBye");
            var output = new byte[100];
            var length = CompressTool.Compress(input, 3, 5, output, 10);
            Assert.AreEqual("Hello", 
                Encoding.ASCII.GetString(CompressTool.Uncompress(output.Skip(10).Take(length).ToArray())));
        }

        [TestMethod]
        public void CompressUncompressSimple()
        {
            Assert.AreEqual("Hello", 
                Encoding.ASCII.GetString(
                CompressTool.Uncompress(CompressTool.Compress(Encoding.ASCII.GetBytes("Hello")))));
        }


        void TestException(Action act, Type type)
        {
            try
            {
                act();
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, type);
            }


        }

        [TestMethod]
        public void CompressUncompressEmpty()
        {
            var compressed = CompressTool.Compress(new byte[0]);
            Assert.IsTrue(compressed.Length>0);
            Assert.AreEqual(0, CompressTool.Uncompress(compressed).Length);
        }

        [TestMethod]
        public void CompressUncompressTest()
        {
            var data = _dataCreator.GenerateTestData(1024 * 1024);
            var compressed = CompressTool.Compress(data);
            Assert.IsTrue(compressed.Length > 0);

            Assert.IsTrue(data.SequenceEqual(CompressTool.Uncompress(compressed)));
        }

        [TestMethod]
        public void CompressExceptions()
        {
            var input = new byte[100];
            var output = new byte[100];
            try
            {
                CompressTool.Compress(null);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
            }


            try
            {
                CompressTool.Compress(null, 0, 3, output, 0);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
            }


            try
            {
                CompressTool.Compress(input, 0, 3, null, 0);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
            }

            try
            {
                CompressTool.Compress(input, -1, 3, output, 0);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentOutOfRangeException));
            }

            try
            {
                CompressTool.Compress(input, -1, 3, output, 0);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentOutOfRangeException));
            }

            try
            {
                CompressTool.Compress(input, 0, -1, output, 0);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentOutOfRangeException));
            }

            try
            {
                CompressTool.Compress(input, 90, 20, output, 0);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentOutOfRangeException));
            }

            try
            {
                CompressTool.Compress(input, 0, 3, output, -1);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentOutOfRangeException));
            }

            try
            {
                CompressTool.Compress(input, 0, 3, output, 100);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentOutOfRangeException));
            }

            try
            {
                CompressTool.Compress(input, 0, 3, output, 101);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentOutOfRangeException));
            }

            try
            {
                CompressTool.Compress(input, 0, 100, new byte[3], 0);
                Assert.Fail("shouldn't reach there");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentOutOfRangeException));
            }
            
        }

        [TestMethod]
        public void UncompressRange()
        {
            var howdy = Encoding.ASCII.GetBytes("Howdy");
            var padded = howdy.Take(3).Concat(CompressTool.Compress(Encoding.ASCII.GetBytes("Hello"))).Concat(howdy.Skip(3)).ToArray();
            var output = new byte[100];
            var length = CompressTool.Uncompress(padded, 3, padded.Length - 5, output, 10);
            Assert.AreEqual(5, length);
            Assert.AreEqual("Hello", Encoding.ASCII.GetString(output.Skip(10).Take(5).ToArray()));
        }

        [TestMethod]
        public void UncompressExceptions()
        {
            var uncompressed = Encoding.ASCII.GetBytes("Hello, hello, howdy?");
            var compressed = CompressTool.Compress(uncompressed);
            var buffer = new byte[100];

            TestException(() => { CompressTool.Uncompress(null); }, typeof(ArgumentNullException));
            TestException(() => { CompressTool.Uncompress(null, 0, 3, buffer, 0); }, typeof(ArgumentNullException));
            TestException(() => { CompressTool.Uncompress(compressed, 0, compressed.Length, null, 0); }, typeof(ArgumentNullException));

            TestException(() => { CompressTool.Uncompress(compressed, -1, uncompressed.Length, buffer, 0); }, typeof(ArgumentOutOfRangeException));
            TestException(() => { CompressTool.Uncompress(compressed, 0, -1, buffer, 0); }, typeof(ArgumentOutOfRangeException));
            TestException(() => { CompressTool.Uncompress(compressed, compressed.Length - 2, 4, buffer, 0); }, typeof(ArgumentOutOfRangeException));


            TestException(() => { CompressTool.Uncompress(compressed, 0, 0, buffer, 0);},typeof(System.IO.IOException));
            TestException(() => { CompressTool.Uncompress(compressed, compressed.Length, 0, buffer, 0); }, typeof(System.IO.IOException));

            TestException(() => { CompressTool.Uncompress(compressed, 0, compressed.Length, buffer, -1);},typeof(ArgumentOutOfRangeException));
            TestException(() => { CompressTool.Uncompress(compressed, 0, compressed.Length, buffer, 101);},typeof(ArgumentOutOfRangeException));
            TestException(() => { CompressTool.Uncompress(compressed, 0, compressed.Length, buffer, 97); }, typeof(ArgumentOutOfRangeException));


        }

        [TestMethod]
        public void GetUncompressedLengthExceptions()
        {
            var uncompressed = Encoding.ASCII.GetBytes("Hello, hello, howdy?");
            var compressed = CompressTool.Compress(uncompressed);
            var buffer = new byte[100];

            TestException(() => { CompressTool.GetUncompressedLength(null, 0, 3); }, typeof(ArgumentNullException));

            TestException(() => { CompressTool.GetUncompressedLength(compressed, -1, uncompressed.Length); }, typeof(ArgumentOutOfRangeException));
            TestException(() => { CompressTool.GetUncompressedLength(compressed, 0, -1); }, typeof(ArgumentOutOfRangeException));
            TestException(() => { CompressTool.GetUncompressedLength(compressed, compressed.Length - 2, 4); }, typeof(ArgumentOutOfRangeException));
            
            TestException(() => { CompressTool.GetUncompressedLength(compressed, 0, 0);},typeof(System.IO.IOException));
            TestException(() => { CompressTool.GetUncompressedLength(compressed, compressed.Length, 0); }, typeof(System.IO.IOException));
        }

    }
}
