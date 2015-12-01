using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SnappyDOTNET.Test
{
    class RepeaterStream : Stream
    {
        byte[] Data;
        int ReadPos;
        Random Random = new Random();

        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return false; } }
        public override bool CanSeek { get { return false; } }
        public override long Length { get { throw new NotSupportedException(); } }
        public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }

        public RepeaterStream(byte[] data)
        {
            Data = data;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > 0)
            {
                count = Math.Min(count, Data.Length - ReadPos);
                count = Random.Next(1, count + 1);
                Array.Copy(Data, ReadPos, buffer, offset, count);
                ReadPos += count;
                if (ReadPos == Data.Length)
                    ReadPos = 0;
                return count;
            }
            else
                return 0;
        }



        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        public override void Flush() { throw new NotSupportedException(); }

        public override void SetLength(long value) { throw new NotSupportedException(); }
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
    }

}
