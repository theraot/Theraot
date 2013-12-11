using System;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class FutureDisposableNeedle<T> : LazyDisposableNeedle<T>
        where T : IDisposable
    {
        public FutureDisposableNeedle(Func<T> load)
            : base(load)
        {
            var waitCallback = new System.Threading.WaitCallback(_ => Initialize());
            System.Threading.ThreadPool.QueueUserWorkItem(waitCallback);
        }
    }
}