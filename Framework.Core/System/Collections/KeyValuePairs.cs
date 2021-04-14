#if LESSTHAN_NETSTANDARD13

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Collections
{
    [DebuggerDisplay("{" + nameof(_value) + "}", Name = "[{" + nameof(_key) + "}]")]
    internal class KeyValuePairs
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object? _key;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object? _value;

        public KeyValuePairs(object? key, object? value)
        {
            _value = value;
            _key = key;
        }
    }
}

#endif