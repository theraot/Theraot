﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Reflection;
using Theraot.Collections;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>
        ///     Creates a <see cref="MemberMemberBinding" /> that represents the recursive initialization of members of a field or
        ///     property.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo" /> to set the <see cref="MemberBinding.Member" /> property equal to.</param>
        /// <param name="bindings">
        ///     An array of <see cref="MemberBinding" /> objects to use to populate the
        ///     <see cref="MemberMemberBinding.Bindings" /> collection.
        /// </param>
        /// <returns>
        ///     A <see cref="MemberMemberBinding" /> that has the <see cref="MemberBinding.BindingType" /> property equal to
        ///     <see cref="MemberBinding" /> and the <see cref="MemberBinding.Member" /> and
        ///     <see cref="MemberMemberBinding.Bindings" /> properties set to the specified values.
        /// </returns>
        public static MemberMemberBinding MemberBind(MemberInfo member, params MemberBinding[] bindings)
        {
            return MemberBind(member, (IEnumerable<MemberBinding>)bindings);
        }

        /// <summary>
        ///     Creates a <see cref="MemberMemberBinding" /> that represents the recursive initialization of members of a field or
        ///     property.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo" /> to set the <see cref="MemberBinding.Member" /> property equal to.</param>
        /// <param name="bindings">
        ///     An <see cref="IEnumerable{T}" /> that contains <see cref="MemberBinding" /> objects to use to
        ///     populate the <see cref="MemberMemberBinding.Bindings" /> collection.
        /// </param>
        /// <returns>
        ///     A <see cref="MemberMemberBinding" /> that has the <see cref="MemberBinding.BindingType" /> property equal to
        ///     <see cref="MemberBinding" /> and the <see cref="MemberBinding.Member" /> and
        ///     <see cref="MemberMemberBinding.Bindings" /> properties set to the specified values.
        /// </returns>
        public static MemberMemberBinding MemberBind(MemberInfo member, IEnumerable<MemberBinding>? bindings)
        {
            ContractUtils.RequiresNotNull(member, nameof(member));
            ContractUtils.RequiresNotNull(bindings, nameof(bindings));
            return MemberBindExtracted(member, bindings);
        }

        /// <summary>
        ///     Creates a <see cref="MemberMemberBinding" /> that represents the recursive initialization of members of a member
        ///     that is accessed by using a property accessor method.
        /// </summary>
        /// <param name="propertyAccessor">The <see cref="MemberInfo" /> that represents a property accessor method.</param>
        /// <param name="bindings">
        ///     An <see cref="IEnumerable{T}" /> that contains <see cref="MemberBinding" /> objects to use to
        ///     populate the <see cref="MemberMemberBinding.Bindings" /> collection.
        /// </param>
        /// <returns>
        ///     A <see cref="MemberMemberBinding" /> that has the <see cref="MemberBinding.BindingType" /> property equal to
        ///     <see cref="MemberBinding" />,
        ///     the Member property set to the <see cref="PropertyInfo" /> that represents the property accessed in
        ///     <paramref name="propertyAccessor" />,
        ///     and <see cref="MemberMemberBinding.Bindings" /> properties set to the specified values.
        /// </returns>
        public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, params MemberBinding[] bindings)
        {
            return MemberBind(propertyAccessor, (IEnumerable<MemberBinding>)bindings);
        }

        /// <summary>
        ///     Creates a <see cref="MemberMemberBinding" /> that represents the recursive initialization of members of a member
        ///     that is accessed by using a property accessor method.
        /// </summary>
        /// <param name="propertyAccessor">The <see cref="MemberInfo" /> that represents a property accessor method.</param>
        /// <param name="bindings">
        ///     An <see cref="IEnumerable{T}" /> that contains <see cref="MemberBinding" /> objects to use to
        ///     populate the <see cref="MemberMemberBinding.Bindings" /> collection.
        /// </param>
        /// <returns>
        ///     A <see cref="MemberMemberBinding" /> that has the <see cref="MemberBinding.BindingType" /> property equal to
        ///     <see cref="MemberBinding" />,
        ///     the Member property set to the <see cref="PropertyInfo" /> that represents the property accessed in
        ///     <paramref name="propertyAccessor" />,
        ///     and <see cref="MemberMemberBinding.Bindings" /> properties set to the specified values.
        /// </returns>
        public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, IEnumerable<MemberBinding> bindings)
        {
            ContractUtils.RequiresNotNull(propertyAccessor, nameof(propertyAccessor));
            return MemberBind(GetProperty(propertyAccessor, nameof(propertyAccessor)), bindings);
        }

        private static MemberMemberBinding MemberBindExtracted(MemberInfo member, IEnumerable<MemberBinding> bindings)
        {
            var bindingsArray = bindings.AsArrayInternal();
            ValidateGettableFieldOrPropertyMember(member, out var memberType);
            ValidateMemberInitArgs(memberType, bindingsArray);
            return new MemberMemberBinding(member, bindingsArray);
        }

        private static void ValidateGettableFieldOrPropertyMember(MemberInfo member, out Type memberType)
        {
            var decType = member.DeclaringType;
            if (decType == null)
            {
                throw new ArgumentException($"'{member}' is not a member of any type", nameof(member));
            }

            // Null paramName as there are several paths here with different parameter names at the API
            TypeUtils.ValidateType(decType, paramName: null, allowByRef: true, allowPointer: true);
            switch (member)
            {
                case PropertyInfo pi:
                    if (!pi.CanRead)
                    {
                        throw new ArgumentException($"The property '{pi}' has no 'get' accessor", nameof(member));
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

        private static void ValidateMemberInitArgs(Type type, MemberBinding[] bindings)
        {
            for (int i = 0, n = bindings.Length; i < n; i++)
            {
                var b = bindings[i];
                ContractUtils.RequiresNotNull(b, nameof(bindings));
                b.ValidateAsDefinedHere(i);
                if (b.Member.DeclaringType?.IsAssignableFrom(type) != true)
                {
                    throw new ArgumentException($"{b.Member.Name}' is not a member of type '{type}'", i >= 0 ? $"{nameof(bindings)}[{i}]" : nameof(bindings));
                }
            }
        }
    }

    /// <summary>
    ///     Represents initializing members of a member of a newly created object.
    /// </summary>
    /// <remarks>
    ///     The value of the <see cref="MemberBinding.BindingType" /> property of a
    ///     <see cref="MemberMemberBinding" /> object is
    ///     <see cref="MemberBinding" />.
    /// </remarks>
    public sealed class MemberMemberBinding : MemberBinding
    {
        private readonly MemberBinding[] _bindings;
        private readonly ReadOnlyCollectionEx<MemberBinding> _bindingsAsReadOnlyCollection;

        internal MemberMemberBinding(MemberInfo member, MemberBinding[] bindings)
#pragma warning disable CS0618 // Type or member is obsolete
            : base(MemberBindingType.MemberBinding, member)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            _bindings = bindings;
            _bindingsAsReadOnlyCollection = ReadOnlyCollectionEx.Create(_bindings);
        }

        /// <summary>
        ///     Gets the bindings that describe how to initialize the members of a member.
        /// </summary>
        public ReadOnlyCollection<MemberBinding> Bindings => _bindingsAsReadOnlyCollection;

        /// <summary>
        ///     Creates a new expression that is like this one, but using the
        ///     supplied children. If all of the children are the same, it will
        ///     return this expression.
        /// </summary>
        /// <param name="bindings">The <see cref="Bindings" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public MemberMemberBinding Update(IEnumerable<MemberBinding>? bindings)
        {
            if (bindings != null && ExpressionUtils.SameElements(ref bindings, _bindings))
            {
                return this;
            }

            return Expression.MemberBind(Member, bindings);
        }

        internal override void ValidateAsDefinedHere(int index)
        {
            // Empty
        }
    }
}

#endif