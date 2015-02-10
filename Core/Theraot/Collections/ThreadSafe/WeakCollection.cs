﻿using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using Theraot.Core;
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
        private readonly SafeSet<TNeedle> _wrapped;

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
            IEqualityComparer<TNeedle> needleComparer = new NeedleConversionEqualityComparer<TNeedle, T>(_comparer);
            _wrapped = new SafeSet<TNeedle>(needleComparer);
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
            var defaultComparer = EqualityComparerHelper<T>.Default;
            IEqualityComparer<TNeedle> needleComparer;
            if (ReferenceEquals(comparer, null) || ReferenceEquals(comparer, defaultComparer))
            {
                _comparer = defaultComparer;
                needleComparer = EqualityComparerHelper<TNeedle>.Default;
            }
            else
            {
                _comparer = comparer;
                needleComparer = new NeedleConversionEqualityComparer<TNeedle, T>(_comparer);
            }
            _wrapped = new SafeSet<TNeedle>(needleComparer, initialProbing);
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

        protected SafeSet<TNeedle> Wrapped
        {
            get
            {
                return _wrapped;
            }
        }

        public bool Add(T item)
        {
            var needle = NeedleHelper.CreateNeedle<T, TNeedle>(item);
            if (_wrapped.TryAdd(needle, input => !input.IsAlive))
            {
                return true;
            }
            needle.Dispose();
            return false;
        }

        public int AddRange(IEnumerable<T> items)
        {
            int count = 0;
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                if (Add(item))
                {
                    count++;
                }
            }
            return count;
        }

        public void Clear()
        {
            var displaced = _wrapped.ClearEnumerable();
            foreach (var item in displaced)
            {
                item.Dispose();
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

        public void ExceptWith(IEnumerable<T> other)
        {
            Extensions.ExceptWith(this, other);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.ConvertProgressiveFiltered(input => input.Value, input => input.IsAlive).GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            TNeedle needle = NeedleHelper.CreateNeedle<T, TNeedle>(item);
            if (!_wrapped.TryAdd(needle))
            {
                needle.Dispose();
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            Extensions.IntersectWith(this, other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return Extensions.IsProperSubsetOf(this, other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Extensions.IsProperSupersetOf(this, other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return Extensions.IsSubsetOf(this, other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return Extensions.IsSupersetOf(this, other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return Extensions.Overlaps(this, other);
        }

        public bool Remove(T item)
        {
            foreach (var removed in _wrapped.RemoveWhereEnumerable(value => _comparer.Equals(item, value.Value)))
            {
                removed.Dispose();
                return true;
            }
            return false;
        }

        public int RemoveDeadItems()
        {
            return _wrapped.RemoveWhere(input => !input.IsAlive);
        }

        public int RemoveWhere(Predicate<T> predicate)
        {
            return _wrapped.RemoveWhere(input => input.IsAlive && predicate(input.Value));
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