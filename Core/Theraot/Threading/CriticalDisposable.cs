#if FAT

using System;

using Theraot.Core;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed partial class CriticalDisposable
#if !NETCOREAPP1_1
        : System.Runtime.ConstrainedExecution.CriticalFinalizerObject
#endif
    {
        private Action _release;

        private CriticalDisposable(Action release)
        {
            if (release == null)
            {
                throw new ArgumentNullException("release");
            }
            _release = release;
        }

        public static CriticalDisposable Create(Action release)
        {
            return new CriticalDisposable(release);
        }

        public bool Dispose(Func<bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            Func<bool> temp = condition;
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