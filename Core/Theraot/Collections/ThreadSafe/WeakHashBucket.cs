#if FAT

using System;
using System.Collections.Generic;
using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public class WeakHashBucket<TKey, TValue, TNeedle> : IEnumerable<KeyValuePair<TKey, TValue>>, IEqualityComparer<TKey>
        where TKey : class
        where TNeedle : WeakNeedle<TKey>
    {
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly HashBucket<TNeedle, TValue> _wrapped;

        private StructNeedle<WeakNeedle<EventHandler>> _eventHandler;

        public WeakHashBucket()
        {
            _comparer = EqualityComparerHelper<TKey>.Default;
            _wrapped = new HashBucket<TNeedle, TValue>(EqualityComparerHelper<TNeedle>.Default);
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
            _comparer = comparer ?? EqualityComparerHelper<TKey>.Default;
            _wrapped = new HashBucket<TNeedle, TValue>(EqualityComparerHelper<TNeedle>.Default);
            RegisterForAutoRemoveDeadItems();
        }

        public WeakHashBucket(bool autoRemoveDeadItems)
        {
            _comparer = EqualityComparerHelper<TKey>.Default;
            _wrapped = new HashBucket<TNeedle, TValue>(EqualityComparerHelper<TNeedle>.Default);
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
            _comparer = comparer ?? EqualityComparerHelper<TKey>.Default;
            _wrapped = new HashBucket<TNeedle, TValue>(EqualityComparerHelper<TNeedle>.Default);
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItems();
            }
        }

        ~WeakHashBucket()
        {
            UnRegisterForAutoRemoveDeadItems();
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

        protected HashBucket<TNeedle, TValue> Wrapped
        {
            get
            {
                return _wrapped;
            }
        }

        public bool Add(KeyValuePair<TKey, TValue> item)
        {
            TNeedle needle = NeedleHelper.CreateNeedle<TKey, TNeedle>(item.Key);
            if (_wrapped.TryAdd(needle, item.Value))
            {
                return true;
            }
            else
            {
                needle.Dispose();
                return false;
            }
        }

        public void Add(TKey key, TValue value)
        {
            TNeedle needle = NeedleHelper.CreateNeedle<TKey, TNeedle>(key);
            if (!_wrapped.TryAdd(needle, value))
            {
                needle.Dispose();
            }
        }

        public int AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
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

        public TValue CharyAdd(TKey key, TValue value)
        {
            var needle = NeedleHelper.CreateNeedle<TKey, TNeedle>(key);
            var result = _wrapped.CharyAdd(needle, value);
            if (!ReferenceEquals(needle, result))
            {
                needle.Dispose();
            }
            return result;
        }

        public void Clear()
        {
            var displaced = _wrapped.ClearEnumerable();
            foreach (var item in displaced)
            {
                item.Key.Dispose();
            }
            BucketHelper.Recycle(ref displaced);
        }

        public WeakHashBucket<TKey, TValue, TNeedle> Clone()
        {
            return OnClone();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            TValue value;
            TNeedle needle = NeedleHelper.CreateNeedle<TKey, TNeedle>(key);
            if (_wrapped.TryGetValue(needle, out value))
            {
                needle.Dispose();
                return EqualityComparer<TValue>.Default.Equals(value, item.Value);
            }
            else
            {
                needle.Dispose();
                return false;
            }
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

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.ConvertProgressiveFiltered(input => new KeyValuePair<TKey, TValue>(input.Key.Value, input.Value), input => input.Key.IsAlive).GetEnumerator();
        }

        public int GetHashCode(TKey obj)
        {
            return _comparer.GetHashCode(obj);
        }

        public bool Remove(TKey key)
        {
            TNeedle needle = NeedleHelper.CreateNeedle<TKey, TNeedle>(key);
            TValue found;
            if (_wrapped.Remove(needle, out found))
            {
                needle.Dispose();
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
            return _wrapped.RemoveWhereKey(key => !key.IsAlive);
        }

        public int RemoveWhereKey(Predicate<TKey> predicate)
        {
            return _wrapped.RemoveWhereKey
            (
                key =>
                {
                    if (key.IsAlive)
                    {
                        return predicate.Invoke(key.Value);
                    }
                    else
                    {
                        return false;
                    }
                }
            );
        }

        public void Set(TKey key, TValue value)
        {
            TNeedle needle = NeedleHelper.CreateNeedle<TKey, TNeedle>(key);
            _wrapped.Set(needle, value);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryAdd(TKey key, TValue value)
        {
            TNeedle needle = NeedleHelper.CreateNeedle<TKey, TNeedle>(key);
            if (_wrapped.TryAdd(needle, value))
            {
                return true;
            }
            else
            {
                needle.Dispose();
                return false;
            }
        }

        public bool TryGet(TKey key, out TValue value)
        {
            TNeedle needle = NeedleHelper.CreateNeedle<TKey, TNeedle>(key);
            if (_wrapped.TryGetValue(needle, out value))
            {
                needle.Dispose();
                return true;
            }
            else
            {
                needle.Dispose();
                return false;
            }
        }

        public bool TryGet(int index, out TKey key, out TValue value)
        {
            TNeedle needle;
            var result = _wrapped.TryGet(index, out needle, out value);
            key = needle.Value;
            if (needle.IsAlive)
            {
                return result;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            TNeedle needle = NeedleHelper.CreateNeedle<TKey, TNeedle>(key);
            if (_wrapped.TryGetValue(needle, out value))
            {
                needle.Dispose();
                return true;
            }
            else
            {
                needle.Dispose();
                return false;
            }
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
            EventHandler eventHandler;
            if (ReferenceEquals(_eventHandler.Value, null))
            {
                eventHandler = new EventHandler(GarbageCollected);
                _eventHandler = new WeakNeedle<EventHandler>(eventHandler);
            }
            else
            {
                eventHandler = _eventHandler.Value.Value;
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
            if (_eventHandler.Value.Retrieve(out eventHandler))
            {
                GCMonitor.Collected -= eventHandler;
            }
            _eventHandler = null;
        }
    }
}

#endif