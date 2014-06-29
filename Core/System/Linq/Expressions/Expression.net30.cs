#if NET20 || NET30

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using Theraot.Collections;
using Theraot.Core;

namespace System.Linq.Expressions
{
    public abstract class Expression
    {
        internal const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        internal const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        internal const BindingFlags AllStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy;
        internal const BindingFlags NonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        internal const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        internal const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        private readonly ExpressionType _nodeType;
        private readonly Type _type;

        protected Expression(ExpressionType nodeType, Type type)
        {
            _nodeType = nodeType;
            _type = type;
        }

        public ExpressionType NodeType
        {
            get
            {
                return _nodeType;
            }
        }

        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public static BinaryExpression Add(Expression left, Expression right)
        {
            return Add(left, right, null);
        }

        public static BinaryExpression Add(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryCoreCheck("op_Addition", left, right, method);
            return MakeSimpleBinary(ExpressionType.Add, left, right, method);
        }

        public static BinaryExpression AddChecked(Expression left, Expression right)
        {
            return AddChecked(left, right, null);
        }

        public static BinaryExpression AddChecked(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryCoreCheck("op_Addition", left, right, method);

            // The check in BinaryCoreCheck allows a bit more than we do
            // (byte, sbyte).  Catch that here
            if (method == null)
            {
                if (left.Type == typeof(byte) || left.Type == typeof(sbyte))
                {
                    throw new InvalidOperationException(string.Format("AddChecked not defined for {0} and {1}", left.Type, right.Type));
                }
            }
            return MakeSimpleBinary(ExpressionType.AddChecked, left, right, method);
        }

        public static BinaryExpression And(Expression left, Expression right)
        {
            return And(left, right, null);
        }

        public static BinaryExpression And(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryBitwiseCoreCheck("op_BitwiseAnd", left, right, method);
            return MakeSimpleBinary(ExpressionType.And, left, right, method);
        }

        public static BinaryExpression AndAlso(Expression left, Expression right)
        {
            return AndAlso(left, right, null);
        }

        public static BinaryExpression AndAlso(Expression left, Expression right, MethodInfo method)
        {
            method = ConditionalBinaryCheck("op_BitwiseAnd", left, right, method);
            return MakeBoolBinary(ExpressionType.AndAlso, left, right, true, method);
        }

        public static BinaryExpression ArrayIndex(Expression array, Expression index)
        {
            CheckArray(array);
            if (index == null)
            {
                throw new ArgumentNullException("index");
            }
            else if (array.Type.GetArrayRank() != 1)
            {
                throw new ArgumentException("The array argument must be a single dimensional array");
            }
            else if (index.Type != typeof(int))
            {
                throw new ArgumentException("The index must be of type int");
            }
            else
            {
                return new BinaryExpression(ExpressionType.ArrayIndex, array.Type.GetElementType(), array, index);
            }
        }

        public static MethodCallExpression ArrayIndex(Expression array, params Expression[] indexes)
        {
            return ArrayIndex(array, indexes as IEnumerable<Expression>);
        }

        public static MethodCallExpression ArrayIndex(Expression array, IEnumerable<Expression> indexes)
        {
            CheckArray(array);
            if (indexes == null)
            {
                throw new ArgumentNullException("indexes");
            }
            else
            {
                var args = indexes.ToReadOnlyCollection();
                if (array.Type.GetArrayRank() != args.Count)
                {
                    throw new ArgumentException("The number of arguments doesn't match the rank of the array");
                }
                else
                {
                    foreach (var arg in args)
                    {
                        if (arg.Type != typeof(int))
                        {
                            throw new ArgumentException("The index must be of type int");
                        }
                    }
                    return Call(array, array.Type.GetMethod("Get", PublicInstance), args);
                }
            }
        }

        public static UnaryExpression ArrayLength(Expression array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            else if (!array.Type.IsArray)
            {
                throw new ArgumentException("The type of the expression must me Array");
            }
            else if (array.Type.GetArrayRank() != 1)
            {
                throw new ArgumentException("The array must be a single dimensional array");
            }
            else
            {
                return new UnaryExpression(ExpressionType.ArrayLength, array, typeof(int));
            }
        }

        public static MemberAssignment Bind(MemberInfo member, Expression expression)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            else if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else
            {
                Type type = null;
                var property = member as PropertyInfo;
                if
                (
                    property != null &&
                    property.GetSetMethod(true) != null
                )
                {
                    type = property.PropertyType;
                }
                else
                {
                    var field = member as FieldInfo;
                    if (field != null)
                    {
                        type = field.FieldType;
                    }
                }
                if (type == null)
                {
                    throw new ArgumentException("member");
                }
                else if (!expression.Type.IsAssignableTo(type))
                {
                    throw new ArgumentException("member");
                }
                else
                {
                    return new MemberAssignment(member, expression);
                }
            }
        }

        public static MemberAssignment Bind(MethodInfo propertyAccessor, Expression expression)
        {
            if (propertyAccessor == null)
            {
                throw new ArgumentNullException("propertyAccessor");
            }
            else if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else
            {
                CheckNonGenericMethod(propertyAccessor);
                var property = GetAssociatedProperty(propertyAccessor);
                if (property == null)
                {
                    throw new ArgumentException("propertyAccessor");
                }
                else
                {
                    if (property.GetSetMethod(true) == null)
                    {
                        throw new ArgumentException("setter");
                    }
                    else if (!expression.Type.IsAssignableTo(property.PropertyType))
                    {
                        throw new ArgumentException("member");
                    }
                    else
                    {
                        return new MemberAssignment(property, expression);
                    }
                }
            }
        }

        public static MethodCallExpression Call(Expression instance, MethodInfo method)
        {
            return Call(instance, method, null as IEnumerable<Expression>);
        }

        public static MethodCallExpression Call(MethodInfo method, params Expression[] arguments)
        {
            return Call(null, method, arguments as IEnumerable<Expression>);
        }

        public static MethodCallExpression Call(Expression instance, MethodInfo method, params Expression[] arguments)
        {
            return Call(instance, method, arguments as IEnumerable<Expression>);
        }

