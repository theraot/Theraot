#if FAT

using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public class WeakSetBucket<T, TNeedle> : ICollection<T>, IEnumerable<T>, ISet<T>, IEqualityComparer<T>
        where T : class
        where TNeedle : WeakNeedle<T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly SetBucket<TNeedle> _wrapped;

        private StructNeedle<WeakNeedle<EventHandler>> _eventHandler;

        public WeakSetBucket()
            : this(null as IEqualityComparer<T>, true)
        {
            //Empty
        }

        public WeakSetBucket(T[] prototype)
            : this(Check.NotNullArgument(prototype, "prototype").Length, null as IEqualityComparer<T>, true)
        {
            this.AddRange(prototype);
        }

        public WeakSetBucket(T[] prototype, IEqualityComparer<T> comparer)
            : this(Check.NotNullArgument(prototype, "prototype").Length, comparer, true)
        {
            this.AddRange(prototype);
        }

        public WeakSetBucket(T[] prototype, bool autoRemoveDeadItems)
            : this(Check.NotNullArgument(prototype, "prototype").Length, null as IEqualityComparer<T>, autoRemoveDeadItems)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(T[] prototype, IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
            : this(Check.NotNullArgument(prototype, "prototype").Length, comparer, autoRemoveDeadItems)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(IEnumerable<T> prototype)
            : this(null as IEqualityComparer<T>, true)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(IEnumerable<T> prototype, IEqualityComparer<T> comparer)
            : this(comparer, true)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(IEnumerable<T> prototype, bool autoRemoveDeadItems)
            : this(null as IEqualityComparer<T>, autoRemoveDeadItems)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(IEnumerable<T> prototype, IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
            : this(comparer, autoRemoveDeadItems)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(IEqualityComparer<T> comparer)
            : this(comparer, true)
        {
            //Empty
        }

        public WeakSetBucket(bool autoRemoveDeadItems)
            : this(null as IEqualityComparer<T>, autoRemoveDeadItems)
        {
            //Empty
        }

        public WeakSetBucket(IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
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
            _wrapped = new SetBucket<TNeedle>(needleComparer);
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItemsExtracted();
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        public WeakSetBucket(int capacity)
            : this(capacity, null as IEqualityComparer<T>, true)
        {
            //Empty
        }

        public WeakSetBucket(int capacity, IEnumerable<T> prototype)
            : this(capacity, null as IEqualityComparer<T>, true)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(int capacity, IEnumerable<T> prototype, IEqualityComparer<T> comparer)
            : this(capacity, comparer, true)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(int capacity, IEnumerable<T> prototype, bool autoRemoveDeadItems)
            : this(capacity, null as IEqualityComparer<T>, autoRemoveDeadItems)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(int capacity, IEnumerable<T> prototype, IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
            : this(capacity, comparer, autoRemoveDeadItems)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(int capacity, IEqualityComparer<T> comparer)
            : this(capacity, comparer, true)
        {
            //Empty
        }

        public WeakSetBucket(int capacity, bool autoRemoveDeadItems)
            : this(capacity, null as IEqualityComparer<T>, autoRemoveDeadItems)
        {
            //Empty
        }

        public WeakSetBucket(int capacity, IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
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
            _wrapped = new SetBucket<TNeedle>(capacity, needleComparer);
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItemsExtracted();
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        public WeakSetBucket(T[] prototype, int maxProbing)
            : this(Check.NotNullArgument(prototype, "prototype").Length, null as IEqualityComparer<T>, true, maxProbing)
        {
            this.AddRange(prototype);
        }

        public WeakSetBucket(T[] prototype, IEqualityComparer<T> comparer, int maxProbing)
            : this(Check.NotNullArgument(prototype, "prototype").Length, comparer, true, maxProbing)
        {
            this.AddRange(prototype);
        }

        public WeakSetBucket(T[] prototype, bool autoRemoveDeadItems, int maxProbing)
            : this(Check.NotNullArgument(prototype, "prototype").Length, null as IEqualityComparer<T>, autoRemoveDeadItems, maxProbing)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(T[] prototype, IEqualityComparer<T> comparer, bool autoRemoveDeadItems, int maxProbing)
            : this(Check.NotNullArgument(prototype, "prototype").Length, comparer, autoRemoveDeadItems, maxProbing)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(IEnumerable<T> prototype, int maxProbing)
            : this(null as IEqualityComparer<T>, true, maxProbing)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(IEnumerable<T> prototype, IEqualityComparer<T> comparer, int maxProbing)
            : this(comparer, true, maxProbing)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(IEnumerable<T> prototype, bool autoRemoveDeadItems, int maxProbing)
            : this(null as IEqualityComparer<T>, autoRemoveDeadItems, maxProbing)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(IEnumerable<T> prototype, IEqualityComparer<T> comparer, bool autoRemoveDeadItems, int maxProbing)
            : this(comparer, autoRemoveDeadItems, maxProbing)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(IEqualityComparer<T> comparer, int maxProbing)
            : this(comparer, true, maxProbing)
        {
            //Empty
        }

        public WeakSetBucket(bool autoRemoveDeadItems, int maxProbing)
            : this(null as IEqualityComparer<T>, autoRemoveDeadItems, maxProbing)
        {
            //Empty
        }

        public WeakSetBucket(IEqualityComparer<T> comparer, bool autoRemoveDeadItems, int maxProbing)
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
            _wrapped = new SetBucket<TNeedle>(needleComparer, maxProbing);
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItemsExtracted();
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        public WeakSetBucket(int capacity, int maxProbing)
            : this(capacity, null as IEqualityComparer<T>, true, maxProbing)
        {
            //Empty
        }

        public WeakSetBucket(int capacity, IEnumerable<T> prototype, int maxProbing)
            : this(capacity, null as IEqualityComparer<T>, true, maxProbing)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(int capacity, IEnumerable<T> prototype, IEqualityComparer<T> comparer, int maxProbing)
            : this(capacity, comparer, true, maxProbing)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(int capacity, IEnumerable<T> prototype, bool autoRemoveDeadItems, int maxProbing)
            : this(capacity, null as IEqualityComparer<T>, autoRemoveDeadItems, maxProbing)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(int capacity, IEnumerable<T> prototype, IEqualityComparer<T> comparer, bool autoRemoveDeadItems, int maxProbing)
            : this(capacity, comparer, autoRemoveDeadItems, maxProbing)
        {
            this.AddRange(Check.NotNullArgument(prototype, "prototype"));
        }

        public WeakSetBucket(int capacity, IEqualityComparer<T> comparer, int maxProbing)
            : this(capacity, comparer, true, maxProbing)
        {
            //Empty
        }

        public WeakSetBucket(int capacity, bool autoRemoveDeadItems, int maxProbing)
            : this(capacity, null as IEqualityComparer<T>, autoRemoveDeadItems, maxProbing)
        {
            //Empty
        }

        public WeakSetBucket(int capacity, IEqualityComparer<T> comparer, bool autoRemoveDeadItems, int maxProbing)
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
            _wrapped = new SetBucket<TNeedle>(capacity, needleComparer, maxProbing);
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItemsExtracted();
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        ~WeakSetBucket()
        {
            UnRegisterForAutoRemoveDeadItemsExtracted();
        }

        public bool AutoRemoveDeadItems
        {
            get
            {
                return !object.ReferenceEquals(_eventHandler, null);
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

        protected SetBucket<TNeedle> Wrapped
        {
            get
            {
                return _wrapped;
            }
        }

        public bool Add(T item)
        {
            TNeedle needle = NeedleHelper.CreateNeedle<T, TNeedle>(item);
            if (_wrapped.Add(needle))
            {
                return true;
            }
            else
            {
                needle.Dispose();
                return false;
            }
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
            BucketHelper.Recycle(ref displaced);
        }

        public WeakSetBucket<T, TNeedle> Clone()
        {
            return OnClone();
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

        public int GetHashCode(T obj)
        {
            return _comparer.GetHashCode(obj);
        }

        void ICollection<T>.Add(T item)
        {
            TNeedle needle = NeedleHelper.CreateNeedle<T, TNeedle>(item);
            if (!_wrapped.Add(needle))
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
            TNeedle needle = NeedleHelper.CreateNeedle<T, TNeedle>(item);
            TNeedle found;
            if (_wrapped.Remove(needle, out found))
            {
                needle.Dispose();
                found.Dispose();
                return true;
            }
            else
            {
                needle.Dispose();
                return false;
            }
        }

        public int RemoveDeadItems()
        {
            return _wrapped.RemoveWhere(input => !input.IsAlive);
        }

        public int RemoveWhere(Predicate<T> predicate)
        {
            return _wrapped.RemoveWhere
                   (
                       input =>
                       {
                           if (input.IsAlive)
                           {
                               return predicate(input.Value);
                           }
                           else
                           {
                               return false;
                           }
                       }
                   );
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return Extensions.SetEquals(this, other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            Extensions.SymmetricExceptWith(this, other);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGet(int index, out T item)
        {
            TNeedle needle;
            var result = _wrapped.TryGet(index, out needle);
            item = needle.Value;
            if (needle.IsAlive)
            {
                return result;
            }
            else
            {
                return false;
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            Extensions.UnionWith(this, other);
        }

        protected virtual WeakSetBucket<T, TNeedle> OnClone()
        {
            return new WeakSetBucket<T, TNeedle>(this as IEnumerable<T>, _comparer);
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
                eventHandler = new EventHandler(GarbageCollected);
                _eventHandler = new WeakNeedle<EventHandler>(eventHandler);
                result = true;
            }
            else
            {
                eventHandler = _eventHandler.Value.Value;
                if (!_eventHandler.IsAlive)
                {
                    eventHandler = new EventHandler(GarbageCollected);
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
                _eventHandler = null;
                return true;
            }
            else
            {
                _eventHandler = null;
                return false;
            }
        }
    }
}

#endif