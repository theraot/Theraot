#if FAT

using System;
using System.Threading;

namespace Theraot.Threading
{
    internal sealed partial class NoReentrantReadWriteLock : IExtendedDisposable
    {
        private int _status;

        [System.Diagnostics.DebuggerNonUserCode]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~NoReentrantReadWriteLock()
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
            get
            {
                return _status == -1;
            }
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
                if (!ReferenceEquals(whenDisposed, null))
                {
                    whenDisposed.Invoke();
                }
            }
            else
            {
                if (!ReferenceEquals(whenNotDisposed, null))
                {
                    if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
                    {
                        try
                        {
                            whenNotDisposed.Invoke();
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _status);
                        }
                    }
                    else
                    {
                        if (!ReferenceEquals(whenDisposed, null))
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
                if (ReferenceEquals(whenDisposed, null))
                {
                    return default(TReturn);
                }
                return whenDisposed.Invoke();
            }
            if (ReferenceEquals(whenNotDisposed, null))
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
                    Interlocked.Decrement(ref _status);
                }
            }
            if (ReferenceEquals(whenDisposed, null))
            {
                return default(TReturn);
            }
            return whenDisposed.Invoke();
        }

        [System.Diagnostics.DebuggerNonUserCode]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive", Justification = "By Design")]
        private void Dispose(bool disposeManagedResources)
        {
            if (TakeDisposalExecution())
            {
                try
                {
                    if (disposeManagedResources)
                    {
                        _freeToRead.Dispose();
                        _freeToWrite.Dispose();
                    }
                }
                finally
                {
                    _freeToRead = null;
                    _freeToWrite = null;
                }
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
    }
}

#endif