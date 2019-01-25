#if LESSTHAN_NET40

#pragma warning disable CA1812 // Avoid uninstantiated internal classes

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
    /// <summary>
    ///     Provides a wrapper around an IArgumentProvider which exposes the argument providers
    ///     members out as an IList of Expression.  This is used to avoid allocating an array
    ///     which needs to be stored inside of a ReadOnlyCollection.  Instead this type has
    ///     the same amount of overhead as an array without duplicating the storage of the
    ///     elements.  This ensures that internally we can avoid creating and copying arrays
    ///     while users of the Expression trees also don't pay a size penalty for this internal
    ///     optimization.  See IArgumentProvider for more general information on the Expression
    ///     tree optimizations being used here.
    /// </summary>
    internal sealed class ListArgumentProvider : ListProvider<Expression>
    {
        private readonly IArgumentProvider _provider;

        internal ListArgumentProvider(IArgumentProvider provider, Expression arg0)
        {
            _provider = provider;
            First = arg0;
        }

        protected override int ElementCount => _provider.ArgumentCount;

        protected override Expression First { get; }

        protected override Expression GetElement(int index)
        {
            return _provider.GetArgument(index);
        }
    }

    internal abstract class ListProvider<T> : IList<T>
        where T : class
    {
        protected abstract int ElementCount { get; }
        protected abstract T First { get; }

        public int Count => ElementCount;

        public bool IsReadOnly => true;

        public T this[int index]
        {
            get => index == 0 ? First : GetElement(index);
            set => throw ContractUtils.Unreachable;
        }

        public void Add(T item)
        {
            throw ContractUtils.Unreachable;
        }

        public void Clear()
        {
            throw ContractUtils.Unreachable;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int index)
        {
            ContractUtils.RequiresNotNull(array, nameof(array));
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var n = ElementCount;
            Debug.Assert(n > 0);
            if (index + n > array.Length)
            {
                throw new ArgumentException(string.Empty, nameof(array));
            }

            array[index++] = First;
            for (var i = 1; i < n; i++)
            {
                array[index++] = GetElement(i);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield return First;

            for (int i = 1, n = ElementCount; i < n; i++)
            {
                yield return GetElement(i);
            }
        }

        public int IndexOf(T item)
        {
            if (First == item)
            {
                return 0;
            }

            for (int i = 1, n = ElementCount; i < n; i++)
            {
                if (GetElement(i) == item)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            throw ContractUtils.Unreachable;
        }

        public bool Remove(T item)
        {
            throw ContractUtils.Unreachable;
        }

        public void RemoveAt(int index)
        {
            throw ContractUtils.Unreachable;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract T GetElement(int index);
    }
}

#endif