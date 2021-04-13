#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using Theraot.Collections;

namespace System.Linq.Expressions.Compiler
{
    /// <summary>
    ///     Expression rewriting to spill the CLR stack into temporary variables
    ///     in order to guarantee some properties of code generation, for
    ///     example that we always enter try block on empty stack.
    /// </summary>
    internal sealed partial class StackSpiller
    {
        /// <summary>
        ///     Initial stack state. Normally empty, but when inlining the lambda
        ///     we might have a non-empty starting stack state.
        /// </summary>
        private readonly Stack _startingStack;

        /// <summary>
        ///     Lambda rewrite result. We need this for inlined lambdas to figure
        ///     out whether we need to guarantee it an empty stack.
        /// </summary>
        private RewriteAction _lambdaRewrite;

        private StackSpiller(Stack stack)
        {
            _startingStack = stack;
        }

        /// <summary>
        ///     Indicates whether the evaluation stack is empty.
        /// </summary>
        private enum Stack
        {
            Empty,
            NonEmpty
        }

        internal static LambdaExpression AnalyzeLambda(LambdaExpression lambda)
        {
            return lambda.AcceptStackSpiller(new StackSpiller(Stack.Empty));
        }

        // Called by Expression<T>.Accept(StackSpiller).
        internal Expression<T> Rewrite<T>(Expression<T> lambda)
        {
            VerifyTemps();

            // Lambda starts with an empty stack.
            var body = RewriteExpressionFreeTempsNotNull(lambda.Body, _startingStack);

            _lambdaRewrite = body.Action;

            VerifyTemps();

            if (body.Action == RewriteAction.None)
            {
                return lambda;
            }

            // Create a new scope for temps.
            // Note that none of these will be hoisted so there is no closure impact.
            var newBody = body.Node;
            if (_tm.Temps.Count > 0)
            {
                newBody = Expression.Block(_tm.Temps, ReadOnlyCollectionEx.Create(newBody));
            }

            // Clone the lambda, replacing the body & variables.
            return Expression<T>.Create(newBody, lambda.Name, lambda.TailCall, new ParameterList(lambda).ToArray());
        }

        private static T[] Clone<T>(T[] original, int max)
        {
            Debug.Assert(max < original.Length);

            var clone = new T[original.Length];
            for (var j = 0; j < max; j++)
            {
                clone[j] = original[j];
            }

            return clone;
        }

        private static bool IsRefInstance(Expression? instance)
        {
            // Primitive value types are okay because they are all read-only,
            // but we can't rely on this for non-primitive types. So we have
            // to either throw NotSupported or use ref locals.
            return instance?.Type.IsValueType == true && Type.GetTypeCode(instance.Type) == TypeCode.Object;
        }

        private static void RequireNoRefArgs(MethodBase? method)
        {
            if (method?.GetParameters().Any(p => p.ParameterType.IsByRef) == true)
            {
                throw new NotSupportedException($"TryExpression is not supported as an argument to method '{method}' because it has an argument with by-ref type. Construct the tree so the TryExpression is not nested inside of this expression.");
            }
        }

        private static void RequireNotRefInstance(Expression instance)
        {
            if (IsRefInstance(instance))
            {
                throw new NotSupportedException($"TryExpression is not supported as a child expression when accessing a member on type '{instance.Type}' because it is a value type. Construct the tree so the TryExpression is not nested inside of this expression.");
            }
        }

        private static Result RewriteLambdaExpression(Expression expr)
        {
            var node = (LambdaExpression)expr;

            // Call back into the rewriter
            expr = AnalyzeLambda(node);

            // If the lambda gets rewritten, we don't need to spill the stack,
            // but we do need to rebuild the tree above us so it includes the new node.
            var action = expr == node ? RewriteAction.None : RewriteAction.Copy;

            return new Result(action, expr);
        }

