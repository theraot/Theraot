#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>
        ///     Creates a <see cref="ParameterExpression" /> node that can be used to identify a parameter or a variable in an
        ///     expression tree.
        /// </summary>
        /// <param name="type">The type of the parameter or variable.</param>
        /// <returns>A <see cref="ParameterExpression" /> node with the specified name and type.</returns>
        public static ParameterExpression Parameter(Type type)
        {
            return Parameter(type, null);
        }

        /// <summary>
        ///     Creates a <see cref="ParameterExpression" /> node that can be used to identify a parameter or a variable in an
        ///     expression tree.
        /// </summary>
        /// <param name="type">The type of the parameter or variable.</param>
        /// <param name="name">The name of the parameter or variable, used for debugging or pretty printing purpose only.</param>
        /// <returns>A <see cref="ParameterExpression" /> node with the specified name and type.</returns>
        public static ParameterExpression Parameter(Type type, string name)
        {
            Validate(type, true);
            var byref = type.IsByRef;
            if (byref)
            {
                type = type.GetElementType();
            }

            return ParameterExpression.Make(type, name, byref);
        }

        /// <summary>
        ///     Creates a <see cref="ParameterExpression" /> node that can be used to identify a parameter or a variable in an
        ///     expression tree.
        /// </summary>
        /// <param name="type">The type of the parameter or variable.</param>
        /// <returns>A <see cref="ParameterExpression" /> node with the specified name and type.</returns>
        public static ParameterExpression Variable(Type type)
        {
            return Variable(type, null);
        }

        /// <summary>
        ///     Creates a <see cref="ParameterExpression" /> node that can be used to identify a parameter or a variable in an
        ///     expression tree.
        /// </summary>
        /// <param name="type">The type of the parameter or variable.</param>
        /// <param name="name">The name of the parameter or variable, used for debugging or pretty printing purpose only.</param>
        /// <returns>A <see cref="ParameterExpression" /> node with the specified name and type.</returns>
        public static ParameterExpression Variable(Type type, string name)
        {
            Validate(type, false);
            return ParameterExpression.Make(type, name, false);
        }

        private static void Validate(Type type, bool allowByRef)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));
            TypeUtils.ValidateType(type, nameof(type), allowByRef, false);

            if (type == typeof(void))
            {
                throw new ArgumentException("Argument type cannot be System.Void.", nameof(type));
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Represents a named parameter expression.
    /// </summary>
    [DebuggerTypeProxy(typeof(ParameterExpressionProxy))]
    public class ParameterExpression : Expression
    {
        internal ParameterExpression(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Indicates that this <see cref="ParameterExpression" /> is to be treated as a ByRef parameter.
        /// </summary>
        public bool IsByRef => GetIsByRef();

        /// <summary>
        ///     The Name of the parameter or variable.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Returns the node type of this <see cref="T:System.Linq.Expressions.Expression" />. (Inherited from
        ///     <see cref="T:System.Linq.Expressions.Expression" />.)
        /// </summary>
        /// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents this expression.</returns>
        public sealed override ExpressionType NodeType => ExpressionType.Parameter;

        /// <inheritdoc />
        /// <summary>
        ///     Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.
        ///     (Inherited from <see cref="T:System.Linq.Expressions.Expression" />.)
        /// </summary>
        /// <returns>The <see cref="T:System.Type" /> that represents the static type of the expression.</returns>
        public override Type Type => typeof(object);

        internal static ParameterExpression Make(Type type, string name, bool isByRef)
        {
            if (isByRef)
            {
                return new ByRefParameterExpression(type, name);
            }

            if (type.IsEnum)
            {
                return new TypedParameterExpression(type, name);
            }

            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean: return new PrimitiveParameterExpression<bool>(name);
                case TypeCode.Byte: return new PrimitiveParameterExpression<byte>(name);
                case TypeCode.Char: return new PrimitiveParameterExpression<char>(name);
                case TypeCode.DateTime: return new PrimitiveParameterExpression<DateTime>(name);
                case TypeCode.Decimal: return new PrimitiveParameterExpression<decimal>(name);
                case TypeCode.Double: return new PrimitiveParameterExpression<double>(name);
                case TypeCode.Int16: return new PrimitiveParameterExpression<short>(name);
                case TypeCode.Int32: return new PrimitiveParameterExpression<int>(name);
                case TypeCode.Int64: return new PrimitiveParameterExpression<long>(name);
                case TypeCode.Object:
                    // common reference types which we optimize go here.  Of course object is in
                    // the list, the others are driven by profiling of various workloads.  This list
                    // should be kept short.
                    if (type == typeof(object))
                    {
                        return new ParameterExpression(name);
                    }
                    else if (type == typeof(Exception))
                    {
                        return new PrimitiveParameterExpression<Exception>(name);
                    }
                    else if (type == typeof(object[]))
                    {
                        return new PrimitiveParameterExpression<object[]>(name);
                    }

                    break;

                case TypeCode.SByte: return new PrimitiveParameterExpression<sbyte>(name);
                case TypeCode.Single: return new PrimitiveParameterExpression<float>(name);
                case TypeCode.String: return new PrimitiveParameterExpression<string>(name);
                case TypeCode.UInt16: return new PrimitiveParameterExpression<ushort>(name);
                case TypeCode.UInt32: return new PrimitiveParameterExpression<uint>(name);
                case TypeCode.UInt64: return new PrimitiveParameterExpression<ulong>(name);
                default:
                    break;
            }

            return new TypedParameterExpression(type, name);
        }

        internal virtual bool GetIsByRef()
        {
            return false;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitParameter(this);
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Specialized subclass to avoid holding onto the byref flag in a
    ///     parameter expression.  This version always holds onto the expression
    ///     type explicitly and therefore derives from TypedParameterExpression.
    /// </summary>
    internal sealed class ByRefParameterExpression : TypedParameterExpression
    {
        internal ByRefParameterExpression(Type type, string name)
            : base(type, name)
        {
            // Empty
        }

        internal override bool GetIsByRef()
        {
            return true;
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Generic type to avoid needing explicit storage for primitive data types
    ///     which are commonly used.
    /// </summary>
    internal sealed class PrimitiveParameterExpression<T> : ParameterExpression
    {
        internal PrimitiveParameterExpression(string name)
            : base(name)
        {
            // Empty
        }

        public override Type Type => typeof(T);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Specialized subclass which holds onto the type of the expression for
    ///     uncommon types.
    /// </summary>
    internal class TypedParameterExpression : ParameterExpression
    {
        internal TypedParameterExpression(Type type, string name)
            : base(name)
        {
            Type = type;
        }

        public sealed override Type Type { get; }
    }
}

#endif