// Needed for NET20

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
    [DebuggerNonUserCode]
    public sealed class StrongDelegateCollection : ICollection<Delegate>
    {
        private readonly Action<object[]> _invoke;
        private readonly Action<Action<Exception>, object[]> _invokeWithException;
        private readonly SafeCollection<Delegate> _wrapped;

        public StrongDelegateCollection(bool freeReentry)
        {
            IEqualityComparer<Delegate> comparer = EqualityComparer<Delegate>.Default;
            _wrapped = new SafeCollection<Delegate>(comparer);
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

        public int Count => _wrapped.Count;

        bool ICollection<Delegate>.IsReadOnly => false;

        public void Add(Delegate item)
        {
            if (item != null)
            {
                _wrapped.Add(item);
            }
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public bool Contains(Delegate item)
        {
            return _wrapped.Contains(item);
        }

        public void CopyTo(Delegate[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Delegate> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(Delegate item)
        {
            return _wrapped.Remove(item);
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
            foreach (var handler in _wrapped)
            {
                handler.DynamicInvoke(args);
            }
        }

        private void InvokeExtracted(Action<Exception> onException, object[] args)
        {
            foreach (var handler in _wrapped)
            {
                try
                {
                    handler.DynamicInvoke(args);
                }
                catch (Exception exception)
                {
                    onException(exception);
                }
            }
        }
    }
}