        [Conditional("DEBUG")]
        private static void VerifyRewrite(in Result result, Expression node)
        {
            Debug.Assert(result.Node != null);
            Debug.Assert((result.Action == RewriteAction.None) ^ (node != result.Node), "rewrite action does not match node object identity");

            // if the original node is an extension node, it should have been rewritten
            Debug.Assert(result.Node!.NodeType != ExpressionType.Extension, "extension nodes must be rewritten");

            // if we have Copy, then node type must match
            Debug.Assert
            (
                result.Action != RewriteAction.Copy || node.NodeType == result.Node!.NodeType || node.CanReduce,
                "rewrite action does not match node object kind"
            );

            // New type must be reference assignable to the old type
            // (our rewrites preserve type exactly, but the rules for rewriting
            // an extension node are more lenient, see Expression.ReduceAndCheck())
            Debug.Assert
            (
                node.Type.IsReferenceAssignableFromInternal(result.Node!.Type),
                "rewritten object must be reference assignable to the original type"
            );
        }

        private RewriteAction ProcessHandlers(in Result body, ref CatchBlock[] handlers)
        {
            var action = body.Action;

            CatchBlock[]? clone = null;
            for (var index = 0; index < handlers.Length; index++)
            {
                var curAction = body.Action;

                var handler = handlers[index];

                var filter = handler.Filter;
                if (handler.Filter != null)
                {
                    // Our code gen saves the incoming filter value and provides it as a variable so the stack is empty
                    var catchFilter = RewriteExpression(handler.Filter, Stack.Empty);
                    action |= catchFilter.Action;
                    curAction |= catchFilter.Action;
                    filter = catchFilter.Node;
                }

                // Catch block starts with an empty stack (guaranteed by TryStatement).
                var catchBody = RewriteExpression(handler.Body, Stack.Empty);
                action |= catchBody.Action;
                curAction |= catchBody.Action;

                if (curAction != RewriteAction.None)
                {
                    handler = Expression.MakeCatchBlock(handler.Test, handler.Variable, catchBody.Node, filter);

                    if (clone == null)
                    {
                        clone = Clone(handlers, index);
                    }
                }

                if (clone != null)
                {
                    clone[index] = handler;
                }
            }

            if (clone != null)
            {
                handlers = clone;
            }

            return action;
        }

        private Result RewriteAssignBinaryExpression(Expression expr, Stack stack)
        {
            var node = (BinaryExpression)expr;

            switch (node.Left.NodeType)
            {
                case ExpressionType.Index:
                    return RewriteIndexAssignment(node, stack);

                case ExpressionType.MemberAccess:
                    return RewriteMemberAssignment(node, stack);

                case ExpressionType.Parameter:
                    return RewriteVariableAssignment(node, stack);

                case ExpressionType.Extension:
                    return RewriteExtensionAssignment(node, stack);

                default:
                    throw new InvalidOperationException($"Invalid lvalue for assignment: {node.Left.NodeType}.");
            }
        }

        private Result RewriteBinaryExpression(Expression expr, Stack stack)
        {
            var node = (BinaryExpression)expr;

            var cr = new NullAwareChildRewriter(this, stack, 3);
            // Left expression executes on the stack as left by parent
            cr.Add(node.Left);
            // Right expression always has non-empty stack (left is on it)
            cr.Add(node.Right);
            // conversion is a lambda, stack state will be ignored
            cr.Add(node.Conversion!);

            if (cr.Action == RewriteAction.SpillStack)
            {
                RequireNoRefArgs(node.Method);
            }

            return cr.Finish
            (
                cr.Rewrite
                    ? BinaryExpression.Create
                    (
                        node.NodeType,
                        cr[0]!,
                        cr[1]!,
                        node.Type,
                        node.Method,
                        (LambdaExpression)cr[2]!
                    )
                    : expr
            );
        }

        private Result RewriteBlockExpression(Expression expr, Stack stack)
        {
            var node = (BlockExpression)expr;

            var count = node.ExpressionCount;
            var action = RewriteAction.None;
            Expression[]? clone = null;
            for (var i = 0; i < count; i++)
            {
                var expression = node.GetExpression(i);

                // All statements within the block execute at the
                // same stack state.
                var rewritten = RewriteExpression(expression, stack);
                action |= rewritten.Action;

                if (clone == null && rewritten.Action != RewriteAction.None)
                {
                    clone = Clone(node.Expressions.AsArrayInternal(), i);
                }

                if (clone != null)
                {
                    clone[i] = rewritten.Node;
                }
            }

            if (clone != null)
            {
                // Okay to wrap since we know no one can mutate the clone array.
                expr = node.Rewrite(null, clone);
            }

            return new Result(action, expr);
        }

