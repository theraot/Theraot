#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Collections;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>
        ///     Creates a <see cref="TryExpression" /> representing a try block with the specified elements.
        /// </summary>
        /// <param name="type">The result type of the try expression. If null, body and all handlers must have identical type.</param>
        /// <param name="body">The body of the try block.</param>
        /// <param name="finally">
        ///     The body of the finally block. Pass null if the try block has no finally block associated with
        ///     it.
        /// </param>
        /// <param name="fault">The body of the t block. Pass null if the try block has no fault block associated with it.</param>
        /// <param name="handlers">
        ///     A collection of <see cref="CatchBlock" />s representing the catch statements to be associated
        ///     with the try block.
        /// </param>
        /// <returns>The created <see cref="TryExpression" />.</returns>
        public static TryExpression MakeTry(Type type, Expression body, Expression @finally, Expression fault, IEnumerable<CatchBlock> handlers)
        {
            ExpressionUtils.RequiresCanRead(body, nameof(body));

            var @catch = handlers.AsArrayInternal();
            ContractUtils.RequiresNotNullItems(@catch, nameof(handlers));
            ValidateTryAndCatchHaveSameType(type, body, @catch);

            if (fault != null)
            {
                if (@finally != null || @catch.Length > 0)
                {
                    throw new ArgumentException("fault cannot be used with catch or finally clauses", nameof(fault));
                }

                ExpressionUtils.RequiresCanRead(fault, nameof(fault));
            }
            else if (@finally != null)
            {
                ExpressionUtils.RequiresCanRead(@finally, nameof(@finally));
            }
            else if (@catch.Length == 0)
            {
                throw new ArgumentException("try must have at least one catch, finally, or fault clause");
            }

            return new TryExpression(type ?? body.Type, body, @finally, fault, @catch);
        }

        /// <summary>
        ///     Creates a <see cref="TryExpression" /> representing a try block with any number of catch statements and neither a
        ///     fault nor finally block.
        /// </summary>
        /// <param name="body">The body of the try block.</param>
        /// <param name="handlers">
        ///     The array of zero or more <see cref="CatchBlock" />s representing the catch statements to be
        ///     associated with the try block.
        /// </param>
        /// <returns>The created <see cref="TryExpression" />.</returns>
        public static TryExpression TryCatch(Expression body, params CatchBlock[] handlers)
        {
            return MakeTry(null, body, null, null, handlers);
        }

        /// <summary>
        ///     Creates a <see cref="TryExpression" /> representing a try block with any number of catch statements and a finally
        ///     block.
        /// </summary>
        /// <param name="body">The body of the try block.</param>
        /// <param name="finally">The body of the finally block.</param>
        /// <param name="handlers">
        ///     The array of zero or more <see cref="CatchBlock" />s representing the catch statements to be
        ///     associated with the try block.
        /// </param>
        /// <returns>The created <see cref="TryExpression" />.</returns>
        public static TryExpression TryCatchFinally(Expression body, Expression @finally, params CatchBlock[] handlers)
        {
            return MakeTry(null, body, @finally, null, handlers);
        }

        /// <summary>
        ///     Creates a <see cref="TryExpression" /> representing a try block with a fault block and no catch statements.
        /// </summary>
        /// <param name="body">The body of the try block.</param>
        /// <param name="fault">The body of the fault block.</param>
        /// <returns>The created <see cref="TryExpression" />.</returns>
        public static TryExpression TryFault(Expression body, Expression fault)
        {
            return MakeTry(null, body, null, fault, null);
        }

        /// <summary>
        ///     Creates a <see cref="TryExpression" /> representing a try block with a finally block and no catch statements.
        /// </summary>
        /// <param name="body">The body of the try block.</param>
        /// <param name="finally">The body of the finally block.</param>
        /// <returns>The created <see cref="TryExpression" />.</returns>
        public static TryExpression TryFinally(Expression body, Expression @finally)
        {
            return MakeTry(null, body, @finally, null, null);
        }

        //Validate that the body of the try expression must have the same type as the body of every try block.
        private static void ValidateTryAndCatchHaveSameType(Type type, Expression tryBody, IEnumerable<CatchBlock> handlers)
        {
            Debug.Assert(tryBody != null);
            // Type unification ... all parts must be reference assignable to "type"
            if (type != null)
            {
                if (type == typeof(void))
                {
                    return;
                }

                if (!type.IsReferenceAssignableFromInternal(tryBody.Type))
                {
                    throw new ArgumentException("Argument types do not match");
                }

                if (handlers.Any(cb => !type.IsReferenceAssignableFromInternal(cb.Body.Type)))
                {
                    throw new ArgumentException("Argument types do not match");
                }
            }
            else if (tryBody.Type == typeof(void))
            {
                //The body of every try block must be null or have void type.
                foreach (var cb in handlers)
                {
                    Debug.Assert(cb.Body != null);
                    if (cb.Body.Type != typeof(void))
                    {
                        throw new ArgumentException("Body of catch must have the same type as body of try.");
                    }
                }
            }
            else
            {
                //Body of every catch must have the same type of body of try.
                type = tryBody.Type;
                foreach (var cb in handlers)
                {
                    Debug.Assert(cb.Body != null);
                    if (!TypeUtils.AreEquivalent(cb.Body.Type, type))
                    {
                        throw new ArgumentException("Body of catch must have the same type as body of try.");
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     <para>Represents a try/catch/finally/fault block.</para>
    ///     <para>
    ///         The body is protected by the try block.
    ///         The handlers consist of a set of <see cref="T:System.Linq.Expressions.CatchBlock" />s that can either be catch
    ///         or filters.
    ///         The fault runs if an exception is thrown.
    ///         The finally runs regardless of how control exits the body.
    ///         Only one of fault or finally can be supplied.
    ///         The return type of the try block must match the return type of any associated catch statements.
    ///     </para>
    /// </summary>
    [DebuggerTypeProxy(typeof(TryExpressionProxy))]
    public sealed class TryExpression : Expression
    {
        private readonly CatchBlock[] _handlers;
        private readonly ReadOnlyCollectionEx<CatchBlock> _handlersAsReadOnlyCollection;

        internal TryExpression(Type type, Expression body, Expression @finally, Expression fault, CatchBlock[] handlers)
        {
            Type = type;
            Body = body;
            _handlers = handlers;
            Finally = @finally;
            Fault = fault;
            _handlersAsReadOnlyCollection = ReadOnlyCollectionEx.Create(_handlers);
        }

        /// <summary>
        ///     Gets the <see cref="Expression" /> representing the body of the try block.
        /// </summary>
        public Expression Body { get; }

        /// <summary>
        ///     Gets the <see cref="Expression" /> representing the fault block.
        /// </summary>
        public Expression Fault { get; }

        /// <summary>
        ///     Gets the <see cref="Expression" /> representing the finally block.
        /// </summary>
        public Expression Finally { get; }

        /// <summary>
        ///     Gets the collection of <see cref="CatchBlock" />s associated with the try block.
        /// </summary>
        public ReadOnlyCollection<CatchBlock> Handlers => _handlersAsReadOnlyCollection;

        /// <inheritdoc />
        /// <summary>
        ///     Returns the node type of this <see cref="T:System.Linq.Expressions.Expression" />. (Inherited from
        ///     <see cref="T:System.Linq.Expressions.Expression" />.)
        /// </summary>
        /// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents this expression.</returns>
        public override ExpressionType NodeType => ExpressionType.Try;

        /// <inheritdoc />
        /// <summary>
        ///     Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.
        ///     (Inherited from <see cref="T:System.Linq.Expressions.Expression" />.)
        /// </summary>
        /// <returns>The <see cref="T:System.Type" /> that represents the static type of the expression.</returns>
        public override Type Type { get; }

        /// <summary>
        ///     Creates a new expression that is like this one, but using the
        ///     supplied children. If all of the children are the same, it will
        ///     return this expression.
        /// </summary>
        /// <param name="body">The <see cref="Body" /> property of the result.</param>
        /// <param name="handlers">The <see cref="Handlers" /> property of the result.</param>
        /// <param name="finally">The <see cref="Finally" /> property of the result.</param>
        /// <param name="fault">The <see cref="Fault" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public TryExpression Update(Expression body, IEnumerable<CatchBlock> handlers, Expression @finally, Expression fault)
        {
            if (body == Body && @finally == Finally && fault == Fault && ExpressionUtils.SameElements(ref handlers, _handlers))
            {
                return this;
            }

            return MakeTry(Type, body, @finally, fault, handlers);
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return visitor.VisitTry(this);
        }
    }
}

#endif