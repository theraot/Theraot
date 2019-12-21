#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Collections;

namespace System.Linq.Expressions.Interpreter
{
    [DebuggerTypeProxy(typeof(DebugView))]
    internal readonly struct InstructionArray
    {
        // list of (instruction index, cookie) sorted by instruction index:
        internal readonly KeyValuePair<int, object?>[] DebugCookies;

        internal readonly Instruction[] Instructions;
        internal readonly RuntimeLabel[] Labels;
        internal readonly int MaxContinuationDepth;
        internal readonly int MaxStackDepth;
        internal readonly object[] Objects;

        internal InstructionArray(int maxStackDepth, int maxContinuationDepth, Instruction[] instructions,
            object[] objects, RuntimeLabel[] labels, IEnumerable<KeyValuePair<int, object?>>? debugCookies)
        {
            MaxStackDepth = maxStackDepth;
            MaxContinuationDepth = maxContinuationDepth;
            Instructions = instructions;
            DebugCookies = debugCookies.AsArrayInternal();
            Objects = objects;
            Labels = labels;
        }

        internal sealed class DebugView
        {
            private readonly InstructionArray _array;

            public DebugView(InstructionArray array)
            {
                ContractUtils.RequiresNotNull(array, nameof(array));
                _array = array;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public InstructionList.DebugView.InstructionView[] A0 => GetInstructionViews(true);

            public InstructionList.DebugView.InstructionView[] GetInstructionViews(bool includeDebugCookies = false)
            {
                return InstructionList.DebugView.GetInstructionViews
                (
                    _array.Instructions,
                    _array.Objects,
                    index => _array.Labels[index].Index,
                    includeDebugCookies ? _array.DebugCookies : null
                );
            }
        }
    }
}

#endif