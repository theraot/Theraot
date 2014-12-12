#if FAT

using System;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public partial class LazyDisposableNeedle<T> : LazyNeedle<T>
        where T : IDisposable
    {
        public LazyDisposableNeedle(Func<T> function)
            : base(function)
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
            base.Initialize(() => UnDispose());
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "False Positive")]
        public void Reinitialze()
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
            var waitHandle = WaitHandle.Value;
            if (WaitHandle.IsAlive)
            {
                waitHandle.Dispose();
            }
            WaitHandle.Free();
            SetTargetValue(default(T));
        }
    }
}

#endif