// Needed for Workaround

using System;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class ReadOnlyPromise : IWaitablePromise
    {
        private readonly IPromise _promised;
        private readonly Action _wait;

        public ReadOnlyPromise(IPromise promised, bool allowWait)
        {
            _promised = promised;
            if (allowWait)
            {
                var promise = _promised as IWaitablePromise;
                if (promise != null)
                {
                    _wait = promise.Wait;
                }
                else
                {
                    _wait = () => ThreadingHelper.SpinWaitUntil(() => _promised.IsCompleted);
                }
            }
            else
            {
                _wait = () =>
                {
                    throw new InvalidOperationException();
                };
            }
        }

        public Exception Exception
        {
            get { return _promised.Exception; }
        }

        public bool IsCanceled
        {
            get { return _promised.IsCanceled; }
        }

        public bool IsCompleted
        {
            get { return _promised.IsCompleted; }
        }

        public bool IsFaulted
        {
            get { return _promised.IsFaulted; }
        }

        public override int GetHashCode()
        {
            return _promised.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{{Promise: {0}}}", _promised);
        }

        public void Wait()
        {
            _wait();
        }
    }
}