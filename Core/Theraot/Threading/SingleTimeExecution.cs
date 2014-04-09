#if FAT

using System;
using System.Threading;

namespace Theraot.Threading
{
    [System.Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class SingleTimeExecution
    {
        private const int INT_StatusCompleted = -1;
        private const int INT_StatusFree = 0;
        private const int INT_StatusPrevented = 1;

        [NonSerialized]
        private Thread _executionThread;

        private int _status;

        public SingleTimeExecution()
            : this(false)
        {
            //Empty
        }

        public SingleTimeExecution(bool alreadyExecuted)
        {
            _status = alreadyExecuted ? INT_StatusCompleted : INT_StatusFree;
        }

        public bool Executed
        {
            get
            {
                return Thread.VolatileRead(ref _status) == INT_StatusCompleted;
            }
        }

        public bool IsCurrentThreadExecuting
        {
            get
            {
                return ReferenceEquals(Thread.CurrentThread, _executionThread);
            }
        }

        protected Thread ExecutionThread
        {
            get
            {
                return _executionThread;
            }
        }

        public virtual bool Execute(Action work)
        {
            if (ReferenceEquals(Thread.CurrentThread, _executionThread))
            {
                SafeInvoke(work);
                return true;
            }
            else if (Thread.VolatileRead(ref _status) != INT_StatusCompleted)
            {
                int count = 0;
            retry:
                if (OnExecute(work))
                {
                    return true;
                }
                else if (Thread.VolatileRead(ref _status) == INT_StatusCompleted)
                {
                    return false;
                }
                else
                {
                    ThreadingHelper.SpinOnce(ref count);
                    goto retry;
                }
            }
            else
            {
                return false;
            }
        }

        public T ExecutedConditional<T>(Func<T> whenExecuted, Func<T> whenNotExecuted)
        {
            if (Thread.VolatileRead(ref _status) == INT_StatusCompleted)
            {
                if (whenExecuted == null)
                {
                    return default(T);
                }
                else
                {
                    return whenExecuted.Invoke();
                }
            }
            else
            {
                if (whenNotExecuted == null)
                {
                    return default(T);
                }
                else
                {
                    PreventExecution();
                    if (Thread.VolatileRead(ref _status) == INT_StatusCompleted)
                    {
                        if (whenExecuted == null)
                        {
                            return default(T);
                        }
                        else
                        {
                            return whenNotExecuted.Invoke();
                        }
                    }
                    else
                    {
                        try
                        {
                            return whenNotExecuted.Invoke();
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _status);
                        }
                    }
                }
            }
        }

        public void ExecutedConditional(Action whenExecuted, Action whenNotExecuted)
        {
            if (Thread.VolatileRead(ref _status) == INT_StatusCompleted)
            {
                if (whenExecuted != null)
                {
                    whenExecuted.Invoke();
                }
            }
            else
            {
                if (whenNotExecuted != null)
                {
                    PreventExecution();
                    if (Thread.VolatileRead(ref _status) == INT_StatusCompleted)
                    {
                        if (whenExecuted != null)
                        {
                            whenNotExecuted.Invoke();
                        }
                    }
                    else
                    {
                        try
                        {
                            whenNotExecuted.Invoke();
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _status);
                        }
                    }
                }
            }
        }

        public bool Reset()
        {
            return Reset(null);
        }

        public virtual bool Reset(Action beforeReset)
        {
            if (Thread.VolatileRead(ref _status) == INT_StatusCompleted)
            {
                SafeInvoke(beforeReset);
                Thread.VolatileWrite(ref _status, INT_StatusFree);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryPreventExecution(out IDisposable prevention)
        {
            if (ReferenceEquals(ExecutionThread, Thread.CurrentThread))
            {
                prevention = null;
                return false;
            }
            else if (Thread.VolatileRead(ref _status) == INT_StatusCompleted)
            {
                prevention = null;
                return false;
            }
            else
            {
                PreventExecution();
                if (Thread.VolatileRead(ref _status) == INT_StatusCompleted)
                {
                    prevention = null;
                    return false;
                }
                else
                {
                    prevention = DisposableAkin.Create
                                 (
                                     () => Interlocked.Decrement(ref _status)
                                 );
                    return true;
                }
            }
        }

        public virtual bool TryRetrieveExecution(out IDisposable execution)
        {
            if (ReferenceEquals(Thread.CurrentThread, _executionThread))
            {
                execution = CreateExecution();
                return true;
            }
            else
            {
                return OnTryRetrieveExecution(out execution);
            }
        }

        protected static void SafeInvoke(Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        protected static T SafeInvoke<T>(Func<T> action)
        {
            if (action != null)
            {
                return action();
            }
            else
            {
                return default(T);
            }
        }

        protected IDisposable CreateExecution()
        {
            return DisposableAkin.Create(EndExecution);
        }

        protected void EndExecution()
        {
            _executionThread = null;
        }

        protected bool OnExecute(Action work)
        {
            if (Interlocked.CompareExchange(ref _status, INT_StatusCompleted, INT_StatusFree) == INT_StatusFree)
            {
                try
                {
                    _executionThread = Thread.CurrentThread;
                    SafeInvoke(work);
                    return true;
                }
                finally
                {
                    _executionThread = null;
                }
            }
            else
            {
                return false;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "By Design")]
        protected bool OnTryRetrieveExecution(out IDisposable execution)
        {
            _executionThread = Thread.CurrentThread;
            if (Interlocked.CompareExchange(ref _status, INT_StatusCompleted, INT_StatusFree) == INT_StatusFree)
            {
                execution = CreateExecution();
                return true;
            }
            else
            {
                execution = null;
                return false;
            }
        }

        private void PreventExecution()
        {
            ThreadingHelper.SpinWaitRelativeSetUnless(ref _status, INT_StatusPrevented, INT_StatusCompleted);
        }
    }
}

#endif