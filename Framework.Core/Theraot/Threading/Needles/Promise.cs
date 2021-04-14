// Needed for Workaround

using System;
using System.Diagnostics;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public class Promise : IWaitablePromise, IRecyclable
    {
        private readonly int _hashCode;

        private readonly StrongDelegateCollection _onCompleted;

        public Promise(bool done)
        {
            Exception = null;
            _hashCode = base.GetHashCode();
            if (!done)
            {
                WaitHandle = new ManualResetEventSlim(initialState: false);
            }

            _onCompleted = new StrongDelegateCollection(freeReentry: true);
        }

        public Promise(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            _hashCode = Exception.GetHashCode();
            WaitHandle = new ManualResetEventSlim(initialState: true);
            _onCompleted = new StrongDelegateCollection(freeReentry: true);
        }

        ~Promise()
        {
            ReleaseWaitHandle(done: false);
        }

        public Exception? Exception { get; private set; }

        public bool IsCompleted
        {
            get
            {
                var waitHandle = WaitHandle;
                return waitHandle?.IsSet != false;
            }
        }

        public bool IsFaulted => Exception != null;

        protected ManualResetEventSlim? WaitHandle
        {
            get;
            private set;
        }

        public virtual void Free()
        {
            var waitHandle = WaitHandle;
            if (waitHandle == null)
            {
                WaitHandle = new ManualResetEventSlim(initialState: false);
            }
            else
            {
                waitHandle.Reset();
            }

            Exception = null;
        }

        public virtual void Free(Action beforeFree)
        {
            if (beforeFree == null)
            {
                throw new ArgumentNullException(nameof(beforeFree));
            }

            var waitHandle = WaitHandle;
            if (waitHandle?.IsSet != false)
            {
                try
                {
                    beforeFree();
                }
                finally
                {
                    if (waitHandle == null)
                    {
                        WaitHandle = new ManualResetEventSlim(initialState: false);
                    }
                    else
                    {
                        waitHandle.Reset();
                    }

                    Exception = null;
                }
            }
            else
            {
                waitHandle.Reset();
                Exception = null;
            }
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public void OnCompleted(Action continuation)
        {
            _onCompleted.Add(continuation);
        }

        public void SetCompleted()
        {
            Exception = null;
            ReleaseWaitHandle(done: true);
        }

        public void SetError(Exception error)
        {
            Exception = error;
            ReleaseWaitHandle(done: true);
        }

        public override string ToString()
        {
            return IsCompleted ? Exception?.ToString() ?? "[Done]" : "[Not Created]";
        }

        public virtual void Wait()
        {
            var waitHandle = WaitHandle;
            if (waitHandle == null)
            {
                return;
            }

            try
            {
                waitHandle.Wait();
            }
            catch (ObjectDisposedException exception)
            {
                // Came late to the party, initialization was done
                _ = exception;
            }
        }

        public virtual void Wait(CancellationToken cancellationToken)
        {
            var waitHandle = WaitHandle;
            if (waitHandle == null)
            {
                return;
            }

            try
            {
                waitHandle.Wait(cancellationToken);
            }
            catch (ObjectDisposedException exception)
            {
                // Came late to the party, initialization was done
                _ = exception;
            }
        }

        public virtual void Wait(int milliseconds)
        {
            var waitHandle = WaitHandle;
            if (waitHandle == null)
            {
                return;
            }

            try
            {
                waitHandle.Wait(milliseconds);
            }
            catch (ObjectDisposedException exception)
            {
                // Came late to the party, initialization was done
                _ = exception;
            }
        }

        public virtual void Wait(TimeSpan timeout)
        {
            var waitHandle = WaitHandle;
            if (waitHandle == null)
            {
                return;
            }

            try
            {
                waitHandle.Wait(timeout);
            }
            catch (ObjectDisposedException exception)
            {
                // Came late to the party, initialization was done
                _ = exception;
            }
        }

        public virtual void Wait(int milliseconds, CancellationToken cancellationToken)
        {
            var waitHandle = WaitHandle;
            if (waitHandle == null)
            {
                return;
            }

            try
            {
                waitHandle.Wait(milliseconds, cancellationToken);
            }
            catch (ObjectDisposedException exception)
            {
                // Came late to the party, initialization was done
                _ = exception;
            }
        }

        public virtual void Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var waitHandle = WaitHandle;
            if (waitHandle == null)
            {
                return;
            }

            try
            {
                waitHandle.Wait(timeout, cancellationToken);
            }
            catch (ObjectDisposedException exception)
            {
                // Came late to the party, initialization was done
                _ = exception;
            }
        }

        protected void ReleaseWaitHandle(bool done)
        {
            var waitHandle = WaitHandle;
            if (waitHandle != null)
            {
                if (done)
                {
                    waitHandle.Set();
                }

                waitHandle.Dispose();
            }

            _onCompleted.Invoke(onException: null, DelegateCollectionInvokeOptions.RemoveDelegates);
            WaitHandle = null;
        }
    }
}