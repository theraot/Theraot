using System;
using System.Threading;

namespace Theraot.Threading
{
    [System.Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class SingleTimeExecution
    {
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
            _status = alreadyExecuted ? -1 : 0;
        }

        public bool Executed
        {
            get
            {
                return _status == -1;
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
            else if (_status != -1)
            {
                ThreadingHelper.SpinWait(() => _status == -1 || OnExecute(work));
                return _status != -1;
            }
            else
            {
                return false;
            }
        }

        public T ExecutedConditional<T>(Func<T> whenExecuted, Func<T> whenNotExecuted)
        {
            if (_status == -1)
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
                    if (_status == -1)
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
            if (_status == -1)
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
                    if (_status == -1)
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
            if (Thread.VolatileRead(ref _status) == -1)
            {
                SafeInvoke(beforeReset);
                Thread.VolatileWrite(ref _status, 0);
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
            else if (_status == -1)
            {
                prevention = null;
                return false;
            }
            else
            {
                PreventExecution();
                if (_status == -1)
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
            if (Interlocked.CompareExchange(ref _status, -1, 0) == 0)
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
            if (Interlocked.CompareExchange(ref _status, -1, 0) == 0)
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
            ThreadingHelper.SpinWait
            (
                () =>
                {
                    var status = _status;
                    if (status != -1)
                    {
                        if (Interlocked.CompareExchange(ref _status, _status++, status) == status)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            );
        }
    }
}