// Needed for Workaround

using System;

namespace Theraot.Threading.Needles
{
    public partial class WeakNeedle<T> // T is used in another file, this is a partial class
    {
        private int _disposeStatus;

        [System.Diagnostics.DebuggerNonUserCode]
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
            [System.Diagnostics.DebuggerNonUserCode]
            get { return _disposeStatus == -1; }
        }

        [System.Diagnostics.DebuggerNonUserCode]
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

        [System.Diagnostics.DebuggerNonUserCode]
        public void DisposedConditional(Action whenDisposed, Action whenNotDisposed)
        {
            if (_disposeStatus == -1)
            {
                if (whenDisposed != null)
                {
                    whenDisposed.Invoke();
                }
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
                            System.Threading.Interlocked.Decrement(ref _disposeStatus);
                        }
                    }
                    else
                    {
                        if (whenDisposed != null)
                        {
                            whenDisposed.Invoke();
                        }
                    }
                }
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
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
                    System.Threading.Interlocked.Decrement(ref _disposeStatus);
                }
            }
            if (whenDisposed == null)
            {
                return default;
            }
            return whenDisposed.Invoke();
        }

        [System.Diagnostics.DebuggerNonUserCode]
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
                // Catch'em all - fields may be partially collected.
                GC.KeepAlive(exception);
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        protected void ProtectedCheckDisposed(string exceptionMessegeWhenDisposed)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(exceptionMessegeWhenDisposed);
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

        [System.Diagnostics.DebuggerNonUserCode]
        protected void ThrowDisposedexception()
        {
            throw new ObjectDisposedException(nameof(WeakNeedle<T>));
        }

        [System.Diagnostics.DebuggerNonUserCode]
        protected TReturn ThrowDisposedexception<TReturn>()
        {
            throw new ObjectDisposedException(nameof(WeakNeedle<T>));
        }

        [System.Diagnostics.DebuggerNonUserCode]
        protected bool UnDispose()
        {
            if (System.Threading.Volatile.Read(ref _disposeStatus) == -1)
            {
                System.Threading.Volatile.Write(ref _disposeStatus, 0);
                return true;
            }
            return false;
        }
    }
}