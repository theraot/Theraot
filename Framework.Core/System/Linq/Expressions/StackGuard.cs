#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Expressions
{
    internal sealed class StackGuard
    {
        private const int _maxExecutionStackCount = 1024;

        private int _executionStackCount;

        public void RunOnEmptyStack<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            RunOnEmptyStackCore(s =>
            {
                var t = (Tuple<Action<T1, T2>, T1, T2>)s;
                t.Item1(t.Item2, t.Item3);
                return default(object);
            }, Tuple.Create(action, arg1, arg2));
        }

        public void RunOnEmptyStack<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            RunOnEmptyStackCore(s =>
            {
                var t = (Tuple<Action<T1, T2, T3>, T1, T2, T3>)s;
                t.Item1(t.Item2, t.Item3, t.Item4);
                return default(object);
            }, Tuple.Create(action, arg1, arg2, arg3));
        }

        public TR RunOnEmptyStack<T1, T2, TR>(Func<T1, T2, TR> action, T1 arg1, T2 arg2)
        {
            return RunOnEmptyStackCore(s =>
            {
                var t = (Tuple<Func<T1, T2, TR>, T1, T2>)s;
                return t.Item1(t.Item2, t.Item3);
            }, Tuple.Create(action, arg1, arg2));
        }

        public TR RunOnEmptyStack<T1, T2, T3, TR>(Func<T1, T2, T3, TR> action, T1 arg1, T2 arg2, T3 arg3)
        {
            return RunOnEmptyStackCore(s =>
            {
                var t = (Tuple<Func<T1, T2, T3, TR>, T1, T2, T3>)s;
                return t.Item1(t.Item2, t.Item3, t.Item4);
            }, Tuple.Create(action, arg1, arg2, arg3));
        }

        public bool TryEnterOnCurrentStack()
        {
            return _executionStackCount < _maxExecutionStackCount;
        }

        private TR RunOnEmptyStackCore<TR>(Func<object, TR> action, object state)
        {
            _executionStackCount++;

            try
            {
                // Using default scheduler rather than picking up the current scheduler.
                var task = Task.Factory.StartNew(action, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

                var awaiter = task.GetAwaiter();

                // Avoid AsyncWaitHandle lazy allocation of ManualResetEvent in the rare case we finish quickly.
                if (!awaiter.IsCompleted)
                {
                    // Task.Wait has the potential of inlining the task's execution on the current thread; avoid this.
                    ((IAsyncResult)task).AsyncWaitHandle.WaitOne();
                }

                // Using awaiter here to unwrap AggregateException.
                return awaiter.GetResult();
            }
            finally
            {
                _executionStackCount--;
            }
        }
    }
}

#endif