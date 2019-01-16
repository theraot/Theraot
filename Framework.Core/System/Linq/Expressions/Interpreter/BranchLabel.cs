#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace System.Linq.Expressions.Interpreter
{
    internal /*readonly*/ struct RuntimeLabel
    {
        public readonly int ContinuationStackDepth;
        public readonly int Index;
        public readonly int StackDepth;

        public RuntimeLabel(int index, int continuationStackDepth, int stackDepth)
        {
            Index = index;
            ContinuationStackDepth = continuationStackDepth;
            StackDepth = stackDepth;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "->{0} C({1}) S({2})", Index, ContinuationStackDepth, StackDepth);
        }
    }

    internal sealed class BranchLabel
    {
        internal const int UnknownDepth = int.MinValue;
        internal const int UnknownIndex = int.MinValue;
        private int _continuationStackDepth = UnknownDepth;

        // Offsets of forward branching instructions targeting this label
        // that need to be updated after we emit the label.
        private List<int> _forwardBranchFixups;

        private int _stackDepth = UnknownDepth;

        internal bool HasRuntimeLabel => LabelIndex != UnknownIndex;
        internal int LabelIndex { get; set; } = UnknownIndex;
        internal int TargetIndex { get; private set; } = UnknownIndex;

        internal void AddBranch(InstructionList instructions, int branchIndex)
        {
            Debug.Assert(TargetIndex == UnknownIndex == (_stackDepth == UnknownDepth));
            Debug.Assert(TargetIndex == UnknownIndex == (_continuationStackDepth == UnknownDepth));

            if (TargetIndex == UnknownIndex)
            {
                (_forwardBranchFixups ?? (_forwardBranchFixups = new List<int>())).Add(branchIndex);
            }
            else
            {
                FixupBranch(instructions, branchIndex);
            }
        }

        internal void FixupBranch(InstructionList instructions, int branchIndex)
        {
            Debug.Assert(TargetIndex != UnknownIndex);
            instructions.FixupBranch(branchIndex, TargetIndex - branchIndex);
        }

        internal void Mark(InstructionList instructions)
        {
            //ContractUtils.Requires(_targetIndex == UnknownIndex && _stackDepth == UnknownDepth && _continuationStackDepth == UnknownDepth);

            _stackDepth = instructions.CurrentStackDepth;
            _continuationStackDepth = instructions.CurrentContinuationsDepth;
            TargetIndex = instructions.Count;

            if (_forwardBranchFixups != null)
            {
                foreach (var branchIndex in _forwardBranchFixups)
                {
                    FixupBranch(instructions, branchIndex);
                }
                _forwardBranchFixups = null;
            }
        }

        internal RuntimeLabel ToRuntimeLabel()
        {
            Debug.Assert(TargetIndex != UnknownIndex && _stackDepth != UnknownDepth && _continuationStackDepth != UnknownDepth);
            return new RuntimeLabel(TargetIndex, _continuationStackDepth, _stackDepth);
        }
    }
}

#endif