        public static MethodCallExpression Call(Expression instance, MethodInfo method, IEnumerable<Expression> arguments)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (method.IsStatic)
            {
                if (instance != null)
                {
                    throw new ArgumentException("instance");
                }
            }
            else
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }
                else
                {
                    if (!instance.Type.IsAssignableTo(method.DeclaringType))
                    {
                        throw new ArgumentException("Type is not assignable to the declaring type of the method");
                    }
                }
            }
            var _arguments = CheckMethodArguments(method, arguments);
            return new MethodCallExpression(instance, method, _arguments);
        }

        public static MethodCallExpression Call(Expression instance, string methodName, Type[] typeArguments, params Expression[] arguments)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else if (methodName == null)
            {
                throw new ArgumentNullException("methodName");
            }
            else
            {
                var method = TryGetMethod(instance.Type, methodName, AllInstance, CollectTypes(arguments), typeArguments);
                var args = CheckMethodArguments(method, arguments);
                return new MethodCallExpression(instance, method, args);
            }
        }

        public static MethodCallExpression Call(Type type, string methodName, Type[] typeArguments, params Expression[] arguments)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (methodName == null)
            {
                throw new ArgumentNullException("methodName");
            }
            else
            {
                var method = TryGetMethod(type, methodName, AllStatic, CollectTypes(arguments), typeArguments);
                var args = CheckMethodArguments(method, arguments);
                return new MethodCallExpression(method, args);
            }
        }

        public static BinaryExpression Coalesce(Expression left, Expression right)
        {
            return Coalesce(left, right, null);
        }

        public static BinaryExpression Coalesce(Expression left, Expression right, LambdaExpression conversion)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            else if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            else if (left.Type.CanBeNull())
            {
                throw new InvalidOperationException("Left expression can never be null");
            }
            else if (conversion != null)
            {
                return MakeConvertedCoalesce(left, right, conversion);
            }
            else
            {
                return MakeCoalesce(left, right);
            }
        }

        public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse)
        {
            if (test == null)
            {
                throw new ArgumentNullException("test");
            }
            else if (ifTrue == null)
            {
                throw new ArgumentNullException("ifTrue");
            }
            else if (ifFalse == null)
            {
                throw new ArgumentNullException("ifFalse");
            }
            else if (test.Type != typeof(bool))
            {
                throw new ArgumentException("Test expression should be of type bool");
            }
            else if (ifTrue.Type != ifFalse.Type)
            {
                throw new ArgumentException("The ifTrue and ifFalse type do not match");
            }
            else
            {
                return new ConditionalExpression(test, ifTrue, ifFalse);
            }
        }

        public static ConstantExpression Constant(object value)
        {
            if (value == null)
            {
                return new ConstantExpression(null, typeof(object));
            }
            else
            {
                return Constant(value, value.GetType());
            }
        }

        public static ConstantExpression Constant(object value, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else
            {
                if (value == null)
                {
                    if (!type.CanBeNull())
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    if (!type.IsAssignableFrom(value.GetType())) //OK
                    {
                        throw new ArgumentException();
                    }
                }
                return new ConstantExpression(value, type);
            }
        }

        public static UnaryExpression Convert(Expression expression, Type type)
        {
            return Convert(expression, type, null);
        }

        public static UnaryExpression Convert(Expression expression, Type type, MethodInfo method)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else
            {
                var expressionType = expression.Type;
                if (method != null)
                {
                    CheckUnaryMethod(method, expressionType);
                }
                else if (!IsPrimitiveConversion(expressionType, type) && !IsReferenceConversion(expressionType, type))
                {
                    method = GetUserConversionMethod(expressionType, type);
                }
                return new UnaryExpression
                (
                    ExpressionType.Convert,
                    expression,
                    type,
                    method,
                    IsConvertNodeLifted(method, expression, type)
                );
            }
        }

        public static UnaryExpression ConvertChecked(Expression expression, Type type)
        {
            return ConvertChecked(expression, type, null);
        }

        public static UnaryExpression ConvertChecked(Expression expression, Type type, MethodInfo method)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            var et = expression.Type;
            if (method == null)
            {
                if (IsReferenceConversion(et, type))
                {
                    return Convert(expression, type, null);
                }
                else if (!IsPrimitiveConversion(et, type))
                {
                    method = GetUserConversionMethod(et, type);
                }
            }
            else
            {
                CheckUnaryMethod(method, et);
            }
            return new UnaryExpression(ExpressionType.ConvertChecked, expression, type, method, IsConvertNodeLifted(method, expression, type));
        }

        public static BinaryExpression Divide(Expression left, Expression right)
        {
            return Divide(left, right, null);
        }

        public static BinaryExpression Divide(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryCoreCheck("op_Division", left, right, method);
            return MakeSimpleBinary(ExpressionType.Divide, left, right, method);
        }

        public static ElementInit ElementInit(MethodInfo addMethod, params Expression[] arguments)
        {
            return ElementInit(addMethod, arguments as IEnumerable<Expression>);
        }

        public static ElementInit ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
        {
            if (addMethod == null)
            {
                throw new ArgumentNullException("addMethod");
            }
            else if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            else if (addMethod.Name.ToLower(CultureInfo.InvariantCulture) != "add")
            {
                throw new ArgumentException("addMethod");
            }
            else if (addMethod.IsStatic)
            {
                throw new ArgumentException("addMethod must be an instance method", "addMethod");
            }
            else
            {
                var args = CheckMethodArguments(addMethod, arguments);
                return new ElementInit(addMethod, args);
            }
        }

        public static BinaryExpression Equal(Expression left, Expression right)
        {
            return Equal(left, right, false, null);
        }

        public static BinaryExpression Equal(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            method = BinaryCoreCheck("op_Equality", left, right, method);
            return MakeBoolBinary(ExpressionType.Equal, left, right, liftToNull, method);
        }

        public static BinaryExpression ExclusiveOr(Expression left, Expression right)
        {
            return ExclusiveOr(left, right, null);
        }

        public static BinaryExpression ExclusiveOr(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryBitwiseCoreCheck("op_ExclusiveOr", left, right, method);
            return MakeSimpleBinary(ExpressionType.ExclusiveOr, left, right, method);
        }

        public static MemberExpression Field(Expression expression, FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            else
            {
                if (field.IsStatic)
                {
                    if (expression != null)
                    {
                        throw new ArgumentException("expression");
                    }
                }
                else
                {
                    if (expression == null)
                    {
                        throw new ArgumentNullException("expression");
                    }
                    else
                    {
                        if (!expression.Type.IsAssignableTo(field.DeclaringType))
                        {
                            throw new ArgumentException("field");
                        }
                    }
                }
                return new MemberExpression(expression, field, field.FieldType);
            }
        }

        public static MemberExpression Field(Expression expression, string fieldName)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else
            {
                var field = expression.Type.GetField(fieldName, AllInstance);
                if (field == null)
                {
                    throw new ArgumentException(string.Format("No field named {0} on {1}", fieldName, expression.Type));
                }
                else
                {
                    return new MemberExpression(expression, field, field.FieldType);
                }
            }
        }

        public static Type GetActionType(params Type[] typeArgs)
        {
            if (typeArgs == null)
            {
                throw new ArgumentNullException("typeArgs");
            }
            else if (typeArgs.Length > 4)
            {
                throw new ArgumentException("No Action type of this arity");
            }
            else if (typeArgs.Length == 0)
            {
                return typeof(Action);
            }
            else
            {
                Type action = null;
                switch (typeArgs.Length)
                {
                    case 1:
                        action = typeof(Action<>);
                        break;

                    case 2:
                        action = typeof(Action<,>);
                        break;

                    case 3:
                        action = typeof(Action<,,>);
                        break;

                    case 4:
                        action = typeof(Action<,,,>);
                        break;

                    case 5:
                        action = typeof(Action<,,,,>);
                        break;

                    case 6:
                        action = typeof(Action<,,,,,>);
                        break;

                    case 7:
                        action = typeof(Action<,,,,,,>);
                        break;

                    case 8:
                        action = typeof(Action<,,,,,,,>);
                        break;

                    case 9:
                        action = typeof(Action<,,,,,,,,>);
                        break;

                    case 10:
                        action = typeof(Action<,,,,,,,,,>);
                        break;

                    case 11:
                        action = typeof(Action<,,,,,,,,,,>);
                        break;

                    case 12:
                        action = typeof(Action<,,,,,,,,,,,>);
                        break;

                    case 13:
                        action = typeof(Action<,,,,,,,,,,,,>);
                        break;

                    case 14:
                        action = typeof(Action<,,,,,,,,,,,,,>);
                        break;

                    case 15:
                        action = typeof(Action<,,,,,,,,,,,,,,>);
                        break;

                    case 16:
                        action = typeof(Action<,,,,,,,,,,,,,,,>);
                        break;

                    default:
                        return null;
                }
                return action.MakeGenericType(typeArgs);
            }
        }

        public static Type GetFuncType(params Type[] typeArgs)
        {
            if (typeArgs == null)
            {
                throw new ArgumentNullException("typeArgs");
            }
            else if (typeArgs.Length < 1 || typeArgs.Length > 5)
            {
                throw new ArgumentException("No Func type of this arity");
            }
            else
            {
                Type func = null;
                switch (typeArgs.Length)
                {
                    case 1:
                        func = typeof(Func<>);
                        break;

                    case 2:
                        func = typeof(Func<,>);
                        break;

                    case 3:
                        func = typeof(Func<,,>);
                        break;

                    case 4:
                        func = typeof(Func<,,,>);
                        break;

                    case 5:
                        func = typeof(Func<,,,,>);
                        break;

                    case 6:
                        func = typeof(Func<,,,,,>);
                        break;

                    case 7:
                        func = typeof(Func<,,,,,,>);
                        break;

                    case 8:
                        func = typeof(Func<,,,,,,,>);
                        break;

                    case 9:
                        func = typeof(Func<,,,,,,,,>);
                        break;

                    case 10:
                        func = typeof(Func<,,,,,,,,,>);
                        break;

                    case 11:
                        func = typeof(Func<,,,,,,,,,,>);
                        break;

                    case 12:
                        func = typeof(Func<,,,,,,,,,,,>);
                        break;

                    case 13:
                        func = typeof(Func<,,,,,,,,,,,,>);
                        break;

                    case 14:
                        func = typeof(Func<,,,,,,,,,,,,,>);
                        break;

                    case 15:
                        func = typeof(Func<,,,,,,,,,,,,,,>);
                        break;

                    case 16:
                        func = typeof(Func<,,,,,,,,,,,,,,,>);
                        break;

                    default:
                        return null;
                }
                return func.MakeGenericType(typeArgs);
            }
        }

        public static BinaryExpression GreaterThan(Expression left, Expression right)
        {
            return GreaterThan(left, right, false, null);
        }

        public static BinaryExpression GreaterThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            method = BinaryCoreCheck("op_GreaterThan", left, right, method);
            return MakeBoolBinary(ExpressionType.GreaterThan, left, right, liftToNull, method);
        }

        public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right)
        {
            return GreaterThanOrEqual(left, right, false, null);
        }

        public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            method = BinaryCoreCheck("op_GreaterThanOrEqual", left, right, method);
            return MakeBoolBinary(ExpressionType.GreaterThanOrEqual, left, right, liftToNull, method);
        }

        public static InvocationExpression Invoke(Expression expression, params Expression[] arguments)
        {
            return Invoke(expression, arguments as IEnumerable<Expression>);
        }

        public static InvocationExpression Invoke(Expression expression, IEnumerable<Expression> arguments)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else
            {
                var type = GetInvokableType(expression.Type);
                if (type == null)
                {
                    throw new ArgumentException("The type of the expression is not invokable");
                }
                else
                {
                    var args = arguments.ToReadOnlyCollection();
                    CheckForNull(args, "arguments");
                    var invoke = type.GetInvokeMethod();
                    if (invoke == null)
                    {
                        throw new ArgumentException("expression");
                    }
                    else if (invoke.GetParameters().Length != args.Count)
                    {
                        throw new InvalidOperationException("Arguments count doesn't match parameters length");
                    }
                    else
                    {
                        args = CheckMethodArguments(invoke, args);
                        return new InvocationExpression(expression, invoke.ReturnType, args);
                    }
                }
            }
        }

        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
        {
            return Lambda<TDelegate>(body, parameters as IEnumerable<ParameterExpression>);
        }

        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }
            else
            {
                var ps = parameters.ToReadOnlyCollection();
                body = CheckLambda(typeof(TDelegate), body, ps);
                return new Expression<TDelegate>(body, ps);
            }
        }

        public static LambdaExpression Lambda(Expression body, params ParameterExpression[] parameters)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }
            else if (parameters.Length > 4)
            {
                throw new ArgumentException("Too many parameters");
            }
            else
            {
                return Lambda(GetDelegateType(body.Type, parameters), body, parameters);
            }
        }

        public static LambdaExpression Lambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
        {
            return Lambda(delegateType, body, parameters as IEnumerable<ParameterExpression>);
        }

        public static LambdaExpression Lambda(Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (delegateType == null)
            {
                throw new ArgumentNullException("delegateType");
            }
            else if (body == null)
            {
                throw new ArgumentNullException("body");
            }
            else
            {
                var ps = parameters.ToReadOnlyCollection();
                body = CheckLambda(delegateType, body, ps);
                return CreateExpressionOf(delegateType, body, ps);
            }
        }

        public static BinaryExpression LeftShift(Expression left, Expression right)
        {
            return LeftShift(left, right, null);
        }

        public static BinaryExpression LeftShift(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryBitwiseCoreCheck("op_LeftShift", left, right, method);
            return MakeSimpleBinary(ExpressionType.LeftShift, left, right, method);
        }

        public static BinaryExpression LessThan(Expression left, Expression right)
        {
            return LessThan(left, right, false, null);
        }

        public static BinaryExpression LessThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            method = BinaryCoreCheck("op_LessThan", left, right, method);
            return MakeBoolBinary(ExpressionType.LessThan, left, right, liftToNull, method);
        }

        public static BinaryExpression LessThanOrEqual(Expression left, Expression right)
        {
            return LessThanOrEqual(left, right, false, null);
        }

        public static BinaryExpression LessThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            method = BinaryCoreCheck("op_LessThanOrEqual", left, right, method);
            return MakeBoolBinary(ExpressionType.LessThanOrEqual, left, right, liftToNull, method);
        }

        public static MemberListBinding ListBind(MemberInfo member, params ElementInit[] initializers)
        {
            return ListBind(member, initializers as IEnumerable<ElementInit>);
        }

        public static MemberListBinding ListBind(MemberInfo member, IEnumerable<ElementInit> initializers)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            else if (initializers == null)
            {
                throw new ArgumentNullException("initializers");
            }
            else
            {
                var inits = initializers.ToReadOnlyCollection();
                CheckForNull(inits, "initializers");
                member.OnFieldOrProperty
                (
                    field => CheckIsAssignableToIEnumerable(field.FieldType),
                    prop => CheckIsAssignableToIEnumerable(prop.PropertyType)
                );
                return new MemberListBinding(member, inits);
            }
        }

        public static MemberListBinding ListBind(MethodInfo propertyAccessor, params ElementInit[] initializers)
        {
            return ListBind(propertyAccessor, initializers as IEnumerable<ElementInit>);
        }

        public static MemberListBinding ListBind(MethodInfo propertyAccessor, IEnumerable<ElementInit> initializers)
        {
            if (propertyAccessor == null)
            {
                throw new ArgumentNullException("propertyAccessor");
            }
            else if (initializers == null)
            {
                throw new ArgumentNullException("initializers");
            }
            else
            {
                var inits = initializers.ToReadOnlyCollection();
                CheckForNull(inits, "initializers");
                var prop = GetAssociatedProperty(propertyAccessor);
                if (prop == null)
                {
                    throw new ArgumentException("propertyAccessor");
                }
                CheckIsAssignableToIEnumerable(prop.PropertyType);
                return new MemberListBinding(prop, inits);
            }
        }

        public static ListInitExpression ListInit(NewExpression newExpression, params ElementInit[] initializers)
        {
            return ListInit(newExpression, initializers as IEnumerable<ElementInit>);
        }

        public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<ElementInit> initializers)
        {
            var inits = CheckListInit(newExpression, initializers);
            return new ListInitExpression(newExpression, inits);
        }

        public static ListInitExpression ListInit(NewExpression newExpression, params Expression[] initializers)
        {
            return ListInit(newExpression, initializers as IEnumerable<Expression>);
        }

        public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<Expression> initializers)
        {
            var inits = CheckListInit(newExpression, initializers);
            var add_method = GetAddMethod(newExpression.Type, inits[0].Type);
            if (add_method == null)
            {
                throw new InvalidOperationException("No suitable add method found");
            }
            else
            {
                return new ListInitExpression(newExpression, CreateInitializers(add_method, inits));
            }
        }

        public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, params Expression[] initializers)
        {
            return ListInit(newExpression, addMethod, initializers as IEnumerable<Expression>);
        }

        public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, IEnumerable<Expression> initializers)
        {
            if (addMethod == null)
            {
                return Expression.ListInit(newExpression, initializers);
            }
            else
            {
                if (newExpression == null)
                {
                    throw new ArgumentNullException("newExpression");
                }
                else if (initializers == null)
                {
                    throw new ArgumentNullException("initializers");
                }
                else
                {
                    var readOnly = initializers.ToReadOnlyCollection();
                    if (readOnly.Count == 0)
                    {
                        throw new ArgumentException();
                    }
                    else
                    {
                        return Expression.ListInit(newExpression, CreateInitializers(addMethod, readOnly));
                    }
                }
            }
        }

        public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right)
        {
            return MakeBinary(binaryType, left, right, false, null);
        }

        public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            return MakeBinary(binaryType, left, right, liftToNull, method, null);
        }

        public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method, LambdaExpression conversion)
        {
            switch (binaryType)
            {
                case ExpressionType.Add:
                    return Add(left, right, method);

                case ExpressionType.AddChecked:
                    return AddChecked(left, right, method);

                case ExpressionType.AndAlso:
                    return AndAlso(left, right);

                case ExpressionType.ArrayIndex:
                    return ArrayIndex(left, right);

                case ExpressionType.Coalesce:
                    return Coalesce(left, right, conversion);

                case ExpressionType.Divide:
                    return Divide(left, right, method);

                case ExpressionType.Equal:
                    return Equal(left, right, liftToNull, method);

                case ExpressionType.ExclusiveOr:
                    return ExclusiveOr(left, right, method);

                case ExpressionType.GreaterThan:
                    return GreaterThan(left, right, liftToNull, method);

                case ExpressionType.GreaterThanOrEqual:
                    return GreaterThanOrEqual(left, right, liftToNull, method);

                case ExpressionType.LeftShift:
                    return LeftShift(left, right, method);

                case ExpressionType.LessThan:
                    return LessThan(left, right, liftToNull, method);

                case ExpressionType.LessThanOrEqual:
                    return LessThanOrEqual(left, right, liftToNull, method);

                case ExpressionType.Modulo:
                    return Modulo(left, right, method);

                case ExpressionType.Multiply:
                    return Multiply(left, right, method);

                case ExpressionType.MultiplyChecked:
                    return MultiplyChecked(left, right, method);

                case ExpressionType.NotEqual:
                    return NotEqual(left, right, liftToNull, method);

                case ExpressionType.OrElse:
                    return OrElse(left, right);

                case ExpressionType.Power:
                    return Power(left, right, method);

                case ExpressionType.RightShift:
                    return RightShift(left, right, method);

                case ExpressionType.Subtract:
                    return Subtract(left, right, method);

                case ExpressionType.SubtractChecked:
                    return SubtractChecked(left, right, method);

                case ExpressionType.And:
                    return And(left, right, method);

                case ExpressionType.Or:
                    return Or(left, right, method);

                default:
                    throw new ArgumentException("MakeBinary expect a binary node type");
            }
        }

        public static MemberExpression MakeMemberAccess(Expression expression, MemberInfo member)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            else
            {
                var field = member as FieldInfo;
                if (field != null)
                {
                    return Field(expression, field);
                }
                var property = member as PropertyInfo;
                if (property != null)
                {
                    return Property(expression, property);
                }
                else
                {
                    throw new ArgumentException("Member should either be a field or a property");
                }
            }
        }

        public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type)
        {
            return MakeUnary(unaryType, operand, type, null);
        }

        public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type, MethodInfo method)
        {
            switch (unaryType)
            {
                case ExpressionType.ArrayLength:
                    return ArrayLength(operand);

                case ExpressionType.Convert:
                    return Convert(operand, type, method);

                case ExpressionType.ConvertChecked:
                    return ConvertChecked(operand, type, method);

                case ExpressionType.Negate:
                    return Negate(operand, method);

                case ExpressionType.NegateChecked:
                    return NegateChecked(operand, method);

                case ExpressionType.Not:
                    return Not(operand, method);

                case ExpressionType.Quote:
                    return Quote(operand);

                case ExpressionType.TypeAs:
                    return TypeAs(operand, type);

                case ExpressionType.UnaryPlus:
                    return UnaryPlus(operand, method);

                default:
                    throw new ArgumentException("MakeUnary expect an unary operator");
            }
        }

        public static MemberMemberBinding MemberBind(MemberInfo member, params MemberBinding[] bindings)
        {
            return MemberBind(member, bindings as IEnumerable<MemberBinding>);
        }

        public static MemberMemberBinding MemberBind(MemberInfo member, IEnumerable<MemberBinding> bindings)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            else
            {
                var type = member.OnFieldOrProperty
                (
                    field => field.FieldType,
                    prop => prop.PropertyType
                );
                return new MemberMemberBinding(member, CheckMemberBindings(type, bindings));
            }
        }

        public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, params MemberBinding[] bindings)
        {
            return MemberBind(propertyAccessor, bindings as IEnumerable<MemberBinding>);
        }

        public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, IEnumerable<MemberBinding> bindings)
        {
            if (propertyAccessor == null)
            {
                throw new ArgumentNullException("propertyAccessor");
            }
            else
            {
                var bds = bindings.ToReadOnlyCollection();
                CheckForNull(bds, "bindings");
                var prop = GetAssociatedProperty(propertyAccessor);
                if (prop == null)
                {
                    throw new ArgumentException("propertyAccessor");
                }
                return new MemberMemberBinding(prop, CheckMemberBindings(prop.PropertyType, bindings));
            }
        }

        public static MemberInitExpression MemberInit(NewExpression newExpression, params MemberBinding[] bindings)
        {
            return MemberInit(newExpression, bindings as IEnumerable<MemberBinding>);
        }

        public static MemberInitExpression MemberInit(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
        {
            if (newExpression == null)
            {
                throw new ArgumentNullException("newExpression");
            }
            else
            {
                return new MemberInitExpression(newExpression, CheckMemberBindings(newExpression.Type, bindings));
            }
        }

        public static BinaryExpression Modulo(Expression left, Expression right)
        {
            return Modulo(left, right, null);
        }

        public static BinaryExpression Modulo(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryCoreCheck("op_Modulus", left, right, method);
            return MakeSimpleBinary(ExpressionType.Modulo, left, right, method);
        }

        public static BinaryExpression Multiply(Expression left, Expression right)
        {
            return Multiply(left, right, null);
        }

        public static BinaryExpression Multiply(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryCoreCheck("op_Multiply", left, right, method);
            return MakeSimpleBinary(ExpressionType.Multiply, left, right, method);
        }

        public static BinaryExpression MultiplyChecked(Expression left, Expression right)
        {
            return MultiplyChecked(left, right, null);
        }

        public static BinaryExpression MultiplyChecked(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryCoreCheck("op_Multiply", left, right, method);
            return MakeSimpleBinary(ExpressionType.MultiplyChecked, left, right, method);
        }

        public static UnaryExpression Negate(Expression expression)
        {
            return Negate(expression, null);
        }

        public static UnaryExpression Negate(Expression expression, MethodInfo method)
        {
            method = UnaryCoreCheck("op_UnaryNegation", expression, method, IsSignedNumber);
            return MakeSimpleUnary(ExpressionType.Negate, expression, method);
        }

        public static UnaryExpression NegateChecked(Expression expression)
        {
            return NegateChecked(expression, null);
        }

        public static UnaryExpression NegateChecked(Expression expression, MethodInfo method)
        {
            method = UnaryCoreCheck("op_UnaryNegation", expression, method, IsSignedNumber);
            return MakeSimpleUnary(ExpressionType.NegateChecked, expression, method);
        }

        public static NewExpression New(ConstructorInfo constructor)
        {
            if (constructor == null)
            {
                throw new ArgumentNullException("constructor");
            }
            else if (constructor.GetParameters().Length > 0)
            {
                throw new ArgumentException("Constructor must be parameter less");
            }
            else
            {
                return new NewExpression(constructor, (null as IEnumerable<Expression>).ToReadOnlyCollection(), null);
            }
        }

        public static NewExpression New(Type type)
        {
            CheckNotNullNorVoid(type, "type");
            var args = (null as IEnumerable<Expression>).ToReadOnlyCollection();
            if (type.IsValueType)
            {
                return new NewExpression(type, args);
            }
            else
            {
                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                {
                    throw new ArgumentException("Type doesn't have a parameter less constructor");
                }
                else
                {
                    return new NewExpression(ctor, args, null);
                }
            }
        }

        public static NewExpression New(ConstructorInfo constructor, params Expression[] arguments)
        {
            return New(constructor, arguments as IEnumerable<Expression>);
        }

        public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments)
        {
            if (constructor == null)
            {
                throw new ArgumentNullException("constructor");
            }
            else
            {
                var args = CheckMethodArguments(constructor, arguments);
                return new NewExpression(constructor, args, null);
            }
        }

        public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, params MemberInfo[] members)
        {
            return New(constructor, arguments, members as IEnumerable<MemberInfo>);
        }

        public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, IEnumerable<MemberInfo> members)
        {
            if (constructor == null)
            {
                throw new ArgumentNullException("constructor");
            }
            else
            {
                var args = arguments.ToReadOnlyCollection();
                var mmbs = members.ToReadOnlyCollection();
                CheckForNull(args, "arguments");
                CheckForNull(mmbs, "members");
                args = CheckMethodArguments(constructor, arguments);
                if (args.Count != mmbs.Count)
                {
                    throw new ArgumentException("Arguments count does not match members count");
                }
                else
                {
                    for (int index = 0; index < mmbs.Count; index++)
                    {
                        var member = mmbs[index];
                        Type type = null;
                        switch (member.MemberType)
                        {
                            case MemberTypes.Field:
                                type = (member as FieldInfo).FieldType;
                                break;

                            case MemberTypes.Method:
                                type = (member as MethodInfo).ReturnType;
                                break;

                            case MemberTypes.Property:
                                var prop = member as PropertyInfo;
                                if (prop.GetGetMethod(true) == null)
                                {
                                    throw new ArgumentException("Property must have a getter");
                                }
                                else
                                {
                                    type = prop.PropertyType;
                                }
                                break;

                            default:
                                throw new ArgumentException("Member type not allowed");
                        }
                        if (!args[index].Type.IsAssignableTo(type))
                        {
                            throw new ArgumentException("Argument type not assignable to member type");
                        }
                    }
                    return new NewExpression(constructor, args, mmbs);
                }
            }
        }

        public static NewArrayExpression NewArrayBounds(Type type, params Expression[] bounds)
        {
            return NewArrayBounds(type, bounds as IEnumerable<Expression>);
        }

        public static NewArrayExpression NewArrayBounds(Type type, IEnumerable<Expression> bounds)
        {
            if (bounds == null)
            {
                throw new ArgumentNullException("bounds");
            }
            else
            {
                CheckNotNullNorVoid(type, "type");
                var array_bounds = bounds.ToReadOnlyCollection();
                foreach (var expression in array_bounds)
                {
                    if (!IsInt(expression.Type))
                    {
                        throw new ArgumentException("The bounds collection can only contain expression of integers types");
                    }
                }
                return new NewArrayExpression(ExpressionType.NewArrayBounds, type.MakeArrayType(array_bounds.Count), array_bounds);
            }
        }

        public static NewArrayExpression NewArrayInit(Type type, params Expression[] initializers)
        {
            return NewArrayInit(type, initializers as IEnumerable<Expression>);
        }

        public static NewArrayExpression NewArrayInit(Type type, IEnumerable<Expression> initializers)
        {
            if (initializers == null)
            {
                throw new ArgumentNullException("initializers");
            }
            else
            {
                CheckNotNullNorVoid(type, "type");
                var inits = initializers.ToReadOnlyCollection();
                foreach (var expression in inits)
                {
                    if (expression == null)
                    {
                        throw new ArgumentNullException("initializers");
                    }
                    if (!expression.Type.IsAssignableTo(type))
                    {
                        throw new InvalidOperationException(string.Format("{0} IsAssignableTo {1}, expression [ {2} ] : {3}", expression.Type, type, expression.NodeType, expression));
                    }

                    // TODO: Quote elements if type == typeof (Expression)
                }
                return new NewArrayExpression(ExpressionType.NewArrayInit, type.MakeArrayType(), inits);
            }
        }

        public static UnaryExpression Not(Expression expression)
        {
            return Not(expression, null);
        }

        public static UnaryExpression Not(Expression expression, MethodInfo method)
        {
            Func<Type, bool> validator = type => IsIntOrBool(type);
            method = UnaryCoreCheck("op_LogicalNot", expression, method, validator);
            if (method == null)
            {
                method = UnaryCoreCheck("op_OnesComplement", expression, method, validator);
            }
            return MakeSimpleUnary(ExpressionType.Not, expression, method);
        }

        public static BinaryExpression NotEqual(Expression left, Expression right)
        {
            return NotEqual(left, right, false, null);
        }

        public static BinaryExpression NotEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            method = BinaryCoreCheck("op_Inequality", left, right, method);
            return MakeBoolBinary(ExpressionType.NotEqual, left, right, liftToNull, method);
        }

        public static BinaryExpression Or(Expression left, Expression right)
        {
            return Or(left, right, null);
        }

        public static BinaryExpression Or(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryBitwiseCoreCheck("op_BitwiseOr", left, right, method);
            return MakeSimpleBinary(ExpressionType.Or, left, right, method);
        }

        public static BinaryExpression OrElse(Expression left, Expression right)
        {
            return OrElse(left, right, null);
        }

        public static BinaryExpression OrElse(Expression left, Expression right, MethodInfo method)
        {
            method = ConditionalBinaryCheck("op_BitwiseOr", left, right, method);
            return MakeBoolBinary(ExpressionType.OrElse, left, right, true, method);
        }

        public static ParameterExpression Parameter(Type type, string name)
        {
            CheckNotNullNorVoid(type, "type");
            return new ParameterExpression(type, name);
        }

        public static BinaryExpression Power(Expression left, Expression right)
        {
            return Power(left, right, null);
        }

        public static BinaryExpression Power(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryCoreCheck(null, left, right, method);
            if (left.Type.GetNotNullableType() != typeof(double))
            {
                throw new InvalidOperationException("Power only supports double arguments");
            }
            return MakeSimpleBinary(ExpressionType.Power, left, right, method);
        }

        public static MemberExpression Property(Expression expression, MethodInfo propertyAccessor)
        {
            if (propertyAccessor == null)
            {
                throw new ArgumentNullException("propertyAccessor");
            }
            else
            {
                CheckNonGenericMethod(propertyAccessor);
                if (!propertyAccessor.IsStatic)
                {
                    if (expression == null)
                    {
                        throw new ArgumentNullException("expression");
                    }
                    if (!expression.Type.IsAssignableTo(propertyAccessor.DeclaringType))
                    {
                        throw new ArgumentException("expression");
                    }
                }

                // .NET does not mandate that if the property is static, that the expression must be null
                // fixes a bug exposed by Orchard's ContentItemRecordAlteration.Alteration
                // else if (expression != null)
                // throw new ArgumentException ("expression");
                var prop = GetAssociatedProperty(propertyAccessor);
                if (prop == null)
                {
                    throw new ArgumentException(string.Format("Method {0} has no associated property", propertyAccessor));
                }
                return new MemberExpression(expression, prop, prop.PropertyType);
            }
        }

        public static MemberExpression Property(Expression expression, PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            else
            {
                var getter = property.GetGetMethod(true);
                if (getter == null)
                {
                    throw new ArgumentException("getter");
                }
                else
                {
                    if (getter.IsStatic)
                    {
                        if (expression != null)
                        {
                            throw new ArgumentException("expression");
                        }
                    }
                    else
                    {
                        if (expression == null)
                        {
                            throw new ArgumentNullException("expression");
                        }
                        else if (!expression.Type.IsAssignableTo(property.DeclaringType))
                        {
                            throw new ArgumentException("expression");
                        }
                    }
                }
                return new MemberExpression(expression, property, property.PropertyType);
            }
        }

        public static MemberExpression Property(Expression expression, string propertyName)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else
            {
                var property = expression.Type.GetProperty(propertyName, AllInstance);
                if (property == null)
                {
                    throw new ArgumentException(string.Format("No property named {0} on {1}", propertyName, expression.Type));
                }
                else
                {
                    return new MemberExpression(expression, property, property.PropertyType);
                }
            }
        }

        public static MemberExpression PropertyOrField(Expression expression, string propertyOrFieldName)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else if (propertyOrFieldName == null)
            {
                throw new ArgumentNullException("propertyOrFieldName");
            }
            else
            {
                var property = expression.Type.GetProperty(propertyOrFieldName, AllInstance);
                if (property == null)
                {
                    var field = expression.Type.GetField(propertyOrFieldName, AllInstance);
                    if (field == null)
                    {
                        throw new ArgumentException(string.Format("No field or property named {0} on {1}", propertyOrFieldName, expression.Type));
                    }
                    else
                    {
                        return new MemberExpression(expression, field, field.FieldType);
                    }
                }
                else
                {
                    return new MemberExpression(expression, property, property.PropertyType);
                }
            }
        }

        public static UnaryExpression Quote(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else
            {
                return new UnaryExpression(ExpressionType.Quote, expression, expression.GetType());
            }
        }

        public static BinaryExpression RightShift(Expression left, Expression right)
        {
            return RightShift(left, right, null);
        }

        public static BinaryExpression RightShift(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryCoreCheck("op_RightShift", left, right, method);
            return MakeSimpleBinary(ExpressionType.RightShift, left, right, method);
        }

        public static BinaryExpression Subtract(Expression left, Expression right)
        {
            return Subtract(left, right, null);
        }

        public static BinaryExpression Subtract(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryCoreCheck("op_Subtraction", left, right, method);
            return MakeSimpleBinary(ExpressionType.Subtract, left, right, method);
        }

        public static BinaryExpression SubtractChecked(Expression left, Expression right)
        {
            return SubtractChecked(left, right, null);
        }

        public static BinaryExpression SubtractChecked(Expression left, Expression right, MethodInfo method)
        {
            method = BinaryCoreCheck("op_Subtraction", left, right, method);

            // The check in BinaryCoreCheck allows a bit more than we do
            // (byte, sbyte).  Catch that here
            if (method == null)
            {
                if (left.Type == typeof(byte) || left.Type == typeof(sbyte))
                {
                    throw new InvalidOperationException(string.Format("SubtractChecked not defined for {0} and {1}", left.Type, right.Type));
                }
            }
            return MakeSimpleBinary(ExpressionType.SubtractChecked, left, right, method);
        }

        public static UnaryExpression TypeAs(Expression expression, Type type)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (type.IsValueType && !type.IsNullable())
            {
                throw new ArgumentException("TypeAs expect a reference or a nullable type");
            }
            else
            {
                return new UnaryExpression(ExpressionType.TypeAs, expression, type);
            }
        }

        public static TypeBinaryExpression TypeIs(Expression expression, Type type)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            else
            {
                CheckNotNullNorVoid(type, "type");
                return new TypeBinaryExpression(ExpressionType.TypeIs, expression, type, typeof(bool));
            }
        }

        public static UnaryExpression UnaryPlus(Expression expression)
        {
            return UnaryPlus(expression, null);
        }

        public static UnaryExpression UnaryPlus(Expression expression, MethodInfo method)
        {
            method = UnaryCoreCheck("op_UnaryPlus", expression, method, type => IsNumber(type));
            return MakeSimpleUnary(ExpressionType.UnaryPlus, expression, method);
        }

        public override string ToString()
        {
            return ExpressionPrinter.ToString(this);
        }

        internal static MethodInfo GetFalseOperator(Type self)
        {
            return GetBooleanOperator("op_False", self);
        }

        internal static MethodInfo GetTrueOperator(Type self)
        {
            return GetBooleanOperator("op_True", self);
        }

        internal static bool IsPrimitiveConversion(Type type, Type target)
        {
            if (type == target)
            {
                return true;
            }
            else if (type.IsNullable() && target == type.GetNotNullableType())
            {
                return true;
            }
            else if (target.IsNullable() && type == target.GetNotNullableType())
            {
                return true;
            }
            else if (IsConvertiblePrimitive(type) && IsConvertiblePrimitive(target))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool IsReferenceConversion(Type type, Type target)
        {
            if (type == target)
            {
                return true;
            }
            else if (type == typeof(object) || target == typeof(object))
            {
                return true;
            }
            else if (type.IsInterface || target.IsInterface)
            {
                return true;
            }
            else if (type.IsEnum && target == typeof(Enum))
            {
                return true;
            }
            else if (type.IsValueType || target.IsValueType)
            {
                return false;
            }
            else if (type.IsAssignableTo(target) || target.IsAssignableTo(type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool IsUnsigned(Type t)
        {
            if (t.IsPointer)
            {
                return IsUnsigned(t.GetElementType());
            }
            else
            {
                return t == typeof(ushort) ||
                       t == typeof(uint) ||
                       t == typeof(ulong) ||
                       t == typeof(byte);
            }
        }

        // This method must be overwritten by derived classes to
        // compile the expression
        internal virtual void Emit(EmitContext emitContext)
        {
            throw new NotImplementedException(string.Format("Emit method is not implemented in expression type {0}", GetType()));
        }

        // This is like BinaryCoreCheck, but if no method is used adds the restriction that
        // only ints and bools are allowed
        private static MethodInfo BinaryBitwiseCoreCheck(string operName, Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            if (method == null)
            {
                // avoid reflection shortcut and catches Ints/bools before we check Numbers in general
                if (left.Type == right.Type && IsIntOrBool(left.Type))
                {
                    return null;
                }
            }
            method = BinaryCoreCheck(operName, left, right, method);
            if (method == null)
            {
                // The check in BinaryCoreCheck allows a bit more than we do
                // (floats and doubles).  Catch this here
                if (left.Type == typeof(double) || left.Type == typeof(float))
                {
                    throw new InvalidOperationException("Types not supported");
                }
            }
            return method;
        }

        // Performs basic checks on the incoming expressions for binary expressions
        // and any provided MethodInfo.
        private static MethodInfo BinaryCoreCheck(string operName, Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            if (method != null)
            {
                if (method.ReturnType == typeof(void))
                {
                    throw new ArgumentException("Specified method must return a value", "method");
                }
                if (!method.IsStatic)
                {
                    throw new ArgumentException("Method must be static", "method");
                }
                var parameters = method.GetParameters();
                if (parameters.Length != 2)
                {
                    throw new ArgumentException("Must have only two parameters", "method");
                }
                if (!IsAssignableToParameterType(left.Type, parameters[0]))
                {
                    throw new InvalidOperationException("left-side argument type does not match left expression type");
                }
                if (!IsAssignableToParameterType(right.Type, parameters[1]))
                {
                    throw new InvalidOperationException("right-side argument type does not match right expression type");
                }
                return method;
            }
            else
            {
                Type ltype = left.Type;
                Type rtype = right.Type;
                Type ultype = ltype.GetNotNullableType();
                Type urtype = rtype.GetNotNullableType();
                if (operName == "op_BitwiseOr" || operName == "op_BitwiseAnd")
                {
                    if (ultype == typeof(bool))
                    {
                        if (ultype == urtype && ltype == rtype)
                        {
                            return null;
                        }
                    }
                }

                // Use IsNumber to avoid expensive reflection.
                if (IsNumber(ultype))
                {
                    if (ultype == urtype && ltype == rtype)
                    {
                        return null;
                    }
                    if (operName != null)
                    {
                        method = GetBinaryOperator(operName, urtype, left, right);
                        if (method != null)
                        {
                            return method;
                        }
                    }
                }
                if (operName != null)
                {
                    method = GetBinaryOperator(operName, ultype, left, right);
                    if (method != null)
                    {
                        return method;
                    }
                }
                if (operName == "op_Equality" || operName == "op_Inequality")
                {
                    // == and != allow reference types without operators defined.
                    if (!ltype.IsValueType && !rtype.IsValueType)
                    {
                        return null;
                    }
                    if (ltype == rtype && ultype.IsEnum)
                    {
                        return null;
                    }
                    if (ltype == rtype && ultype == typeof(bool))
                    {
                        return null;
                    }
                }
                if (operName == "op_LeftShift" || operName == "op_RightShift")
                {
                    if (IsInt(ultype) && urtype == typeof(int))
                    {
                        return null;
                    }
                }
                throw new InvalidOperationException(
                    string.Format("Operation {0} not defined for {1} and {2}", operName != null ? operName.Substring(3) : "is", ltype, rtype));
            }
        }

        private static bool CanAssign(Type target, Type source)
        {
            // This catches object and value type mixage, type compatibility is handled later
            if (target.IsValueType ^ source.IsValueType)
            {
                return false;
            }
            else
            {
                return source.IsAssignableTo(target);
            }
        }

        // Miscelaneous
        private static void CheckArray(Expression array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            else if (!array.Type.IsArray)
            {
                throw new ArgumentException("The array argument must be of type array");
            }
        }

        private static void CheckForNull<T>(IEnumerable<T> collection, string name)
            where T : class
        {
            foreach (var t in collection)
            {
                if (t == null)
                {
                    throw new ArgumentNullException(name);
                }
            }
        }

        private static void CheckIsAssignableToIEnumerable(Type t)
        {
            if (!t.IsAssignableTo(typeof(IEnumerable)))
            {
                throw new ArgumentException(string.Format("Type {0} doesn't implemen IEnumerable", t));
            }
        }

        private static Expression CheckLambda(Type delegateType, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            if (!delegateType.IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("delegateType");
            }
            var invoke = delegateType.GetInvokeMethod();
            if (invoke == null)
            {
                throw new ArgumentException("delegate must contain an Invoke method", "delegateType");
            }
            var invoke_parameters = invoke.GetParameters();
            if (invoke_parameters.Length != parameters.Count)
            {
                throw new ArgumentException(string.Format("Different number of arguments in delegate {0}", delegateType), "delegateType");
            }
            for (int i = 0; i < invoke_parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter == null)
                {
                    throw new ArgumentNullException("parameters");
                }
                if (!CanAssign(parameter.Type, invoke_parameters[i].ParameterType))
                {
                    throw new ArgumentException(string.Format("Can not assign a {0} to a {1}", invoke_parameters[i].ParameterType, parameter.Type));
                }
            }
            if (invoke.ReturnType != typeof(void))
            {
                if (!CanAssign(invoke.ReturnType, body.Type))
                {
                    if (invoke.ReturnType.IsExpression())
                    {
                        return Expression.Quote(body);
                    }
                    throw new ArgumentException(string.Format("body type {0} can not be assigned to {1}", body.Type, invoke.ReturnType));
                }
            }
            return body;
        }

        private static ReadOnlyCollection<T> CheckListInit<T>(NewExpression newExpression, IEnumerable<T> initializers)
            where T : class
        {
            if (newExpression == null)
            {
                throw new ArgumentNullException("newExpression");
            }
            else if (initializers == null)
            {
                throw new ArgumentNullException("initializers");
            }
            else if (!newExpression.Type.IsAssignableTo(typeof(IEnumerable)))
            {
                throw new InvalidOperationException("The type of the new expression does not implement IEnumerable");
            }
            else
            {
                var inits = initializers.ToReadOnlyCollection();
                if (inits.Count == 0)
                {
                    throw new ArgumentException("Empty initializers");
                }
                else
                {
                    CheckForNull(inits, "initializers");
                    return inits;
                }
            }
        }

        private static ReadOnlyCollection<MemberBinding> CheckMemberBindings(Type type, IEnumerable<MemberBinding> bindings)
        {
            if (bindings == null)
            {
                throw new ArgumentNullException("bindings");
            }
            else
            {
                var bds = bindings.ToReadOnlyCollection();
                CheckForNull(bds, "bindings");
                foreach (var binding in bds)
                {
                    if (!type.IsAssignableTo(binding.Member.DeclaringType))
                    {
                        throw new ArgumentException("Type not assignable to member type");
                    }
                }
                return bds;
            }
        }

        private static ReadOnlyCollection<Expression> CheckMethodArguments(MethodBase method, IEnumerable<Expression> arguments)
        {
            CheckNonGenericMethod(method);
            var _arguments = CreateArgumentList(arguments);
            var parameters = method.GetParameters();
            if (_arguments.Count != parameters.Length)
            {
                throw new ArgumentException("The number of arguments doesn't match the number of parameters");
            }
            else
            {
                for (int index = 0; index < parameters.Length; index++)
                {
                    if (_arguments[index] == null)
                    {
                        throw new ArgumentNullException("_arguments");
                    }
                    else if (!IsAssignableToParameterType(_arguments[index].Type, parameters[index]))
                    {
                        if (!parameters[index].ParameterType.IsExpression())
                        {
                            throw new ArgumentException("_arguments");
                        }
                        _arguments[index] = Expression.Quote(_arguments[index]);
                    }
                }
                return _arguments.ToReadOnlyCollection();
            }
        }

        private static void CheckNonGenericMethod(MethodBase method)
        {
            if (method.IsGenericMethodDefinition || method.ContainsGenericParameters)
            {
                throw new ArgumentException("Can not used open generic methods");
            }
        }

        private static void CheckNotNullNorVoid(Type type, string parameterName)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (type == typeof(void))
            {
                throw new ArgumentException("Type can't be void", parameterName);
            }
        }

        private static MethodInfo CheckUnaryMethod(MethodInfo method, Type param)
        {
            if (method.ReturnType == typeof(void))
            {
                throw new ArgumentException("Specified method must return a value", "method");
            }
            else if (!method.IsStatic)
            {
                throw new ArgumentException("Method must be static", "method");
            }
            else
            {
                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                {
                    throw new ArgumentException("Must have only one parameters", "method");
                }
                else if (!IsAssignableToParameterType(param.GetNotNullableType(), parameters[0]))
                {
                    throw new InvalidOperationException("left-side argument type does not match expression type");
                }
                else
                {
                    return method;
                }
            }
        }

        private static Type[] CollectTypes(IEnumerable<Expression> expressions)
        {
            IEnumerable<Type> temp = System.Linq.Enumerable.Select(expressions, input => input.Type);
            return Enumerable.ToArray(temp);
        }

        private static MethodInfo ConditionalBinaryCheck(string oper, Expression left, Expression right, MethodInfo method)
        {
            method = Expression.BinaryCoreCheck(oper, left, right, method);
            if (method == null)
            {
                if (left.Type.GetNotNullableType() != typeof(bool))
                {
                    throw new InvalidOperationException("Only booleans are allowed");
                }
            }
            else
            {
                Type notNullableType = left.Type.GetNotNullableType();
                if (left.Type != right.Type || method.ReturnType != notNullableType)
                {
                    throw new ArgumentException("left, right and return type must match");
                }
                else
                {
                    MethodInfo trueOperator = Expression.GetTrueOperator(notNullableType);
                    MethodInfo falseOperator = Expression.GetFalseOperator(notNullableType);
                    if (trueOperator == null || falseOperator == null)
                    {
                        throw new ArgumentException("Operators true and false are required but not defined");
                    }
                }
            }
            return method;
        }

        private static IList<Expression> CreateArgumentList(IEnumerable<Expression> arguments)
        {
            if (arguments == null)
            {
                return arguments.ToReadOnlyCollection();
            }
            else
            {
                return System.Linq.Enumerable.ToList(arguments);
            }
        }

        private static LambdaExpression CreateExpressionOf(Type type, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            return (LambdaExpression)typeof(Expression<>).MakeGenericType(type).GetConstructor
            (
                NonPublicInstance,
                null,
                new[] { typeof(Expression), typeof(ReadOnlyCollection<ParameterExpression>) },
                null
            ).Invoke(new object[] { body, parameters });
        }

        private static ReadOnlyCollection<ElementInit> CreateInitializers(MethodInfo addMethod, ReadOnlyCollection<Expression> initializers)
        {
            var temp = System.Linq.Enumerable.Select(initializers, input => Expression.ElementInit(addMethod, input));
            return Theraot.Collections.Extensions.ToReadOnlyCollection(temp);
        }

        private static MethodInfo GetAddMethod(Type type, Type arg)
        {
            return type.GetMethod("Add", PublicInstance | BindingFlags.IgnoreCase, null, new[] { arg }, null);
        }

        private static PropertyInfo GetAssociatedProperty(MethodInfo method)
        {
            if (method == null)
            {
                return null;
            }
            else
            {
                foreach (var property in method.DeclaringType.GetProperties(All))
                {
                    if (method.Equals(property.GetGetMethod(true)) || method.Equals(property.GetSetMethod(true)))
                    {
                        return property;
                    }
                }
                return null;
            }
        }

        private static MethodInfo GetBinaryOperator(string operName, Type onType, Expression left, Expression right)
        {
            MethodInfo[] methods = onType.GetMethods(PublicStatic);
            foreach (var method in methods)
            {
                if (method.Name == operName)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length != 2)
                    {
                        continue;
                    }
                    else if (method.IsGenericMethod)
                    {
                        continue;
                    }
                    else if (!IsAssignableToParameterType(left.Type, parameters[0]))
                    {
                        continue;
                    }
                    else if (!IsAssignableToParameterType(right.Type, parameters[1]))
                    {
                        continue;
                    }

                    // Method has papers in order.
                    return method;
                }
                else
                {
                    continue;
                }
            }
            return null;
        }

        private static MethodInfo GetBooleanOperator(string op, Type self)
        {
            return GetUnaryOperator(op, self, self, typeof(bool));
        }

        private static Type GetDelegateType(Type return_type, ParameterExpression[] parameters)
        {
            if (parameters == null)
            {
                parameters = new ParameterExpression[0];
            }
            if (return_type != typeof(void))
            {
                Type[] type = new Type[(int)parameters.Length + 1];
                for (int index = 0; index < (int)type.Length - 1; index++)
                {
                    type[index] = parameters[index].Type;
                }
                type[(int)type.Length - 1] = return_type;
                return Expression.GetFuncType(type);
            }
            else
            {
                ParameterExpression[] parameterExpressionArray = parameters;
                return Expression.GetActionType
                (
                    System.Linq.Enumerable.ToArray
                    (
                        System.Linq.Enumerable.Select
                        (
                            parameterExpressionArray,
                            p => p.Type
                        )
                    )
                );
            }
        }

        private static Type GetGenericType(Type type, Type def)
        {
            if (type == null)
            {
                return null;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == def)
            {
                return type;
            }
            else
            {
                return GetGenericType(type.BaseType, def);
            }
        }

        private static Type GetInvokableType(Type type)
        {
            if (type.IsAssignableTo(typeof(Delegate)))
            {
                return type;
            }
            else
            {
                return GetGenericType(type, typeof(Expression<>));
            }
        }

        private static MethodInfo GetUnaryOperator(string operName, Type declaring, Type param)
        {
            return GetUnaryOperator(operName, declaring, param, null);
        }

        private static MethodInfo GetUnaryOperator(string operName, Type declaring, Type param, Type ret)
        {
            var methods = declaring.GetNotNullableType().GetMethods(PublicStatic);
            foreach (var method in methods)
            {
                if (method.Name == operName)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length != 1)
                    {
                        continue;
                    }
                    else if (method.IsGenericMethod)
                    {
                        continue;
                    }
                    else if (!IsAssignableToParameterType(param.GetNotNullableType(), parameters[0]))
                    {
                        continue;
                    }
                    else if (ret != null && method.ReturnType != ret.GetNotNullableType())
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                return method;
            }
            return null;
        }

        private static MethodInfo GetUserConversionMethod(Type type, Type target)
        {
            var method =
                GetUnaryOperator("op_Explicit", type, type, target) ??
                GetUnaryOperator("op_Implicit", type, type, target) ??
                GetUnaryOperator("op_Explicit", target, type, target) ??
                GetUnaryOperator("op_Implicit", target, type, target);
            if (method == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return method;
            }
        }

        private static bool IsAssignableToOperatorParameter(Expression expression, ParameterInfo parameter)
        {
            return
                expression.Type == parameter.ParameterType ||
                (
                    !expression.Type.IsNullable() &&
                    !parameter.ParameterType.IsNullable() &&
                    IsAssignableToParameterType(expression.Type, parameter)
                );
        }

        private static bool IsAssignableToParameterType(Type type, ParameterInfo param)
        {
            var ptype = param.ParameterType;
            if (ptype.IsByRef)
            {
                ptype = ptype.GetElementType();
            }
            return type.GetNotNullableType().IsAssignableTo(ptype);
        }

        private static bool IsConvertiblePrimitive(Type type)
        {
            var t = type.GetNotNullableType();
            if (t == typeof(bool))
            {
                return false;
            }
            else if (t.IsEnum)
            {
                return true;
            }
            else
            {
                return t.IsPrimitive;
            }
        }

        private static bool IsConvertNodeLifted(MethodInfo method, Expression operand, Type target)
        {
            if (method == null)
            {
                return operand.Type.IsNullable() || target.IsNullable();
            }
            else if (operand.Type.IsNullable() && !ParameterMatch(method, operand.Type))
            {
                return true;
            }
            else if (target.IsNullable() && !ReturnTypeMatch(method, target))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsExpressionOfParameter(Type type, Type ptype)
        {
            return ptype.IsGenericInstanceOf(typeof(Expression<>)) && ptype.GetFirstGenericArgument() == type;
        }

        private static bool IsInt(Type t)
        {
            return t == typeof(byte) || t == typeof(sbyte) ||
                   t == typeof(short) || t == typeof(ushort) ||
                   t == typeof(int) || t == typeof(uint) ||
                   t == typeof(long) || t == typeof(ulong);
        }

        private static bool IsIntOrBool(Type t)
        {
            return IsInt(t) || t == typeof(bool);
        }

        private static bool IsNumber(Type t)
        {
            if (IsInt(t))
            {
                return true;
            }
            else
            {
                return t == typeof(float) || t == typeof(double);
            }
        }

        private static bool IsSignedNumber(Type t)
        {
            return IsNumber(t) && !IsUnsigned(t);
        }

        private static BinaryExpression MakeBoolBinary(ExpressionType expressionType, Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            bool is_lifted;
            Type type;
            if (method == null)
            {
                if (!left.Type.IsNullable() && !right.Type.IsNullable())
                {
                    is_lifted = false;
                    liftToNull = false;
                    type = typeof(bool);
                }
                else if (left.Type.IsNullable() && right.Type.IsNullable())
                {
                    is_lifted = true;
                    type = liftToNull ? typeof(bool?) : typeof(bool);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                var parameters = method.GetParameters();
                var lp = parameters[0];
                var rp = parameters[1];
                if (IsAssignableToOperatorParameter(left, lp) && IsAssignableToOperatorParameter(right, rp))
                {
                    is_lifted = false;
                    liftToNull = false;
                    type = method.ReturnType;
                }
                else if (left.Type.IsNullable()
                         && right.Type.IsNullable()
                         && left.Type.GetNotNullableType() == lp.ParameterType
                         && right.Type.GetNotNullableType() == rp.ParameterType)
                {
                    is_lifted = true;
                    if (method.ReturnType == typeof(bool))
                    {
                        type = liftToNull ? typeof(bool?) : typeof(bool);
                    }
                    else if (!method.ReturnType.IsNullable())
                    {
                        // This behavior is not documented: what
                        // happens if the result is not typeof(bool), but
                        // the parameters are nullable: the result
                        // becomes nullable<returntype>
                        // See:
                        // https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=323139
                        type = method.ReturnType.MakeNullableType();
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            return new BinaryExpression(expressionType, type, left, right, liftToNull, is_lifted, method, null);
        }

        private static BinaryExpression MakeCoalesce(Expression left, Expression right)
        {
            Type result = null;
            if (left.Type.IsNullable())
            {
                Type lbase = left.Type.GetNotNullableType();
                if (!right.Type.IsNullable() && right.Type.IsAssignableTo(lbase))
                {
                    result = lbase;
                }
            }
            if (result == null && right.Type.IsAssignableTo(left.Type))
            {
                result = left.Type;
            }
            if (result == null)
            {
                if (left.Type.IsNullable() && left.Type.GetNotNullableType().IsAssignableTo(right.Type))
                {
                    result = right.Type;
                }
            }
            if (result == null)
            {
                throw new ArgumentException("Incompatible argument types");
            }
            return new BinaryExpression(ExpressionType.Coalesce, result, left, right, false, false, null, null);
        }

        private static BinaryExpression MakeConvertedCoalesce(Expression left, Expression right, LambdaExpression conversion)
        {
            var invoke = conversion.Type.GetInvokeMethod();
            if (invoke.ReturnType == typeof(void))
            {
                throw new ArgumentException("Return type can't be void");
            }
            if (invoke.ReturnType != right.Type)
            {
                throw new InvalidOperationException("Conversion return type doesn't march right type");
            }
            var parameters = invoke.GetParameters();
            if (parameters.Length != 1)
            {
                throw new ArgumentException("Conversion has wrong number of parameters");
            }
            if (!IsAssignableToParameterType(left.Type, parameters[0]))
            {
                throw new InvalidOperationException("Conversion argument doesn't marcht left type");
            }
            return new BinaryExpression(ExpressionType.Coalesce, right.Type, left, right, false, false, null, conversion);
        }

        private static BinaryExpression MakeSimpleBinary(ExpressionType expressionType, Expression left, Expression right, MethodInfo method)
        {
            bool is_lifted;
            Type type;
            if (method == null)
            {
                is_lifted = left.Type.IsNullable();
                type = left.Type;
            }
            else
            {
                var parameters = method.GetParameters();
                var lp = parameters[0];
                var rp = parameters[1];
                if (IsAssignableToOperatorParameter(left, lp) && IsAssignableToOperatorParameter(right, rp))
                {
                    is_lifted = false;
                    type = method.ReturnType;
                }
                else if (left.Type.IsNullable()
                         && right.Type.IsNullable()
                         && left.Type.GetNotNullableType() == lp.ParameterType
                         && right.Type.GetNotNullableType() == rp.ParameterType
                         && !method.ReturnType.IsNullable())
                {
                    is_lifted = true;
                    type = method.ReturnType.MakeNullableType();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            return new BinaryExpression(expressionType, type, left, right, is_lifted, is_lifted, method, null);
        }

        private static UnaryExpression MakeSimpleUnary(ExpressionType expressionType, Expression expression, MethodInfo method)
        {
            bool is_lifted;
            Type type;
            if (method == null)
            {
                type = expression.Type;
                is_lifted = type.IsNullable();
            }
            else
            {
                var parameter = method.GetParameters()[0];
                if (IsAssignableToOperatorParameter(expression, parameter))
                {
                    is_lifted = false;
                    type = method.ReturnType;
                }
                else if (expression.Type.IsNullable()
                         && expression.Type.GetNotNullableType() == parameter.ParameterType
                         && !method.ReturnType.IsNullable())
                {
                    is_lifted = true;
                    type = method.ReturnType.MakeNullableType();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            return new UnaryExpression(expressionType, expression, type, method, is_lifted);
        }

        private static bool MethodMatch(MethodInfo method, string name, Type[] parameterTypes, Type[] argumentTypes)
        {
            if (method.Name != name)
            {
                return false;
            }
            var parameters = method.GetParameters();
            if (parameters.Length != parameterTypes.Length)
            {
                return false;
            }
            if (method.IsGenericMethod && method.IsGenericMethodDefinition)
            {
                var closed = TryMakeGeneric(method, argumentTypes);
                if (closed == null)
                {
                    return false;
                }
                return MethodMatch(closed, name, parameterTypes, argumentTypes);
            }
            else if (!method.IsGenericMethod && (argumentTypes != null && argumentTypes.Length > 0))
            {
                return false;
            }
            for (int i = 0; i < parameters.Length; i++)
            {
                var type = parameterTypes[i];
                var parameter = parameters[i];
                if (!IsAssignableToParameterType(type, parameter)
                        && !IsExpressionOfParameter(type, parameter.ParameterType))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ParameterMatch(MethodInfo method, Type type)
        {
            return method.GetParameters()[0].ParameterType == type;
        }

        private static bool ReturnTypeMatch(MethodInfo method, Type type)
        {
            return method.ReturnType == type;
        }

        private static MethodInfo TryGetMethod(Type type, string methodName, BindingFlags flags, Type[] parameterTypes, Type[] argumentTypes)
        {
            var methods = System.Linq.Enumerable.Where
                (
                    type.GetMethods(flags),
                    input =>
                    MethodMatch(input, methodName, parameterTypes, argumentTypes)
                );
            if (Theraot.Collections.Extensions.HasAtLeast(methods, 2))
            {
                throw new InvalidOperationException("Too many method candidates");
            }
            var method = TryMakeGeneric(System.Linq.Enumerable.FirstOrDefault(methods), argumentTypes);
            if (method != null)
            {
                return method;
            }
            throw new InvalidOperationException("No such method");
        }

        private static MethodInfo TryMakeGeneric(MethodInfo method, Type[] args)
        {
            if (method == null)
            {
                return null;
            }
            if (!method.IsGenericMethod && (args == null || args.Length == 0))
            {
                return method;
            }
            if (args.Length == method.GetGenericArguments().Length)
            {
                return method.MakeGenericMethod(args);
            }
            return null;
        }

        private static MethodInfo UnaryCoreCheck(string operName, Expression expression, MethodInfo method, Func<Type, bool> validator)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (method != null)
            {
                return CheckUnaryMethod(method, expression.Type);
            }
            var type = expression.Type.GetNotNullableType();
            if (validator(type))
            {
                return null;
            }
            if (operName != null)
            {
                method = GetUnaryOperator(operName, type, expression.Type);
                if (method != null)
                {
                    return method;
                }
            }
            throw new InvalidOperationException
            (
                string.Format("Operation {0} not defined for {1}", operName != null ? operName.Substring(3) : "is", expression.Type)
            );
        }
    }
}

#endif