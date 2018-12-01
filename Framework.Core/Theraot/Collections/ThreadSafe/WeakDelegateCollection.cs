// Needed for Workaround

using System;
using System.Reflection;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class WeakDelegateCollection : WeakCollection<Delegate, WeakDelegateNeedle>
    {
        private readonly Action<object[]> _invoke;
        private readonly Action<Action<Exception>, object[]> _invokeWithException;

        public WeakDelegateCollection()
        {
            _invoke = InvokeExtracted;
        }

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

        public void Add(MethodInfo method, object target)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            Add(new WeakDelegateNeedle(method, target));
        }

        public bool Contains(MethodInfo method, object target)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            return Contains(item => item.Equals(method, target));
        }

        public void Invoke(params object[] args)
        {
            _invoke(args);
        }

        public void InvokeWithException(Action<Exception> onException, params object[] args)
        {
            _invokeWithException(onException, args);
        }

        public bool Remove(MethodInfo method, object target)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            foreach (var item in RemoveWhereEnumerable(item => item.Equals(method, target)))
            {
                GC.KeepAlive(item);
                return true;
            }
            return false;
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
                    onException(exception);
                }
            }
        }
    }
}