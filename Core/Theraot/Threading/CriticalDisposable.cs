#if FAT

using System;
using System.Runtime.ConstrainedExecution;

using Theraot.Core;

namespace Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed partial class CriticalDisposable : CriticalFinalizerObject
    {
        private Action _release;

        private CriticalDisposable(Action release)
        {
            _release = Check.NotNullArgument(release, "release");
        }

        public static CriticalDisposable Create(Action release)
        {
            return new CriticalDisposable(release);
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

#endif