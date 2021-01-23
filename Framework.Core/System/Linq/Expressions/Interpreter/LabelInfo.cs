#if LESSTHAN_NET35

#pragma warning disable CC0031 // Check for null before calling a delegate

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
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
    ///     Contains compiler state corresponding to a LabelTarget
    ///     <seealso cref="LabelScopeInfo" />
    /// </summary>
    internal sealed class LabelInfo
    {
        // The tree node representing this label
        private readonly LabelTarget? _node;

        // Blocks that jump to this block
        private readonly List<LabelScopeInfo> _references = new List<LabelScopeInfo>();

        // True if at least one jump is across blocks
        // If we have any jump across blocks to this label, then the
        // LabelTarget can only be defined in one place
        private bool _acrossBlockJump;

        // The blocks where this label is defined. If it has more than one item,
        // the blocks can't be jumped to except from a child block
        // If there's only 1 block (the common case) it's stored here, if there's multiple blocks it's stored
        // as a HashSet<LabelScopeInfo>
        private object? _definitions;

        // The BranchLabel label, will be mutated if Node is redefined
        private BranchLabel? _label;

        internal LabelInfo(LabelTarget? node)
        {
            _node = node;
        }

        private bool HasDefinitions => _definitions != null;

        private bool HasMultipleDefinitions => _definitions is HashSet<LabelScopeInfo>;

        internal static T? CommonNode<T>(T first, T second, Func<T, T> parent)
            where T : class
        {
            var cmp = EqualityComparer<T>.Default;
            if (cmp.Equals(first, second))
            {
                return first;
            }

            var set = new HashSet<T>(cmp);
            for (var t = first; t != null; t = parent(t))
            {
                set.Add(t);
            }

            for (var t = second; t != null; t = parent(t))
            {
                if (set.Contains(t))
                {
                    return t;
                }
            }

            return null;
        }

        internal void Define(LabelScopeInfo block)
        {
            // Prevent the label from being shadowed, which enforces cleaner
            // trees. Also we depend on this for simplicity (keeping only one
            // active IL Label per LabelInfo)
            foreach (var j in SequenceHelper.ExploreSequenceUntilNull(block, found => found.Parent))
            {
                if (j.ContainsTarget(_node!))
                {
                    throw new InvalidOperationException($"Cannot redefine label '{_node!.Name}' in an inner block.");
                }
            }

            AddDefinition(block);
            block.AddLabelInfo(_node!, this);

            // Once defined, validate all jumps
            if (HasDefinitions && !HasMultipleDefinitions)
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
                    throw new InvalidOperationException($"Cannot jump to ambiguous label '{_node!.Name}'.");
                }

                // For local jumps, we need a new IL label
                // This is okay because:
                //   1. no across block jumps have been made or will be made
                //   2. we don't allow the label to be shadowed
                _label = null;
            }
        }

        internal BranchLabel GetLabel(LightCompiler compiler)
        {
            return EnsureLabel(compiler);
        }

        internal void Reference(LabelScopeInfo block)
        {
            _references.Add(block);
            if (HasDefinitions)
            {
                ValidateJump(block);
            }
        }

        internal void ValidateFinish()
        {
            // Make sure that if this label was jumped to, it is also defined
            if (_references.Count > 0 && !HasDefinitions)
            {
                throw new InvalidOperationException($"Cannot jump to undefined label '{_node!.Name}'.");
            }
        }

        private void AddDefinition(LabelScopeInfo scope)
        {
            if (_definitions == null)
            {
                _definitions = scope;
            }
            else
            {
                if (!(_definitions is HashSet<LabelScopeInfo> set))
                {
                    _definitions = set = new HashSet<LabelScopeInfo>
                    {
                        (LabelScopeInfo)_definitions
                    };
                }

                set.Add(scope);
            }
        }

        private bool DefinedIn(LabelScopeInfo scope)
        {
            if (_definitions == scope)
            {
                return true;
            }

            if (_definitions is HashSet<LabelScopeInfo> definitions)
            {
                return definitions.Contains(scope);
            }

            return false;
        }

        private BranchLabel EnsureLabel(LightCompiler compiler)
        {
            return _label ??= compiler.Instructions.MakeLabel();
        }

        private LabelScopeInfo FirstDefinition()
        {
            if (_definitions is LabelScopeInfo scope)
            {
                return scope;
            }

            foreach (var x in (HashSet<LabelScopeInfo>)_definitions!)
            {
                return x;
            }

            throw new InvalidOperationException();
        }

        private void ValidateJump(LabelScopeInfo reference)
        {
            // look for a simple jump out
            foreach (var j in SequenceHelper.ExploreSequenceUntilNull(reference, found => found.Parent))
            {
                if (DefinedIn(j))
                {
                    // found it, jump is valid!
                    return;
                }

                if (j.Kind == LabelScopeKind.Finally || j.Kind == LabelScopeKind.Filter)
                {
                    break;
                }
            }

            _acrossBlockJump = true;
            if (_node != null && _node.Type != typeof(void))
            {
                throw new InvalidOperationException($"Cannot jump to non-local label '{_node.Name}' with a value. Only jumps to labels defined in outer blocks can pass values.");
            }

            if (HasMultipleDefinitions)
            {
                throw new InvalidOperationException($"Cannot jump to ambiguous label '{_node?.Name}'.");
            }

            // We didn't find an outward jump. Look for a jump across blocks
            var def = FirstDefinition();
            var common = SequenceHelper.CommonNode(def, reference, b => b.Parent);

            // Validate that we aren't jumping across a finally
            foreach (var j in SequenceHelper.ExploreSequenceUntilNull(reference, common, found => found.Parent))
            {
                switch (j.Kind)
                {
                    case LabelScopeKind.Finally:
                        throw new InvalidOperationException("Control cannot leave a finally block.");
                    case LabelScopeKind.Filter:
                        throw new InvalidOperationException("Control cannot leave a filter test.");
                    default:
                        break;
                }
            }

            // Validate that we aren't jumping into a catch or an expression
            foreach (var j in SequenceHelper.ExploreSequenceUntilNull(def, common, found => found.Parent))
            {
                if (j.CanJumpInto)
                {
                    continue;
                }

                if (j.Kind == LabelScopeKind.Expression)
                {
                    throw new InvalidOperationException("Control cannot enter an expression--only statements can be jumped into.");
                }

                throw new InvalidOperationException("Control cannot enter a try block.");
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
        internal readonly LabelScopeInfo? Parent;
        private HybridReferenceDictionary<LabelTarget, LabelInfo>? _labels; // lazily allocated, we typically use this only once every 6th-7th block

        internal LabelScopeInfo(LabelScopeInfo? parent, LabelScopeKind kind)
        {
            Parent = parent;
            Kind = kind;
        }

        /// <summary>
        ///     Returns true if we can jump into this node
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

            if (_labels == null)
            {
                _labels = new HybridReferenceDictionary<LabelTarget, LabelInfo>();
            }

            _labels[target] = info;
        }

        internal bool ContainsTarget(LabelTarget target)
        {
            return _labels?.ContainsKey(target) == true;
        }

        internal LabelInfo? GetLabelInfo(LabelTarget target)
        {
            if (_labels != null && _labels.TryGetValue(target, out var info))
            {
                return info;
            }

            return null;
        }
    }
}

#endif