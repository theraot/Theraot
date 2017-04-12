// Needed for NET40

using System;

using Theraot.Core;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed partial class Disposable
    {
        private Action _release;

        private Disposable(Action release)
        {
            if (release == null)
            {
                throw new ArgumentNullException("release");
            }
            _release = release;
        }

        public static Disposable Create()
        {
            return new Disposable(ActionHelper.GetNoopAction());
        }

        public static Disposable Create(Action release)
        {
            return new Disposable(release);
        }

        public bool Dispose(Func<bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            return DisposedConditional
                   (
                       FuncHelper.GetFallacyFunc(),
                       () =>
                       {
                           if (condition.Invoke())
                           {
                               Dispose();
                               return true;
                           }
                           else
                           {
                               return false;
                           }
                       }
                   );
        }
    }
}