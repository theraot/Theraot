#if LESSTHAN_NET40

#pragma warning disable CA1721 // Property names should not match get methods
#pragma warning disable S927 // parameter names should match base declaration and other partial definitions

using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Theraot;
using Theraot.Collections;
using Theraot.Collections.Specialized;

namespace System.Collections.Generic
{
    [Serializable]
    public class SortedSet<T> : ISet<T>, ICollection, ISerializable, IDeserializationCallback
    {
        private readonly AVLTree<T, VoidStruct> _wrapped;

        public SortedSet()
        {
            Comparer = Comparer<T>.Default;
            _wrapped = new AVLTree<T, VoidStruct>();
        }

        public SortedSet(IComparer<T> comparer)
        {
            Comparer = comparer ?? Comparer<T>.Default;
            _wrapped = new AVLTree<T, VoidStruct>(Comparer);
        }

        public SortedSet(IEnumerable<T> collection)
        {
            Comparer = Comparer<T>.Default;
            _wrapped = new AVLTree<T, VoidStruct>();
            foreach (var item in collection ?? throw new ArgumentNullException(nameof(collection)))
            {
                _wrapped.AddNonDuplicate(item, default);
            }
        }

        public SortedSet(IEnumerable<T> collection, IComparer<T> comparer)
        {
            Comparer = comparer ?? Comparer<T>.Default;
            _wrapped = new AVLTree<T, VoidStruct>();
            foreach (var item in collection ?? throw new ArgumentNullException(nameof(collection)))
            {
                _wrapped.AddNonDuplicate(item, default);
            }
        }

        protected SortedSet(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            _ = context;
            Comparer = (IComparer<T>)info.GetValue(nameof(Comparer), typeof(IComparer<T>));
            _wrapped = new AVLTree<T, VoidStruct>(Comparer);
            var count = info.GetInt32(nameof(Count));
            if (count != 0)
            {
                var value = (T[])info.GetValue("Items", typeof(T[]));
                if (value == null)
                {
                    throw new SerializationException();
                }

                foreach (var item in value)
                {
                    Add(item);
                }
            }

            info.GetInt32("Version");
            if (Count != count)
            {
                throw new SerializationException();
            }
        }

        private SortedSet(AVLTree<T, VoidStruct> wrapped, IComparer<T> comparer)
        {
            _wrapped = wrapped ?? new AVLTree<T, VoidStruct>();
            Comparer = comparer ?? Comparer<T>.Default;
        }

        public IComparer<T> Comparer { get; }

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

        public void CopyTo(Array array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            this.DeprecatedCopyTo(array, index);
        }

        public void CopyTo(T[] array, int index)
        {
            Extensions.CanCopyTo(Count, array, index);
            Extensions.CopyTo(this, array, index);
        }

        public void CopyTo(T[] array)
        {
            Extensions.CanCopyTo(Count, array);
            Extensions.CopyTo(this, array);
        }

        public void CopyTo(T[] array, int index, int count)
        {
            Extensions.CanCopyTo(array, index, count);
            Extensions.CopyTo(this, array, index, count);
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
            _ = sender;
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
            return this.RemoveWhere(new Func<T, bool>(match));
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

        public bool TryGetValue(T equalValue, out T actualValue)
        {
            var node = _wrapped.Get(equalValue);
            if (node != null)
            {
                actualValue = node.Key;
                return true;
            }

            actualValue = default!;
            return false;
        }

        public void UnionWith(IEnumerable<T> other)
        {
            Extensions.UnionWith(this, other);
        }

        protected virtual bool AddExtracted(T item)
        {
            return _wrapped.AddNonDuplicate(item, default);
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
                return default!;
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
                return default!;
            }

            while (node.Left != null)
            {
                node = node.Left;
            }

            return node.Key;
        }

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ = context;
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Comparer), Comparer, typeof(IComparer<T>));
            info.AddValue(nameof(Count), Count);
            if (Count > 0)
            {
                info.AddValue("Items", this.ToArray(), typeof(T[]));
            }

            info.AddValue("Version", 0);
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
                return InRange(item) && _wrapped.Contains(item);
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
                    return default!;
                }

                return bound.Key;
            }

            protected override T GetMin()
            {
                var bound = _wrapped._wrapped.GetNearestRight(_lower);
                if (bound == null || Comparer.Compare(_upper, bound.Key) < 0)
                {
                    return default!;
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
                return InRange(item) && _wrapped.RemoveExtracted(item);
            }

            private bool InRange(T item)
            {
                return Comparer.Compare(item, _lower) >= 0 && Comparer.Compare(item, _upper) <= 0;
            }
        }
    }
}

#endif