#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace System.Linq.Expressions.Interpreter
{
    internal struct RuntimeLabel
    {
        public readonly int Index;
        public readonly int StackDepth;
        public readonly int ContinuationStackDepth;

        public RuntimeLabel(int index, int continuationStackDepth, int stackDepth)
        {
            Index = index;
            ContinuationStackDepth = continuationStackDepth;
            StackDepth = stackDepth;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "->{0} C({1}) S({2})", Index, ContinuationStackDepth, StackDepth);
        }
    }

    internal sealed class BranchLabel
    {
        internal const int UnknownIndex = Int32.MinValue;
        internal const int UnknownDepth = Int32.MinValue;

        internal int LabelIndex = UnknownIndex;
        internal int TargetIndex = UnknownIndex;
        internal int StackDepth = UnknownDepth;
        internal int ContinuationStackDepth = UnknownDepth;

        // Offsets of forward branching instructions targetting this label
        // that need to be updated after we emit the label.
        private List<int> _forwardBranchFixups;

        internal bool HasRuntimeLabel
        {
            get { return LabelIndex != UnknownIndex; }
        }

        internal RuntimeLabel ToRuntimeLabel()
        {
            Debug.Assert(TargetIndex != UnknownIndex && StackDepth != UnknownDepth && ContinuationStackDepth != UnknownDepth);
            return new RuntimeLabel(TargetIndex, ContinuationStackDepth, StackDepth);
        }

        internal void Mark(InstructionList instructions)
        {
            //ContractUtils.Requires(_targetIndex == UnknownIndex && _stackDepth == UnknownDepth && _continuationStackDepth == UnknownDepth);

            StackDepth = instructions.CurrentStackDepth;
            ContinuationStackDepth = instructions.CurrentContinuationsDepth;
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

        internal void AddBranch(InstructionList instructions, int branchIndex)
        {
            Debug.Assert(((TargetIndex == UnknownIndex) == (StackDepth == UnknownDepth)));
            Debug.Assert(((TargetIndex == UnknownIndex) == (ContinuationStackDepth == UnknownDepth)));

            if (TargetIndex == UnknownIndex)
            {
                if (_forwardBranchFixups == null)
                {
                    _forwardBranchFixups = new List<int>();
                }
                _forwardBranchFixups.Add(branchIndex);
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
    }
}

#endif