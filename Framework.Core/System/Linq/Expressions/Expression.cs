﻿#if LESSTHAN_NET35

#pragma warning disable CA1062 // Validate arguments of public methods

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Collections.ThreadSafe;
using Theraot.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    ///     The base type for all nodes in Expression Trees.
    /// </summary>
    public abstract partial class Expression
    {
        private static readonly CacheDict<Type, MethodInfo> _lambdaDelegateCache = new(40);
        private static CacheDict<Type, Func<Expression, string?, bool, ParameterExpression[], LambdaExpression>>? _lambdaFactories;

        // For 4.0, many frequently used Expression nodes have had their memory
        // footprint reduced by removing the Type and NodeType fields. This has
        // large performance benefits to all users of Expression Trees.
        //
        // To support the 3.5 protected constructor, we store the fields that
        // used to be here in a ConditionalWeakTable.

        private static ConditionalWeakTable<Expression, ExtensionInfo>? _legacyCtorSupportTable;

        /// <summary>
        ///     Constructs a new instance of <see cref="Expression" />.
        /// </summary>
        /// <param name="nodeType">The <see ctype="ExpressionType" /> of the <see cref="Expression" />.</param>
        /// <param name="type">The <see cref="Type" /> of the <see cref="Expression" />.</param>
        [Obsolete("use a different constructor that does not take ExpressionType. Then override NodeType and Type properties to provide the values that would be specified to this constructor.")]
        protected Expression(ExpressionType nodeType, Type type)
        {
            var legacyCtorSupportTable = TypeHelper.LazyCreateNew(ref _legacyCtorSupportTable);
            legacyCtorSupportTable.Add(this, new ExtensionInfo(nodeType, type));
        }

        /// <summary>
        ///     Constructs a new instance of <see cref="Expression" />.
        /// </summary>
        protected Expression()
        {
            // Empty
        }

        /// <summary>
        ///     Indicates that the node can be reduced to a simpler node. If this
        ///     returns true, Reduce() can be called to produce the reduced form.
        /// </summary>
        public virtual bool CanReduce => false;

        /// <summary>
        ///     The <see cref="ExpressionType" /> of the <see cref="Expression" />.
        /// </summary>
        public virtual ExpressionType NodeType
        {
            get
            {
                if (_legacyCtorSupportTable != null && _legacyCtorSupportTable.TryGetValue(this, out var extInfo))
                {
                    return extInfo.NodeType;
                }

                // the extension expression failed to override NodeType
                throw new InvalidOperationException("Extension node must override the property Expression.NodeType.");
            }
        }

        /// <summary>
        ///     The <see cref="Type" /> of the value represented by this <see cref="Expression" />.
        /// </summary>
        public virtual Type Type
        {
            get
            {
                if (_legacyCtorSupportTable != null && _legacyCtorSupportTable.TryGetValue(this, out var extInfo))
                {
                    return extInfo.Type;
                }

                // the extension expression failed to override Type
                throw new InvalidOperationException("Extension node must override the property Expression.Type.");
            }
        }

        /// <summary>
        ///     Creates a <see cref="string" /> representation of the Expression.
        /// </summary>
        /// <returns>A <see cref="string" /> representation of the Expression.</returns>
        private string DebugView
        {
            // Note that this property is often accessed using reflection. As such it will have more dependencies than one
            // might surmise from its being internal, and removing it requires greater caution than with other internal methods.
            get
            {
                using (var writer = new StringWriter(CultureInfo.CurrentCulture))
                {
                    DebugViewWriter.WriteTo(this, writer);
                    return writer.ToString();
                }
            }
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, IEnumerable<Expression> arguments)
        {
            return DynamicExpression.Dynamic(binder, returnType, arguments);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0)
        {
            return DynamicExpression.Dynamic(binder, returnType, arg0);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1)
        {
            return DynamicExpression.Dynamic(binder, returnType, arg0, arg1);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2)
        {
            return DynamicExpression.Dynamic(binder, returnType, arg0, arg1, arg2);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <param name="arg3">The fourth argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
        {
            return DynamicExpression.Dynamic(binder, returnType, arg0, arg1, arg2, arg3);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, params Expression[] arguments)
        {
            return DynamicExpression.Dynamic(binder, returnType, arguments);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, IEnumerable<Expression> arguments)
        {
            return DynamicExpression.MakeDynamic(delegateType, binder, arguments);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and one argument.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0)
        {
            return DynamicExpression.MakeDynamic(delegateType, binder, arg0);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and two arguments.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
        {
            return DynamicExpression.MakeDynamic(delegateType, binder, arg0, arg1);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and three arguments.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
        {
            return DynamicExpression.MakeDynamic(delegateType, binder, arg0, arg1, arg2);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and four arguments.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <param name="arg3">The fourth argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
        {
            return DynamicExpression.MakeDynamic(delegateType, binder, arg0, arg1, arg2, arg3);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, params Expression[] arguments)
        {
            return MakeDynamic(delegateType, binder, (IEnumerable<Expression>)arguments);
        }

        /// <summary>
        ///     Reduces this node to a simpler expression. If CanReduce returns
        ///     true, this should return a valid expression. This method is
        ///     allowed to return another node which itself must be reduced.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public virtual Expression Reduce()
        {
            if (CanReduce)
            {
                throw new ArgumentException("reducible nodes must override Expression.Reduce()", string.Empty);
            }

            return this;
        }

        /// <summary>
        ///     Reduces this node to a simpler expression. If CanReduce returns
        ///     true, this should return a valid expression. This method is
        ///     allowed to return another node which itself must be reduced.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        /// <remarks>
        ///     Unlike Reduce, this method checks that the reduced node satisfies
        ///     certain invariants.
        /// </remarks>
        public Expression ReduceAndCheck()
        {
            if (!CanReduce)
            {
                throw new ArgumentException("must be reducible node", string.Empty);
            }

            var newNode = Reduce();

            // 1. Reduction must return a new, non-null node
            // 2. Reduction must return a new node whose result type can be assigned to the type of the original node
            if (newNode == null || newNode == this)
            {
                throw new ArgumentException("node cannot reduce to itself or null", string.Empty);
            }

            if (!Type.IsReferenceAssignableFromInternal(newNode.Type))
            {
                throw new ArgumentException("cannot assign from the reduced node type to the original node type", string.Empty);
            }

            return newNode;
        }

        /// <summary>
        ///     Reduces the expression to a known node type (i.e. not an Extension node)
        ///     or simply returns the expression if it is already a known type.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public Expression ReduceExtensions()
        {
            var node = this;
            while (node.NodeType == ExpressionType.Extension)
            {
                node = node.ReduceAndCheck();
            }

            return node;
        }

        /// <summary>
        ///     Creates a <see cref="string" /> representation of the Expression.
        /// </summary>
        /// <returns>A <see cref="string" /> representation of the Expression.</returns>
        public override string ToString()
        {
            return ExpressionStringBuilder.ExpressionToString(this);
        }

        /// <summary>
        ///     Dispatches to the specific visit method for this node type. For
        ///     example, <see cref="MethodCallExpression" /> will call into
        ///     <see cref="ExpressionVisitor.VisitMethodCall" />.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        /// <remarks>
        ///     This default implementation for <see cref="ExpressionType.Extension" />
        ///     nodes will call <see cref="ExpressionVisitor.VisitExtension" />.
        ///     Override this method to call into a more specific method on a derived
        ///     visitor class of ExpressionVisitor. However, it should still
        ///     support unknown visitors by calling VisitExtension.
        /// </remarks>
        protected internal virtual Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return visitor.VisitExtension(this);
        }

        /// <summary>
        ///     Reduces the node and then calls the <see cref="ExpressionVisitor.Visit(Expression)" /> method passing the reduced
        ///     expression.
        ///     Throws an exception if the node isn't reducible.
        /// </summary>
        /// <param name="visitor">An instance of <see cref="ExpressionVisitor" />.</param>
        /// <returns>The expression being visited, or an expression which should replace it in the tree.</returns>
        /// <remarks>
        ///     Override this method to provide logic to walk the node's children.
        ///     A typical implementation will call visitor.Visit on each of its
        ///     children, and if any of them change, should return a new copy of
        ///     itself with the modified children.
        /// </remarks>
        protected internal virtual Expression VisitChildren(ExpressionVisitor visitor)
        {
            if (!CanReduce)
            {
                throw new ArgumentException("must be reducible node", string.Empty);
            }

            return visitor.Visit(ReduceAndCheck());
        }

        private static void RequiresCanRead(Expression[] items, string paramName)
        {
            // this is called a lot, avoid allocating an enumerator if we can...
            for (int i = 0, n = items.Length; i < n; i++)
            {
                ContractUtils.RequiresNotNull(items[i], paramName, i);
                ExpressionUtils.RequiresCanRead(items[i], paramName, i);
            }
        }

        private static void RequiresCanWrite(Expression expression, string paramName)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Index:
                    var indexer = ((IndexExpression)expression).Indexer;
                    if (indexer?.CanWrite != false)
                    {
                        return;
                    }

                    break;

                case ExpressionType.MemberAccess:
                    var member = ((MemberExpression)expression).Member;
                    if (member is PropertyInfo prop)
                    {
                        if (prop.CanWrite)
                        {
                            return;
                        }
                    }
                    else
                    {
                        Debug.Assert(member is FieldInfo);
                        var field = (FieldInfo)member;
                        if (!field.IsInitOnly && !field.IsLiteral)
                        {
                            return;
                        }
                    }

                    break;

                case ExpressionType.Parameter:
                    return;

                default:
                    break;
            }

            throw new ArgumentException("Expression must be writable", paramName);
        }

        private sealed class ExtensionInfo
        {
            internal readonly ExpressionType NodeType;

            internal readonly Type Type;

            public ExtensionInfo(ExpressionType nodeType, Type type)
            {
                NodeType = nodeType;
                Type = type;
            }
        }
    }
}

#endif