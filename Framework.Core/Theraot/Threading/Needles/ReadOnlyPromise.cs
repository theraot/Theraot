// Needed for Workaround

using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public class ReadOnlyPromise : IWaitablePromise
    {
        private readonly IWaitablePromise _promised;

        public ReadOnlyPromise(IWaitablePromise promised)
        {
            _promised = promised;
        }

        public bool IsCompleted => _promised.IsCompleted;

        public void OnCompleted(Action continuation)
        {
            _promised.OnCompleted(continuation);
        }

        public void Wait()
        {
            _promised.Wait();
        }

        public override int GetHashCode()
        {
            return _promised.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{Promise: {_promised}}}";
        }
    }
}