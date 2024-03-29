﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions.Compiler
{
    internal partial class StackSpiller
    {
        /// <summary>
        ///     <para>
        ///         Rewrites child expressions, spilling them into temps if needed. The
        ///         stack starts in the initial state, and after the first subexpression
        ///         is added it is changed to non-empty.
        ///     </para>
        ///     <para>
        ///         When all children have been added, the caller should rewrite the
        ///         node if Rewrite is true. Then, it should call Finish with either
        ///         the original expression or the rewritten expression. Finish will call
        ///         Expression.Block if necessary and return a new Result.
        ///     </para>
        /// </summary>
        private sealed class ChildRewriter
        {
            /// <summary>
            ///     The child expressions being rewritten.
            /// </summary>
            private readonly Expression[] _expressions;

            /// <summary>
            ///     The parent stack spiller, used to perform rewrites of expressions
            ///     and to allocate temporary variables.
            /// </summary>
            private readonly StackSpiller _self;

            /// <summary>
            ///     Indicates whether a child expression represents a ByRef value and
            ///     requires use of a <see cref="ByRefAssignBinaryExpression" /> node
            ///     to perform spilling.
            /// </summary>
            private bool[]? _byRefs;

            /// <summary>
            ///     The comma of expressions that will evaluate the parent expression
            ///     using temporary variables introduced by stack spilling. This field
            ///     is populated in <see cref="EnsureDone" /> which gets called upon
            ///     the first access to an indexer or the <see cref="Rewrite" /> method
            ///     on the child rewriter instance. A comma only gets built if the
            ///     rewrite action in <see cref="Action" /> is set to SpillStack.
            /// </summary>
            /// <example>
            ///     When stack spilling the following expression:
            ///     <code>
            ///         bar.Foo(try { 42 } finally { ; })
            ///     </code>
            ///     the resulting comma will contain three expressions:
            ///     <code>
            ///         $temp$0 = bar
            ///         $temp$1 = try { 42 } finally { ; }
            ///         $temp$0.Foo($temp$1)
            ///     </code>
            ///     These get wrapped in a Block in the <see cref="Rewrite" /> method.
            /// </example>
            private List<Expression>? _comma;

            private bool _done;

            /// <summary>
            ///     The index of the next expression to rewrite in <see cref="_expressions" />.
            /// </summary>
            private int _expressionsCount;

            /// <summary>
            ///     The index of the last expression that requires a SpillStack action.
            /// </summary>
            private int _lastSpillIndex;

            private Stack _stack;

            /// <summary>
            ///     Creates a new child rewriter instance using the specified initial
            ///     evaluation <paramref name="stack" /> state and the number of child
            ///     expressions specified in <paramref name="count" />.
            /// </summary>
            /// <param name="self">The parent stack spiller.</param>
            /// <param name="stack">The initial evaluation stack state.</param>
            /// <param name="count">The number of child expressions that will be added.</param>
            internal ChildRewriter(StackSpiller self, Stack stack, int count)
            {
                _self = self;
                _stack = stack;
                _expressions = new Expression[count];
            }

            internal RewriteAction Action { get; private set; }

            /// <summary>
            ///     Gets a Boolean value indicating whether the parent expression should be
            ///     rewritten by using <see cref="Finish" />.
            /// </summary>
            internal bool Rewrite => Action != RewriteAction.None;

            /// <summary>
            ///     Gets the rewritten child expression at the specified <paramref name="index" />,
            ///     used to rewrite the parent expression. In case stack spilling has taken place,
            ///     the returned expression will be a temporary variable.
            /// </summary>
            /// <param name="index">
            ///     The index of the child expression to retrieve. Negative values indicate -1-based
            ///     offsets from the end of the child expressions array. Positive values indicate
            ///     0-based offsets from the start of the child expressions array.
            /// </param>
            /// <returns>
            ///     The rewritten child expression at the specified <paramref name="index" />.
            /// </returns>
            internal Expression this[int index]
            {
                get
                {
                    EnsureDone();

                    if (index < 0)
                    {
                        index += _expressions.Length;
                    }

                    return _expressions[index];
                }
            }

            /// <summary>
            ///     Gets the rewritten child expressions between the specified <paramref name="first" />
            ///     and <paramref name="last" /> (inclusive) indexes, used to rewrite the parent
            ///     expression. In case stack spilling has taken place, the returned expressions will
            ///     contain temporary variables.
            /// </summary>
            /// <param name="first">
            ///     The index of the first child expression to retrieve. This value should always be
            ///     positive.
            /// </param>
            /// <param name="last">
            ///     The (inclusive) index of the last child expression to retrieve. Negative values
            ///     indicate -1-based offsets from the end of the child expressions array. Positive values
            ///     indicate 0-based offsets from the start of the child expressions array.
            /// </param>
            /// <returns>
            ///     The rewritten child expressions between the specified <paramref name="first" />
            ///     and <paramref name="last" /> (inclusive) indexes.
            /// </returns>
            internal Expression[] this[int first, int last]
            {
                get
                {
                    EnsureDone();

                    if (last < 0)
                    {
                        last += _expressions.Length;
                    }

                    var count = last - first + 1;
                    ContractUtils.RequiresArrayRange(_expressions, first, count, nameof(first), nameof(last));

                    if (count == _expressions.Length)
                    {
                        Debug.Assert(first == 0);

                        // If the entire array is requested just return it so we don't make a new array.
                        return _expressions;
                    }

                    var clone = new Expression[count];
                    Array.Copy(_expressions, first, clone, 0, count);
                    return clone;
                }
            }

            /// <summary>
            ///     Adds a child <paramref name="expression" /> to the rewriter, causing
            ///     it to be rewritten using the parent stack spiller, and the evaluation
            ///     stack state and rewrite action to be updated accordingly.
            /// </summary>
            /// <param name="expression">The child expression to add.</param>
            internal void Add(Expression expression)
            {
                Debug.Assert(!_done);

                var exp = _self.RewriteExpression(expression, _stack);
                Action |= exp.Action;
                _stack = Stack.NonEmpty;

                if (exp.Action == RewriteAction.SpillStack)
                {
                    _lastSpillIndex = _expressionsCount;
                }

                // Track items in case we need to copy or spill stack.
                _expressions[_expressionsCount++] = exp.Node;
            }

            /// <summary>
            ///     Adds child <paramref name="expressions" /> to the rewriter, causing
            ///     them to be rewritten using the parent stack spiller, and the evaluation
            ///     stack state and rewrite action to be updated accordingly.
            /// </summary>
            /// <param name="expressions">The child expressions to add.</param>
            internal void Add(ReadOnlyCollection<Expression> expressions)
            {
                for (int i = 0, count = expressions.Count; i < count; i++)
                {
                    Add(expressions[i]);
                }
            }

            /// <summary>
            ///     Adds child <paramref name="expressions" /> provided through an argument
            ///     provider to the rewriter, causing them to be rewritten using the parent
            ///     stack spiller, and the evaluation stack state and rewrite action to be
            ///     updated accordingly.
            /// </summary>
            /// <param name="expressions">
            ///     The argument provider containing the child expression to add.
            /// </param>
            internal void AddArguments(IArgumentProvider expressions)
            {
                for (int i = 0, count = expressions.ArgumentCount; i < count; i++)
                {
                    Add(expressions.GetArgument(i));
                }
            }

            /// <summary>
            ///     Rewrites the parent <paramref name="expression" /> where any stack spilled
            ///     child expressions have been substituted for temporary variables, and returns
            ///     the rewrite result to the caller.
            /// </summary>
            /// <param name="expression">
            ///     The parent expression after substituting stack spilled child expressions
            ///     for temporary variables using the indexers on the child rewriter.
            /// </param>
            /// <returns>
            ///     The result of rewriting the parent <paramref name="expression" />, which
            ///     includes the rewritten expression and the rewrite action. If stack spilling
            ///     has taken place, the resulting expression is a block expression containing
            ///     the expressions kept in <see cref="_comma" />.
            /// </returns>
            internal Result Finish(Expression expression)
            {
                EnsureDone();

                if (Action != RewriteAction.SpillStack)
                {
                    return new Result(Action, expression);
                }

                Debug.Assert(_comma!.Capacity == _comma.Count + 1);
                _comma.Add(expression);
                expression = MakeBlock(_comma.ToArray());

                return new Result(Action, expression);
            }

            /// <summary>
            ///     Marks child expressions representing arguments bound to parameters of
            ///     the specified <paramref name="method" /> as ByRef values if needed.
            /// </summary>
            /// <param name="method">
            ///     The method containing the parameters bound to the arguments held by
            ///     child expressions tracked by this rewriter.
            /// </param>
            /// <param name="startIndex">
            ///     The index of the child expression representing the first argument. This
            ///     value is typically 0 for static methods and 1 for instance methods.
            /// </param>
            internal void MarkRefArgs(MethodBase method, int startIndex)
            {
                var parameters = method.GetParameters();

                for (int i = 0, n = parameters.Length; i < n; i++)
                {
                    if (parameters[i].ParameterType.IsByRef)
                    {
                        MarkRef(startIndex + i);
                    }
                }
            }

            /// <summary>
            ///     Checks whether the given <paramref name="expression" /> representing a
            ///     child expression should be saved in a temporary variable upon spilling
            ///     the stack. If the expression has no have side-effects, the introduction
            ///     of a temporary variable can be avoided, reducing the number of locals.
            /// </summary>
            /// <param name="expression">The expression to check for side-effects.</param>
            /// <returns>
            ///     <c>true</c> if the expression should be saved to a temporary variable;
            ///     otherwise, <c>false</c>.
            /// </returns>
            private static bool ShouldSaveToTemp(Expression expression)
            {
                // Some expressions have no side-effects and don't have to be
                // stored into temporaries, e.g.
#pragma warning disable S125 // Sections of code should not be commented out
                //     xs[0] = try { ... }
                //           |
                //           v
                //        t0 = xs
                //        t1 = 0            // <-- this is redundant
                //        t2 = try { ... }
                //    t0[t1] = t2
                //           |
                //           v
                //        t0 = xs
                //        t1 = try { ... }
                //     t0[0] = t1
#pragma warning restore S125 // Sections of code should not be commented out
                switch (expression.NodeType)
                {
                    // Emits ldnull, ldc, initobj, closure constant access, etc.
                    case ExpressionType.Constant:
                    case ExpressionType.Default:
                        return false;

                    // Emits calls to pure RuntimeOps methods with immutable arguments
                    case ExpressionType.RuntimeVariables:
                        return false;

                    case ExpressionType.MemberAccess:
                        var member = (MemberExpression)expression;
                        if (member.Member is FieldInfo field)
                        {
                            // Emits ldc for the raw value of the field
                            if (field.IsLiteral)
                            {
                                return false;
                            }

                            // For read-only fields we could save the receiver, but
                            // that's more involved, so we'll just handle static fields
                            if (field.IsInitOnly && field.IsStatic)
                            {
                                return false;
                            }
                        }

                        break;

                    default:
                        break;
                }

                // NB: We omit Lambda because it may interfere with the Invoke/Lambda
                //     inlining optimizations. Parameter is out too because we don't
                //     have any sophisticated load/store analysis.

                // NB: We omit Quote because the emitted call to RuntimeOps.Quote will
                //     trigger reduction of extension nodes which can cause the timing
                //     of exceptions thrown from Reduce methods to change.

                return true;
            }

            private void EnsureDone()
            {
                // Done adding child expressions, build the comma if necessary.
                if (_done)
                {
                    return;
                }

                _done = true;

                if (Action != RewriteAction.SpillStack)
                {
                    return;
                }

                var clone = _expressions;
                var count = _lastSpillIndex + 1;
                var comma = new List<Expression>(count + 1);
                for (var index = 0; index < count; index++)
                {
                    var current = clone[index];
                    if (current == null || !ShouldSaveToTemp(current))
                    {
                        continue;
                    }

                    clone[index] = _self.ToTemp(current, out var temp, _byRefs?[index] ?? false);
                    comma.Add(temp);
                }

                comma.Capacity = comma.Count + 1;
                _comma = comma;
            }

            /// <summary>
            ///     Marks the child expression in at the specified <paramref name="index" />
            ///     as having a ByRef value.
            /// </summary>
            /// <param name="index">
            ///     The index of the child expression holding a ByRef value.
            /// </param>
            private void MarkRef(int index)
            {
                if (_byRefs == null)
                {
                    _byRefs = new bool[_expressions.Length];
                }

                _byRefs[index] = true;
            }
        }
    }
}

#endif