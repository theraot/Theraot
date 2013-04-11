#if NET20 || NET30 || NET35

using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Core;

namespace System.Collections.Generic
{
    [SerializableAttribute]
    public class SortedSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, ICollection, IEnumerable, ISerializable, IDeserializationCallback
    {
        private IComparer<T> _comparer;
        private SerializationInfo _serializationInfo;
        private AVLTree<T, T> _wrapped;

        public SortedSet()
        {
            _wrapped = new AVLTree<T, T>();
        }

        public SortedSet(IComparer<T> comparer)
        {
            _comparer = Check.NotNullArgument(comparer, "comparer");
            _wrapped = new AVLTree<T, T>(_comparer);
        }

        public SortedSet(IEnumerable<T> collection)
        {
            _comparer = Comparer<T>.Default;
            _wrapped = new AVLTree<T, T>();
            foreach (var item in Check.NotNullArgument(collection, "collection"))
            {
                _wrapped.AddNonDuplicate(item, item);
            }
        }

        public SortedSet(IEnumerable<T> collection, IComparer<T> comparer)
        {
            _comparer = comparer ?? Comparer<T>.Default;
            _wrapped = new AVLTree<T, T>();
            foreach (var item in Check.NotNullArgument(collection, "collection"))
            {
                _wrapped.AddNonDuplicate(item, item);
            }
        }

        protected SortedSet(SerializationInfo info, StreamingContext context)
        {
            _serializationInfo = info;
        }

