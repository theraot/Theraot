// Needed for NET40

using System;
using System.Collections.Generic;
using System.Linq;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    public partial class ProgressiveList<T> : ProgressiveCollection<T>, IReadOnlyList<T>, IList<T>
    {
        private readonly IList<T> _cache;

        public ProgressiveList(IEnumerable<T> wrapped)
            : this(wrapped, new List<T>(), null)
        {
            // Empty
        }

        public ProgressiveList(Progressor<T> wrapped)
            : this(wrapped, new List<T>(), null)
        {
            // Empty
        }

        public ProgressiveList(IEnumerable<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new List<T>(), comparer)
        {
            // Empty
        }

        public ProgressiveList(Progressor<T> wrapped, IEqualityComparer<T> comparer)
            : this(wrapped, new List<T>(), comparer)
        {
            // Empty
        }

        protected ProgressiveList(IEnumerable<T> wrapped, IList<T> cache, IEqualityComparer<T> comparer)
            : base(wrapped, cache, comparer)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cache = cache;
        }

        protected ProgressiveList(Progressor<T> wrapped, IList<T> cache, IEqualityComparer<T> comparer)
            : base(wrapped, cache, comparer)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cache = cache;
        }

        public T this[int index]
        {
            get
            {
                if (index >= _cache.Count)
                {
                    Progressor.While(() => _cache.Count < index + 1).Consume();
                }
                return _cache[index];
            }
        }

        T IList<T>.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public int IndexOf(T item)
        {
            int cacheIndex = _cache.IndexOf(item, Comparer);
            if (cacheIndex >= 0)
            {
                return cacheIndex;
            }
            else
            {
                int index = _cache.Count - 1;
                bool found = false;
                Progressor.While
                (
                    input =>
                    {
                        index++;
                        if (Comparer.Equals(input, item))
                        {
                            found = true;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                ).Consume();
                if (found)
                {
                    return index;
                }
                else
                {
                    return -1;
                }
            }
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        protected override bool CacheContains(T item)
        {
            return _cache.Contains(item, Comparer);
        }
    }
}