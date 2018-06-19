#if FAT

using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class LazyAction : Promise
    {
        [NonSerialized]
        private Thread _runnerThread;

        private Action _action;

        public LazyAction()
            : base(true)
        {
            _action = null;
        }

        public LazyAction(Action action)
            : base(false)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _action = action;
        }

        public LazyAction(Action action, bool cacheExceptions)
           : base(false)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _action = action;
            if (cacheExceptions)
            {
                _action = () =>
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception exc)
                    {
                        _action = ActionHelper.GetThrowAction(exc);
                        throw;
                    }
                };
            }
        }

        protected Thread RunnerThread
        {
            get { return _runnerThread; }
        }

        public virtual void Execute()
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            var action = Interlocked.Exchange(ref _action, null);
            if (action == null)
            {
                base.Wait();
            }
            else
            {
                ExecuteExtracted(action);
            }
        }

        public void ReleaseAction()
        {
            Volatile.Write(ref _action, null);
        }

        public override void Wait()
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait();
        }

        public override void Wait(CancellationToken cancellationToken)
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(cancellationToken);
        }

        public override void Wait(int milliseconds)
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(milliseconds);
        }

        public override void Wait(TimeSpan timeout)
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(timeout);
        }

        public override void Wait(int milliseconds, CancellationToken cancellationToken)
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(milliseconds, cancellationToken);
        }

        public override void Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(timeout, cancellationToken);
        }

        protected virtual void Execute(Action beforeInitialize)
        {
            if (beforeInitialize == null)
            {
                throw new ArgumentNullException("beforeInitialize");
            }
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            var valueFactory = Interlocked.Exchange(ref _action, null);
            if (valueFactory == null)
            {
                base.Wait();
            }
            else
            {
                try
                {
                    beforeInitialize.Invoke();
                }
                finally
                {
                    ExecuteExtracted(valueFactory);
                }
            }
        }

        private void ExecuteExtracted(Action action)
        {
            _runnerThread = Thread.CurrentThread;
            try
            {
                action.Invoke();
            }
            catch (Exception exception)
            {
                Interlocked.CompareExchange(ref _action, action, null);
                SetError(exception);
                throw;
            }
            finally
            {
                _runnerThread = null;
            }
        }
    }
}

#endif