#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Theraot.Collections.ThreadSafe;

namespace System.Dynamic.Utils
{
    internal static class EmptyReadOnlyCollection<T>
    {
        public static readonly ReadOnlyCollection<T> Instance = new TrueReadOnlyCollection<T>(ArrayReservoir<T>.EmptyArray);
    }
}

#endif