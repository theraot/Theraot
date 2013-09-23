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
        private WeakNeedle<EventHandler> _eventHandler;

        public WeakSetBucket()
        {
            _comparer = EqualityComparerHelper<T>.Default;
            _wrapped = new SetBucket<TNeedle>(EqualityComparerHelper<TNeedle>.Default);
            RegisterForAutoRemoveDeadItems();
        }

        public WeakSetBucket(IEnumerable<T> prototype)
            : this()
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakSetBucket(T[] prototype)
            : this()
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakSetBucket(IEnumerable<T> prototype, IEqualityComparer<T> comparer)
            : this(comparer)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakSetBucket(T[] prototype, IEqualityComparer<T> comparer)
            : this(comparer)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakSetBucket(IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? EqualityComparerHelper<T>.Default;
            _wrapped = new SetBucket<TNeedle>(EqualityComparerHelper<TNeedle>.Default);
            RegisterForAutoRemoveDeadItems();
        }

        public WeakSetBucket(bool autoRemoveDeadItems)
        {
            _comparer = EqualityComparerHelper<T>.Default;
            _wrapped = new SetBucket<TNeedle>(EqualityComparerHelper<TNeedle>.Default);
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItems();
            }
        }

        public WeakSetBucket(IEnumerable<T> prototype, bool autoRemoveDeadItems)
            : this(autoRemoveDeadItems)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakSetBucket(T[] prototype, bool autoRemoveDeadItems)
            : this(autoRemoveDeadItems)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakSetBucket(IEnumerable<T> prototype, IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
            : this(comparer, autoRemoveDeadItems)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
            RegisterForAutoRemoveDeadItems();
        }

        public WeakSetBucket(T[] prototype, IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
            : this(comparer, autoRemoveDeadItems)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakSetBucket(IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
        {
            _comparer = comparer ?? EqualityComparerHelper<T>.Default;
            _wrapped = new SetBucket<TNeedle>(EqualityComparerHelper<TNeedle>.Default);
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItems();
            }
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
            return _wrapped.Add(NeedleHelper.CreateNeedle<T, TNeedle>(item));
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
            _wrapped.Add(NeedleHelper.CreateNeedle<T, TNeedle>(item));
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
            TNeedle pass = NeedleHelper.CreateNeedle<T, TNeedle>(item);
            TNeedle found;
            if (_wrapped.Remove(pass, out found))
            {
                pass.Dispose();
                found.Dispose();
                return true;
            }
            else
            {
                pass.Dispose();
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
            EventHandler eventHandler;
            if (ReferenceEquals(_eventHandler, null))
            {
                eventHandler = new EventHandler(GarbageCollected);
                _eventHandler = new WeakNeedle<EventHandler>(eventHandler);
            }
            else
            {
                eventHandler = _eventHandler.Value;
                if (!_eventHandler.IsAlive)
                {
                    eventHandler = new EventHandler(GarbageCollected);
                    _eventHandler.Value = eventHandler;
                }
            }
            GCMonitor.Collected += eventHandler;
        }

        private void UnRegisterForAutoRemoveDeadItems()
        {
            EventHandler eventHandler;
            if (_eventHandler.Retrieve(out eventHandler))
            {
                GCMonitor.Collected -= eventHandler;
            }
            _eventHandler = null;
        }
    }
}

#endif