        private Result RewriteConditionalExpression(Expression expr, Stack stack)
        {
            var node = (ConditionalExpression)expr;

            // Test executes at the stack as left by parent.
            var test = RewriteExpression(node.Test, stack);
            // The test is popped by conditional jump so branches execute
            // at the stack as left by parent too.
            var ifTrue = RewriteExpression(node.IfTrue, stack);
            var ifFalse = RewriteExpression(node.IfFalse, stack);

            var action = test.Action | ifTrue.Action | ifFalse.Action;
            if (action != RewriteAction.None)
            {
                expr = ConditionalExpression.Make(test.Node, ifTrue.Node, ifFalse.Node, node.Type);
            }

            return new Result(action, expr);
        }

        private Result RewriteDynamicExpression(Expression expr)
        {
            var node = (IDynamicExpression)expr;

            // CallSite is on the stack.
            var cr = new ChildRewriter(this, Stack.NonEmpty, node.ArgumentCount);

            cr.AddArguments(node);

            if (cr.Action == RewriteAction.SpillStack)
            {
                RequireNoRefArgs(node.DelegateType.GetInvokeMethod());
            }

            return cr.Finish(cr.Rewrite ? node.Rewrite(cr[0, -1]) : expr);
        }

        private Result? RewriteExpressionFreeTemps(Expression? expression, Stack stack)
        {
            if (expression != null)
            {
                return RewriteExpressionFreeTempsNotNull(expression, stack);
            }

            return null;
        }

        private Result RewriteExpressionFreeTempsNotNull(Expression expression, Stack stack)
        {
            var mark = Mark();
            var result = RewriteExpression(expression, stack);
            Free(mark);
            return result;
        }

        private Result RewriteExtensionAssignment(BinaryExpression node, Stack stack)
        {
            node = new AssignBinaryExpression(node.Left.ReduceExtensions(), node.Right);

            var result = RewriteAssignBinaryExpression(node, stack);

            // it's at least Copy because we reduced the node
            return new Result(result.Action | RewriteAction.Copy, result.Node);
        }

        private Result RewriteExtensionExpression(Expression expr, Stack stack)
        {
            var result = RewriteExpression(expr.ReduceExtensions(), stack);
            // it's at least Copy because we reduced the node
            return new Result(result.Action | RewriteAction.Copy, result.Node);
        }

        // Note: goto does not necessarily need an empty stack. We could always
        // emit it as a "leave" which would clear the stack for us. That would
        // prevent us from doing certain optimizations we might want to do,
        // however, like the switch-case-goto pattern. For now, be conservative.
        private Result RewriteGotoExpression(Expression expr, Stack stack)
        {
            var node = (GotoExpression)expr;

            // Goto requires empty stack to execute so the expression is
            // going to execute on an empty stack.
            var result = RewriteExpressionFreeTemps(node.Value, Stack.Empty);

            if (result == null)
            {
                // However, the statement itself needs an empty stack for itself
                // so if stack is not empty, rewrite to empty the stack.
                if (stack != Stack.Empty)
                {
                    expr = Expression.MakeGoto(node.Kind, node.Target, null, node.Type);
                }

                return new Result(RewriteAction.None, expr);
            }

            // However, the statement itself needs an empty stack for itself
            // so if stack is not empty, rewrite to empty the stack.
            var value = result.Value;
            var action = value.Action;
            if (stack != Stack.Empty)
            {
                action = RewriteAction.SpillStack;
            }

            if (action != RewriteAction.None)
            {
                expr = Expression.MakeGoto(node.Kind, node.Target, value.Node, node.Type);
            }

            return new Result(action, expr);
        }

