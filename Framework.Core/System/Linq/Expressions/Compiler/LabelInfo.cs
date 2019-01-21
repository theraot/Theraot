#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using Theraot.Core;

namespace System.Linq.Expressions.Compiler
{
    internal enum LabelScopeKind
    {
        // any "statement like" node that can be jumped into
        Statement,

        // these correspond to the node of the same name
        Block,

        Switch,
        Lambda,
        Try,

        // these correspond to the part of the try block we're in
        Catch,

        Finally,
        Filter,

        // the catch-all value for any other expression type
        // (means we can't jump into it)
        Expression
    }

    /// <summary>
    /// Contains compiler state corresponding to a LabelTarget
    /// See also LabelScopeInfo.
    /// </summary>
    internal sealed class LabelInfo
    {

        // True if at least one jump is across blocks
        // If we have any jump across blocks to this label, then the
        // LabelTarget can only be defined in one place
        private bool _acrossBlockJump;
        // The blocks where this label is defined. If it has more than one item,
        // the blocks can't be jumped to except from a child block
        private readonly HashSet<LabelScopeInfo> _definitions = new HashSet<LabelScopeInfo>();

        private readonly ILGenerator _ilg;

        // The IL label, will be mutated if Node is redefined
        private Label _label;

        private bool _labelDefined;

        // The tree node representing this label
        private readonly LabelTarget _node;

        // Until we have more information, default to a leave instruction,
        // which always works. Note: leave spills the stack, so we need to
        // ensure that StackSpiller has guaranteed us an empty stack at this
        // point. Otherwise Leave and Branch are not equivalent
        private OpCode _opCode = OpCodes.Leave;

        // Blocks that jump to this block
        private readonly List<LabelScopeInfo> _references = new List<LabelScopeInfo>();

        // The local that carries the label's value, if any
        private LocalBuilder _value;

        internal LabelInfo(ILGenerator il, LabelTarget node, bool canReturn)
        {
            _ilg = il;
            _node = node;
            CanReturn = canReturn;
        }

        /// <summary>
        /// Indicates if it is legal to emit a "branch" instruction based on
        /// currently available information. Call the Reference method before
        /// using this property.
        /// </summary>
        internal bool CanBranch => _opCode != OpCodes.Leave;

        internal bool CanReturn { get; }

        internal Label Label
        {
            get
            {
                EnsureLabelAndValue();
                return _label;
            }
        }

        // Returns true if the label was successfully defined
        // or false if the label is now ambiguous
        internal void Define(LabelScopeInfo block)
        {
            // Prevent the label from being shadowed, which enforces cleaner
            // trees. Also we depend on this for simplicity (keeping only one
            // active IL Label per LabelInfo)
            for (var j = block; j != null; j = j.Parent)
            {
                if (j.ContainsTarget(_node))
                {
                    throw new InvalidOperationException($"Cannot redefine label '{_node.Name}' in an inner block.");
                }
            }

            _definitions.Add(block);
            block.AddLabelInfo(_node, this);

            // Once defined, validate all jumps
            if (_definitions.Count == 1)
            {
                foreach (var r in _references)
                {
                    ValidateJump(r);
                }
            }
            else
            {
                // Was just redefined, if we had any across block jumps, they're
                // now invalid
                if (_acrossBlockJump)
                {
                    throw new InvalidOperationException($"Cannot jump to ambiguous label '{_node.Name}'.");
                }
                // For local jumps, we need a new IL label
                // This is okay because:
                //   1. no across block jumps have been made or will be made
                //   2. we don't allow the label to be shadowed
                _labelDefined = false;
            }
        }

        internal void EmitJump()
        {
            // Return directly if we can
            if (_opCode == OpCodes.Ret)
            {
                _ilg.Emit(OpCodes.Ret);
            }
            else
            {
                StoreValue();
                _ilg.Emit(_opCode, Label);
            }
        }

        internal void Mark()
        {
            if (CanReturn)
            {
                // Don't mark return labels unless they were actually jumped to
                // (returns are last so we know for sure if anyone jumped to it)
                if (!_labelDefined)
                {
                    // We don't even need to emit the "ret" because
                    // LambdaCompiler does that for us.
                    return;
                }

                // Otherwise, emit something like:
                // ret
                // <marked label>:
                // ldloc <value>
                _ilg.Emit(OpCodes.Ret);
            }
            else
            {
                // For the normal case, we emit:
                // stloc <value>
                // <marked label>:
                // ldloc <value>
                StoreValue();
            }
            MarkWithEmptyStack();
        }

        // Like Mark, but assumes the stack is empty
        internal void MarkWithEmptyStack()
        {
            _ilg.MarkLabel(Label);
            if (_value != null)
            {
                // We always read the value from a local, because we don't know
                // if there will be a "leave" instruction targeting it ("branch"
                // preserves its stack, but "leave" empties the stack)
                _ilg.Emit(OpCodes.Ldloc, _value);
            }
        }

        internal void Reference(LabelScopeInfo block)
        {
            _references.Add(block);
            if (_definitions.Count > 0)
            {
                ValidateJump(block);
            }
        }

