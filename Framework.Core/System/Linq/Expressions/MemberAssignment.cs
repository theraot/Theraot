#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>
        ///     Creates a <see cref="MemberAssignment" /> binding the specified value to the given member.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo" /> for the member which is being assigned to.</param>
        /// <param name="expression">The value to be assigned to <paramref name="member" />.</param>
        /// <returns>The created <see cref="MemberAssignment" />.</returns>
        public static MemberAssignment Bind(MemberInfo member, Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            ContractUtils.RequiresNotNull(member, nameof(member));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            ValidateSettableFieldOrPropertyMember(member, out var memberType);
            if (!memberType.IsAssignableFrom(expression.Type))
            {
                throw new ArgumentException("Argument types do not match", string.Empty);
            }

            return new MemberAssignment(member, expression);
        }

        /// <summary>
        ///     Creates a <see cref="MemberAssignment" /> binding the specified value to the given property.
        /// </summary>
        /// <param name="propertyAccessor">The <see cref="PropertyInfo" /> for the property which is being assigned to.</param>
        /// <param name="expression">The value to be assigned to <paramref name="propertyAccessor" />.</param>
        /// <returns>The created <see cref="MemberAssignment" />.</returns>
        public static MemberAssignment Bind(MethodInfo propertyAccessor, Expression expression)
        {
            ContractUtils.RequiresNotNull(propertyAccessor, nameof(propertyAccessor));
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ValidateMethodInfo(propertyAccessor, nameof(propertyAccessor));
            return Bind(GetProperty(propertyAccessor, nameof(propertyAccessor)), expression);
        }

        private static void ValidateSettableFieldOrPropertyMember(MemberInfo member, out Type memberType)
        {
            var decType = member.DeclaringType;
            if (decType == null)
            {
                throw new ArgumentException($"'{member}' is not a member of any type", nameof(member));
            }

            // Null paramName as there are two paths here with different parameter names at the API
            TypeUtils.ValidateType(decType, nameof(member));
            switch (member)
            {
                case PropertyInfo pi:
                    if (!pi.CanWrite)
                    {
                        throw new ArgumentException($"The property '{pi}' has no 'set' accessor", nameof(member));
                    }

                    memberType = pi.PropertyType;
                    break;

                case FieldInfo fi:
                    memberType = fi.FieldType;
                    break;

                default:
                    throw new ArgumentException("Argument must be either a FieldInfo or PropertyInfo", nameof(member));
            }
        }
    }

    /// <summary>
    ///     Represents assignment to a member of an object.
    /// </summary>
    public sealed class MemberAssignment : MemberBinding
    {
        internal MemberAssignment(MemberInfo member, Expression expression)
#pragma warning disable CS0618 // Type or member is obsolete
            : base(MemberBindingType.Assignment, member)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            Expression = expression;
        }

        /// <summary>
        ///     Gets the <see cref="Expression" /> which represents the object whose member is being assigned to.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        ///     Creates a new expression that is like this one, but using the
        ///     supplied children. If all of the children are the same, it will
        ///     return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public MemberAssignment Update(Expression expression)
        {
            return expression == Expression ? this : Expression.Bind(Member, expression);
        }

        internal override void ValidateAsDefinedHere(int index)
        {
            // Empty
        }
    }
}

#endif