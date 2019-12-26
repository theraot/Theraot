#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using Theraot.Core;

namespace System.Linq.Expressions.Compiler
{
    /// <summary>
    ///     Determines if variables are closed over in nested lambdas and need to
    ///     be hoisted.
    /// </summary>
    internal sealed class VariableBinder : ExpressionVisitor
    {
        private readonly Stack<BoundConstants> _constants = new Stack<BoundConstants>();
        private readonly StackGuard _guard = new StackGuard();
        private readonly Stack<CompilerScope> _scopes = new Stack<CompilerScope>();
        private readonly AnalyzedTree _tree = new AnalyzedTree();
        private bool _inQuote;

        private VariableBinder()
        {
            // Empty
        }

        public override Expression? Visit(Expression? node)
        {
            // When compiling deep trees, we run the risk of triggering a terminating StackOverflowException,
            // so we use the StackGuard utility here to probe for sufficient stack and continue the work on
            // another thread when we run out of stack space.
            return !_guard.TryEnterOnCurrentStack() ? _guard.RunOnEmptyStack((@this, e) => @this.Visit(e), this, node) : base.Visit(node);
        }

        internal static AnalyzedTree Bind(LambdaExpression lambda)
        {
            var binder = new VariableBinder();
            binder.Visit(lambda);
            return binder._tree;
        }

        protected internal override Expression VisitBlock(BlockExpression node)
        {
            if (node.Variables.Count == 0)
            {
                Visit(node.Expressions);
                return node;
            }

            var createdScope = new CompilerScope(node, false);
            _tree.Scopes[node] = createdScope;
            _scopes.Push(createdScope);
            Visit(MergeScopes(node));
            _scopes.Pop();
            return node;
        }

        protected internal override Expression VisitConstant(ConstantExpression node)
        {
            // If we're in Quote, we can ignore constants completely
            if (_inQuote)
            {
                return node;
            }

            // Constants that can be emitted into IL don't need to be stored on
            // the delegate
            if (ILGen.CanEmitConstant(node.Value, node.Type))
            {
                return node;
            }

            _constants.Peek().AddReference(node.Value, node.Type);
            return node;
        }

        protected internal override Expression VisitInvocation(InvocationExpression node)
        {
            var lambda = node.LambdaOperand;

            // optimization: inline code for literal lambda's directly
            if (lambda == null)
            {
                return base.VisitInvocation(node);
            }

            // visit the lambda, but treat it like a scope associated with invocation
            var createdScope = new CompilerScope(lambda, false);
            _tree.Scopes[node] = createdScope;
            _scopes.Push(createdScope);
            Visit(MergeScopes(lambda));
            _scopes.Pop();
            // visit the invoke arguments
            for (int i = 0, n = node.ArgumentCount; i < n; i++)
            {
                Visit(node.GetArgument(i));
            }

            return node;
        }

        protected internal override Expression VisitLambda<T>(Expression<T> node)
        {
            var createdScope = new CompilerScope(node, true);
            _tree.Scopes[node] = createdScope;
            _scopes.Push(createdScope);
            var createdConstants = new BoundConstants();
            _tree.Constants[node] = createdConstants;
            _constants.Push(createdConstants);
            Visit(MergeScopes(node));
            _constants.Pop();
            _scopes.Pop();
            return node;
        }

        protected internal override Expression VisitParameter(ParameterExpression node)
        {
            var storage = VariableStorageKind.Local;
            CompilerScope? definition = null;
            CompilerScope? referenceScope = null;
            foreach (var scope in _scopes)
            {
                if (scope.Definitions.ContainsKey(node))
                {
                    definition = scope;
                    break;
                }

                scope.NeedsClosure = true;
                if (!scope.IsMethod)
                {
                    continue;
                }

                storage = VariableStorageKind.Hoisted;
                referenceScope = scope;
            }

            if (definition == null)
            {
                throw new InvalidOperationException($"variable '{node.Name}' of type '{node.Type}' referenced from scope '{GetCurrentLambdaName()}', but it is not defined");
            }

            if (storage == VariableStorageKind.Hoisted)
            {
                if (node.IsByRef)
                {
                    throw new InvalidOperationException($"Cannot close over byref parameter '{node.Name}' referenced in lambda '{GetCurrentLambdaName()}'");
                }

                definition.Definitions[node] = VariableStorageKind.Hoisted;
            }

            referenceScope ??= definition;

            (referenceScope.ReferenceCount ?? (referenceScope.ReferenceCount = new Dictionary<ParameterExpression, int>())).TryGetValue(node, out var count);
            referenceScope.ReferenceCount[node] = count + 1;
            return node;
        }

        protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            foreach (var v in node.Variables)
            {
                // Force hoisting of these variables
                CompilerScope? definition = null;
                foreach (var scope in _scopes)
                {
                    if (scope.Definitions.ContainsKey(v))
                    {
                        definition = scope;
                        break;
                    }

                    scope.NeedsClosure = true;
                }

                if (definition == null)
                {
                    throw new InvalidOperationException($"variable '{v.Name}' of type '{v.Type}' referenced from scope '{GetCurrentLambdaName()}', but it is not defined");
                }

                if (v.IsByRef)
                {
                    throw new InvalidOperationException($"Cannot close over byref parameter '{v.Name}' referenced in lambda '{GetCurrentLambdaName()}'");
                }

                definition.Definitions[v] = VariableStorageKind.Hoisted;
            }

            return node;
        }

        protected internal override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Quote)
            {
                var savedInQuote = _inQuote;
                _inQuote = true;
                Visit(node.Operand!);
                _inQuote = savedInQuote;
            }
            else
            {
                Visit(node.Operand);
            }

            return node;
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            if (node.Variable == null)
            {
                Visit(node.Filter);
                Visit(node.Body);
                return node;
            }

            var createdScope = new CompilerScope(node, false);
            _tree.Scopes[node] = createdScope;
            _scopes.Push(createdScope);
            Visit(node.Filter);
            Visit(node.Body);
            _scopes.Pop();
            return node;
        }

        private string? GetCurrentLambdaName()
        {
            foreach (var scope in _scopes)
            {
                if (scope.Node is LambdaExpression lambda)
                {
                    return lambda.Name;
                }
            }

            throw ContractUtils.Unreachable;
        }

        // If the immediate child is another scope, merge it into this one
        // This is an optimization to save environment allocations and
        // array accesses.
        private ReadOnlyCollection<Expression> MergeScopes(Expression node)
        {
            var body = node is LambdaExpression lambda ? new ReadOnlyCollection<Expression>(new[] { lambda.Body }) : ((BlockExpression)node).Expressions;

            var currentScope = _scopes.Peek();

            // A block body is mergeable if the body only contains one single block node containing variables,
            // and the child block has the same type as the parent block.
            while (body.Count == 1 && body[0].NodeType == ExpressionType.Block)
            {
                var block = (BlockExpression)body[0];

                if (block.Variables.Count > 0)
                {
                    // Make sure none of the variables are shadowed. If any
                    // are, we can't merge it.
                    foreach (var v in block.Variables)
                    {
                        if (currentScope.Definitions.ContainsKey(v))
                        {
                            return body;
                        }
                    }

                    // Otherwise, merge it

                    (currentScope.MergedScopes ?? (currentScope.MergedScopes = new HashSet<BlockExpression>(ReferenceEqualityComparer<BlockExpression>.Instance))).Add(block);
                    foreach (var v in block.Variables)
                    {
                        currentScope.Definitions.Add(v, VariableStorageKind.Local);
                    }
                }

                body = block.Expressions;
            }

            return body;
        }
    }
}

#endif