// Needed for Workaround

using System;
using System.Diagnostics;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [DebuggerNonUserCode]
    public sealed class WeakDelegateCollection : WeakCollection<Delegate, WeakDelegateNeedle>
    {
        private readonly Action<object[]> _invoke;
        private readonly Action<Action<Exception>, object[]> _invokeWithException;

        public WeakDelegateCollection(bool autoRemoveDeadItems, bool freeReentry)
            : base(autoRemoveDeadItems)
        {
            if (freeReentry)
            {
                _invoke = InvokeExtracted;
                _invokeWithException = InvokeExtracted;
            }
            else
            {
                var guard = new ReentryGuard();
                _invoke = input => guard.Execute(() => InvokeExtracted(input));
                _invokeWithException = (onException, input) => guard.Execute(() => InvokeExtracted(onException, input));
            }
        }

        public void Invoke(params object[] args)
        {
            _invoke(args);
        }

        public void InvokeWithException(Action<Exception> onException, params object[] args)
        {
            _invokeWithException(onException, args);
        }

        private void InvokeExtracted(object[] args)
        {
            foreach (var handler in GetNeedleEnumerable())
            {
                handler.TryInvoke(args);
            }
        }

        private void InvokeExtracted(Action<Exception> onException, object[] args)
        {
            foreach (var handler in GetNeedleEnumerable())
            {
                try
                {
                    handler.TryInvoke(args);
                }
                catch (Exception exception)
                {
                    onException?.Invoke(exception);
                }
            }
        }
    }
}