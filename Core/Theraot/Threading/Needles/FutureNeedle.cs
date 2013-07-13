using System;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class FutureNeedle<T> : LazyNeedle<T>
    {
        public FutureNeedle(Func<T> load)
            : base(load)
        {
            var waitCallback = new System.Threading.WaitCallback
            (
                _ =>
                {
                    Initialize();
                }
            );
            System.Threading.ThreadPool.QueueUserWorkItem(waitCallback);
        }
    }
}