        private Result RewriteIndexAssignment(BinaryExpression node, Stack stack)
        {
            var index = (IndexExpression)node.Left;

            var cr = new NullAwareChildRewriter(this, stack, 2 + index.ArgumentCount);

            cr.Add(index.Object);
            cr.AddArguments(index);
            cr.Add(node.Right);

            if (cr.Action == RewriteAction.SpillStack)
            {
                cr.MarkRefInstance(index.Object);
            }

            if (cr.Rewrite)
            {
                node = new AssignBinaryExpression
                (
                    new IndexExpression
                    (
                        cr[0], // Object
                        index.Indexer,
                        cr[1, -2]! // arguments
                    ),
                    cr[-1]! // value
                );
            }

            return cr.Finish(node);
        }

        private Result RewriteIndexExpression(Expression expr, Stack stack)
        {
            var node = (IndexExpression)expr;

            var cr = new NullAwareChildRewriter(this, stack, node.ArgumentCount + 1);

            // For instance methods, the instance executes on the
            // stack as is, but stays on the stack, making it non-empty.
            cr.Add(node.Object);
            cr.AddArguments(node);

            if (cr.Action == RewriteAction.SpillStack)
            {
                cr.MarkRefInstance(node.Object);
            }

            if (cr.Rewrite)
            {
                expr = new IndexExpression
                (
                    cr[0],
                    node.Indexer,
                    cr[1, -1]!
                );
            }

            return cr.Finish(expr);
        }

        private Result RewriteInvocationExpression(Expression expr, Stack stack)
        {
            var node = (InvocationExpression)expr;

            ChildRewriter cr;

            // See if the lambda will be inlined.
            var lambda = node.LambdaOperand;
            if (lambda != null)
            {
                // Arguments execute on current stack.
                cr = new ChildRewriter(this, stack, node.ArgumentCount);
                cr.AddArguments(node);

                if (cr.Action == RewriteAction.SpillStack)
                {
                    cr.MarkRefArgs(Expression.GetInvokeMethod(node.Expression), 0);
                }

                // Lambda body also executes on current stack.
                var spiller = new StackSpiller(stack);
                lambda = lambda.AcceptStackSpiller(spiller);

                if (cr.Rewrite || spiller._lambdaRewrite != RewriteAction.None)
                {
                    node = new InvocationExpressionN(lambda, cr[0, -1], node.Type);
                }

                var result = cr.Finish(node);
                return new Result(result.Action | spiller._lambdaRewrite, result.Node);
            }

            cr = new ChildRewriter(this, stack, node.ArgumentCount + 1);

            // First argument starts on stack as provided.
            cr.Add(node.Expression);

            // Rest of arguments have non-empty stack (the delegate instance is on the stack).
            cr.AddArguments(node);

            if (cr.Action == RewriteAction.SpillStack)
            {
                cr.MarkRefArgs(Expression.GetInvokeMethod(node.Expression), 1);
            }

            return cr.Finish(cr.Rewrite ? new InvocationExpressionN(cr[0], cr[1, -1], node.Type) : expr);
        }

        private Result RewriteLabelExpression(Expression expr, Stack stack)
        {
            var node = (LabelExpression)expr;

            Result? result = null;

            if (node.DefaultValue != null)
            {
                result = RewriteExpression(node.DefaultValue, stack);
            }

            if (result == null)
            {
                return new Result(RewriteAction.None, expr);
            }

            var expression = result.Value;
            if (expression.Action != RewriteAction.None)
            {
                expr = new LabelExpression(node.Target, expression.Node);
            }

            return new Result(expression.Action, expr);
        }

