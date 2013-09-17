#if FAT

using System;
using System.Collections.Generic;
using System.Reflection;
using Theraot.Collections;
using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class WeakDelegateSet : WeakSet<Delegate, WeakDelegateNeedle>
    {
        private readonly Action<object[]> _invoke;

        public WeakDelegateSet()
            : base()
        {
            _invoke = input => InvokeExtracted(input);
        }

        public WeakDelegateSet(WeakDelegateSet prototype)
            : base(prototype as IEnumerable<Delegate>)
        {
            _invoke = input => InvokeExtracted(input);
        }

        public WeakDelegateSet(WeakDelegateSet prototype, bool autoRemoveDeadItems, bool reentryGuard)
            : base(prototype as IEnumerable<Delegate>, autoRemoveDeadItems)
        {
            if (reentryGuard)
            {
                _invoke = input => InvokeExtracted(input);
            }
            else
            {
                var guard = new ReentryGuard();
                _invoke =
                (input) =>
                {
                    guard.Execute
                    (
                        () => InvokeExtracted(input)
                    );
                };
            }
        }

        public WeakDelegateSet(bool autoRemoveDeadItems, bool reentryGuard)
            : base(autoRemoveDeadItems)
        {
            if (reentryGuard)
            {
                _invoke = input => InvokeExtracted(input);
            }
            else
            {
                var guard = new ReentryGuard();
                _invoke =
                (input) =>
                {
                    guard.Execute
                    (
                        () => InvokeExtracted(input)
                    );
                };
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "By Design")]
        public bool Add(MethodInfo method, object target)
        {
            Check.NotNullArgument(method, "method");
            Wrapped.Add(new WeakDelegateNeedle(method, target));
            return true;
        }

        public new WeakDelegateSet Clone()
        {
            return new WeakDelegateSet(this);
        }

        public bool Contains(MethodInfo method, object target)
        {
            Check.NotNullArgument(method, "method");
            return Wrapped.Exists(_item => _item.Equals(method, target));
        }

        public int CountItems(MethodInfo method, object target)
        {
            Check.NotNullArgument(method, "method");
            return Wrapped.CountItemsWhere(_item => _item.Equals(method, target));
        }

        public void Invoke(params object[] args)
        {
            _invoke(args);
        }

        public bool Remove(MethodInfo method, object target)
        {
            Check.NotNullArgument(method, "method");
            foreach (var item in Wrapped.RemoveWhereEnumerable(_item => _item.Equals(method, target)))
            {
                item.Dispose();
                return true;
            }
            return false;
        }

        protected override WeakSet<Delegate, WeakDelegateNeedle> OnClone()
        {
            return Clone();
        }

        private void InvokeExtracted(object[] args)
        {
            foreach (var handler in Wrapped)
            {
                try
                {
                    handler.TryInvoke(args);
                }
                catch (NullReferenceException)
                {
                    //Empty
                }
            }
        }
    }
}

#endif