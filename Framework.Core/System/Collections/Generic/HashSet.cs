#if LESSTHAN_NET35

#pragma warning disable CC0091 // Use static method

using System.Runtime.Serialization;
using System.Security.Permissions;
using Theraot.Collections;
using Theraot.Collections.Specialized;

namespace System.Collections.Generic
{
    [Serializable]
    public class HashSet<T> : ISet<T>, IReadOnlyCollection<T>, ISerializable
    {
        private readonly NullAwareDictionary<T, object> _wrapped;

        public HashSet()
        {
            _wrapped = new NullAwareDictionary<T, object>();
        }

        public HashSet(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _wrapped = new NullAwareDictionary<T, object>();
            foreach (var item in collection)
            {
                _wrapped[item] = null;
            }
        }

        public HashSet(IEqualityComparer<T> comparer)
        {
            _wrapped = new NullAwareDictionary<T, object>(comparer);
        }

        public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _wrapped = new NullAwareDictionary<T, object>(comparer);
            foreach (var item in collection)
            {
                _wrapped[item] = null;
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        protected HashSet(SerializationInfo info, StreamingContext context)
            : this()
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Theraot.No.Op(context);
            _wrapped = new NullAwareDictionary<T, object>(info.GetValue("dictionary", typeof(KeyValuePair<T, object>[])) as KeyValuePair<T, object>[]);
        }

        public IEqualityComparer<T> Comparer => _wrapped.Comparer;

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            _wrapped.Deconstruct(out var dictionary);
            info.AddValue(nameof(dictionary), dictionary);
        }

        public int Count => _wrapped.Count;

        public bool IsReadOnly => false;

        public bool Add(T item)
        {
            if (_wrapped.ContainsKey(item))
            {
                return false;
            }

            _wrapped[item] = null;
            return true;
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public bool Contains(T item)
        {
            // item can be null
            // ReSharper disable once AssignNullToNotNullAttribute
            return _wrapped.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "arrayIndex < 0");
            }

            if (Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }

            _wrapped.Keys.CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (var item in other)
            {
                _wrapped.Remove(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            this.IntersectWith(other, _wrapped.Comparer);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return IsSubsetOf(ToHashSet(other), true);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(ToHashSet(other), true);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return IsSubsetOf(ToHashSet(other), false);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(ToHashSet(other), false);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (var item in other)
            {
                if (_wrapped.ContainsKey(item))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Remove(T item)
        {
            // item can be null
            // ReSharper disable once AssignNullToNotNullAttribute
            return _wrapped.Remove(item);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var containsCount = 0;
            foreach (var item in ToHashSet(other))
            {
                if (!_wrapped.ContainsKey(item))
                {
                    return false;
                }

                containsCount++;
            }

            return containsCount == _wrapped.Count;
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var tmpSet = new HashSet<T>(other);
            foreach (var item in tmpSet)
            {
                if (_wrapped.ContainsKey(item))
                {
                    _wrapped.Remove(item);
                }
                else
                {
                    _wrapped[item] = null;
                }
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            foreach (var item in other)
            {
                if (!_wrapped.ContainsKey(item))
                {
                    _wrapped[item] = null;
                }
            }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static IEqualityComparer<HashSet<T>> CreateSetComparer()
        {
            return HashSetEqualityComparer.Instance;
        }

        public void CopyTo(T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (Count > array.Length)
            {
                throw new ArgumentException("the Count property is larger than the size of the destination array.");
            }

            _wrapped.Keys.CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number is required.");
            }

            if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }

            var copiedCount = 0;
            var currentIndex = arrayIndex;
            foreach (var item in this)
            {
                array[currentIndex] = item;
                currentIndex++;
                copiedCount++;
                if (copiedCount >= count)
                {
                    break;
                }
            }
        }

        public int RemoveWhere(Predicate<T> match)
        {
            return Extensions.RemoveWhere(this, match);
        }

        public void TrimExcess()
        {
            // Should not be static
            // Empty
        }

        private bool IsSubsetOf(IEnumerable<T> other, bool proper)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var elementCount = 0;
            var matchCount = 0;
            foreach (var item in other)
            {
                elementCount++;
                if (_wrapped.ContainsKey(item))
                {
                    matchCount++;
                }
            }

            if (proper)
            {
                return matchCount == _wrapped.Count && elementCount > _wrapped.Count;
            }

            return matchCount == _wrapped.Count;
        }

        private bool IsSupersetOf(IEnumerable<T> other, bool proper)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var elementCount = 0;
            foreach (var item in other)
            {
                elementCount++;
                if (!_wrapped.ContainsKey(item))
                {
                    return false;
                }
            }

            if (proper)
            {
                return elementCount < _wrapped.Count;
            }

            return true;
        }

        private HashSet<T> ToHashSet(IEnumerable<T> other)
        {
            var comparer = Comparer;
            if (other is HashSet<T> test && comparer.Equals(test.Comparer))
            {
                return test;
            }

            return new HashSet<T>(other, comparer);
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly IEnumerator<KeyValuePair<T, object>> _enumerator;
            private bool _valid;

            public Enumerator(HashSet<T> hashSet)
            {
                _enumerator = hashSet._wrapped.GetEnumerator();
                _valid = false;
                Current = default;
            }

            public T Current { get; private set; }

            object IEnumerator.Current
            {
                get
                {
                    if (_valid)
                    {
                        return Current;
                    }

                    throw new InvalidOperationException("Call MoveNext first or use IEnumerator<T>");
                }
            }

            public void Dispose()
            {
                var enumerator = _enumerator;
                enumerator?.Dispose();
            }

            public bool MoveNext()
            {
                var enumerator = _enumerator;
                if (enumerator != null)
                {
                    _valid = _enumerator.MoveNext();
                    Current = _enumerator.Current.Key;
                    return _valid;
                }

                return false;
            }

            void IEnumerator.Reset()
            {
                _valid = false;
                var enumerator = _enumerator;
                if (enumerator != null)
                {
                    Current = _enumerator.Current.Key;
                    _enumerator.Reset();
                }
            }
        }

        private sealed class HashSetEqualityComparer : IEqualityComparer<HashSet<T>>
        {
            public static readonly HashSetEqualityComparer Instance = new HashSetEqualityComparer();

            public bool Equals(HashSet<T> x, HashSet<T> y)
            {
                if (x == y)
                {
                    return true;
                }

                if (x == null || y == null || x.Count != y.Count)
                {
                    return false;
                }

                foreach (var item in x)
                {
                    if (!y.Contains(item))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(HashSet<T> obj)
            {
                try
                {
                    var comparer = EqualityComparer<T>.Default;
                    var hash = 0;
                    foreach (var item in obj)
                    {
                        hash ^= comparer.GetHashCode(item);
                    }

                    return hash;
                }
                catch (NullReferenceException)
                {
                    return 0;
                }
            }
        }
    }
}

#endif