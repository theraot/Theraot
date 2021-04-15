#if LESSTHAN_NET40

#pragma warning disable CA1812 // Avoid uninstantiated internal classes

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Dynamic.Utils
{
    internal abstract class ListProvider<T> : IList<T>
        where T : class
    {
        public int Count => ElementCount;

        public bool IsReadOnly => true;
        protected abstract int ElementCount { get; }
        protected abstract T First { get; }

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

        public void CopyTo(T[] array, int arrayIndex)
        {
            ContractUtils.RequiresNotNull(array, nameof(array));
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            var n = ElementCount;
            Debug.Assert(n > 0);
            if (arrayIndex + n > array.Length)
            {
                throw new ArgumentException(string.Empty, nameof(array));
            }

            array[arrayIndex++] = First;
            for (var i = 1; i < n; i++)
            {
                array[arrayIndex++] = GetElement(i);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        protected abstract T GetElement(int index);
    }
}

#endif