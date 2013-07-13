using System;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public partial class LazyDisposableNeedle<T> : LazyNeedle<T>, ICacheNeedle<T>
        where T : IDisposable
    {
        public LazyDisposableNeedle(Func<T> function)
            : base(function)
        {
            //Empty
        }

        public LazyDisposableNeedle(Func<T> function, T target)
            : base(function, target)
        {
            //Empty
        }

        public override void Initialize()
        {
            Initialize(() => UnDispose());
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
            SetTarget(default(T));
        }
    }
}