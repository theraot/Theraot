using System;
using System.Collections.Generic;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class ReentryGuard
    {
        private StructNeedle<NoTrackingThreadLocal<Tuple<Queue<Action>, Guard>>> _workQueue;

        public ReentryGuard()
        {
            _workQueue = new StructNeedle<NoTrackingThreadLocal<Tuple<Queue<Action>, Guard>>>
                (
                    new NoTrackingThreadLocal<Tuple<Queue<Action>, Guard>>
                    (
                        () => new Tuple<Queue<Action>, Guard>(new Queue<Action>(), new Guard())
                    )
                );
        }

        ~ReentryGuard()
        {
            var workQueue = _workQueue.Value;
            if (!ReferenceEquals(workQueue, null))
            {
                workQueue.Dispose();
            }
            _workQueue.Value = null;
        }

        public bool IsTaken
        {
            get
            {
                var workQueue = _workQueue.Value;
                if (workQueue != null)
                {
                    var local = workQueue.Value;
                    return local.Item2.IsTaken;
                }
                return false;
            }
        }

        public IPromise Execute(Action action)
        {
            var workQueue = _workQueue.Value;
            if (workQueue != null)
            {
                var local = workQueue.Value;
                var result = AddExecution(action, local);
                IDisposable engagement;
                if (local.Item2.Enter(out engagement))
                {
                    using (engagement)
                    {
                        ExecutePending(local);
                    }
                }
                return result;
            }
            return null;
        }

        public IPromise<T> Execute<T>(Func<T> action)
        {
            var workQueue = _workQueue.Value;
            if (workQueue != null)
            {
                var local = workQueue.Value;
                var result = AddExecution(action, local);
                IDisposable engagement;
                if (local.Item2.Enter(out engagement))
                {
                    using (engagement)
                    {
                        ExecutePending(local);
                    }
                }
                return result;
            }
            return null;
        }

        private static IPromise AddExecution(Action action, Tuple<Queue<Action>, Guard> local)
        {
            PromiseNeedle.Promised promised;
            var result = new PromiseNeedle(out promised, false);
            local.Item1.Enqueue
            (
                () =>
                {
                    try
                    {
                        action.Invoke();
                        promised.OnCompleted();
                    }
                    catch (Exception exception)
                    {
                        promised.OnError(exception);
                    }
                }
            );
            return result;
        }

        private static IPromise<T> AddExecution<T>(Func<T> action, Tuple<Queue<Action>, Guard> local)
        {
            PromiseNeedle<T>.Promised promised;
            var result = new PromiseNeedle<T>(out promised, false);
            local.Item1.Enqueue
            (
                () =>
                {
                    try
                    {
                        promised.OnNext(action.Invoke());
                    }
                    catch (Exception exception)
                    {
                        promised.OnError(exception);
                    }
                }
            );
            return result;
        }

        private static void ExecutePending(Tuple<Queue<Action>, Guard> local)
        {
            while (local.Item1.Count > 0)
            {
                Action action = local.Item1.Dequeue();
                action.Invoke();
            }
        }
    }
}