#if FAT

using System;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public partial class LazyDisposableNeedle<T> : LazyNeedle<T>
        where T : IDisposable
    {
        public LazyDisposableNeedle(Func<T> valueFactory)
            : base(valueFactory)
        {
            //Empty
        }

        public LazyDisposableNeedle(T target)
            : base(target)
        {
            //Empty
        }

        public override void Initialize()
        {
            Initialize(() => UnDispose());
        }

        public void Reinitialize()
        {
            OnDispose();
            Initialize();
        }

        protected override void Initialize(Action beforeInitialize)
        {
            base.Initialize
                (
                    () =>
                    {
                        try
                        {
                            var waitHandle = WaitHandle.Value;
                            if (!WaitHandle.IsAlive)
                            {
                                WaitHandle.Value = new System.Threading.ManualResetEventSlim(false);
                                GC.KeepAlive(waitHandle);
                            }
                            beforeInitialize.SafeInvoke();
                        }
                        finally
                        {
                            UnDispose();
                        }
                    }
                );
        }

        private void OnDispose()
        {
            var target = Value;
            target.Dispose();
            Free();
            ReleaseWaitHandle(false);
        }
    }
}

#endif