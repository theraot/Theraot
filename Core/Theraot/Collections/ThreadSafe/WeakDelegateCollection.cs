using System;
using System.Reflection;
using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class WeakDelegateCollection : WeakCollection<Delegate, WeakDelegateNeedle>
    {
        private readonly Action<object[]> _invoke;

        public WeakDelegateCollection()
        {
            _invoke = InvokeExtracted;
        }

        public WeakDelegateCollection(bool autoRemoveDeadItems, bool reentryGuard)
            : base(autoRemoveDeadItems)
        {
            if (reentryGuard)
            {
                _invoke = InvokeExtracted;
            }
            else
            {
                var guard = new ReentryGuard();
                _invoke = input => guard.Execute(() => InvokeExtracted(input));
            }
        }

        public WeakDelegateCollection(bool autoRemoveDeadItems, bool reentryGuard, int maxProbing)
            : base(autoRemoveDeadItems, maxProbing)
        {
            if (reentryGuard)
            {
                _invoke = InvokeExtracted;
            }
            else
            {
                var guard = new ReentryGuard();
                _invoke = input => guard.Execute(() => InvokeExtracted(input));
            }
        }

        public WeakDelegateCollection(int maxProbing)
            : base(maxProbing)
        {
            _invoke = InvokeExtracted;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "By Design")]
        public void Add(MethodInfo method, object target)
        {
            Check.NotNullArgument(method, "method");
            Add(new WeakDelegateNeedle(method, target)); // Since it is a new object, it should not fail
        }

        public bool Contains(MethodInfo method, object target)
        {
            Check.NotNullArgument(method, "method");
            return Contains(item => item.Equals(method, target));
        }

        public void Invoke(params object[] args)
        {
            _invoke(args);
        }

        public bool Remove(MethodInfo method, object target)
        {
            Check.NotNullArgument(method, "method");
            foreach (var item in RemoveWhereEnumerable(_item => _item.Equals(method, target)))
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
                try
                {
                    handler.TryInvoke(args);
                }
                catch (NullReferenceException)
                {
                    // Empty
                }
            }
        }
    }
}