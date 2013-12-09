// SpinWait.cs
//
// Copyright (c) 2008 Jérémie "Garuma" Laval, Alfonso J. Ramos
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if NET20 || NET30 || NET35

namespace System.Threading
{
    public struct SpinWait
    {
        private const int INT_MaxTime = 200;

        private static readonly int _sleepCountHint = 10;

        private static readonly bool _isSingleCpu = Environment.ProcessorCount == 1;

        private int _count;

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public bool NextSpinWillYield
        {
            get
            {
                return _isSingleCpu ? true : _count % _sleepCountHint == 0;
            }
        }

        public static void SpinUntil(Func<bool> condition)
        {
            Theraot.Threading.ThreadingHelper.SpinWaitUntil(condition);
        }

        public static bool SpinUntil(Func<bool> condition, TimeSpan timeout)
        {
            return SpinUntil(condition, (int)timeout.TotalMilliseconds);
        }

        public static bool SpinUntil(Func<bool> condition, int millisecondsTimeout)
        {
            return Theraot.Threading.ThreadingHelper.SpinWaitUntil(condition, TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public void Reset()
        {
            _count = 0;
        }

        public void SpinOnce()
        {
            _count++;
            if (_isSingleCpu)
            {
                Thread.Sleep(0);
            }
            else
            {
                if (_count % _sleepCountHint == 0)
                {
                    Thread.Sleep(0);
                }
                else
                {
                    Thread.SpinWait(Math.Min(_count, INT_MaxTime) << 1);
                }
            }
        }
    }
}

#endif