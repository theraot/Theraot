// Needed for Workaround

using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public class ReadOnlyPromise : IWaitablePromise
    {
        private readonly IPromise _promised;
        private readonly Action _wait;

        public ReadOnlyPromise(IPromise promised, bool allowWait)
        {
            _promised = promised;
            if (allowWait)
            {
                if (_promised is IWaitablePromise promise)
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
                _wait =
                () => throw new InvalidOperationException();
            }
        }

        public Exception Exception => _promised.Exception;

        public bool IsCanceled => _promised.IsCanceled;

        public bool IsCompleted => _promised.IsCompleted;

        public bool IsFaulted => _promised.IsFaulted;

        public override int GetHashCode()
        {
            return _promised.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{Promise: {_promised}}}";
        }

        public void Wait()
        {
            _wait();
        }
    }
}