        private Result RewriteListInitExpression(Expression expr, Stack stack)
        {
            var node = (ListInitExpression)expr;

            // Constructor runs on initial stack.
            var newResult = RewriteExpression(node.NewExpression, stack);
            var rewrittenNew = newResult.Node;
            var action = newResult.Action;

            var initializers = node.Initializers;
            var count = initializers.Count;

            var cloneCrs = new ChildRewriter[count];

            for (var i = 0; i < count; i++)
            {
                var init = initializers[i];

                // Initializers all run on non-empty stack (the list instance is on it).
                var cr = new ChildRewriter(this, Stack.NonEmpty, init.Arguments.Count);
                cr.Add(init.Arguments);

                action |= cr.Action;
                cloneCrs[i] = cr;
            }

            switch (action)
            {
                case RewriteAction.None:
                    break;

                case RewriteAction.Copy:
                    var newInitializer = new ElementInit[count];
                    for (var i = 0; i < count; i++)
                    {
                        var cr = cloneCrs[i];
                        if (cr.Action == RewriteAction.None)
                        {
                            newInitializer[i] = initializers[i];
                        }
                        else
                        {
                            newInitializer[i] = new ElementInit(initializers[i].AddMethod, cr[0, -1]);
                        }
                    }

                    expr = new ListInitExpression((NewExpression)rewrittenNew, ReadOnlyCollectionEx.Create(newInitializer));
                    break;

                case RewriteAction.SpillStack:
                    var isRefNew = IsRefInstance(node.NewExpression);

                    var comma = new ArrayBuilder<Expression>(count + 2 + (isRefNew ? 1 : 0));
                    var tempNew = MakeTemp(rewrittenNew.Type);
                    comma.UncheckedAdd(new AssignBinaryExpression(tempNew, rewrittenNew));

                    var refTempNew = tempNew;
                    if (isRefNew)
                    {
                        refTempNew = MakeTemp(tempNew.Type.MakeByRefType());
                        comma.UncheckedAdd(new ByRefAssignBinaryExpression(refTempNew, tempNew));
                    }

                    for (var i = 0; i < count; i++)
                    {
                        var cr = cloneCrs[i];
                        var add = cr.Finish(new InstanceMethodCallExpressionN(initializers[i].AddMethod, refTempNew, cr[0, -1]));
                        comma.UncheckedAdd(add.Node);
                    }

                    comma.UncheckedAdd(tempNew);

                    expr = MakeBlock(comma);
                    break;

                default:
                    throw ContractUtils.Unreachable;
            }

            return new Result(action, expr);
        }

        // BinaryExpression: AndAlso, OrElse
        private Result RewriteLogicalBinaryExpression(Expression expr, Stack stack)
        {
            var node = (BinaryExpression)expr;

            // Left expression runs on a stack as left by parent
            var left = RewriteExpression(node.Left, stack);
            // ... and so does the right one
            var right = RewriteExpression(node.Right, stack);
            //conversion is a lambda. stack state will be ignored.

            Result? conversion = null;

            if (node.Conversion != null)
            {
                conversion = RewriteExpression(node.Conversion, stack);
            }

            if (conversion == null)
            {
                return new Result(RewriteAction.None, expr);
            }

            var action = left.Action | right.Action | conversion.Value.Action;

            if (action != RewriteAction.None)
            {
                // We don't have to worry about byref parameters here, because the
                // factory doesn't allow it (it requires identical parameters and
                // return type from the AndAlso/OrElse method)

                expr = BinaryExpression.Create
                (
                    node.NodeType,
                    left.Node,
                    right.Node,
                    node.Type,
                    node.Method,
                    (LambdaExpression)conversion.Value.Node
                );
            }

            return new Result(action, expr);
        }

        private Result RewriteLoopExpression(Expression expr, Stack stack)
        {
            var node = (LoopExpression)expr;

            // The loop statement requires empty stack for itself, so it
            // can guarantee it to the child nodes.
            var body = RewriteExpression(node.Body, Stack.Empty);

            var action = body.Action;

            // However, the loop itself requires that it executes on an empty stack
            // so we need to rewrite if the stack is not empty.
            if (stack != Stack.Empty)
            {
                action = RewriteAction.SpillStack;
            }

            if (action != RewriteAction.None)
            {
                expr = new LoopExpression(body.Node, node.BreakLabel, node.ContinueLabel);
            }

            return new Result(action, expr);
        }

