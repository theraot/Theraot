#if FAT
using System;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class LazyDisposableNeedle<T> : LazyNeedle<T>, IDisposable
        where T : IDisposable
    {
        private int _status;

        public LazyDisposableNeedle(Func<T> valueFactory)
            : base(valueFactory)
        {
            // Empty
        }

        public LazyDisposableNeedle(T target)
            : base(target)
        {
            // Empty
        }

        [System.Diagnostics.DebuggerNonUserCode]
        ~LazyDisposableNeedle()
        {
            Dispose(false);
        }

        public bool IsDisposed => _status == -1;

        [System.Diagnostics.DebuggerNonUserCode]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public void DisposedConditional(Action whenDisposed, Action whenNotDisposed)
        {
            if (_status == -1)
            {
                whenDisposed?.Invoke();
            }
            else
            {
                if (whenNotDisposed == null)
                {
                    return;
                }

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
                    whenDisposed?.Invoke();
                }
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
        {
            if (_status == -1)
            {
                return whenDisposed == null ? default : whenDisposed.Invoke();
            }
            if (whenNotDisposed == null)
            {
                return default;
            }

            if (!ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
            {
                return whenDisposed == null ? default : whenDisposed.Invoke();
            }

            try
            {
                return whenNotDisposed.Invoke();
            }
            finally
            {
                System.Threading.Interlocked.Decrement(ref _status);
            }
        }

        public override void Initialize()
        {
            Initialize(UnDispose);
        }

        public void Reinitialize()
        {
            OnDispose();
            Initialize();
        }

        [System.Diagnostics.DebuggerNonUserCode]
        protected virtual void Dispose(bool disposeManagedResources)
        {
            try
            {
                if (TakeDisposalExecution() && disposeManagedResources)
                {
                    OnDispose();
                }
            }
            catch (Exception exception)
            {
                // Fields may be partially collected
                No.Op(exception);
            }
        }

        protected override void Initialize(Action beforeInitialize)
        {
            void BeforeInitializeReplacement()
            {
                try
                {
                    var waitHandle = WaitHandle.Value;
                    if (!WaitHandle.IsAlive)
                    {
                        WaitHandle.Value = new System.Threading.ManualResetEventSlim(false);
                        GC.KeepAlive(waitHandle);
                    }
                    beforeInitialize?.Invoke();
                }
                finally
                {
                    UnDispose();
                }
            }
            base.Initialize(BeforeInitializeReplacement);
        }

        private void OnDispose()
        {
            var target = Value;
            target.Dispose();
            Free();
            ReleaseWaitHandle(false);
        }

        private bool TakeDisposalExecution()
        {
            return _status != -1 && ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void UnDispose()
        {
            if (System.Threading.Volatile.Read(ref _status) == -1)
            {
                System.Threading.Volatile.Write(ref _status, 0);
            }
        }
    }
}

#endif