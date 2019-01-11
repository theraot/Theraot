// Needed for NET40

using System;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
    internal class Pool<T>
        where T : class
    {
        private readonly FixedSizeQueue<T> _entries;
        private readonly UniqueId _id;
        private readonly Action<T> _recycler;

        public Pool(int capacity, Action<T> recycler)
        {
            _id = RuntimeUniqueIdProvider.GetNextId();
            _entries = new FixedSizeQueue<T>(capacity);
            _recycler = recycler;
        }

        internal bool Donate(T entry)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (entry != null && ReentryGuardHelper.Enter(_id))
            {
                try
                {
                    var entries = _entries;
                    var recycler = _recycler;
                    if (entries != null && recycler != null)
                    {
                        recycler.Invoke(entry);
                        return entries.TryAdd(entry);
                    }
                    return true;
                }
                catch (ObjectDisposedException exception)
                {
                    GC.KeepAlive(exception);
                }
                catch (InvalidOperationException exception)
                {
                    GC.KeepAlive(exception);
                }
                catch (NullReferenceException exception)
                {
                    GC.KeepAlive(exception);
                }
                finally
                {
                    ReentryGuardHelper.Leave(_id);
                }
                if (entry is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            return false;
        }

        internal bool TryGet(out T result)
        {
            return _entries.TryTake(out result);
        }
    }
}