        private Result RewriteMemberAssignment(BinaryExpression node, Stack stack)
        {
            var lvalue = (MemberExpression)node.Left;

            var cr = new NullAwareChildRewriter(this, stack, 2);

            // If there's an instance, it executes on the stack in current state
            // and rest is executed on non-empty stack.
            // Otherwise the stack is left unchanged.
            cr.Add(lvalue.Expression);

            cr.Add(node.Right);

            if (cr.Action == RewriteAction.SpillStack)
            {
                cr.MarkRefInstance(lvalue.Expression);
            }

            if (cr.Rewrite)
            {
                return cr.Finish
                (
                    new AssignBinaryExpression
                    (
                        MemberExpression.Make(cr[0]!, lvalue.Member),
                        cr[1]!
                    )
                );
            }

            return new Result(RewriteAction.None, node);
        }

        private Result RewriteMemberExpression(Expression expr, Stack stack)
        {
            var node = (MemberExpression)expr;

            // Expression is emitted on top of the stack in current state.

            Result? expression = null;

            if (node.Expression != null)
            {
                expression = RewriteExpression(node.Expression!, stack);
            }

            if (expression == null)
            {
                return new Result(RewriteAction.None, expr);
            }

            switch (expression.Value.Action)
            {
                case RewriteAction.None:
                    return new Result(expression.Value.Action, expr);

                case RewriteAction.SpillStack when node.Member is PropertyInfo:
                    // Only need to validate properties because reading a field
                    // is always side-effect free.
                    RequireNotRefInstance(node.Expression!);
                    break;

                default:
                    break;
            }

            expr = MemberExpression.Make(expression.Value.Node, node.Member);

            return new Result(expression.Value.Action, expr);
        }

        private Result RewriteMemberInitExpression(Expression expr, Stack stack)
        {
            var node = (MemberInitExpression)expr;

            // Constructor runs on initial stack.
            var result = RewriteExpression(node.NewExpression, stack);
            var rewrittenNew = result.Node;
            var action = result.Action;

            var bindings = node.Bindings;
            var count = bindings.Count;

            var bindingRewriters = new BindingRewriter[count];

            for (var i = 0; i < count; i++)
            {
                var binding = bindings[i];

                // Bindings run on non-empty stack (the object instance is on it).
                var rewriter = BindingRewriter.Create(binding, this, Stack.NonEmpty);
                bindingRewriters[i] = rewriter;

                action |= rewriter.Action;
            }

            switch (action)
            {
                case RewriteAction.None:
                    break;

                case RewriteAction.Copy:
                    var newBindings = new MemberBinding[count];
                    for (var i = 0; i < count; i++)
                    {
                        newBindings[i] = bindingRewriters[i].AsBinding();
                    }

                    expr = new MemberInitExpression((NewExpression)rewrittenNew, newBindings);
                    break;

                case RewriteAction.SpillStack:
                    var isRefNew = IsRefInstance(node.NewExpression);

                    var comma = new ArrayBuilder<Expression>(count + 2 + (isRefNew ? 1 : 0));

                    var tempNew = MakeTemp(rewrittenNew.Type);
                    comma.UncheckedAdd(new AssignBinaryExpression(tempNew, rewrittenNew));

                    var refTempNew = tempNew;
                    if (isRefNew)
                    {
                        refTempNew = MakeTemp(tempNew.Type.MakeByRefType());
                        comma.UncheckedAdd(new ByRefAssignBinaryExpression(refTempNew, tempNew));
                    }

                    for (var i = 0; i < count; i++)
                    {
                        var cr = bindingRewriters[i];
                        var initExpr = cr.AsExpression(refTempNew);
                        comma.UncheckedAdd(initExpr);
                    }

                    comma.UncheckedAdd(tempNew);

                    expr = MakeBlock(comma);
                    break;

                default:
                    throw ContractUtils.Unreachable;
            }

            return new Result(action, expr);
        }

        private Result RewriteMethodCallExpression(Expression expr, Stack stack)
        {
            var node = (MethodCallExpression)expr;

            var cr = new NullAwareChildRewriter(this, stack, node.ArgumentCount + 1);

            // For instance methods, the instance executes on the
            // stack as is, but stays on the stack, making it non-empty.
            cr.Add(node.Object);

            cr.AddArguments(node);

            if (cr.Action == RewriteAction.SpillStack)
            {
                cr.MarkRefInstance(node.Object);
                cr.MarkRefArgs(node.Method, 1);
            }

            if (cr.Rewrite)
            {
                expr = node.Object != null
                    ? new InstanceMethodCallExpressionN(node.Method, cr[0], cr[1, -1]!)
                    : new MethodCallExpressionN(node.Method, cr[1, -1]!);
            }

            return cr.Finish(expr);
        }

