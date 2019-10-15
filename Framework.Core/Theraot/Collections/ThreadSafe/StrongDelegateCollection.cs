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
        private readonly Action<object?[]> _invoke;
        private readonly Action<object?[]> _invokeAndClear;
        private readonly Action<Action<Exception>?, object?[]> _invokeAndClearWithException;
        private readonly Action<Action<Exception>?, object?[]> _invokeWithException;
        private readonly ThreadSafeCollection<Delegate> _wrapped;

        public StrongDelegateCollection(bool freeReentry)
        {
            IEqualityComparer<Delegate> comparer = EqualityComparer<Delegate>.Default;
            _wrapped = new ThreadSafeCollection<Delegate>(comparer);
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

        public void Invoke(DelegateCollectionInvokeOptions options, params object?[] args)
        {
            if ((options & DelegateCollectionInvokeOptions.RemoveDelegates) != DelegateCollectionInvokeOptions.None)
            {
                _invokeAndClear(args);
            }
            else
            {
                _invoke(args);
            }
        }

        public void Invoke(Action<Exception>? onException, DelegateCollectionInvokeOptions options, params object?[] args)
        {
            if ((options & DelegateCollectionInvokeOptions.RemoveDelegates) != DelegateCollectionInvokeOptions.None)
            {
                _invokeAndClearWithException(onException, args);
            }
            else
            {
                _invokeWithException(onException, args);
            }
        }

        private void InvokeExtracted(object?[] args)
        {
            foreach (var handler in _wrapped)
            {
                handler.DynamicInvoke(args);
            }
        }

        private void InvokeAndClearExtracted(object?[] args)
        {
            foreach (var handler in _wrapped.ClearEnumerable())
            {
                handler.DynamicInvoke(args);
            }
        }

        private void InvokeExtracted(Action<Exception>? onException, object?[] args)
        {
            foreach (var handler in _wrapped)
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

        private void InvokeAndClearExtracted(Action<Exception>? onException, object?[] args)
        {
            foreach (var handler in _wrapped.ClearEnumerable())
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