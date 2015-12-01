using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnappyDOTNET.Test
{
    internal class TestDataCreator
    {
        private static readonly Random random = new Random();

        private float _compressionRatio = 0.5f;

        public TestDataCreator(float compressRatio)
        {
            _compressionRatio = compressRatio;
        }


        public float CompressionRatio
        {
            get
            {
                return _compressionRatio;
            }
        }

        public byte[] GenerateTestData(int length)
        {
            var data = new byte[length];
            for (var i = 0; i < data.Length; )
            {
                var segment = GenerateCompressibleData(128, _compressionRatio);
                var lengthToCopy = Math.Min(segment.Length, data.Length - i);
                Array.Copy(segment, 0, data, i, lengthToCopy);
                i += lengthToCopy;
            }
            return data;
        }

        private byte[] GenerateCompressibleData(int length, double ratio)
        {
            var rawLength = Math.Max(1, (int)(length * ratio));
            var rawData = new byte[rawLength];
            random.NextBytes(rawData);

            var data = new byte[length];
            for (var i = 0; i < length; i += rawLength)
            {
                Array.Copy(rawData, 0, data, i, Math.Min(length - i, rawLength));
            }
            return data;
        }
    }

}
