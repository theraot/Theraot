#if LESSTHAN_NET40 || NETSTANDARD1_0

// BASEDON: https://github.com/dotnet/corefx/blob/e0ba7aa8026280ee3571179cc06431baf1dfaaac/src/System.Threading.Tasks.Parallel/src/System/Threading/Tasks/ParallelRangeManager.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// Implements the algorithm for distributing loop indices to parallel loop workers
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Represents an index range
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    internal struct IndexRange
    {
        // the From and To values for this range. These do not change.
        internal long FromInclusive;
        internal long ToExclusive;

        // The shared index, stored as the offset from nFromInclusive. Using an offset rather than the actual
        // value saves us from overflows that can happen due to multiple workers racing to increment this.
        // All updates to this field need to be interlocked.  To avoid split interlockeds across cache-lines
        // in 32-bit processes, in 32-bit processes when the range fits in a 32-bit value, we prefer to use
        // a 32-bit field, and just use the first 32-bits of the long.  And to minimize false sharing, each
        // value is stored in its own heap-allocated object, which is lazily allocated by the thread using
        // that range, minimizing the chances it'll be near the objects from other threads.
        internal volatile StrongBox<long>? SharedCurrentIndexOffset;

        // to be set to 1 by the worker that finishes this range. It's OK to do a non-interlocked write here.
        internal int RangeFinished;
    }
}

#endif