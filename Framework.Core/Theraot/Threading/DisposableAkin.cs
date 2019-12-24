using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Theraot.Threading
{
    [DebuggerNonUserCode]
    public sealed class DisposableAkin :
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
        System.Runtime.ConstrainedExecution.CriticalFinalizerObject,
#endif
        IDisposable
    {
        private Action? _release;
        private StrongBox<UniqueId>? _threadUniqueId;

        private DisposableAkin(Action release, UniqueId threadUniqueId)
        {
            _release = release ?? throw new ArgumentNullException(nameof(release));
            _threadUniqueId = new StrongBox<UniqueId>(threadUniqueId);
        }

        ~DisposableAkin()
        {
            try
            {
                // Empty
            }
            finally
            {
                try
                {
                    Dispose(false);
                }
                catch (Exception exception)
                {
                    // Catch them all - fields may be partially collected.
                    _ = exception;
                }
            }
        }

        public bool IsDisposed => _threadUniqueId == null;

        public static DisposableAkin Create(Action release)
        {
            return new DisposableAkin(release, ThreadUniqueId.CurrentThreadId);
        }

        public bool Dispose(Func<bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var found = Interlocked.CompareExchange(ref _threadUniqueId, null, new StrongBox<UniqueId>(ThreadUniqueId.CurrentThreadId));
            if (found == null || found.Value != ThreadUniqueId.CurrentThreadId)
            {
                return false;
            }

            if (!condition.Invoke())
            {
                return false;
            }

            if (_release == null)
            {
                return true;
            }

            try
            {
                _release.Invoke();
                return true;
            }
            finally
            {
                _release = null;
            }
        }

        [DebuggerNonUserCode]
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                var found = Interlocked.CompareExchange(ref _threadUniqueId, null, new StrongBox<UniqueId>(ThreadUniqueId.CurrentThreadId));
                if (found == null || found.Value != ThreadUniqueId.CurrentThreadId)
                {
                    return;
                }
            }

            if (_release == null)
            {
                return;
            }

            try
            {
                _release.Invoke();
            }
            finally
            {
                _release = null;
            }
        }
    }
}