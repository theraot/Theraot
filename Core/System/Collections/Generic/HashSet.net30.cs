#if NET20 || NET30

using System.Runtime.Serialization;
using System.Security.Permissions;
using Theraot.Collections;
using Theraot.Collections.Specialized;

namespace System.Collections.Generic
{
    [Serializable]
    public class HashSet<T> : ISet<T>, ISerializable
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
                throw new ArgumentNullException("collection");
            }
            else
            {
                _wrapped = new NullAwareDictionary<T, object>();
                foreach (T item in collection)
                {
                    _wrapped[item] = null;
                }
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
                throw new ArgumentNullException("collection");
            }
            else
            {
                _wrapped = new NullAwareDictionary<T, object>(comparer);
                foreach (T item in collection)
                {
                    _wrapped[item] = null;
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        protected HashSet(SerializationInfo info, StreamingContext context)
            : this()
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            else
            {
                _wrapped.GetObjectData(info, context);
            }
        }

        public IEqualityComparer<T> Comparer
        {
            get
            {
                return _wrapped.Comparer;
            }
        }

        public int Count
        {
            get
            {
                return _wrapped.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public static IEqualityComparer<HashSet<T>> CreateSetComparer()
        {
            return HashSetEqualityComparer.Instance;
        }

        public bool Add(T item)
        {
            if (_wrapped.ContainsKey(item))
            {
                return false;
            }
            else
            {
                _wrapped[item] = null;
                return true;
            }
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public bool Contains(T item)
        {
            return _wrapped.ContainsKey(item);
        }

        public void CopyTo(T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            else if (Count > array.Length)
            {
                throw new ArgumentException("the Count property is larger than the size of the destination array.");
            }
            else
            {
                _wrapped.Keys.CopyTo(array, 0);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex < 0");
            }
            if (Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
            _wrapped.Keys.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Non-negative number is required.");
            }
            if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", "array");
            }
            else
            {
                int copiedCount = 0;
                int currentIndex = arrayIndex;
                foreach (T item in this)
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
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            else
            {
                foreach (T item in other)
                {
                    _wrapped.Remove(item);
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            else
            {
                _wrapped.GetObjectData(info, context);
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

        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            else
            {
                this.IntersectWith(other, _wrapped.Comparer);
            }
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
                throw new ArgumentNullException("other");
            }
            foreach (T item in other)
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
            if (_wrapped.ContainsKey(item))
            {
                return _wrapped.Remove(item);
            }
            else
            {
                return false;
            }
        }

        public int RemoveWhere(Predicate<T> match)
        {
            return Extensions.RemoveWhere(this, match);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            int containsCount = 0;
            foreach (T item in ToHashSet(other))
            {
                if (!_wrapped.ContainsKey(item))
                {
                    return false;
                }
                else
                {
                    containsCount++;
                }
            }
            return containsCount == _wrapped.Count;
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            var tmpSet = new HashSet<T>(other);
            foreach (T item in tmpSet)
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

        public void TrimExcess()
        {
            //Empty
        }

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            foreach (T item in other)
            {
                if (!_wrapped.ContainsKey(item))
                {
                    _wrapped[item] = null;
                }
            }
        }

        private bool IsSubsetOf(IEnumerable<T> other, bool proper)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            else
            {
                int elementCount = 0;
                int matchCount = 0;
                foreach (T item in other)
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
                else
                {
                    return matchCount == _wrapped.Count;
                }
            }
        }

        private bool IsSupersetOf(IEnumerable<T> other, bool proper)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            else
            {
                int elementCount = 0;
                foreach (T item in other)
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
                else
                {
                    return true;
                }
            }
        }

        private IEnumerable<T> ToHashSet(IEnumerable<T> other)
        {
            var test = other as HashSet<T>;
            var comparer = Comparer;
            if (test != null && comparer.Equals(test.Comparer))
            {
                return test;
            }
            else
            {
                return new HashSet<T>(other, comparer);
            }
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly IEnumerator<KeyValuePair<T, object>> _enumerator;
            private bool valid;
            private T current;

            public Enumerator(HashSet<T> hashSet)
            {
                _enumerator = hashSet._wrapped.GetEnumerator();
                valid = false;
                current = default(T);
            }

            public T Current
            {
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (valid)
                    {
                        return current;
                    }
                    else
                    {
                        throw new InvalidOperationException("Call MoveNext first or use IEnumerator<T>");
                    }
                }
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            void IEnumerator.Reset()
            {
                valid = false;
                current = _enumerator.Current.Key;
                _enumerator.Reset();
            }

            public bool MoveNext()
            {
                valid = _enumerator.MoveNext();
                current = _enumerator.Current.Key;
                return valid;
            }
        }

        private sealed class HashSetEqualityComparer : IEqualityComparer<HashSet<T>>
        {
            public static readonly HashSetEqualityComparer Instance = new HashSetEqualityComparer();

            public bool Equals(HashSet<T> left, HashSet<T> right)
            {
                if (left == right)
                {
                    return true;
                }
                else
                {
                    if (left == null || right == null || left.Count != right.Count)
                    {
                        return false;
                    }
                    else
                    {
                        foreach (var item in left)
                        {
                            if (!right.Contains(item))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }

            public int GetHashCode(HashSet<T> hashset)
            {
                try
                {
                    IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
                    int hash = 0;
                    foreach (var item in hashset)
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