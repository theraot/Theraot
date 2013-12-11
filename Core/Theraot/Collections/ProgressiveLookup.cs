using System;
using System.Collections.Generic;
using System.Linq;

using Theraot.Core;

namespace Theraot.Collections
{
    [System.Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class ProgressiveLookup<TKey, TValue> : IReadOnlyDictionary<TKey, IExtendedGrouping<TKey, TValue>>, IExtendedReadOnlyDictionary<TKey, IExtendedGrouping<TKey, TValue>>, IExtendedDictionary<TKey, IExtendedGrouping<TKey, TValue>>, IDictionary<TKey, IExtendedGrouping<TKey, TValue>>, IReadOnlyCollection<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>>, IEnumerable<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>>, IExtendedCollection<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>>, IReadOnlyCollection<IExtendedGrouping<TKey, TValue>>, IEnumerable<IExtendedGrouping<TKey, TValue>>, IExtendedCollection<IExtendedGrouping<TKey, TValue>>, ILookup<TKey, TValue>
    {
        private readonly IDictionary<TKey, IExtendedGrouping<TKey, TValue>> _cache;
        private readonly IEqualityComparer<TValue> _itemComparer;
        private readonly IEqualityComparer<TKey> _keyComparer;
        private readonly ProgressiveSet<TKey> _keysReadonly;
        private readonly IProgressor<KeyValuePair<TKey, TValue>> _lowProgressor;
        private readonly IEqualityComparer<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>> _pairComparer;
        private readonly IProgressor<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>> _progressor;
        private readonly IEqualityComparer<IExtendedGrouping<TKey, TValue>> _valueComparer;
        private readonly ProgressiveSet<IExtendedGrouping<TKey, TValue>> _valuesReadonly;

        public ProgressiveLookup(IEnumerable<KeyValuePair<TKey, TValue>> wrapped)
            : this(wrapped, new Dictionary<TKey, IExtendedGrouping<TKey, TValue>>(), null, null)
        {
            //Empty
        }

        public ProgressiveLookup(IEnumerable<KeyValuePair<TKey, TValue>> wrapped, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> itemComparer)
            : this(wrapped, new Dictionary<TKey, IExtendedGrouping<TKey, TValue>>(keyComparer), keyComparer, itemComparer)
        {
            //Empty
        }

        protected ProgressiveLookup(IEnumerable<KeyValuePair<TKey, TValue>> wrapped, IDictionary<TKey, IExtendedGrouping<TKey, TValue>> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> itemComparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _itemComparer = itemComparer ?? EqualityComparer<TValue>.Default;
            _valueComparer = EqualityComparer<IExtendedGrouping<TKey, TValue>>.Default;
            _pairComparer = new KeyValuePairEqualityComparer<TKey, IExtendedGrouping<TKey, TValue>>(_keyComparer, _valueComparer);
            _lowProgressor = new Progressor<KeyValuePair<TKey, TValue>>(wrapped);
            _progressor = ProgressorBuilder.CreateConversionProgressor(_lowProgressor, Process);
            _valuesReadonly = new ProgressiveSet<IExtendedGrouping<TKey, TValue>>(ProgressorBuilder.CreateConversionProgressor(_progressor, input => input.Value), _valueComparer);
            _keysReadonly = new ProgressiveSet<TKey>(ProgressorBuilder.CreateConversionProgressor(_progressor, input => input.Key), keyComparer);
        }

        protected ProgressiveLookup(IProgressor<KeyValuePair<TKey, TValue>> wrapped, IDictionary<TKey, IExtendedGrouping<TKey, TValue>> cache, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> itemComparer)
        {
            _cache = Check.NotNullArgument(cache, "cache");
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _itemComparer = itemComparer ?? EqualityComparer<TValue>.Default;
            _valueComparer = EqualityComparer<IExtendedGrouping<TKey, TValue>>.Default;
            _pairComparer = new KeyValuePairEqualityComparer<TKey, IExtendedGrouping<TKey, TValue>>(_keyComparer, _valueComparer);
            _lowProgressor = ProgressorBuilder.CreateProgressor(wrapped);
            _progressor = ProgressorBuilder.CreateConversionProgressor(_lowProgressor, Process);
            _valuesReadonly = new ProgressiveSet<IExtendedGrouping<TKey, TValue>>(ProgressorBuilder.CreateConversionProgressor(_progressor, input => input.Value), _valueComparer);
            _keysReadonly = new ProgressiveSet<TKey>(ProgressorBuilder.CreateConversionProgressor(_progressor, input => input.Key), keyComparer);
        }

        public int Count
        {
            get
            {
                _progressor.TakeAll();
                return _cache.Count;
            }
        }

        public bool EndOfEnumeration
        {
            get
            {
                return _progressor.IsClosed;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns this")]
        bool ICollection<IExtendedGrouping<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns True")]
        bool ICollection<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        ICollection<TKey> IDictionary<TKey, IExtendedGrouping<TKey, TValue>>.Keys
        {
            get
            {
                return _keysReadonly;
            }
        }

        ICollection<IExtendedGrouping<TKey, TValue>> IDictionary<TKey, IExtendedGrouping<TKey, TValue>>.Values
        {
            get
            {
                return _valuesReadonly;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns this")]
        IReadOnlyCollection<IExtendedGrouping<TKey, TValue>> IExtendedCollection<IExtendedGrouping<TKey, TValue>>.AsReadOnly
        {
            get
            {
                return this;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns this")]
        IReadOnlyCollection<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>> IExtendedCollection<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>>.AsReadOnly
        {
            get
            {
                return this;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns this")]
        IReadOnlyDictionary<TKey, IExtendedGrouping<TKey, TValue>> IExtendedDictionary<TKey, IExtendedGrouping<TKey, TValue>>.AsReadOnly
        {
            get
            {
                return this;
            }
        }

        IReadOnlyCollection<TKey> IExtendedReadOnlyDictionary<TKey, IExtendedGrouping<TKey, TValue>>.Keys
        {
            get
            {
                return _keysReadonly;
            }
        }

        IReadOnlyCollection<IExtendedGrouping<TKey, TValue>> IExtendedReadOnlyDictionary<TKey, IExtendedGrouping<TKey, TValue>>.Values
        {
            get
            {
                return _valuesReadonly;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, IExtendedGrouping<TKey, TValue>>.Keys
        {
            get
            {
                return _keysReadonly;
            }
        }

        IEnumerable<IExtendedGrouping<TKey, TValue>> IReadOnlyDictionary<TKey, IExtendedGrouping<TKey, TValue>>.Values
        {
            get
            {
                return _valuesReadonly;
            }
        }

        public IReadOnlyCollection<TKey> Keys
        {
            get
            {
                return _keysReadonly;
            }
        }

        public IReadOnlyCollection<IExtendedGrouping<TKey, TValue>> Values
        {
            get
            {
                return _valuesReadonly;
            }
        }

        protected IEqualityComparer<TValue> ItemComparer
        {
            get
            {
                return _itemComparer;
            }
        }

        protected IEqualityComparer<TKey> KeyComparer
        {
            get
            {
                return _keyComparer;
            }
        }

        protected IEqualityComparer<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>> PairComparer
        {
            get
            {
                return _pairComparer;
            }
        }

        protected IProgressor<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>> Progressor
        {
            get
            {
                return _progressor;
            }
        }

        protected IEqualityComparer<IExtendedGrouping<TKey, TValue>> ValueComparer
        {
            get
            {
                return _valueComparer;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        IExtendedGrouping<TKey, TValue> IDictionary<TKey, IExtendedGrouping<TKey, TValue>>.this[TKey key]
        {
            get
            {
                try
                {
                    return _cache[key];
                }
                catch (KeyNotFoundException)
                {
                    KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> _item;
                    while (_progressor.TryTake(out _item))
                    {
                        if (_keyComparer.Equals(key, _item.Key))
                        {
                            return _item.Value;
                        }
                    }
                    throw;
                }
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        IEnumerable<TValue> ILookup<TKey, TValue>.this[TKey key]
        {
            get
            {
                return this[key];
            }
        }

        public IExtendedGrouping<TKey, TValue> this[TKey key]
        {
            get
            {
                try
                {
                    return _cache[key];
                }
                catch (KeyNotFoundException)
                {
                    KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> _item;
                    while (_progressor.TryTake(out _item))
                    {
                        if (_keyComparer.Equals(key, _item.Key))
                        {
                            return _item.Value;
                        }
                    }
                    throw;
                }
            }
        }

        public bool Contains(KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> item)
        {
            if (_cache.Contains(item))
            {
                return true;
            }
            else
            {
                KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> _item;
                while (_progressor.TryTake(out _item))
                {
                    if (_pairComparer.Equals(item, _item))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool Contains(KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> item, IEqualityComparer<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>> comparer)
        {
            return Enumerable.Contains(this, item, comparer);
        }

        public bool Contains(IExtendedGrouping<TKey, TValue> item)
        {
            if (!ReferenceEquals(item, null))
            {
                return Contains(item);
            }
            else
            {
                return false;
            }
        }

        public bool Contains(IExtendedGrouping<TKey, TValue> item, IEqualityComparer<IExtendedGrouping<TKey, TValue>> comparer)
        {
            return Enumerable.Contains(Values, item, comparer);
        }

        public bool ContainsKey(TKey key)
        {
            if (_cache.ContainsKey(key))
            {
                return true;
            }
            else
            {
                KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> _item;
                while (_progressor.TryTake(out _item))
                {
                    if (_keyComparer.Equals(key, _item.Key))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void CopyTo(KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>[] array)
        {
            _progressor.TakeAll();
            _cache.CopyTo(array);
        }

        public void CopyTo(KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>[] array, int arrayIndex)
        {
            _progressor.TakeAll();
            _cache.CopyTo(array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            _progressor.TakeWhile(() => _cache.Count < countLimit);
            _cache.CopyTo(array, arrayIndex, countLimit);
        }

        public void CopyTo(IExtendedGrouping<TKey, TValue>[] array, int arrayIndex)
        {
            _progressor.TakeAll();
            _cache.Values.CopyTo(array, arrayIndex);
        }

        public void CopyTo(IExtendedGrouping<TKey, TValue>[] array)
        {
            _progressor.TakeAll();
            _cache.Values.CopyTo(array);
        }

        public void CopyTo(IExtendedGrouping<TKey, TValue>[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            _progressor.TakeWhile(() => _cache.Count < countLimit);
            _cache.Values.CopyTo(array, arrayIndex, countLimit);
        }

        public IEnumerator<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>> GetEnumerator()
        {
            foreach (var item in _cache)
            {
                yield return item;
            }
            {
                KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> item;
                while (_progressor.TryTake(out item))
                {
                    yield return item;
                }
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ICollection<IExtendedGrouping<TKey, TValue>>.Add(IExtendedGrouping<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ICollection<IExtendedGrouping<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool ICollection<IExtendedGrouping<TKey, TValue>>.Remove(IExtendedGrouping<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ICollection<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>>.Add(KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> item)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void ICollection<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>>.Clear()
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool ICollection<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>>.Remove(KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> item)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        void IDictionary<TKey, IExtendedGrouping<TKey, TValue>>.Add(TKey key, IExtendedGrouping<TKey, TValue> value)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool IDictionary<TKey, IExtendedGrouping<TKey, TValue>>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        IEnumerator<IExtendedGrouping<TKey, TValue>> IEnumerable<IExtendedGrouping<TKey, TValue>>.GetEnumerator()
        {
            foreach (var item in _cache)
            {
                yield return item.Value;
            }
            {
                KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> item;
                while (_progressor.TryTake(out item))
                {
                    yield return item.Value;
                }
            }
        }

        IEnumerator<IGrouping<TKey, TValue>> IEnumerable<IGrouping<TKey, TValue>>.GetEnumerator()
        {
            foreach (var item in _cache)
            {
                yield return item.Value;
            }
            {
                KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> item;
                while (_progressor.TryTake(out item))
                {
                    yield return item.Value;
                }
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool IExtendedCollection<IExtendedGrouping<TKey, TValue>>.Remove(IExtendedGrouping<TKey, TValue> item, IEqualityComparer<IExtendedGrouping<TKey, TValue>> comparer)
        {
            throw new NotSupportedException();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Not Supported")]
        bool IExtendedCollection<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>>.Remove(KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> item, IEqualityComparer<KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>> comparer)
        {
            throw new NotSupportedException();
        }

        bool ILookup<TKey, TValue>.Contains(TKey key)
        {
            return ContainsKey(key);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(TKey key, out IExtendedGrouping<TKey, TValue> value)
        {
            try
            {
                value = this[key];
                return true;
            }
            catch (KeyNotFoundException)
            {
                value = null;
                return false;
            }
        }

        private KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>> Process(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            if (_cache.ContainsKey(key))
            {
                (_cache[key] as Grouping).Add(item.Value);
            }
            else
            {
                var newCollection = new Grouping
                (
                    ProgressorBuilder.CreateFilterProgressor
                    (
                        _lowProgressor,
                        input => _keyComparer.Equals(input.Key, key),
                        input => input.Value
                    ),
                    _itemComparer,
                    key
                );
                _cache.Add(key, newCollection);
            }
            return new KeyValuePair<TKey, IExtendedGrouping<TKey, TValue>>(key, _cache[key]);
        }

        [System.Serializable]
        [global::System.Diagnostics.DebuggerNonUserCode]
        private sealed class Grouping : IExtendedGrouping<TKey, TValue>, IExtendedReadOnlyCollection<TValue>, IExtendedCollection<TValue>, ICollection<TValue>, IGrouping<TKey, TValue>, IEqualityComparer<TValue>
        {
            private readonly ICollection<TValue> _cache;
            private readonly IEqualityComparer<TValue> _comparer;
            private readonly TKey _key;
            private readonly IProgressor<TValue> _progressor;

            internal Grouping(IProgressor<TValue> wrapped, IEqualityComparer<TValue> comparer, TKey key)
            {
                _progressor = Check.NotNullArgument(wrapped, "wrapped");
                _key = key;
                _comparer = comparer ?? EqualityComparer<TValue>.Default;
                _cache = new List<TValue>();
            }

            public IEqualityComparer<TValue> Comparer
            {
                get
                {
                    return _comparer;
                }
            }

            public int Count
            {
                get
                {
                    _progressor.TakeAll();
                    return _cache.Count;
                }
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            IReadOnlyCollection<TValue> IExtendedCollection<TValue>.AsReadOnly
            {
                get
                {
                    return this;
                }
            }

            public TKey Key
            {
                get
                {
                    return _key;
                }
            }

            internal ICollection<TValue> Cache
            {
                get
                {
                    return _cache;
                }
            }

            public void Close()
            {
                _progressor.Close();
            }

            public bool Contains(TValue item)
            {
                if (_cache.Contains(item))
                {
                    return true;
                }
                else
                {
                    TValue _item;
                    while (_progressor.TryTake(out _item))
                    {
                        if (_comparer.Equals(item, _item))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            public bool Contains(TValue item, IEqualityComparer<TValue> comparer)
            {
                return Enumerable.Contains(this, item, comparer);
            }

            public void CopyTo(TValue[] array)
            {
                _progressor.TakeAll();
                _cache.CopyTo(array);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                _progressor.TakeAll();
                _cache.CopyTo(array, arrayIndex);
            }

            public void CopyTo(TValue[] array, int arrayIndex, int countLimit)
            {
                Extensions.CanCopyTo(array, arrayIndex, countLimit);
                _progressor.TakeWhile(() => _cache.Count < countLimit);
                _cache.CopyTo(array, arrayIndex, countLimit);
            }

            public bool Equals(TValue x, TValue y)
            {
                return _comparer.Equals(x, y);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                foreach (TValue item in _cache)
                {
                    yield return item;
                }
                {
                    TValue item;
                    while (_progressor.TryTake(out item))
                    {
                        yield return item;
                    }
                }
            }

            public int GetHashCode(TValue obj)
            {
                return _comparer.GetHashCode(obj);
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            bool IExtendedCollection<TValue>.Remove(TValue item, IEqualityComparer<TValue> comparer)
            {
                throw new NotSupportedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}