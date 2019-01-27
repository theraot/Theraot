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
        private readonly UniqueId _reentryGuardId;

        public Pool(int capacity, Action<T> recycler)
        {
            _reentryGuardId = RuntimeUniqueIdProvider.GetNextId();
            _entries = new FixedSizeQueue<T>(capacity);
            _recycler = recycler;
        }

        internal bool Donate(T entry)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (entry != null && ReentryGuard.Enter(_reentryGuardId))
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
                    ReentryGuard.Leave(_reentryGuardId);
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