        internal void ValidateFinish()
        {
            // Make sure that if this label was jumped to, it is also defined
            if (_references.Count > 0 && _definitions.Count == 0)
            {
                throw new InvalidOperationException($"Cannot jump to undefined label '{_node.Name}'.");
            }
        }

        private void EnsureLabelAndValue()
        {
            if (!_labelDefined)
            {
                _labelDefined = true;
                _label = _ilg.DefineLabel();
                if (_node != null && _node.Type != typeof(void))
                {
                    _value = _ilg.DeclareLocal(_node.Type);
                }
            }
        }

        private void StoreValue()
        {
            EnsureLabelAndValue();
            if (_value != null)
            {
                _ilg.Emit(OpCodes.Stloc, _value);
            }
        }

        private void ValidateJump(LabelScopeInfo reference)
        {
            // Assume we can do a ret/branch
            _opCode = CanReturn ? OpCodes.Ret : OpCodes.Br;

            // look for a simple jump out
            for (var j = reference; j != null; j = j.Parent)
            {
                if (_definitions.Contains(j))
                {
                    // found it, jump is valid!
                    return;
                }
                if (j.Kind == LabelScopeKind.Finally || j.Kind == LabelScopeKind.Filter)
                {
                    break;
                }
                if (j.Kind == LabelScopeKind.Try || j.Kind == LabelScopeKind.Catch)
                {
                    _opCode = OpCodes.Leave;
                }
            }

            _acrossBlockJump = true;

            if (_node != null && _node.Type != typeof(void))
            {
                throw new InvalidOperationException($"Cannot jump to non-local label '{_node.Name}' with a value. Only jumps to labels defined in outer blocks can pass values.");
            }

            if (_definitions.Count > 1)
            {
                throw new InvalidOperationException($"Cannot jump to ambiguous label '{_node?.Name}'.");
            }

            // We didn't find an outward jump. Look for a jump across blocks
            var def = _definitions.First();
            var common = GraphHelper.CommonNode(def, reference, b => b.Parent);

            // Assume we can do a ret/branch
            _opCode = CanReturn ? OpCodes.Ret : OpCodes.Br;

            // Validate that we aren't jumping across a finally
            for (var j = reference; j != common; j = j.Parent)
            {
                if (j.Kind == LabelScopeKind.Finally)
                {
                    throw new InvalidOperationException("Control cannot leave a finally block.");
                }
                if (j.Kind == LabelScopeKind.Filter)
                {
                    throw new InvalidOperationException("Control cannot leave a filter test.");
                }
                if (j.Kind == LabelScopeKind.Try || j.Kind == LabelScopeKind.Catch)
                {
                    _opCode = OpCodes.Leave;
                }
            }

            // Validate that we aren't jumping into a catch or an expression
            for (var j = def; j != common; j = j.Parent)
            {
                if (!j.CanJumpInto)
                {
                    if (j.Kind == LabelScopeKind.Expression)
                    {
                        throw new InvalidOperationException("Control cannot enter an expression--only statements can be jumped into.");
                    }

                    throw new InvalidOperationException("Control cannot enter a try block.");
                }
            }
        }
    }

    //
    // Tracks scoping information for LabelTargets. Logically corresponds to a
    // "label scope". Even though we have arbitrary goto support, we still need
    // to track what kinds of nodes that gotos are jumping through, both to
    // emit property IL ("leave" out of a try block), and for validation, and
    // to allow labels to be duplicated in the tree, as long as the jumps are
    // considered "up only" jumps.
    //
    // We create one of these for every Expression that can be jumped into, as
    // well as creating them for the first expression we can't jump into. The
    // "Kind" property indicates what kind of scope this is.
    //
    internal sealed class LabelScopeInfo
    {
        internal readonly LabelScopeKind Kind;
        internal readonly LabelScopeInfo Parent;
        private Dictionary<LabelTarget, LabelInfo> _labels; // lazily allocated, we typically use this only once every 6th-7th block

        internal LabelScopeInfo(LabelScopeInfo parent, LabelScopeKind kind)
        {
            Parent = parent;
            Kind = kind;
        }

        /// <summary>
        /// Returns true if we can jump into this node
        /// </summary>
        internal bool CanJumpInto
        {
            get
            {
                switch (Kind)
                {
                    case LabelScopeKind.Block:
                    case LabelScopeKind.Statement:
                    case LabelScopeKind.Switch:
                    case LabelScopeKind.Lambda:
                        return true;

                    default:
                        return false;
                }
            }
        }

        internal void AddLabelInfo(LabelTarget target, LabelInfo info)
        {
            Debug.Assert(CanJumpInto);
            (_labels ?? (_labels = new Dictionary<LabelTarget, LabelInfo>())).Add(target, info);
        }

        internal bool ContainsTarget(LabelTarget target)
        {
            if (_labels == null)
            {
                return false;
            }

            return _labels.ContainsKey(target);
        }

        internal bool TryGetLabelInfo(LabelTarget target, out LabelInfo info)
        {
            if (_labels == null)
            {
                info = null;
                return false;
            }

            return _labels.TryGetValue(target, out info);
        }
    }
}

#endif