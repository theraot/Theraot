using System;
using System.Threading;

namespace Theraot.Threading
{
    public sealed class Guard : IDisposable
    {
        private int _value;

        public bool IsTaken
        {
            get
            {
                return Thread.VolatileRead(ref _value) == 1;
            }
        }

        public void Dispose()
        {
            int lastValue;
            ThreadingHelper.SpinWaitRelativeExchangeUnlessNegative(ref _value, -1, out lastValue);
        }

        public bool Enter()
        {
            var result = Interlocked.Increment(ref _value);
            if (result == 1)
            {
                return true;
            }
            return false;
        }

        public bool TryEnter()
        {
            if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
            {
                return true;
            }
            return false;
        }
    }
}