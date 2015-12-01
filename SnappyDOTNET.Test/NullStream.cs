using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SnappyDOTNET.Test
{
    class NullStream : Stream
    {

        public long Written { get; private set; }

        public override bool CanRead { get { return false; } }
        public override bool CanWrite { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override long Length { get { throw new NotSupportedException(); } }
        public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }



        public override int Read(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        public override void Write(byte[] buffer, int offset, int count) { Written += count; }

        public override void Flush() { }

        public override void SetLength(long value) { throw new NotSupportedException(); }
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
    }

}
