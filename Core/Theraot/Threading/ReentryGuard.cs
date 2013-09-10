using System;

using Theraot.Collections;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class ReentryGuard
    {
        private NotNull<NoTrackingThreadLocal<Tuple<ExtendedQueue<Action>, Guard>>> _workQueue;

        public ReentryGuard()
        {
            _workQueue = new NotNull<NoTrackingThreadLocal<Tuple<ExtendedQueue<Action>, Guard>>>
                (
                    new NoTrackingThreadLocal<Tuple<ExtendedQueue<Action>, Guard>>
                    (
                        () => new Tuple<ExtendedQueue<Action>, Guard>(new ExtendedQueue<Action>(), new Guard())
                    )
                );
        }

        private static Action ExecutePending(Tuple<ExtendedQueue<Action>, Guard> local)
        {
            Action action;
            while (local.Item1.TryTake(out action))
            {
                action.Invoke();
            }
            return action;
        }

        public IPromise Execute(Action action)
        {
            var local = _workQueue.Value.Value;
            IDisposable engagement;
            if (local.Item2.Enter(out engagement))
            {
                using (engagement)
                {
                    try
                    {
                        action.Invoke();
                        return new PromiseNeedle(true);
                    }
                    catch (Exception exception)
                    {
                        return new PromiseNeedle(exception);
                    }
                    finally
                    {
                        ExecutePending(local);
                    }
                }
            }
            else
            {
                IPromised promised;
                var result = new PromiseNeedle(out promised, false);
                local.Item1.Add
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
        }

        public IPromise<T> Execute<T>(Func<T> action)
        {
            var local = _workQueue.Value.Value;
            IDisposable engagement;
            if (local.Item2.Enter(out engagement))
            {
                using (engagement)
                {
                    try
                    {
                        return new PromiseNeedle<T>(action.Invoke());
                    }
                    catch (Exception exception)
                    {
                        return new PromiseNeedle<T>(exception);
                    }
                    finally
                    {
                        ExecutePending(local);
                    }
                }
            }
            else
            {
                IPromised<T> promised;
                var result = new PromiseNeedle<T>(out promised, false);
                local.Item1.Add
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
        }
    }
}