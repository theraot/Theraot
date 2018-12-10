// Needed for NET40

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public class ProgressiveList<T> : ProgressiveCollection<T>, IList<T>
    {
        private readonly IList<T> _cache;

        public ProgressiveList(IEnumerable<T> enumerable)
            : this(Progressor<T>.CreateFromIEnumerable(enumerable), new List<T>(), null)
        {
            // Empty
        }

        public ProgressiveList(IObservable<T> observable)
            : this(Progressor<T>.CreateFromIObservable(observable, null), new List<T>(), null)
        {
            // Empty
        }

        public ProgressiveList(IEnumerable<T> enumerable, IEqualityComparer<T> comparer)
            : this(Progressor<T>.CreateFromIEnumerable(enumerable), new List<T>(), comparer)
        {
            // Empty
        }

        public ProgressiveList(IObservable<T> observable, IEqualityComparer<T> comparer)
            : this(Progressor<T>.CreateFromIObservable(observable, null), new List<T>(), comparer)
        {
            // Empty
        }

        protected ProgressiveList(Progressor<T> progressor, IList<T> cache, IEqualityComparer<T> comparer)
            : base(progressor, cache, comparer)
        {
            _cache = cache;
            Cache = new ExtendedReadOnlyList<T>(_cache);
        }

        public new IReadOnlyList<T> Cache { get; }

        public T this[int index]
        {
            get
            {
                if (index >= _cache.Count)
                {
                    ProgressorWhile(_ => _cache.Count < index + 1).Consume();
                }
                return _cache[index];
            }
        }

        T IList<T>.this[int index]
        {
            get => this[index];

            set => throw new NotSupportedException();
        }

        public new static ProgressiveList<T> Create<TList>(Progressor<T> progressor, IEqualityComparer<T> comparer)
            where TList : IList<T>, new()
        {
            return new ProgressiveList<T>(progressor, new TList(), comparer);
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public int IndexOf(T item)
        {
            var cacheIndex = _cache.IndexOf(item, Comparer);
            if (cacheIndex >= 0)
            {
                return cacheIndex;
            }
            var index = _cache.Count - 1;
            var found = false;
            ProgressorWhile
            (
                input =>
                {
                    index++;
                    if (Comparer.Equals(input, item))
                    {
                        found = true;
                        return false;
                    }
                    return true;
                }
            ).Consume();
            if (found)
            {
                return index;
            }
            return -1;
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