// Needed for NET40

using System;
using System.Diagnostics;
using Theraot.Core;

namespace Theraot.Threading
{
    [DebuggerNonUserCode]
    public sealed partial class Disposable
    {
        private Action _release;

        private Disposable(Action release)
        {
            _release = release ?? throw new ArgumentNullException(nameof(release));
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