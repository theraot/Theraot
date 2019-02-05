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
        private readonly Action<object[]> _invokeAndClear;
        private readonly Action<Action<Exception>, object[]> _invokeAndClearWithException;
        private readonly Action<Action<Exception>, object[]> _invokeWithException;

        public WeakDelegateCollection(bool autoRemoveDeadItems, bool freeReentry)
            : base(autoRemoveDeadItems)
        {
            if (freeReentry)
            {
                _invoke = InvokeExtracted;
                _invokeAndClear = InvokeAndClearExtracted;
                _invokeWithException = InvokeExtracted;
                _invokeAndClearWithException = InvokeAndClearExtracted;
            }
            else
            {
                var guard = new ReentryGuard();
                _invoke = input => guard.Execute(() => InvokeExtracted(input));
                _invokeAndClear = input => guard.Execute(() => InvokeAndClearExtracted(input));
                _invokeWithException = (onException, input) => guard.Execute(() => InvokeExtracted(onException, input));
                _invokeAndClearWithException = (onException, input) => guard.Execute(() => InvokeAndClearExtracted(onException, input));
            }
        }

        public void Invoke(params object[] args)
        {
            _invoke(args);
        }

        public void InvokeAndClear(params object[] args)
        {
            _invokeAndClear(args);
        }

        public void InvokeWithException(Action<Exception> onException, params object[] args)
        {
            _invokeWithException(onException, args);
        }

        public void InvokeAndClearWithException(Action<Exception> onException, params object[] args)
        {
            _invokeAndClearWithException(onException, args);
        }

        private void InvokeExtracted(object[] args)
        {
            foreach (var handler in this)
            {
                handler.DynamicInvoke(args);
            }
        }

        private void InvokeAndClearExtracted(object[] args)
        {
            foreach (var handler in ClearEnumerable())
            {
                handler.DynamicInvoke(args);
            }
        }

        private void InvokeExtracted(Action<Exception> onException, object[] args)
        {
            foreach (var handler in this)
            {
                try
                {
                    handler.DynamicInvoke(args);
                }
                catch (Exception exception)
                {
                    onException?.Invoke(exception);
                }
            }
        }

        private void InvokeAndClearExtracted(Action<Exception> onException, object[] args)
        {
            foreach (var handler in ClearEnumerable())
            {
                try
                {
                    handler.DynamicInvoke(args);
                }
                catch (Exception exception)
                {
                    onException?.Invoke(exception);
                }
            }
        }
    }
}