        public IComparer<T> Comparer
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
                return GetCount();
            }
        }

        public T Max
        {
            get
            {
                return GetMax();
            }
        }

        public T Min
        {
            get
            {
                return GetMin();
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns false")]
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns this")]
        object ICollection.SyncRoot
        {
            get
            {
                return this;
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

        public bool Add(T item)
        {
            return AddExtracted(item);
        }

        public virtual void Clear()
        {
            _wrapped.Clear();
        }

        public virtual bool Contains(T item)
        {
            return _wrapped.Search(item) != null;
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
            Extensions.DeprecatedCopyTo(this, array, index);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            Extensions.ExceptWith(this, other);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumeratorExtracted();
        }

        public virtual SortedSet<T> GetViewBetween(T lowerValue, T upperValue)
        {
            if (Comparer.Compare(lowerValue, upperValue) <= 0)
            {
                return new SortedSubSet(this, lowerValue, upperValue);
            }
            else
            {
                throw new ArgumentException("lowerBound is greater than upperBound.");
            }
        }

        void ICollection<T>.Add(T item)
        {
            AddExtracted(item);
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            OnDeserialization(sender);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void IntersectWith(IEnumerable<T> other)
        {
            Extensions.IntersectWith(this, other);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
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

        internal virtual bool AddExtracted(T item)
        {
            return _wrapped.AddNonDuplicate(item, item);
        }

        internal virtual int GetCount()
        {
            return _wrapped.Count;
        }

        internal virtual IEnumerator<T> GetEnumeratorExtracted()
        {
            return _wrapped.ConvertProgressive(input => input.Key).GetEnumerator();
        }

        internal virtual T GetMax()
        {
            var node = _wrapped.Root;
            if (node == null)
            {
                return default(T);
            }
            else
            {
                while (node.Right != null)
                {
                    node = node.Right;
                }
                return node.Key;
            }
        }

        internal virtual T GetMin()
        {
            var node = _wrapped.Root;
            if (node == null)
            {
                return default(T);
            }
            else
            {
                while (node.Left != null)
                {
                    node = node.Left;
                }
                return node.Key;
            }
        }

        internal virtual bool RemoveExtracted(T item)
        {
            return _wrapped.Remove(item);
        }

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var _info = Check.NotNullArgument(info, "info");
            info.AddValue("Comparer", _comparer, typeof(IComparer<T>));
            info.AddValue("Count", Count);
            if (Count > 0)
            {
                info.AddValue("Items", this.ToArray(), typeof(T[]));
            }
            info.AddValue("Version", 0);
        }

        protected virtual void OnDeserialization(object sender)
        {
            if (_comparer == null)
            {
                if (_serializationInfo == null)
                {
                    throw new SerializationException();
                }
                else
                {
                    _comparer = (IComparer<T>)_serializationInfo.GetValue("Comparer", typeof(IComparer<T>));
                    int count = _serializationInfo.GetInt32("Count");
                    if (count != 0)
                    {
                        var value = (T[])_serializationInfo.GetValue("Items", typeof(T[]));
                        if (value == null)
                        {
                            throw new SerializationException();
                        }
                        else
                        {
                            for (int index = 0; index < (int)value.Length; index++)
                            {
                                Add(value[index]);
                            }
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
        }

        [Serializable]
        private sealed class SortedSubSet : SortedSet<T>, IEnumerable<T>, IEnumerable
        {
            private new SortedSet<T> _wrapped;

            private T _lower;
            private T _upper;

            public SortedSubSet(SortedSet<T> set, T lower, T upper)
                : base(set.Comparer)
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
                else
                {
                    return _wrapped.Contains(item);
                }
            }

            public override SortedSet<T> GetViewBetween(T lowerValue, T upperValue)
            {
                if (Comparer.Compare(lowerValue, upperValue) > 0)
                {
                    throw new ArgumentException("The lowerValue is bigger than upperValue");
                }
                if (!InRange(lowerValue))
                {
                    throw new ArgumentOutOfRangeException("lowerValue");
                }
                if (!InRange(upperValue))
                {
                    throw new ArgumentOutOfRangeException("upperValue");
                }
                return new SortedSubSet(_wrapped, lowerValue, upperValue);
            }

            public override void IntersectWith(IEnumerable<T> other)
            {
                if (other == null)
                {
                    throw new ArgumentNullException("other");
                }
                else
                {
                    var slice = new SortedSet<T>(this);
                    slice.IntersectWith(other);
                    Clear();
                    _wrapped.UnionWith(slice);
                }
            }

            internal override bool AddExtracted(T item)
            {
                if (!InRange(item))
                {
                    throw new ArgumentOutOfRangeException("item");
                }
                else
                {
                    return _wrapped.AddExtracted(item);
                }
            }

            internal override int GetCount()
            {
                return System.Linq.Enumerable.Count(_wrapped._wrapped.Range(_lower, _upper));
            }

            internal override IEnumerator<T> GetEnumeratorExtracted()
            {
                return _wrapped._wrapped.Range(_lower, _upper).ConvertProgressive(input => input.Key).GetEnumerator();
            }

            internal override T GetMax()
            {
                var bound = _wrapped._wrapped.SearchNearestLeft(_upper);
                if (bound == null || Comparer.Compare(_lower, bound.Key) > 0)
                {
                    return default(T);
                }
                else
                {
                    return bound.Key;
                }
            }

            internal override T GetMin()
            {
                var bound = _wrapped._wrapped.SearchNearestRight(_lower);
                if (bound == null || Comparer.Compare(_upper, bound.Key) < 0)
                {
                    return default(T);
                }
                else
                {
                    return bound.Key;
                }
            }

            internal override bool RemoveExtracted(T item)
            {
                if (!InRange(item))
                {
                    return false;
                }
                else
                {
                    return _wrapped.RemoveExtracted(item);
                }
            }

            protected override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                {
                    throw new ArgumentNullException("info");
                }
                else
                {
                    info.AddValue("Max", _lower, typeof(T));
                    info.AddValue("Min", _upper, typeof(T));
                    info.AddValue("lBoundActive", true);
                    info.AddValue("uBoundActive", true);
                    base.GetObjectData(info, context);
                }
            }

            protected override void OnDeserialization(object sender)
            {
                base.OnDeserialization(sender);
            }

            private bool InRange(T item)
            {
                return Comparer.Compare(item, _lower) >= 0 && Comparer.Compare(item, _upper) <= 0;
            }
        }
    }
}

#endif