        private Result RewriteNewArrayExpression(Expression expr, Stack stack)
        {
            var node = (NewArrayExpression)expr;

            if (node.NodeType == ExpressionType.NewArrayInit)
            {
                // In a case of array construction with element initialization
                // the element expressions are never emitted on an empty stack because
                // the array reference and the index are on the stack.
                stack = Stack.NonEmpty;
            }

            var cr = new ChildRewriter(this, stack, node.Expressions.Count);
            cr.Add(node.Expressions);

            if (cr.Rewrite)
            {
                expr = NewArrayExpression.Make(node.NodeType, node.Type, ReadOnlyCollectionEx.Create(cr[0, -1]));
            }

            return cr.Finish(expr);
        }

        private Result RewriteNewExpression(Expression expr, Stack stack)
        {
            var node = (NewExpression)expr;

            // The first expression starts on a stack as provided by parent,
            // rest are definitely non-empty (which ChildRewriter guarantees).
            var cr = new ChildRewriter(this, stack, node.ArgumentCount);
            cr.AddArguments(node);

            if (cr.Action == RewriteAction.SpillStack)
            {
                cr.MarkRefArgs(node.Constructor!, 0);
            }

            return cr.Finish(cr.Rewrite ? new NewExpression(node.Constructor, cr[0, -1], node.Members) : expr);
        }

        private Result RewriteReducibleExpression(Expression expr, Stack stack)
        {
            var result = RewriteExpression(expr.Reduce(), stack);

            // It's at least Copy because we reduced the node.
            return new Result(result.Action | RewriteAction.Copy, result.Node);
        }

        private Result RewriteSwitchExpression(Expression expr, Stack stack)
        {
            var node = (SwitchExpression)expr;

            // The switch statement test is emitted on the stack in current state.
            var result = RewriteExpressionFreeTempsNotNull(node.SwitchValue, stack);

            var action = result.Action;
            var cases = node.Cases.AsArrayInternal();
            SwitchCase[]? clone = null;
            for (var i = 0; i < cases.Length; i++)
            {
                var @case = cases[i];

                Expression[]? cloneTests = null;
                var testValues = @case.TestValues.AsArrayInternal();
                for (var j = 0; j < testValues.Length; j++)
                {
                    // All tests execute at the same stack state as the switch.
                    // This is guaranteed by the compiler (to simplify spilling).
                    var test = RewriteExpression(testValues[j], stack);
                    action |= test.Action;

                    if (cloneTests == null && test.Action != RewriteAction.None)
                    {
                        cloneTests = Clone(testValues, j);
                    }

                    if (cloneTests != null)
                    {
                        cloneTests[j] = test.Node;
                    }
                }

                // And all the cases also run on the same stack level.
                var body = RewriteExpression(@case.Body, stack);
                action |= body.Action;

                if (body.Action != RewriteAction.None || cloneTests != null)
                {
                    if (cloneTests != null)
                    {
                        testValues = cloneTests;
                    }

                    @case = new SwitchCase(body.Node, testValues);

                    if (clone == null)
                    {
                        clone = Clone(cases, i);
                    }
                }

                if (clone != null)
                {
                    clone[i] = @case;
                }
            }

            if (clone != null)
            {
                // okay to wrap because we aren't modifying the array
                cases = clone;
            }

            // default body also runs on initial stack

            Result? defaultBody = null;

            if (node.DefaultBody != null)
            {
                defaultBody = RewriteExpression(node.DefaultBody, stack);
            }

            action |= defaultBody?.Action ?? RewriteAction.None;

            if (action == RewriteAction.None)
            {
                return new Result(action, expr);
            }

            expr = new SwitchExpression(node.Type, result.Node, defaultBody?.Node, node.Comparison, cases);

            return new Result(action, expr);
        }

