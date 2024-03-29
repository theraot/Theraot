﻿using System;
using System.Diagnostics;
using System.Threading;

namespace Theraot.Threading
{
    public sealed partial class Disposable : IDisposable
    {
        private int _disposeStatus;

        [DebuggerNonUserCode]
        ~Disposable()
        {
            try
            {
                // Empty
            }
            finally
            {
                try
                {
                    Dispose(disposeManagedResources: false);
                }
                catch (Exception exception)
                {
                    // Catch them all - fields may be partially collected.
                    _ = exception;
                }
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
            Dispose(disposeManagedResources: true);
            GC.SuppressFinalize(this);
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

        [DebuggerNonUserCode]
        public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
        {
            if (_disposeStatus == -1)
            {
                return whenDisposed == null ? default! : whenDisposed.Invoke();
            }

            if (whenNotDisposed == null)
            {
                return default!;
            }

            if (!ThreadingHelper.SpinWaitRelativeSet(ref _disposeStatus, 1, -1))
            {
                return whenDisposed == null ? default! : whenDisposed.Invoke();
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

        [DebuggerNonUserCode]
        private void Dispose(bool disposeManagedResources)
        {
            _ = disposeManagedResources;
            if (!TakeDisposalExecution())
            {
                return;
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

        private bool TakeDisposalExecution()
        {
            return _disposeStatus != -1 && ThreadingHelper.SpinWaitSetUnless(ref _disposeStatus, -1, 0, -1);
        }
    }
}