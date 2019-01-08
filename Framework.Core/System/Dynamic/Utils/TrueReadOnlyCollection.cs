#if LESSTHAN_NET40

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Runtime.CompilerServices
{
    internal sealed class TrueReadOnlyCollection<T> : ReadOnlyCollection<T>, IReadOnlyList<T>
    {
        /// <summary>
        /// Creates instance of TrueReadOnlyCollection, wrapping passed in array.
        /// !!! DOES NOT COPY THE ARRAY !!!
        /// </summary>
        public TrueReadOnlyCollection(params T[] list)
            : base(list)
        {
            Wrapped = list;
        }

        public T[] Wrapped { get; }
    }
}

#endif