#if LESSTHAN_NET45

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Collections.Generic
{
    /// <summary>
    ///     Helper type for avoiding allocations while building arrays.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    internal struct ArrayBuilder<T>
    {
        /// <summary>
        ///     Initializes the <see cref="ArrayBuilder{T}" /> with a specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity of the array to allocate.</param>
        public ArrayBuilder(int capacity)
            : this()
        {
            Debug.Assert(capacity >= 0);
            if (capacity > 0)
            {
                Buffer = new T[capacity];
            }
            else
            {
                Buffer = ArrayEx.Empty<T>();
            }
        }

        /// <summary>Gets the current underlying array.</summary>
        private T[] Buffer { get; }

        /// <summary>
        ///     Gets the number of items this instance can store without re-allocating,
        ///     or 0 if the backing array is <c>null</c>.
        /// </summary>
        private int Capacity => Buffer.Length;

        /// <summary>
        ///     Gets the number of items in the array currently in use.
        /// </summary>
        private int Count { get; set; }

        /// <summary>
        ///     Creates an array from the contents of this builder.
        /// </summary>
        /// <remarks>
        ///     Do not call this method twice on the same builder.
        /// </remarks>
        public T[] ToArray()
        {
            if (Count == 0)
            {
                return ArrayEx.Empty<T>();
            }

            if (Count == Buffer.Length)
            {
                return Buffer;
            }

            // Avoid a bit of overhead (method call, some branches, extra code-gen)
            // which would be incurred by using Array.Resize
            var result = new T[Count];
            Array.Copy(Buffer, 0, result, 0, Count);
            return result;
        }

        /// <summary>
        ///     Adds an item to the backing array, without checking if there is room.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        ///     Use this method if you know there is enough space in the <see cref="ArrayBuilder{T}" />
        ///     for another item, and you are writing performance-sensitive code.
        /// </remarks>
        public void UncheckedAdd(T item)
        {
            Debug.Assert(Count < Capacity);

            Buffer[Count++] = item;
        }
    }
}

#endif