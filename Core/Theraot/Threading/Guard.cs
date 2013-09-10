using System;
using System.Threading;

namespace Theraot.Threading
{
    public sealed class Guard
    {
        private int _value;

        public bool IsTaken
        {
            get
            {
                return Thread.VolatileRead(ref _value) == 1;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "By Design")]
        public bool Enter(out IDisposable engagement)
        {
            if (Interlocked.CompareExchange(ref _value, 1, 0) == 0)
            {
                engagement = DisposableAkin.Create(Release);
                return true;
            }
            else
            {
                engagement = null;
                return false;
            }
        }

        private void Release()
        {
            Thread.VolatileWrite(ref _value, 0);
        }
    }
}