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
    internal sealed partial class InstructionList
    {
        internal sealed class DebugView
        {
            private readonly InstructionList _list;

            public DebugView(InstructionList list)
            {
                ContractUtils.RequiresNotNull(list, nameof(list));
                _list = list;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public InstructionView[] A0 => GetInstructionViews(includeDebugCookies: true);

            public InstructionView[] GetInstructionViews(bool includeDebugCookies = false)
            {
                return GetInstructionViews
                (
                    _list._instructions,
                    _list._objects,
                    index => _list._labels![index].TargetIndex,
                    includeDebugCookies ? _list._debugCookies : null
                );
            }

            internal static InstructionView[] GetInstructionViews(IList<Instruction> instructions, IList<object>? objects,
                Func<int, int> labelIndexer, IList<KeyValuePair<int, object?>>? debugCookies)
            {
                var result = new List<InstructionView>();
                var stackDepth = 0;
                var continuationsDepth = 0;

                using
                (
                    var cookieEnumerator =
                        (
                            debugCookies ??
                            ArrayEx.Empty<KeyValuePair<int, object?>>()
                        )
                        .GetEnumerator()
                )
                {
                    var hasCookie = cookieEnumerator.MoveNext();

                    for (int i = 0, n = instructions.Count; i < n; i++)
                    {
                        var instruction = instructions[i];

                        object? cookie = null;
                        while (hasCookie && cookieEnumerator.Current.Key == i)
                        {
                            cookie = cookieEnumerator.Current.Value;
                            hasCookie = cookieEnumerator.MoveNext();
                        }

                        var stackDiff = instruction.StackBalance;
                        var contDiff = instruction.ContinuationsBalance;
                        var name = instruction.ToDebugString(i, cookie, labelIndexer, objects);
                        result.Add(new InstructionView(instruction, name, i, stackDepth, continuationsDepth));

                        stackDepth += stackDiff;
                        continuationsDepth += contDiff;
                    }

                    return result.AsArrayInternal();
                }
            }

            [DebuggerDisplay("{" + nameof(GetValue) + "(),nq}", Name = "{GetName(),nq}", Type = "{GetDisplayType(), nq}")]
            internal readonly struct InstructionView
            {
                private readonly int _continuationsDepth;
                private readonly int _index;
                private readonly Instruction _instruction;
                private readonly string _name;
                private readonly int _stackDepth;

                public InstructionView(Instruction instruction, string name, int index, int stackDepth, int continuationsDepth)
                {
                    _instruction = instruction;
                    _name = name;
                    _index = index;
                    _stackDepth = stackDepth;
                    _continuationsDepth = continuationsDepth;
                }

                internal string GetDisplayType()
                {
                    return $"{_instruction.ContinuationsBalance}/{_instruction.StackBalance}";
                }

                internal string GetName()
                {
                    return $"{_index}{(_continuationsDepth == 0 ? "" : $" C({_continuationsDepth})")}{(_stackDepth == 0 ? "" : $" S({_stackDepth})")}";
                }

                internal string GetValue()
                {
                    return _name;
                }
            }
        }
    }
}

#endif