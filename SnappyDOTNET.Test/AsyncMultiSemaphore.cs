using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SnappyDOTNET.Test
{
    class AsyncMultiSemaphore
    {
        int CurrentCount;

        ManualResetEvent Available = new ManualResetEvent(false);

        public void Add(int count)
        {
            lock (this)
            {
                if (count < 0)
                    throw new ArgumentOutOfRangeException();
                if (CurrentCount > 0)
                    CurrentCount += count;
                else
                {
                    CurrentCount = count;

                    Available.Set();
                }
            }
        }

        public int Take(int max)
        {
            if (max <= 0)
                throw new ArgumentOutOfRangeException();
            while (true)
            {
                lock (this)
                {
                    if (CurrentCount > 0)
                    {
                        var taken = Math.Min(CurrentCount, max);
                        CurrentCount -= taken;
                        if (CurrentCount == 0)
                            Available.Reset();
                        return taken;
                    }
                }
                Available.WaitOne();
            }
        }
    }

}
