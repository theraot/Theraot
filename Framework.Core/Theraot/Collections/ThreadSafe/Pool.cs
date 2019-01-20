// Needed for NET40

using System;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
    internal class Pool<T>
        where T : class
    {
        private readonly FixedSizeQueue<T> _entries;
        private readonly Action<T> _recycler;
        private readonly ReentryGuard _reentryGuard;

        public Pool(int capacity, Action<T> recycler)
        {
            _reentryGuard = new ReentryGuard();
            _entries = new FixedSizeQueue<T>(capacity);
            _recycler = recycler;
        }

        internal bool Donate(T entry)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var reentryGuard = _reentryGuard;
            if (entry != null && reentryGuard != null && ReentryGuard.Enter(reentryGuard.Id))
            {
                try
                {
                    var entries = _entries;
                    var recycler = _recycler;
                    if (entries == null || recycler == null)
                    {
                        return false;
                    }

                    recycler.Invoke(entry);
                    return entries.TryAdd(entry);
                }
                catch (ObjectDisposedException exception)
                {
                    No.Op(exception);
                }
                catch (InvalidOperationException exception)
                {
                    No.Op(exception);
                }
                catch (NullReferenceException exception)
                {
                    No.Op(exception);
                }
                finally
                {
                    ReentryGuard.Leave(reentryGuard.Id);
                }
            }
            if (entry is IDisposable disposable)
            {
                disposable.Dispose();
            }
            return false;
        }

        internal bool TryGet(out T result)
        {
            return _entries.TryTake(out result);
        }
    }
}
