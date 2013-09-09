#if FAT

using System;
using System.Collections.Generic;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public class WeakHashBucket<TKey, TValue, TNeedle> : IEnumerable<KeyValuePair<TKey, TValue>>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IEqualityComparer<TKey>
        where TKey : class
        where TNeedle : WeakNeedle<TKey>
    {
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly HashBucket<TNeedle, TValue> _wrapped;
        private EventHandler _eventHandler;

        public WeakHashBucket()
        {
            RegisterForAutoRemoveDeadItems();
        }

        public WeakHashBucket(IEnumerable<KeyValuePair<TKey, TValue>> prototype)
            : this()
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakHashBucket(KeyValuePair<TKey, TValue>[] prototype)
            : this()
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakHashBucket(IEnumerable<KeyValuePair<TKey, TValue>> prototype, IEqualityComparer<TKey> comparer)
            : this(comparer)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakHashBucket(KeyValuePair<TKey, TValue>[] prototype, IEqualityComparer<TKey> comparer)
            : this(comparer)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakHashBucket(IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _wrapped = new HashBucket<TNeedle, TValue>
            (
                new ConversionEqualityComparer<TNeedle, TKey>(_comparer, input => input.Value)
            );
            RegisterForAutoRemoveDeadItems();
        }

        public WeakHashBucket(bool autoRemoveDeadItems)
        {
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItems();
            }
        }

        public WeakHashBucket(IEnumerable<KeyValuePair<TKey, TValue>> prototype, bool autoRemoveDeadItems)
            : this(autoRemoveDeadItems)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakHashBucket(KeyValuePair<TKey, TValue>[] prototype, bool autoRemoveDeadItems)
            : this(autoRemoveDeadItems)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakHashBucket(IEnumerable<KeyValuePair<TKey, TValue>> prototype, IEqualityComparer<TKey> comparer, bool autoRemoveDeadItems)
            : this(comparer, autoRemoveDeadItems)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
            RegisterForAutoRemoveDeadItems();
        }

        public WeakHashBucket(KeyValuePair<TKey, TValue>[] prototype, IEqualityComparer<TKey> comparer, bool autoRemoveDeadItems)
            : this(comparer, autoRemoveDeadItems)
        {
            Check.NotNullArgument(prototype, "prototype");
            this.AddRange(prototype);
        }

        public WeakHashBucket(IEqualityComparer<TKey> comparer, bool autoRemoveDeadItems)
        {
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _wrapped = new HashBucket<TNeedle, TValue>
            (
                new ConversionEqualityComparer<TNeedle, TKey>(_comparer, input => input.Value)
            );
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
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        protected HashBucket<TNeedle, TValue> Wrapped
        {
            get
            {
                return _wrapped;
            }
        }

        public bool Add(KeyValuePair<TKey, TValue> item)
        {
            return _wrapped.TryAdd(NeedleHelper.CreateNeedle<TKey, TNeedle>(item.Key), item.Value);
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public WeakHashBucket<TKey, TValue, TNeedle> Clone()
        {
            return OnClone();
        }

        public bool ContainsKey(TKey key)
        {
            foreach (var _item in this)
            {
                if (_comparer.Equals(_item.Key, key))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public bool Equals(TKey x, TKey y)
        {
            return _comparer.Equals(x, y);
        }

        public void ExceptWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.ExceptWith(this, other);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.ConvertProgressiveFiltered(input => new KeyValuePair<TKey, TValue>(input.Key.Value, input.Value), input => input.Key.IsAlive).GetEnumerator();
        }

        public int GetHashCode(TKey obj)
        {
            return _comparer.GetHashCode(obj);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            _wrapped.Add(NeedleHelper.CreateNeedle<TKey, TNeedle>(item.Key), item.Value);
        }

        public void IntersectWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.IntersectWith(this, other);
        }

        public bool IsProperSubsetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.IsProperSubsetOf(this, other);
        }

        public bool IsProperSupersetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.IsProperSupersetOf(this, other);
        }

        public bool IsSubsetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.IsSubsetOf(this, other);
        }

        public bool IsSupersetOf(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.IsSupersetOf(this, other);
        }

        public bool Overlaps(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.Overlaps(this, other);
        }

        public bool Remove(TKey item)
        {
            return _wrapped.Remove(NeedleHelper.CreateNeedle<TKey, TNeedle>(item));
        }

        public int RemoveDeadItems()
        {
            return _wrapped.RemoveWhereKey(key => !key.IsAlive);
        }

        public bool SetEquals(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            return Extensions.SetEquals(this, other);
        }

        public void SymmetricExceptWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.SymmetricExceptWith(this, other);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void UnionWith(IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            Extensions.UnionWith(this, other);
        }

        protected virtual WeakHashBucket<TKey, TValue, TNeedle> OnClone()
        {
            return new WeakHashBucket<TKey, TValue, TNeedle>(this as IEnumerable<KeyValuePair<TKey, TValue>>, _comparer);
        }

        private void GarbageCollected(object sender, EventArgs e)
        {
            RemoveDeadItems();
        }

        private void RegisterForAutoRemoveDeadItems()
        {
            _eventHandler = new EventHandler(GarbageCollected);
            GCMonitor.Collected += _eventHandler;
        }

        private void UnRegisterForAutoRemoveDeadItems()
        {
            if (!object.ReferenceEquals(_eventHandler, null))
            {
                GCMonitor.Collected -= _eventHandler;
                _eventHandler = null;
            }
        }
    }
}

#endif
