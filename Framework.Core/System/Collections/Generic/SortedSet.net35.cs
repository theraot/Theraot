#if LESSTHAN_NET40

using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Theraot.Collections;
using Theraot.Collections.Specialized;

namespace System.Collections.Generic
{
    [Serializable]
    public class SortedSet<T> : ISet<T>, ICollection, ISerializable, IDeserializationCallback
    {
        private readonly AVLTree<T, T> _wrapped;
        private SerializationInfo _serializationInfo;

        public SortedSet()
        {
            Comparer = Comparer<T>.Default;
            _wrapped = new AVLTree<T, T>();
        }

        public SortedSet(IComparer<T> comparer)
        {
            Comparer = comparer ?? Comparer<T>.Default;
            _wrapped = new AVLTree<T, T>(Comparer);
        }

        public SortedSet(IEnumerable<T> collection)
        {
            Comparer = Comparer<T>.Default;
            _wrapped = new AVLTree<T, T>();
            foreach (var item in collection ?? throw new ArgumentNullException(nameof(collection)))
            {
                _wrapped.AddNonDuplicate(item, item);
            }
        }

        public SortedSet(IEnumerable<T> collection, IComparer<T> comparer)
        {
            Comparer = comparer ?? Comparer<T>.Default;
            _wrapped = new AVLTree<T, T>();
            foreach (var item in collection ?? throw new ArgumentNullException(nameof(collection)))
            {
                _wrapped.AddNonDuplicate(item, item);
            }
        }

        protected SortedSet(AVLTree<T, T> wrapped, IComparer<T> comparer)
        {
            _wrapped = wrapped ?? new AVLTree<T, T>();
            Comparer = comparer ?? Comparer<T>.Default;
        }

        protected SortedSet(SerializationInfo info, StreamingContext context)
        {
            GC.KeepAlive(context);
            _serializationInfo = info;
        }

        public IComparer<T> Comparer { get; private set; }

        public int Count => GetCount();

        bool ICollection<T>.IsReadOnly => false;
        bool ICollection.IsSynchronized => false;

        public T Max => GetMax();
        public T Min => GetMin();
        object ICollection.SyncRoot => this;

        public bool Add(T item)
        {
            return AddExtracted(item);
        }

        void ICollection<T>.Add(T item)
        {
            AddExtracted(item);
        }

        public virtual void Clear()
        {
            _wrapped.Clear();
        }

        public virtual bool Contains(T item)
        {
            return _wrapped.Get(item) != null;
        }

        public void CopyTo(T[] array)
        {
            Extensions.CanCopyTo(Count, array);
            Extensions.CopyTo(this, array);
        }

        public void CopyTo(T[] array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            Extensions.CopyTo(this, array, index);
        }

        public void CopyTo(T[] array, int index, int count)
        {
            Extensions.CanCopyTo(array, index, count);
            Extensions.CopyTo(this, array, index, count);
        }

        public void CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            this.DeprecatedCopyTo(array, index);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            Extensions.ExceptWith(this, other);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumeratorExtracted();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        public virtual SortedSet<T> GetViewBetween(T lowerValue, T upperValue)
        {
            if (Comparer.Compare(lowerValue, upperValue) <= 0)
            {
                return new SortedSubSet(this, lowerValue, upperValue);
            }
            throw new ArgumentException("lowerBound is greater than upperBound.");
        }

        public virtual void IntersectWith(IEnumerable<T> other)
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

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            OnDeserialization(sender);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return Extensions.Overlaps(this, other);
        }

        public bool Remove(T item)
        {
            return RemoveExtracted(item);
        }

        public int RemoveWhere(Predicate<T> match)
        {
            return Extensions.RemoveWhere(this, match);
        }

