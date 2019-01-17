#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        #region Field

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a field.
        /// </summary>
        /// <param name="expression">The containing object of the field.  This can be null for static fields.</param>
        /// <param name="field">The field to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Field(Expression expression, FieldInfo field)
        {
            ContractUtils.RequiresNotNull(field, nameof(field));

            if (field.IsStatic)
            {
                if (expression != null)
                {
                    throw new ArgumentException("Static field requires null instance, non-static field requires non-null instance.", nameof(expression));
                }
            }
            else
            {
                if (expression == null)
                {
                    throw new ArgumentException("Static field requires null instance, non-static field requires non-null instance.", nameof(field));
                }

                ExpressionUtils.RequiresCanRead(expression, nameof(expression));
                if (!field.DeclaringType.IsReferenceAssignableFromInternal(expression.Type))
                {
                    throw new ArgumentException($"Field '{field.DeclaringType}.{field.Name}' is not defined for type '{expression.Type}'");
                }
            }
            return MemberExpression.Make(expression, field);
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a field.
        /// </summary>
        /// <param name="expression">The containing object of the field.  This can be null for static fields.</param>
        /// <param name="fieldName">The field to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Field(Expression expression, string fieldName)
        {
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(fieldName, nameof(fieldName));

            // bind to public names first
            var fi = expression.Type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy)
                           ?? expression.Type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (fi == null)
            {
                throw new ArgumentException($"Instance field '{fieldName}' is not defined for type '{expression.Type}'");
            }
            return Field(expression, fi);
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a field.
        /// </summary>
        /// <param name="expression">The containing object of the field.  This can be null for static fields.</param>
        /// <param name="type">The <see cref="Type"/> containing the field.</param>
        /// <param name="fieldName">The field to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Field(Expression expression, Type type, string fieldName)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));

            // bind to public names first
            var fi = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy)
                           ?? type.GetField(fieldName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);

            if (fi == null)
            {
                throw new ArgumentException($"Field '{fieldName}' is not defined for type '{type}'");
            }
            return Field(expression, fi);
        }

        #endregion Field

        #region Property

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property.
        /// </summary>
        /// <param name="expression">The containing object of the property.  This can be null for static properties.</param>
        /// <param name="propertyName">The property to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Property(Expression expression, string propertyName)
        {
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(propertyName, nameof(propertyName));
            // bind to public names first
            var pi = expression.Type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy)
                              ?? expression.Type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (pi == null)
            {
                throw new ArgumentException($"Instance property '{propertyName}' is not defined for type '{expression.Type}'", nameof(propertyName));
            }
            return Property(expression, pi);
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property.
        /// </summary>
        /// <param name="expression">The containing object of the property.  This can be null for static properties.</param>
        /// <param name="type">The <see cref="Type"/> containing the property.</param>
        /// <param name="propertyName">The property to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Property(Expression expression, Type type, string propertyName)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));
            ContractUtils.RequiresNotNull(propertyName, nameof(propertyName));
            // bind to public names first
            var pi = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy)
                              ?? type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (pi == null)
            {
                throw new ArgumentException($"Property '{propertyName}' is not defined for type '{type}'", nameof(propertyName));
            }
            return Property(expression, pi);
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property.
        /// </summary>
        /// <param name="expression">The containing object of the property.  This can be null for static properties.</param>
        /// <param name="property">The property to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Property(Expression expression, PropertyInfo property)
        {
            ContractUtils.RequiresNotNull(property, nameof(property));

            var mi = property.GetGetMethod(nonPublic: true);

            if (mi == null)
            {
                mi = property.GetSetMethod(nonPublic: true);

                if (mi == null)
                {
                    throw new ArgumentException($"The property '{property}' has no 'get' or 'set' accessors", nameof(property));
                }

                if (mi.GetParameters().Length != 1)
                {
                    throw new ArgumentException($"Incorrect number of arguments supplied for call to method '{mi}'", nameof(property));
                }
            }
            else if (mi.GetParameters().Length != 0)
            {
                throw new ArgumentException($"Incorrect number of arguments supplied for call to method '{mi}'", nameof(property));
            }

            if (mi.IsStatic)
            {
                if (expression != null)
                {
                    throw new ArgumentException("Static property requires null instance, non-static property requires non-null instance.", nameof(expression));
                }
            }
            else
            {
                if (expression == null)
                {
                    throw new ArgumentException("Static property requires null instance, non-static property requires non-null instance.", nameof(property));
                }

                ExpressionUtils.RequiresCanRead(expression, nameof(expression));
                if (!TypeUtils.IsValidInstanceType(property, expression.Type))
                {
                    throw new ArgumentException($"Property '{property}' is not defined for type '{expression.Type}'", nameof(property));
                }
            }

            ValidateMethodInfo(mi, nameof(property));

            return MemberExpression.Make(expression, property);
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property.
        /// </summary>
        /// <param name="expression">The containing object of the property.  This can be null for static properties.</param>
        /// <param name="propertyAccessor">An accessor method of the property to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression Property(Expression expression, MethodInfo propertyAccessor)
        {
            ContractUtils.RequiresNotNull(propertyAccessor, nameof(propertyAccessor));
            ValidateMethodInfo(propertyAccessor, nameof(propertyAccessor));
            return Property(expression, GetProperty(propertyAccessor, nameof(propertyAccessor)));
        }

        private static bool CheckMethod(MethodInfo method, MethodInfo propertyMethod)
        {
            if (method.Equals(propertyMethod))
            {
                return true;
            }
            // If the type is an interface then the handle for the method got by the compiler will not be the
            // same as that returned by reflection.
            // Check for this condition and try and get the method from reflection.
            var type = method.DeclaringType;
            return type.IsInterface && method.Name == propertyMethod.Name && type.GetMethod(method.Name) == propertyMethod;
        }

        private static PropertyInfo GetProperty(MethodInfo mi, string paramName, int index = -1)
        {
            var type = mi.DeclaringType;
            if (type != null)
            {
                var flags = BindingFlags.Public | BindingFlags.NonPublic;
                flags |= mi.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
                var props = type.GetProperties(flags);
                foreach (var pi in props)
                {
                    if (pi.CanRead && CheckMethod(mi, pi.GetGetMethod(nonPublic: true)))
                    {
                        return pi;
                    }
                    if (pi.CanWrite && CheckMethod(mi, pi.GetSetMethod(nonPublic: true)))
                    {
                        return pi;
                    }
                }
            }

            throw new ArgumentException($"The method '{mi.DeclaringType}.{mi.Name}' is not a property accessor", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        #endregion Property

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property or field.
        /// </summary>
        /// <param name="expression">The containing object of the member.  This can be null for static members.</param>
        /// <param name="member">The member to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression MakeMemberAccess(Expression expression, MemberInfo member)
        {
            ContractUtils.RequiresNotNull(member, nameof(member));
            if (member is FieldInfo fi)
            {
                return Field(expression, fi);
            }
            if (member is PropertyInfo pi)
            {
                return Property(expression, pi);
            }
            throw new ArgumentException($"Member '{member}' not field or property", nameof(member));
        }

        /// <summary>
        /// Creates a <see cref="MemberExpression"/> accessing a property or field.
        /// </summary>
        /// <param name="expression">The containing object of the member.  This can be null for static members.</param>
        /// <param name="propertyOrFieldName">The member to be accessed.</param>
        /// <returns>The created <see cref="MemberExpression"/>.</returns>
        public static MemberExpression PropertyOrField(Expression expression, string propertyOrFieldName)
        {
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            // bind to public names first
            var pi = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (pi != null)
            {
                return Property(expression, pi);
            }

            var fi = expression.Type.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (fi != null)
            {
                return Field(expression, fi);
            }

            pi = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (pi != null)
            {
                return Property(expression, pi);
            }

            fi = expression.Type.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy);
            if (fi != null)
            {
                return Field(expression, fi);
            }
            throw new ArgumentException($"{propertyOrFieldName}' is not a member of type '{expression.Type}'", nameof(propertyOrFieldName));
        }
    }

    /// <summary>
    /// Represents accessing a field or property.
    /// </summary>
    [DebuggerTypeProxy(typeof(MemberExpressionProxy))]
    public class MemberExpression : Expression
    {
        // param order: factories args in order, then other args
        internal MemberExpression(Expression expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// Gets the containing object of the field or property.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the field or property to be accessed.
        /// </summary>
        public MemberInfo Member => GetMember();

        /// <summary>
        /// Returns the node type of this <see cref="Expression"/>. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        public sealed override ExpressionType NodeType => ExpressionType.MemberAccess;

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public MemberExpression Update(Expression expression)
        {
            if (expression == Expression)
            {
                return this;
            }
            return MakeMemberAccess(expression, Member);
        }

        internal static PropertyExpression Make(Expression expression, PropertyInfo property)
        {
            Debug.Assert(property != null);
            return new PropertyExpression(expression, property);
        }

        internal static FieldExpression Make(Expression expression, FieldInfo field)
        {
            Debug.Assert(field != null);
            return new FieldExpression(expression, field);
        }

        internal static MemberExpression Make(Expression expression, MemberInfo member)
        {
            return !(member is FieldInfo fi) ? (MemberExpression)Make(expression, (PropertyInfo)member) : Make(expression, fi);
        }

        internal virtual MemberInfo GetMember()
        {
            throw ContractUtils.Unreachable;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitMember(this);
        }
    }

    internal sealed class FieldExpression : MemberExpression
    {
        private readonly FieldInfo _field;

        public FieldExpression(Expression expression, FieldInfo member)
            : base(expression)
        {
            _field = member;
        }

        public override Type Type => _field.FieldType;

        internal override MemberInfo GetMember() => _field;
    }

    internal sealed class PropertyExpression : MemberExpression
    {
        private readonly PropertyInfo _property;

        public PropertyExpression(Expression expression, PropertyInfo member)
            : base(expression)
        {
            _property = member;
        }

        public override Type Type => _property.PropertyType;

        internal override MemberInfo GetMember() => _property;
    }
}

#endif