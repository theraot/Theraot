#if FAT
using System;
using System.Threading;

namespace Theraot.Threading
{
    internal sealed partial class NoReentrantReadWriteLock : IExtendedDisposable
    {
        private int _disposeStatus;

        [System.Diagnostics.DebuggerNonUserCode]
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

        public bool IsDisposed => _disposeStatus == -1;

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
                whenDisposed?.Invoke();
            }
            else
            {
                if (whenNotDisposed == null)
                {
                    return;
                }

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

        [System.Diagnostics.DebuggerNonUserCode]
        public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
        {
            if (_disposeStatus == -1)
            {
                return whenDisposed == null ? default : whenDisposed.Invoke();
            }
            if (whenNotDisposed == null)
            {
                return default;
            }

            if (!ThreadingHelper.SpinWaitRelativeSet(ref _disposeStatus, 1, -1))
            {
                return whenDisposed == null ? default : whenDisposed.Invoke();
            }

            try
            {
                return whenNotDisposed.Invoke();
            }
            finally
            {
                Interlocked.Decrement(ref _disposeStatus);
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void Dispose(bool disposeManagedResources)
        {
            if (!TakeDisposalExecution())
            {
                return;
            }

            if (!disposeManagedResources)
            {
                return;
            }

            try
            {
                _freeToRead.Dispose();
                _freeToWrite.Dispose();
            }
            finally
            {
                _freeToRead = null;
                _freeToWrite = null;
            }
        }

        private bool TakeDisposalExecution()
        {
            return _disposeStatus != -1 && ThreadingHelper.SpinWaitSetUnless(ref _disposeStatus, -1, 0, -1);
        }
    }
}

#endif