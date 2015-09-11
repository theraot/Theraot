#if FAT

using System;

namespace Theraot.Threading.Needles
{
    public partial class LazyDisposableNeedle<T> : IDisposable, IExtendedDisposable
    {
        private int _status;

        [System.Diagnostics.DebuggerNonUserCode]
        ~LazyDisposableNeedle()
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
            [System.Diagnostics.DebuggerNonUserCode] get { return _status == -1; }
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
            if (_status == -1)
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
                    if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
                    {
                        try
                        {
                            whenNotDisposed.Invoke();
                        }
                        finally
                        {
                            System.Threading.Interlocked.Decrement(ref _status);
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
            if (_status == -1)
            {
                if (whenDisposed == null)
                {
                    return default(TReturn);
                }
                return whenDisposed.Invoke();
            }
            if (whenNotDisposed == null)
            {
                return default(TReturn);
            }
            if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
            {
                try
                {
                    return whenNotDisposed.Invoke();
                }
                finally
                {
                    System.Threading.Interlocked.Decrement(ref _status);
                }
            }
            if (whenDisposed == null)
            {
                return default(TReturn);
            }
            return whenDisposed.Invoke();
        }

        [System.Diagnostics.DebuggerNonUserCode]
        internal void Dispose(bool disposeManagedResources)
        {
            try
            {
                if (TakeDisposalExecution())
                {
                    if (disposeManagedResources)
                    {
                        OnDispose();
                    }
                }
            }
            catch (Exception exception)
            {
                // Pokemon - fields may be partially collected.
                GC.KeepAlive(exception);
            }
        }

        private bool TakeDisposalExecution()
        {
            if (_status == -1)
            {
                return false;
            }
            return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private bool UnDispose()
        {
            if (System.Threading.Thread.VolatileRead(ref _status) == -1)
            {
                System.Threading.Thread.VolatileWrite(ref _status, 0);
                return true;
            }
            return false;
        }
    }
}

#endif