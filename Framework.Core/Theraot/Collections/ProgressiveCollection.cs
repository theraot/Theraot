// Needed for NET40

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Theraot.Collections.Specialized;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public sealed class ProgressiveCollection<T> : IReadOnlyCollection<T>, ICollection<T>, IHasComparer<T>, IProgressive<T>, IClosable
    {
        private readonly IDisposable _subscription;

        private ProgressiveCollection(Progressor<T> progressor, ICollection<T> cache, IEqualityComparer<T>? comparer)
        {
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Progressor = progressor ?? throw new ArgumentNullException(nameof(progressor));
            _subscription = Progressor.SubscribeAction(obj => Cache.Add(obj));
            Comparer = comparer ?? EqualityComparer<T>.Default;
        }

        ~ProgressiveCollection()
        {
            Close();
        }

        public ICollection<T> Cache { get; }
        public IEqualityComparer<T> Comparer { get; }

        public int Count
        {
            get
            {
                Progressor.Consume();
                return Cache.Count;
            }
        }

        public bool IsClosed => Progressor.IsClosed;
        bool ICollection<T>.IsReadOnly => true;
        private Progressor<T> Progressor { get; }

        public static ProgressiveCollection<T> Create(Progressor<T> progressor, ICollection<T> cache, IEqualityComparer<T>? comparer)
        {
            return new ProgressiveCollection<T>(progressor, cache, comparer);
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public void Close()
        {
            _subscription?.Dispose();
            Progressor?.Close();
            GC.SuppressFinalize(this);
        }

        public bool Contains(T item)
        {
            return Cache.Contains(item) || Progress().Any(Check);

            bool Check(T found)
            {
                return Comparer.Equals(item, found);
            }
        }

        public void CopyTo(T[] array)
        {
            Progressor.Consume();
            Cache.CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Progressor.Consume();
            Cache.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Progressor.While(() => Cache.Count < countLimit).Consume();
            Cache.CopyTo(array, arrayIndex, countLimit);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in Cache)
            {
                yield return item;
            }

            var knownCount = Cache.Count;
            while (Progressor.TryTake(out var item))
            {
                if (Cache.Count <= knownCount)
                {
                    continue;
                }

                yield return item;

                knownCount = Cache.Count;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> Progress()
        {
            var knownCount = Cache.Count;
            while (Progressor.TryTake(out var item))
            {
                if (Cache.Count <= knownCount)
                {
                    continue;
                }

                yield return item;

                knownCount = Cache.Count;
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }
    }
}