// Needed for Workaround

using System;
using System.Diagnostics;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public partial class WeakNeedle<T> // T is used in another file, this is a partial class
    {
        private int _disposeStatus;

        [DebuggerNonUserCode]
        ~WeakNeedle()
        {
            try
            {
                // Empty
            }
            finally
            {
                Dispose(false);
            }
        }

        public bool IsDisposed
        {
            [DebuggerNonUserCode]
            get => _disposeStatus == -1;
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

        [DebuggerNonUserCode]
        public void DisposedConditional(Action whenDisposed, Action whenNotDisposed)
        {
            if (_disposeStatus == -1)
            {
                whenDisposed?.Invoke();
            }
            else
            {
                if (whenNotDisposed != null)
                {
                    if (ThreadingHelper.SpinWaitRelativeSet(ref _disposeStatus, 1, -1))
                    {
                        try
                        {
                            whenNotDisposed.Invoke();
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _disposeStatus);
                        }
                    }
                    else
                    {
                        whenDisposed?.Invoke();
                    }
                }
            }
        }

        [DebuggerNonUserCode]
        public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
        {
            if (_disposeStatus == -1)
            {
                if (whenDisposed == null)
                {
                    return default;
                }
                return whenDisposed.Invoke();
            }
            if (whenNotDisposed == null)
            {
                return default;
            }
            if (ThreadingHelper.SpinWaitRelativeSet(ref _disposeStatus, 1, -1))
            {
                try
                {
                    return whenNotDisposed.Invoke();
                }
                finally
                {
                    Interlocked.Decrement(ref _disposeStatus);
                }
            }
            if (whenDisposed == null)
            {
                return default;
            }
            return whenDisposed.Invoke();
        }

        [DebuggerNonUserCode]
        protected virtual void Dispose(bool disposeManagedResources)
        {
            try
            {
                if (TakeDisposalExecution())
                {
                    try
                    {
                        if (disposeManagedResources)
                        {
                            ReportManagedDisposal();
                        }
                    }
                    finally
                    {
                        ReleaseExtracted();
                    }
                }
            }
            catch (Exception exception)
            {
                // Catch them all - fields may be partially collected.
                GC.KeepAlive(exception);
            }
        }

        [DebuggerNonUserCode]
        protected void ProtectedCheckDisposed(string exceptionMessageWhenDisposed)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(exceptionMessageWhenDisposed);
            }
        }

        protected bool TakeDisposalExecution()
        {
            if (_disposeStatus == -1)
            {
                return false;
            }
            return ThreadingHelper.SpinWaitSetUnless(ref _disposeStatus, -1, 0, -1);
        }

        [DebuggerNonUserCode]
        protected void ThrowDisposedException()
        {
            throw new ObjectDisposedException(nameof(WeakNeedle<T>));
        }

        [DebuggerNonUserCode]
        protected TReturn ThrowDisposedException<TReturn>()
        {
            throw new ObjectDisposedException(nameof(WeakNeedle<T>));
        }

        [DebuggerNonUserCode]
        protected bool UnDispose()
        {
            if (Volatile.Read(ref _disposeStatus) == -1)
            {
                Volatile.Write(ref _disposeStatus, 0);
                return true;
            }
            return false;
        }
    }
}