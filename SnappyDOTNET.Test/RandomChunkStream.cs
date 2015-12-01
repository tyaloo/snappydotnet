﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SnappyDOTNET.Test
{
    class RandomChunkStream : Stream
    {
        static Random Random = new Random();
        readonly int Capacity = Random.Next(5000, 100000);
        readonly byte[] Buffer;
        readonly AsyncMultiSemaphore ReadSemaphore = new AsyncMultiSemaphore();
        readonly AsyncMultiSemaphore WriteSemaphore = new AsyncMultiSemaphore();
        int WriteAt;
        int ReadAt;
        long ClosedAt = -1;
        public long TotalRead;
        public long TotalWritten;

        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }
        public override long Length { get { throw new NotSupportedException(); } }

        public RandomChunkStream()
        {
            Buffer = new byte[Capacity + 1];
            WriteSemaphore.Add(Capacity);
        }

        protected override void Dispose(bool disposing)
        {
            ClosedAt = TotalWritten;
            ReadSemaphore.Add(1);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                var block = WriteSemaphore.Take(Math.Min(count, Buffer.Length - WriteAt));
                Array.Copy(buffer, offset, Buffer, WriteAt, block);
                WriteAt += block;
                if (WriteAt == Buffer.Length)
                    WriteAt = 0;
                offset += block;
                count -= block;
                TotalWritten += block;
                ReadSemaphore.Add(block);
            }
        }



        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = Random.Next(1, count + 1);
            int total = 0;
            while (count > 0)
            {
                var block = ReadSemaphore.Take(Math.Min(count, Buffer.Length - ReadAt));
                if (ClosedAt == TotalRead)
                    break;
                Array.Copy(Buffer, ReadAt, buffer, offset, block);
                ReadAt += block;
                if (ReadAt == Buffer.Length)
                    ReadAt = 0;
                offset += block;
                count -= block;
                total += block;
                TotalRead += block;
                WriteSemaphore.Add(block);
            }
            return total;
        }


        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
        public override void SetLength(long value) { throw new NotSupportedException(); }
    }

}
