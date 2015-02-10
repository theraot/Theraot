﻿using System;
using System.Collections.Generic;
﻿using System.Threading;
﻿using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public class WeakCollection<T, TNeedle> : ICollection<T>
        where T : class
        where TNeedle : WeakNeedle<T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly SafeDictionary<int, TNeedle> _wrapped;
        private int _maxIndex;

        private StructNeedle<WeakNeedle<EventHandler>> _eventHandler;

        public WeakCollection()
            : this(null, true)
        {
            //Empty
        }

        public WeakCollection(IEqualityComparer<T> comparer)
            : this(comparer, true)
        {
            //Empty
        }

        public WeakCollection(bool autoRemoveDeadItems)
            : this(null, autoRemoveDeadItems)
        {
            //Empty
        }

        public WeakCollection(IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
        {
            _comparer = comparer ?? EqualityComparerHelper<T>.Default;
            _wrapped = new SafeDictionary<int, TNeedle>();
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItemsExtracted();
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        public WeakCollection(IEqualityComparer<T> comparer, int initialProbing)
            : this(comparer, true, initialProbing)
        {
            //Empty
        }

        public WeakCollection(bool autoRemoveDeadItems, int initialProbing)
            : this(null, autoRemoveDeadItems, initialProbing)
        {
            //Empty
        }

        public WeakCollection(IEqualityComparer<T> comparer, bool autoRemoveDeadItems, int initialProbing)
        {
            _comparer = comparer ?? EqualityComparerHelper<T>.Default;
            _wrapped = new SafeDictionary<int, TNeedle>(initialProbing);
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItemsExtracted();
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        public WeakCollection(int initialProbing)
            : this(null, true, initialProbing)
        {
            //Empty
        }

        ~WeakCollection()
        {
            UnRegisterForAutoRemoveDeadItemsExtracted();
        }

        public bool AutoRemoveDeadItems
        {
            get
            {
                return _eventHandler.IsAlive;
            }
            set
            {
                if (value)
                {
                    RegisterForAutoRemoveDeadItems();
                }
                else
                {
                    UnRegisterForAutoRemoveDeadItems();
                }
            }
        }

        public int Count
        {
            get
            {
                return _wrapped.Count;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns false")]
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        protected SafeDictionary<int, TNeedle> Wrapped
        {
            get
            {
                return _wrapped;
            }
        }

        public void Clear()
        {
            var displaced = _wrapped.ClearEnumerable();
            foreach (var item in displaced)
            {
                item.Value.Dispose();
            }
        }

        public bool Contains(T item)
        {
            foreach (var _item in this)
            {
                if (_comparer.Equals(_item, item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public bool Equals(T x, T y)
        {
            return _comparer.Equals(x, y);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var pair in _wrapped)
            {
                T result;
                if (pair.Value.TryGetValue(out result))
                {
                    yield return result;
                }
            }
        }

        public void Add(T item)
        {
            var needle = NeedleHelper.CreateNeedle<T, TNeedle>(item);
            _wrapped.Set(Interlocked.Increment(ref _maxIndex) - 1, needle);
        }

        public bool Remove(T item)
        {
            Predicate<TNeedle> check = input =>
            {
                T _value;
                if (input.TryGetValue(out _value))
                {
                    return _comparer.Equals(item, _value);
                }
                return false;
            };
            foreach (var removed in _wrapped.RemoveWhereValueEnumerable(check))
            {
                removed.Dispose();
                return true;
            }
            return false;
        }

        public int RemoveDeadItems()
        {
            return _wrapped.RemoveWhere(input => !input.Value.IsAlive);
        }

        public int RemoveWhere(Predicate<T> valueCheck)
        {
            Predicate<TNeedle> check = input =>
            {
                T _value;
                if (input.TryGetValue(out _value))
                {
                    return valueCheck(_value);
                }
                return false;
            };
            return _wrapped.RemoveWhereValue(check);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void GarbageCollected(object sender, EventArgs e)
        {
            RemoveDeadItems();
        }

        private void RegisterForAutoRemoveDeadItems()
        {
            if (RegisterForAutoRemoveDeadItemsExtracted())
            {
                GC.ReRegisterForFinalize(this);
            }
        }

        private bool RegisterForAutoRemoveDeadItemsExtracted()
        {
            bool result = false;
            EventHandler eventHandler;
            if (ReferenceEquals(_eventHandler.Value, null))
            {
                eventHandler = GarbageCollected;
                _eventHandler = new WeakNeedle<EventHandler>(eventHandler);
                result = true;
            }
            else
            {
                eventHandler = _eventHandler.Value.Value;
                if (!_eventHandler.IsAlive)
                {
                    eventHandler = GarbageCollected;
                    _eventHandler.Value = eventHandler;
                    result = true;
                }
            }
            GCMonitor.Collected += eventHandler;
            return result;
        }

        private void UnRegisterForAutoRemoveDeadItems()
        {
            if (UnRegisterForAutoRemoveDeadItemsExtracted())
            {
                GC.SuppressFinalize(this);
            }
        }

        private bool UnRegisterForAutoRemoveDeadItemsExtracted()
        {
            EventHandler eventHandler;
            if (_eventHandler.Value.Retrieve(out eventHandler))
            {
                GCMonitor.Collected -= eventHandler;
                _eventHandler.Value = null;
                return true;
            }
            _eventHandler.Value = null;
            return false;
        }
    }
}