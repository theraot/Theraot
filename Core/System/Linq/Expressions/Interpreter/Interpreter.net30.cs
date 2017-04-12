#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Interpreter
{
    /// <summary>
    /// A simple forth-style stack machine for executing Expression trees
    /// without the need to compile to IL and then invoke the JIT.  This trades
    /// off much faster compilation time for a slower execution performance.
    /// For code that is only run a small number of times this can be a
    /// sweet spot.
    ///
    /// The core loop in the interpreter is the RunInstructions method.
    /// </summary>
    internal sealed class Interpreter
    {
        internal static readonly object NoValue = new object();
        internal const int RethrowOnReturn = Int32.MaxValue;

        private readonly int _localCount;
        private readonly HybridReferenceDictionary<LabelTarget, BranchLabel> _labelMapping;
        private readonly Dictionary<ParameterExpression, LocalVariable> _closureVariables;

        private readonly InstructionArray _instructions;
        internal readonly object[] Objects;
        internal readonly RuntimeLabel[] Labels;

        internal readonly string Name;
        internal readonly DebugInfo[] DebugInfos;

        internal Interpreter(string name, LocalVariables locals, HybridReferenceDictionary<LabelTarget, BranchLabel> labelMapping,
            InstructionArray instructions, DebugInfo[] debugInfos)
        {
            Name = name;
            _localCount = locals.LocalCount;
            _closureVariables = locals.ClosureVariables;

            _instructions = instructions;
            Objects = instructions.Objects;
            Labels = instructions.Labels;
            _labelMapping = labelMapping;

            DebugInfos = debugInfos;
        }

        internal int ClosureSize
        {
            get
            {
                if (_closureVariables == null)
                {
                    return 0;
                }
                return _closureVariables.Count;
            }
        }

        internal int LocalCount
        {
            get { return _localCount; }
        }

        internal InstructionArray Instructions
        {
            get { return _instructions; }
        }

        internal Dictionary<ParameterExpression, LocalVariable> ClosureVariables
        {
            get { return _closureVariables; }
        }

        internal HybridReferenceDictionary<LabelTarget, BranchLabel> LabelMapping
        {
            get { return _labelMapping; }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Run(InterpretedFrame frame)
        {
            var instructions = _instructions.Instructions;
            var index = frame.InstructionIndex;
            while (index < instructions.Length)
            {
                index += instructions[index].Run(frame);
                frame.InstructionIndex = index;
            }
        }

        internal int ReturnAndRethrowLabelIndex
        {
            get
            {
                // the last label is "return and rethrow" label:
                Debug.Assert(Labels[Labels.Length - 1].Index == RethrowOnReturn);
                return Labels.Length - 1;
            }
        }
    }
}

#endif