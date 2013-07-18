#if FAT

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

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Pokemon")]
        public void Execute(Action action)
        {
            var local = _workQueue.Value.Value;
            IDisposable engagement;
            if (local.Item2.Enter(out engagement))
            {
                using (engagement)
                {
                    try
                    {
                        action();
                    }
                    catch
                    {
                        //Pokemon
                    }
                    finally
                    {
                        while (local.Item1.TryTake(out action))
                        {
                            try
                            {
                                action();
                            }
                            catch
                            {
                                //Pokemon
                            }
                        }
                    }
                }
            }
            else
            {
                local.Item1.Add(action);
            }
        }
    }
}

#endif