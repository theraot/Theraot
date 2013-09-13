#if NET20 || NET30

using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Collections.Generic
{
    [Serializable]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Backport")]
    public class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ISerializable, IDeserializationCallback, ISet<T>
    {
        private readonly Dictionary<T, object> _wrapped;

        public HashSet()
        {
            _wrapped = new Dictionary<T, object>();
        }

        public HashSet(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            else
            {
                _wrapped = new Dictionary<T, object>();
                foreach (T item in collection)
                {
                    _wrapped[item] = null;
                }
            }
        }

        public HashSet(IEqualityComparer<T> comparer)
        {
            _wrapped = new Dictionary<T, object>(comparer);
        }

        public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            else
            {
                _wrapped = new Dictionary<T, object>(comparer);
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
            if (arrayIndex >= array.Length)
            {
                throw new ArgumentException("arrayIndex is greater than the length of the destination array.");
            }
            if (Count > array.Length)
            {
                throw new ArgumentException("the Count property is larger than the size of the destination array.");
            }
            _wrapped.Keys.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            else if (arrayIndex < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex < 0");
            }
            else if (arrayIndex >= array.Length)
            {
                throw new ArgumentException("arrayIndex is greater than the length of the destination array.");
            }
            else if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("count is greater than the available space from the index to the end of the destination array.");
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
                _wrapped.Clear();
                foreach (T item in other)
                {
                    _wrapped[item] = null;
                }
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return IsSubsetOf(other, true);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(other, true);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return IsSubsetOf(other, false);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(other, false);
        }

        public void OnDeserialization(object sender)
        {
            _wrapped.OnDeserialization(sender);
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
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            else
            {
                int removeCount = 0;
                foreach (KeyValuePair<T, object> item in _wrapped)
                {
                    if (match(item.Key))
                    {
                        if (_wrapped.Remove(item.Key))
                        {
                            removeCount++;
                        }
                    }
                }
                return removeCount;
            }
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            int containsCount = 0;
            foreach (T item in Enumerable.Distinct(other))
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

        private struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private Dictionary<T, object>.Enumerator _enumerator;

            public Enumerator(HashSet<T> hashSet)
            {
                _enumerator = hashSet._wrapped.GetEnumerator();
            }

            public T Current
            {
                get
                {
                    return _enumerator.Current.Key;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return _enumerator.Current.Key;
                }
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            void IEnumerator.Reset()
            {
                (_enumerator as IEnumerator).Reset();
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }
        }
    }
}

#endif