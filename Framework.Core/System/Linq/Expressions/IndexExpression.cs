#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Text;
using Theraot.Collections;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>
        ///     Creates an <see cref="IndexExpression" /> to access an array.
        /// </summary>
        /// <param name="array">An expression representing the array to index.</param>
        /// <param name="indexes">An array containing expressions used to index the array.</param>
        /// <remarks>
        ///     The expression representing the array can be obtained by using the <see cref="MakeMemberAccess" /> method,
        ///     or through <see cref="NewArrayBounds(System.Type,System.Linq.Expressions.Expression[])" /> or
        ///     <see cref="NewArrayInit(System.Type,System.Linq.Expressions.Expression[])" />.
        /// </remarks>
        /// <returns>The created <see cref="IndexExpression" />.</returns>
        public static IndexExpression ArrayAccess(Expression array, params Expression[] indexes)
        {
            return ArrayAccess(array, (IEnumerable<Expression>)indexes);
        }

        /// <summary>
        ///     Creates an <see cref="IndexExpression" /> to access an array.
        /// </summary>
        /// <param name="array">An expression representing the array to index.</param>
        /// <param name="indexes">An <see cref="IEnumerable{T}" /> containing expressions used to index the array.</param>
        /// <remarks>
        ///     The expression representing the array can be obtained by using the <see cref="MakeMemberAccess" /> method,
        ///     or through <see cref="NewArrayBounds(System.Type,System.Linq.Expressions.Expression[])" /> or
        ///     <see cref="NewArrayInit(System.Type,System.Linq.Expressions.Expression[])" />.
        /// </remarks>
        /// <returns>The created <see cref="IndexExpression" />.</returns>
        public static IndexExpression ArrayAccess(Expression array, IEnumerable<Expression>? indexes)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            ExpressionUtils.RequiresCanRead(array, nameof(array));

            var arrayType = array.Type;
            if (!arrayType.IsArray)
            {
                throw new ArgumentException("Argument must be array", nameof(array));
            }

            if (indexes != null)
            {
                return ArrayAccessExtracted(array, indexes, arrayType);
            }

            if (arrayType.GetArrayRank() != 0)
            {
                throw new ArgumentException("Incorrect number of indexes", nameof(array));
            }

            return new IndexExpression(array, indexer: null, ArrayEx.Empty<Expression>());
        }

        /// <summary>
        ///     Creates an <see cref="IndexExpression" /> that represents accessing an indexed property in an object.
        /// </summary>
        /// <param name="instance">The object to which the property belongs. Should be null if the property is static(shared).</param>
        /// <param name="indexer">An <see cref="Expression" /> representing the property to index.</param>
        /// <param name="arguments">
        ///     An <see cref="IEnumerable{Expression}" /> containing the arguments to be used to index the
        ///     property.
        /// </param>
        /// <returns>The created <see cref="IndexExpression" />.</returns>
        public static IndexExpression MakeIndex(Expression? instance, PropertyInfo? indexer, IEnumerable<Expression>? arguments)
        {
            if (indexer != null)
            {
                return Property(instance, indexer, arguments);
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return ArrayAccess(instance, arguments);
        }

        /// <summary>
        ///     Creates an <see cref="IndexExpression" /> representing the access to an indexed property.
        /// </summary>
        /// <param name="instance">The object to which the property belongs. If the property is static/shared, it must be null.</param>
        /// <param name="propertyName">The name of the indexer.</param>
        /// <param name="arguments">An array of <see cref="Expression" /> objects that are used to index the property.</param>
        /// <returns>The created <see cref="IndexExpression" />.</returns>
        public static IndexExpression Property(Expression instance, string propertyName, params Expression[] arguments)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            ContractUtils.RequiresNotNull(propertyName, nameof(propertyName));
            ExpressionUtils.RequiresCanRead(instance, nameof(instance));
            var pi = FindInstanceProperty(instance.Type, propertyName, arguments);
            return MakeIndexProperty(instance, pi, nameof(propertyName), arguments);
        }

        /// <summary>
        ///     Creates an <see cref="IndexExpression" /> representing the access to an indexed property.
        /// </summary>
        /// <param name="instance">The object to which the property belongs. If the property is static/shared, it must be null.</param>
        /// <param name="indexer">The <see cref="PropertyInfo" /> that represents the property to index.</param>
        /// <param name="arguments">An array of <see cref="Expression" /> objects that are used to index the property.</param>
        /// <returns>The created <see cref="IndexExpression" />.</returns>
        public static IndexExpression Property(Expression? instance, PropertyInfo indexer, params Expression[] arguments)
        {
            return Property(instance, indexer, (IEnumerable<Expression>)arguments);
        }

        /// <summary>
        ///     Creates an <see cref="IndexExpression" /> representing the access to an indexed property.
        /// </summary>
        /// <param name="instance">The object to which the property belongs. If the property is static/shared, it must be null.</param>
        /// <param name="indexer">The <see cref="PropertyInfo" /> that represents the property to index.</param>
        /// <param name="arguments">
        ///     An <see cref="IEnumerable{T}" /> of <see cref="Expression" /> objects that are used to index
        ///     the property.
        /// </param>
        /// <returns>The created <see cref="IndexExpression" />.</returns>
        public static IndexExpression Property(Expression? instance, PropertyInfo indexer, IEnumerable<Expression>? arguments)
        {
            return MakeIndexProperty(instance, indexer, nameof(indexer), arguments.AsArrayInternal());
        }

        private static IndexExpression ArrayAccessExtracted(Expression array, IEnumerable<Expression> indexes, Type arrayType)
        {
            var indexList = indexes.AsArrayInternal();

            if (arrayType.GetArrayRank() != indexList.Length)
            {
                throw new ArgumentException("Incorrect number of indexes", string.Empty);
            }

            foreach (var e in indexList)
            {
                ContractUtils.RequiresNotNull(e, nameof(indexes));
                ExpressionUtils.RequiresCanRead(e, nameof(indexes));
                if (e.Type != typeof(int))
                {
                    throw new ArgumentException("Argument for array index must be of type Int32", nameof(indexes));
                }
            }

            return new IndexExpression(array, indexer: null, indexList);
        }

        private static PropertyInfo FindInstanceProperty(Type type, string propertyName, Expression[] arguments)
        {
            // bind to public names first
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy;
            var pi = FindProperty(type, propertyName, arguments, flags);
            if (pi == null)
            {
                flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy;
                pi = FindProperty(type, propertyName, arguments, flags);
            }

            if (pi != null)
            {
                return pi;
            }

            if (arguments == null || arguments.Length == 0)
            {
                throw new ArgumentException($"Instance property '{propertyName}' that takes no argument is not defined for type '{type}'", string.Empty);
            }

            throw new ArgumentException($"Instance property '{propertyName}{GetArgTypesString(arguments)}' is not defined for type '{type}'", nameof(propertyName));
        }

        private static PropertyInfo? FindProperty(Type type, string propertyName, Expression[] arguments, BindingFlags flags)
        {
            PropertyInfo? property = null;

            foreach (var pi in type.GetProperties(flags))
            {
                if (!pi.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase) || !IsCompatible(pi, arguments))
                {
                    continue;
                }

                if (property == null)
                {
                    property = pi;
                }
                else
                {
                    throw new InvalidOperationException($"More than one property '{propertyName}' on type '{type}' is compatible with the supplied arguments.");
                }
            }

            return property;
        }

        private static string GetArgTypesString(Expression[] arguments)
        {
            var argTypesStr = new StringBuilder();
            argTypesStr.Append('(');
            for (var i = 0; i < arguments.Length; i++)
            {
                if (i != 0)
                {
                    argTypesStr.Append(", ");
                }

                argTypesStr.Append(arguments[i]?.Type.Name);
            }

            argTypesStr.Append(')');
            return argTypesStr.ToString();
        }

        private static bool IsCompatible(PropertyInfo pi, Expression[] args)
        {
            var mi = pi.GetGetMethod(nonPublic: true);
            ParameterInfo[] parameters;
            if (mi != null)
            {
                parameters = mi.GetParameters();
            }
            else
            {
                mi = pi.GetSetMethod(nonPublic: true);
                if (mi == null)
                {
                    return false;
                }

                //The setter has an additional parameter for the value to set,
                //need to remove the last type to match the arguments.
                parameters = mi.GetParameters();
                if (parameters.Length == 0)
                {
                    return false;
                }

                parameters = parameters.RemoveLast();
            }

            if (args == null)
            {
                return parameters.Length == 0;
            }

            if (parameters.Length != args.Length)
            {
                return false;
            }

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    return false;
                }

                if (!parameters[i].ParameterType.IsReferenceAssignableFromInternal(args[i].Type))
                {
                    return false;
                }
            }

            return true;
        }

        private static IndexExpression MakeIndexProperty(Expression? instance, PropertyInfo indexer, string paramName, Expression[] argList)
        {
            ValidateIndexedProperty(instance, indexer, paramName, ref argList);
            return new IndexExpression(instance, indexer, argList);
        }

        private static void ValidateAccessor(Expression? instance, MethodInfo method, ParameterInfo[] indexes, ref Expression[] arguments, string paramName)
        {
            ContractUtils.RequiresNotNull(arguments, nameof(arguments));

            ValidateMethodInfo(method, nameof(method));
            if ((method.CallingConvention & CallingConventions.VarArgs) != 0)
            {
                throw new ArgumentException("Accessor method should not have VarArgs.", paramName);
            }

            if (method.IsStatic)
            {
                if (instance != null)
                {
                    throw new ArgumentException("Static property requires null instance, non-static property requires non-null instance.", nameof(instance));
                }
            }
            else
            {
                if (instance == null)
                {
                    throw new ArgumentException("Static property requires null instance, non-static property requires non-null instance.", nameof(instance));
                }

                ExpressionUtils.RequiresCanRead(instance, nameof(instance));
                ValidateCallInstanceType(instance.Type, method);
            }

            ValidateAccessorArgumentTypes(method, indexes, ref arguments, paramName);
        }

        private static void ValidateAccessorArgumentTypes(MethodInfo method, ParameterInfo[] indexes, ref Expression[] arguments, string paramName)
        {
            if (indexes.Length > 0)
            {
                if (indexes.Length != arguments.Length)
                {
                    throw new ArgumentException($"Incorrect number of arguments supplied for call to method '{method}'", paramName);
                }

                Expression[]? newArgs = null;
                for (int i = 0, n = indexes.Length; i < n; i++)
                {
                    var arg = arguments[i];
                    var pi = indexes[i];
                    ContractUtils.RequiresNotNull(arg, nameof(arguments), i);
                    ExpressionUtils.RequiresCanRead(arg, nameof(arguments), i);

                    var pType = pi.ParameterType;
                    if (pType.IsByRef)
                    {
                        throw new ArgumentException("Accessor indexes cannot be passed ByRef.", i >= 0 ? $"{nameof(indexes)}[{i}]" : nameof(indexes));
                    }

                    TypeUtils.ValidateType(pType, nameof(indexes), i);

                    if (!pType.IsReferenceAssignableFromInternal(arg.Type) && !TryQuote(pType, ref arg))
                    {
                        throw new ArgumentException($"Expression of type '{arg.Type}' cannot be used for parameter of type '{pType}' of method '{method}'", i >= 0 ? $"{nameof(arguments)}[{i}]" : nameof(arguments));
                    }

                    if (newArgs == null && arg != arguments[i])
                    {
                        newArgs = new Expression[arguments.Length];
                        for (var j = 0; j < i; j++)
                        {
                            newArgs[j] = arguments[j];
                        }
                    }

                    if (newArgs != null)
                    {
                        newArgs[i] = arg;
                    }
                }

                if (newArgs != null)
                {
                    arguments = newArgs;
                }
            }
            else if (arguments.Length > 0)
            {
                throw new ArgumentException($"Incorrect number of arguments supplied for call to method '{method}'", paramName);
            }
        }

        private static (ParameterInfo[] parameters, MethodInfo methodInfo)? ValidateIndexedGetter(Expression? instance, PropertyInfo indexer, string paramName, ref Expression[] argList)
        {
            var getter = indexer.GetGetMethod(nonPublic: true);
            if (getter == null)
            {
                return null;
            }

            if (getter.ReturnType != indexer.PropertyType)
            {
                throw new ArgumentException("Property type must match the value type of getter", paramName);
            }

            var getParameters = getter.GetParameters();
            ValidateAccessor(instance, getter, getParameters, ref argList, paramName);
            return (getParameters, getter);
        }

        // CTS places no restrictions on properties (see ECMA-335 8.11.3),
        // so we validate that the property conforms to CLS rules here.
        //
        // Does reflection help us out at all? Expression.Property skips all of
        // these checks, so either it needs more checks or we need less here.
        private static void ValidateIndexedProperty(Expression? instance, PropertyInfo indexer, string paramName, ref Expression[] argList)
        {
            // If both getter and setter specified, all their parameter types
            // should match, with exception of the last setter parameter which
            // should match the type returned by the get method.
            // Accessor parameters cannot be ByRef.

            ContractUtils.RequiresNotNull(indexer, paramName);
            if (indexer.PropertyType.IsByRef)
            {
                throw new ArgumentException("Property cannot have a managed pointer type.", paramName);
            }

            if (indexer.PropertyType == typeof(void))
            {
                throw new ArgumentException("Property cannot have a void type.", paramName);
            }

            var getter = ValidateIndexedGetter(instance, indexer, paramName, ref argList);

            var setter = indexer.GetSetMethod(nonPublic: true);
            if (setter != null)
            {
                var setParameters = setter.GetParameters();
                if (setParameters.Length == 0)
                {
                    throw new ArgumentException("Setter must have parameters.", paramName);
                }

                // valueType is the type of the value passed to the setter (last parameter)
                var valueType = setParameters[^1].ParameterType;
                if (valueType.IsByRef)
                {
                    throw new ArgumentException("Property cannot have a managed pointer type.", paramName);
                }

                if (setter.ReturnType != typeof(void))
                {
                    throw new ArgumentException("Setter should have void type.", paramName);
                }

                if (indexer.PropertyType != valueType)
                {
                    throw new ArgumentException("Property type must match the value type of setter", paramName);
                }

                if (getter != null)
                {
                    var (parameters, methodInfo) = getter.Value;
                    if (methodInfo.IsStatic ^ setter.IsStatic)
                    {
                        throw new ArgumentException("Both accessors must be static.", paramName);
                    }

                    if (parameters.Length != setParameters.Length - 1)
                    {
                        throw new ArgumentException("Indexing parameters of getter and setter must match.", paramName);
                    }

                    if (parameters.Where((t, i) => t.ParameterType != setParameters[i].ParameterType).Any())
                    {
                        throw new ArgumentException("Indexing parameters of getter and setter must match.", paramName);
                    }
                }
                else
                {
                    ValidateAccessor(instance, setter, setParameters.RemoveLast(), ref argList, paramName);
                }
            }
            else if (getter == null)
            {
                throw new ArgumentException($"The property '{indexer}' has no 'get' or 'set' accessors", paramName);
            }
        }
    }

    /// <summary>
    ///     Represents indexing a property or array.
    /// </summary>
    [DebuggerTypeProxy(typeof(IndexExpressionProxy))]
    public sealed class IndexExpression : Expression, IArgumentProvider
    {
        private readonly Expression[] _arguments;
        private readonly ReadOnlyCollectionEx<Expression> _argumentsAsReadOnlyCollection;

        internal IndexExpression(
            Expression? instance,
            PropertyInfo? indexer,
            Expression[] arguments)
        {
            if (indexer == null)
            {
                Debug.Assert(instance!.Type.IsArray && instance.Type.GetArrayRank() == arguments.Length);
            }

            Object = instance;
            Indexer = indexer;
            _arguments = arguments;
            _argumentsAsReadOnlyCollection = ReadOnlyCollectionEx.Create(_arguments);
        }

        /// <summary>
        ///     Gets the number of argument expressions of the node.
        /// </summary>
        public int ArgumentCount => _arguments.Length;

        /// <summary>
        ///     Gets the arguments to be used to index the property or array.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments => _argumentsAsReadOnlyCollection;

        /// <summary>
        ///     Gets the <see cref="PropertyInfo" /> for the property if the expression represents an indexed property, returns
        ///     null otherwise.
        /// </summary>
        /// <remarks>Null if array access.</remarks>
        public PropertyInfo? Indexer { get; }

        /// <summary>
        ///     Returns the node type of this <see cref="Expression" />. (Inherited from
        ///     <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType" /> that represents this expression.</returns>
        public override ExpressionType NodeType => ExpressionType.Index;

        /// <summary>
        ///     An object to index.
        /// </summary>
        /// <remarks>Not null if <see cref="Indexer"/> is null (array access). Null if <see cref="Indexer"/> is static.</remarks>
        public Expression? Object { get; }

        /// <summary>
        ///     Gets the static type of the expression that this <see cref="Expression" /> represents.
        ///     (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="System.Type" /> that represents the static type of the expression.</returns>
        public override Type Type => Indexer?.PropertyType ?? Object!.Type.GetElementType();

        /// <summary>
        ///     Gets the argument expression with the specified <paramref name="index" />.
        /// </summary>
        /// <param name="index">The index of the argument expression to get.</param>
        /// <returns>The expression representing the argument at the specified <paramref name="index" />.</returns>
        public Expression GetArgument(int index)
        {
            var argument = _arguments[index];
            return
                argument ??
                (
                    Indexer != null
                        ? Constant(value: null, Indexer.GetIndexParameters()[index].ParameterType)
                        : Constant(value: null)
                );
        }

        /// <summary>
        ///     Creates a new expression that is like this one, but using the
        ///     supplied children. If all of the children are the same, it will
        ///     return this expression.
        /// </summary>
        /// <param name="object">The <see cref="Object" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public IndexExpression Update(Expression @object, IEnumerable<Expression>? arguments)
        {
            if (@object == Object && ExpressionUtils.SameElements(ref arguments, _arguments))
            {
                return this;
            }

            return MakeIndex(@object, Indexer, arguments);
        }

        internal Expression Rewrite(Expression? instance, Expression[]? arguments)
        {
            Debug.Assert(arguments == null || arguments.Length == _arguments.Length);

            return MakeIndex(instance, Indexer, arguments ?? _arguments);
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return visitor.VisitIndex(this);
        }
    }
}

#endif