        public IEnumerable<T> Reverse()
        {
            return Enumerable.Reverse(this);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return Extensions.SetEquals(this, other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            Extensions.SymmetricExceptWith(this, other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            Extensions.UnionWith(this, other);
        }

        protected virtual bool AddExtracted(T item)
        {
            return _wrapped.AddNonDuplicate(item, item);
        }

        protected virtual int GetCount()
        {
            return _wrapped.Count;
        }

        protected virtual IEnumerator<T> GetEnumeratorExtracted()
        {
            return _wrapped.ConvertProgressive(input => input.Key).GetEnumerator();
        }

        protected virtual T GetMax()
        {
            var node = _wrapped.Root;
            if (node == null)
            {
                return default;
            }
            while (node.Right != null)
            {
                node = node.Right;
            }
            return node.Key;
        }

        protected virtual T GetMin()
        {
            var node = _wrapped.Root;
            if (node == null)
            {
                return default;
            }
            while (node.Left != null)
            {
                node = node.Left;
            }
            return node.Key;
        }

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GC.KeepAlive(context);
            if (info == null)
            {
                throw new ArgumentNullException();
            }
            info.AddValue(nameof(Comparer), Comparer, typeof(IComparer<T>));
            info.AddValue(nameof(Count), Count);
            if (Count > 0)
            {
                info.AddValue("Items", Enumerable.ToArray(this), typeof(T[]));
            }
            info.AddValue("Version", 0);
        }

        protected virtual void OnDeserialization(object sender)
        {
            GC.KeepAlive(sender);
            if (Comparer == null)
            {
                if (_serializationInfo == null)
                {
                    throw new SerializationException();
                }
                Comparer = (IComparer<T>)_serializationInfo.GetValue(nameof(Comparer), typeof(IComparer<T>));
                var count = _serializationInfo.GetInt32(nameof(Count));
                if (count != 0)
                {
                    var value = (T[])_serializationInfo.GetValue("Items", typeof(T[]));
                    if (value == null)
                    {
                        throw new SerializationException();
                    }
                    foreach (var item in value)
                    {
                        Add(item);
                    }
                }
                _serializationInfo.GetInt32("Version");
                if (Count != count)
                {
                    throw new SerializationException();
                }
                _serializationInfo = null;
            }
        }

        protected virtual bool RemoveExtracted(T item)
        {
            return _wrapped.Remove(item);
        }

        [Serializable]
        private sealed class SortedSubSet : SortedSet<T>
        {
            private readonly T _lower;
            private readonly T _upper;
            private new readonly SortedSet<T> _wrapped;

            public SortedSubSet(SortedSet<T> set, T lower, T upper)
                : base(set._wrapped, set.Comparer)
            {
                _wrapped = set;
                _lower = lower;
                _upper = upper;
            }

            public override void Clear()
            {
                _wrapped.RemoveWhere(InRange);
            }

            public override bool Contains(T item)
            {
                if (!InRange(item))
                {
                    return false;
                }
                return _wrapped.Contains(item);
            }

            public override SortedSet<T> GetViewBetween(T lowerValue, T upperValue)
            {
                if (Comparer.Compare(lowerValue, upperValue) > 0)
                {
                    throw new ArgumentException("The lowerValue is bigger than upperValue");
                }
                if (!InRange(lowerValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(lowerValue));
                }
                if (!InRange(upperValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(upperValue));
                }
                return new SortedSubSet(_wrapped, lowerValue, upperValue);
            }

            public override void IntersectWith(IEnumerable<T> other)
            {
                if (other == null)
                {
                    throw new ArgumentNullException(nameof(other));
                }
                var slice = new SortedSet<T>(this);
                slice.IntersectWith(other);
                Clear();
                _wrapped.UnionWith(slice);
            }

            protected override bool AddExtracted(T item)
            {
                if (!InRange(item))
                {
                    throw new ArgumentOutOfRangeException(nameof(item));
                }
                return _wrapped.AddExtracted(item);
            }

            protected override int GetCount()
            {
                return _wrapped._wrapped.Range(_lower, _upper).Count();
            }

            protected override IEnumerator<T> GetEnumeratorExtracted()
            {
                return _wrapped._wrapped.Range(_lower, _upper).ConvertProgressive(input => input.Key).GetEnumerator();
            }

            protected override T GetMax()
            {
                var bound = _wrapped._wrapped.GetNearestLeft(_upper);
                if (bound == null || Comparer.Compare(_lower, bound.Key) > 0)
                {
                    return default;
                }
                return bound.Key;
            }

            protected override T GetMin()
            {
                var bound = _wrapped._wrapped.GetNearestRight(_lower);
                if (bound == null || Comparer.Compare(_upper, bound.Key) < 0)
                {
                    return default;
                }
                return bound.Key;
            }

            protected override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                {
                    throw new ArgumentNullException(nameof(info));
                }
                info.AddValue(nameof(Max), _lower, typeof(T));
                info.AddValue(nameof(Min), _upper, typeof(T));
                info.AddValue("lBoundActive", true);
                info.AddValue("uBoundActive", true);
                base.GetObjectData(info, context);
            }

            protected override bool RemoveExtracted(T item)
            {
                if (!InRange(item))
                {
                    return false;
                }
                return _wrapped.RemoveExtracted(item);
            }

            private bool InRange(T item)
            {
                return Comparer.Compare(item, _lower) >= 0 && Comparer.Compare(item, _upper) <= 0;
            }
        }
    }
}

#endif