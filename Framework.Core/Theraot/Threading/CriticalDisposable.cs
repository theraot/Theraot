#if FAT
using System;

using Theraot.Core;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed partial class CriticalDisposable
#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2
        : System.Runtime.ConstrainedExecution.CriticalFinalizerObject
#endif
    {
        private Action _release;

        private CriticalDisposable(Action release)
        {
            _release = release ?? throw new ArgumentNullException(nameof(release));
        }

        public static CriticalDisposable Create(Action release)
        {
            return new CriticalDisposable(release);
        }

        public bool Dispose(Func<bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
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
                           return false;
                       }
                   );
        }
    }
}

#endif