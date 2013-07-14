using System;

namespace Theraot.Threading
{
    public sealed partial class Disposable : IDisposable, IExtendedDisposable
    {
        private int _status;

        public bool IsDisposed
        {
            [global::System.Diagnostics.DebuggerNonUserCode]
            get
            {
                return _status == -1;
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
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

        [global::System.Diagnostics.DebuggerNonUserCode]
        public void DisposedConditional(Action whenDisposed, Action whenNotDisposed)
        {
            if (_status == -1)
            {
                if (!ReferenceEquals(whenDisposed, null))
                {
                    whenDisposed.Invoke();
                }
            }
            else
            {
                if (!ReferenceEquals(whenNotDisposed, null))
                {
                    ThreadingHelper.SpinWaitExchangeIgnoringRelative(-1, ref _status, 1);
                    if (_status == -1)
                    {
                        if (!ReferenceEquals(whenDisposed, null))
                        {
                            whenDisposed.Invoke();
                        }
                    }
                    else
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
                }
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
        {
            if (_status == -1)
            {
                if (ReferenceEquals(whenDisposed, null))
                {
                    return default(TReturn);
                }
                else
                {
                    return whenDisposed.Invoke();
                }
            }
            else
            {
                if (ReferenceEquals(whenNotDisposed, null))
                {
                    return default(TReturn);
                }
                else
                {
                    ThreadingHelper.SpinWaitExchangeIgnoringRelative(-1, ref _status, 1);
                    if (_status == -1)
                    {
                        if (ReferenceEquals(whenDisposed, null))
                        {
                            return default(TReturn);
                        }
                        else
                        {
                            return whenDisposed.Invoke();
                        }
                    }
                    else
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
                }
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive", Justification = "By Design")]
        private void Dispose(bool disposeManagedResources)
        {
            if (TakeDisposalExecution())
            {
                try
                {
                    if (disposeManagedResources)
                    {
                        //Empty
                    }
                }
                finally
                {
                    try
                    {
                        _release();
                    }
                    finally
                    {
                        _release = null;
                    }
                }
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        private void ProtectedCheckDisposed(string exceptionMessegeWhenDisposed)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(exceptionMessegeWhenDisposed);
            }
        }

        private bool TakeDisposalExecution()
        {
            if (_status == -1)
            {
                return false;
            }
            else
            {
                ThreadingHelper.SpinWaitExchangeIgnoring(-1, ref _status, -1, 0);
                if (_status == -1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        private void ThrowDisposedexception()
        {
            throw new ObjectDisposedException(GetType().FullName);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        private TReturn ThrowDisposedexception<TReturn>()
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }
}