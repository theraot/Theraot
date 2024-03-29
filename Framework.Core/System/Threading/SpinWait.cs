﻿#if LESSTHAN_NET40

#pragma warning disable CA1815 // Override equals and operator equals on value types

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

using Theraot.Threading;

namespace System.Threading
{
    public struct SpinWait
    {
        private const int _maxTime = 200;

        public int Count { get; private set; }

        public readonly bool NextSpinWillYield => Environment.ProcessorCount == 1 || Count % ThreadingHelper.SleepCountHint == 0;

        public static void SpinUntil(Func<bool> condition)
        {
            ThreadingHelper.SpinWaitUntil(condition);
        }

        public static bool SpinUntil(Func<bool> condition, TimeSpan timeout)
        {
            return SpinUntil(condition, (int)timeout.TotalMilliseconds);
        }

        public static bool SpinUntil(Func<bool> condition, int millisecondsTimeout)
        {
            return ThreadingHelper.SpinWaitUntil(condition, TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public void Reset()
        {
            Count = 0;
        }

        public void SpinOnce()
        {
            Count++;
            if (Environment.ProcessorCount == 1 || Count % ThreadingHelper.SleepCountHint == 0)
            {
                Thread.Sleep(0);
            }
            else
            {
                Thread.SpinWait(Math.Min(Count, _maxTime) << 1);
            }
        }
    }
}

#endif