        private Result RewriteThrowUnaryExpression(Expression expr, Stack stack)
        {
            var node = (UnaryExpression)expr;

            // Throw statement itself does not care about the stack
            // but it will empty the stack and it may cause stack imbalance
            // it so we need to restore stack after unconditional throw to make JIT happy
            // this has an effect of executing Throw on an empty stack.

            var result = RewriteExpressionFreeTemps(node.Operand, Stack.Empty);

            if (result == null)
            {
                if (stack != Stack.Empty)
                {
                    expr = new UnaryExpression(ExpressionType.Throw, null, node.Type, null);
                }

                return new Result(RewriteAction.None, expr);
            }

            var value = result.Value;
            var action = value.Action;

            if (stack != Stack.Empty)
            {
                action = RewriteAction.SpillStack;
            }

            if (action != RewriteAction.None)
            {
                expr = new UnaryExpression(ExpressionType.Throw, value.Node, node.Type, null);
            }

            return new Result(action, expr);
        }

        private Result RewriteTryExpression(Expression expr, Stack stack)
        {
            var node = (TryExpression)expr;

            // Try statement definitely needs an empty stack so its
            // child nodes execute at empty stack.
            var body = RewriteExpression(node.Body, Stack.Empty);
            var handlers = node.Handlers.AsArrayInternal();

            var action = ProcessHandlers(body, ref handlers);

            var fault = RewriteExpression(node.Fault!, Stack.Empty);
            action |= fault.Action;

            var @finally = RewriteExpression(node.Finally!, Stack.Empty);
            action |= @finally.Action;

            // If the stack is initially not empty, rewrite to spill the stack
            if (stack != Stack.Empty)
            {
                action = RewriteAction.SpillStack;
            }

            if (action == RewriteAction.None)
            {
                return new Result(action, expr);
            }

            expr = new TryExpression(node.Type, body.Node, @finally.Node, fault.Node, handlers);

            return new Result(action, expr);
        }

        private Result RewriteTypeBinaryExpression(Expression expr, Stack stack)
        {
            var node = (TypeBinaryExpression)expr;

            // The expression is emitted on top of current stack.
            var expression = RewriteExpression(node.Expression, stack);

            if (expression.Action != RewriteAction.None)
            {
                expr = new TypeBinaryExpression(expression.Node, node.TypeOperand, node.NodeType);
            }

            return new Result(expression.Action, expr);
        }

        private Result RewriteUnaryExpression(Expression expr, Stack stack)
        {
            var node = (UnaryExpression)expr;

            Debug.Assert(node.NodeType != ExpressionType.Quote, "unexpected Quote");
            Debug.Assert(node.NodeType != ExpressionType.Throw, "unexpected Throw");

            // Operand is emitted on top of the stack as-is.
            Result? result = null;

            if (node.Operand != null)
            {
                result = RewriteExpression(node.Operand, stack);
            }

            if (result == null)
            {
                return new Result(RewriteAction.None, expr);
            }

            var expression = result.Value;

            if (expression.Action == RewriteAction.SpillStack)
            {
                RequireNoRefArgs(node.Method);
            }

            if (expression.Action != RewriteAction.None)
            {
                expr = new UnaryExpression(node.NodeType, expression.Node, node.Type, node.Method);
            }

            return new Result(expression.Action, expr);
        }

        private Result RewriteVariableAssignment(BinaryExpression node, Stack stack)
        {
            // Expression is evaluated on a stack in current state.
            var right = RewriteExpression(node.Right, stack);

            if (right.Action != RewriteAction.None)
            {
                node = new AssignBinaryExpression(node.Left, right.Node);
            }

            return new Result(right.Action, node);
        }

        /// <summary>
        ///     Result of a rewrite operation. Always contains an action and a node.
        /// </summary>
        private readonly struct Result
        {
            internal readonly RewriteAction Action;
            internal readonly Expression Node;

            internal Result(RewriteAction action, Expression node)
            {
                Action = action;
                Node = node;
            }
        }
    }
}

#endif