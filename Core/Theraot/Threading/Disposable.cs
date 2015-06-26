// Needed for NET40

using System;

using Theraot.Core;

namespace Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed partial class Disposable
    {
        private Action _release;

        private Disposable(Action release)
        {
            _release = Check.NotNullArgument(release, "release");
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
            Check.NotNullArgument(condition, "condition");
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