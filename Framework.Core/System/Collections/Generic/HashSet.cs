#if LESSTHAN_NET35

#pragma warning disable CC0091 // Use static method

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Threading.Needles;

namespace System.Collections.Generic
{
    [Serializable]
    public class HashSet<T> : ISet<T>, IReadOnlyCollection<T>, ISerializable
    {
        private readonly NullAwareDictionary<T, T> _wrapped;

        public HashSet()
        {
            _wrapped = new NullAwareDictionary<T, T>();
        }

        public HashSet(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _wrapped = new NullAwareDictionary<T, T>();
            foreach (var item in collection)
            {
                _wrapped[item] = item;
            }
        }

        public HashSet(IEqualityComparer<T> comparer)
        {
            _wrapped = new NullAwareDictionary<T, T>(comparer);
        }

        public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _wrapped = new NullAwareDictionary<T, T>(comparer);
            foreach (var item in collection)
            {
                _wrapped[item] = item;
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

            _ = context;
            var dictionary = info.GetValue("dictionary", typeof(KeyValuePair<T, T>[])) as KeyValuePair<T, T>[] ?? ArrayEx.Empty<KeyValuePair<T, T>>();
            var comparer = info.GetValue("comparer", typeof(IEqualityComparer<T>)) as IEqualityComparer<T> ?? EqualityComparer<T>.Default;
            _wrapped = new NullAwareDictionary<T, T>(dictionary, comparer);
        }

        public IEqualityComparer<T> Comparer => _wrapped.Comparer;

        public int Count => _wrapped.Count;

        public bool IsReadOnly => false;

        public static IEqualityComparer<HashSet<T>> CreateSetComparer()
        {
            return HashSetEqualityComparer<T>.Instance;
        }

        public bool Add(T item)
        {
            if (_wrapped.ContainsKey(item))
            {
                return false;
            }

            _wrapped[item] = item;
            return true;
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public bool Contains([AllowNull] T item)
        {
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

            _wrapped.Keys.ConvertedCopyTo(item => item.Value, array, arrayIndex);
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

            _wrapped.Keys.ConvertedCopyTo(item => item.Value, array, 0);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            _wrapped.Deconstruct(out var dictionary, out var comparer);
            info.AddValue(nameof(dictionary), dictionary);
            info.AddValue(nameof(comparer), comparer);
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
            return IsSubsetOf(other.ToHashSet(Comparer), true);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(other.ToHashSet(Comparer), true);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return IsSubsetOf(other.ToHashSet(Comparer), false);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(other.ToHashSet(Comparer), false);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return other.Any(item => _wrapped.ContainsKey(item));
        }

        public bool Remove([AllowNull] T item)
        {
            // item can be null
            return _wrapped.Remove(item);
        }

        public int RemoveWhere(Predicate<T> match)
        {
            return this.RemoveWhere(new Func<T, bool>(match));
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var containsCount = 0;
            foreach (var item in other.ToHashSet(Comparer))
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

            foreach (var item in new HashSet<T>(other))
            {
                if (_wrapped.ContainsKey(item))
                {
                    _wrapped.Remove(item);
                }
                else
                {
                    _wrapped[item] = item;
                }
            }
        }

        public void TrimExcess()
        {
            // Should not be static
            // Empty
        }

        public bool TryGetValue(T equalValue, out T actualValue)
        {
            if (_wrapped.TryGetValue(equalValue, out actualValue))
            {
                return true;
            }

            actualValue = default!;
            return false;
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
                    _wrapped[item] = item;
                }
            }
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

        public struct Enumerator : IEnumerator<T>
        {
            private readonly IEnumerator<KeyValuePair<ReadOnlyStructNeedle<T>, T>> _enumerator;
            private bool _valid;

            internal Enumerator(HashSet<T> hashSet)
            {
                _enumerator = hashSet._wrapped.GetEnumerator();
                _valid = false;
                Current = default!;
            }

            public T Current { get; private set; }

            object IEnumerator.Current
            {
                get
                {
                    if (_valid)
                    {
                        return Current!;
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
                if (enumerator == null)
                {
                    return false;
                }

                _valid = _enumerator.MoveNext();
                Current = _enumerator.Current.Key.Value;
                return _valid;
            }

            void IEnumerator.Reset()
            {
                _valid = false;
                var enumerator = _enumerator;
                if (enumerator == null)
                {
                    return;
                }

                Current = _enumerator.Current.Key.Value;
                _enumerator.Reset();
            }
        }
    }
}

#endif