// Needed for Workaround

#pragma warning disable RCS1231 // Make parameter ref read-only.

using System;
using System.Diagnostics;
using System.Threading;

using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public class Promise : IWaitablePromise
    {
        private readonly int _hashCode;

        private readonly StrongDelegateCollection _onCompleted;

        private StructNeedle<ManualResetEventSlim> _waitHandle;

        public Promise(bool done)
        {
            Exception = null;
            _hashCode = base.GetHashCode();
            if (!done)
            {
                _waitHandle = new ManualResetEventSlim(false);
            }

            _onCompleted = new StrongDelegateCollection(true);
        }

        public Promise(Exception exception)
        {
            Exception = exception;
            _hashCode = exception.GetHashCode();
            _waitHandle = new ManualResetEventSlim(true);
            _onCompleted = new StrongDelegateCollection(true);
        }

        ~Promise()
        {
            ReleaseWaitHandle(false);
        }

        public Exception Exception { get; private set; }

        public bool IsCompleted
        {
            get
            {
                var waitHandle = _waitHandle.Value;
                return waitHandle?.IsSet != false;
            }
        }

        public bool IsFaulted => Exception != null;

        protected IRecyclableNeedle<ManualResetEventSlim> WaitHandle => _waitHandle;

        public virtual void Free()
        {
            var waitHandle = _waitHandle.Value;
            if (waitHandle == null)
            {
                _waitHandle.Value = new ManualResetEventSlim(false);
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

            var waitHandle = _waitHandle.Value;
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
                        _waitHandle.Value = new ManualResetEventSlim(false);
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
            ReleaseWaitHandle(true);
        }

        public void SetError(Exception error)
        {
            Exception = error;
            ReleaseWaitHandle(true);
        }

        public override string ToString()
        {
            return IsCompleted ? Exception == null ? "[Done]" : Exception.ToString() : "[Not Created]";
        }

        public virtual void Wait()
        {
            var waitHandle = _waitHandle.Value;
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
                No.Op(exception);
            }
        }

        public virtual void Wait(CancellationToken cancellationToken)
        {
            var waitHandle = _waitHandle.Value;
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
                No.Op(exception);
            }
        }

        public virtual void Wait(int milliseconds)
        {
            var waitHandle = _waitHandle.Value;
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
                No.Op(exception);
            }
        }

        public virtual void Wait(TimeSpan timeout)
        {
            var waitHandle = _waitHandle.Value;
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
                No.Op(exception);
            }
        }

        public virtual void Wait(int milliseconds, CancellationToken cancellationToken)
        {
            var waitHandle = _waitHandle.Value;
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
                No.Op(exception);
            }
        }

        public virtual void Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var waitHandle = _waitHandle.Value;
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
                No.Op(exception);
            }
        }

        protected void ReleaseWaitHandle(bool done)
        {
            var waitHandle = _waitHandle.Value;
            if (waitHandle != null)
            {
                if (done)
                {
                    waitHandle.Set();
                }

                waitHandle.Dispose();
            }

            _onCompleted.Invoke(null, DelegateCollectionInvokeOptions.RemoveDelegates);
            _waitHandle.Value = null;
        }
    }
}