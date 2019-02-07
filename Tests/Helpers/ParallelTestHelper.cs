#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

// TestHelper.cs
//
// Copyright (c) 2008 Jérémie "Garuma" Laval
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
//
//

using System;
using System.Threading;

namespace Tests.Helpers
{
    public static class ParallelTestHelper
    {
        private const int DefaultRepetitionCount = 50;

        public static void ParallelStressTest<TSource>(TSource obj, Action<TSource> action, int threadCount)
        {
            if (action == null)
            {
                return;
            }
            var threads = new Thread[threadCount];
            for (var threadIndex = 0; threadIndex < threadCount; threadIndex++)
            {
                threads[threadIndex] = new Thread(() => action(obj));
                threads[threadIndex].Start();
            }

            // Wait for the completion
            for (var threadIndex = 0; threadIndex < threadCount; threadIndex++)
            {
                threads[threadIndex].Join();
            }
        }

        public static void Repeat(Action action)
        {
            Repeat(action, DefaultRepetitionCount);
        }

        public static void Repeat(Action action, int repetitionCount)
        {
            if (action == null)
            {
                return;
            }
            for (var repetitionIndex = 0; repetitionIndex < repetitionCount; repetitionIndex++)
            {
                action();
            }
        }
    }
}