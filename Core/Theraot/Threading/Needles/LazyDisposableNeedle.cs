#if FAT

using System;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class LazyDisposableNeedle<T> : LazyNeedle<T>
        where T : IDisposable
    {
        private int _status;

        public LazyDisposableNeedle(Func<T> valueFactory)
            : base(valueFactory)
        {
            //Empty
        }

        public LazyDisposableNeedle(T target)
            : base(target)
        {
            //Empty
        }

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
            get { return _status == -1; }
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

        public override void Initialize()
        {
            Initialize(() => UnDispose());
        }

        public void Reinitialize()
        {
            OnDispose();
            Initialize();
        }

        [System.Diagnostics.DebuggerNonUserCode]
        internal void Dispose(bool disposeManagedResources)
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
                GC.KeepAlive(exception);
            }
        }

        protected override void Initialize(Action beforeInitialize)
        {
            Action beforeInitializeReplacement = () =>
                       {
                           try
                           {
                               var waitHandle = WaitHandle.Value;
                               if (!WaitHandle.IsAlive)
                               {
                                   WaitHandle.Value = new System.Threading.ManualResetEventSlim(false);
                                   GC.KeepAlive(waitHandle);
                               }
                               if (beforeInitialize != null)
                               {
                                   beforeInitialize.Invoke();
                               }
                           }
                           finally
                           {
                               UnDispose();
                           }
                       };
            base.Initialize(beforeInitializeReplacement);
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
            if (_status == -1)
            {
                return false;
            }
            return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private bool UnDispose()
        {
            if (System.Threading.Volatile.Read(ref _status) == -1)
            {
                System.Threading.Volatile.Write(ref _status, 0);
                return true;
            }
            return false;
        }
    }
}

#endif