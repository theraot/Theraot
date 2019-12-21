#if LESSTHAN_NET35

#pragma warning disable RCS1157 // Composite enum value contains undefined flag.
#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions.Compiler
{
    internal sealed partial class StackSpiller
    {
        /// <summary>
        ///     Should the parent nodes be rewritten, and in what way?
        /// </summary>
        /// <remarks>
        ///     Designed so bitwise-or produces the correct result when merging two
        ///     subtrees. In particular, SpillStack is preferred over Copy which is
        ///     preferred over None.
        /// </remarks>
        [Flags]
        private enum RewriteAction
        {
            /// <summary>
            ///     No rewrite needed.
            /// </summary>
            None = 0,

            /// <summary>
            ///     Copy into a new node.
            /// </summary>
            Copy = 1,

            /// <summary>
            ///     Spill stack into temps.
            /// </summary>
            SpillStack = 3
        }
    }
}

#endif