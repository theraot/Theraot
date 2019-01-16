#if LESSTHAN_NET45

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    ///    Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static class Error
    {
        internal static Exception AccessorsCannotHaveByRefArgs(string paramName, int index)
        {
            return new ArgumentException("Accessor indexes cannot be passed ByRef.", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception AccessorsCannotHaveVarArgs(string paramName)
        {
            return new ArgumentException("Accessor method should not have VarArgs.", paramName);
        }

        internal static Exception AllCaseBodiesMustHaveSameType(string paramName)
        {
            return new ArgumentException("All case bodies and the default body must have the same type.", paramName);
        }

        internal static Exception AllTestValuesMustHaveSameType(string paramName)
        {
            return new ArgumentException("All test values must have the same type.", paramName);
        }

        internal static Exception AmbiguousJump(object p0)
        {
            return new InvalidOperationException($"Cannot jump to ambiguous label '{p0}'.");
        }

        internal static Exception AmbiguousMatchInExpandoObject(object p0)
        {
            return new AmbiguousMatchException($"More than one key matching '{p0}' was found in the ExpandoObject.");
        }

        internal static Exception ArgCntMustBeGreaterThanNameCnt()
        {
            return new ArgumentException("Argument count must be greater than number of named arguments.");
        }

        internal static Exception ArgumentCannotBeOfTypeVoid(string paramName)
        {
            return new ArgumentException(SR.ArgumentCannotBeOfTypeVoid, paramName);
        }

        internal static Exception ArgumentMemberNotDeclOnType(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException($" The member '{p0}' is not declared on type '{p1}' being created", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentMustBeArray(string paramName)
        {
            return new ArgumentException("Argument must be array", paramName);
        }

        internal static Exception ArgumentMustBeArrayIndexType(string paramName, int index)
        {
            return new ArgumentException("Argument for array index must be of type Int32", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentMustBeBoolean(string paramName)
        {
            return new ArgumentException("Argument must be boolean", paramName);
        }

        internal static Exception ArgumentMustBeFieldInfoOrPropertyInfo(string paramName)
        {
            return new ArgumentException("Argument must be either a FieldInfo or PropertyInfo", paramName);
        }

        internal static Exception ArgumentMustBeFieldInfoOrPropertyInfoOrMethod(string paramName, int index)
        {
            return new ArgumentException("Argument must be either a FieldInfo, PropertyInfo or MethodInfo", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentMustBeInstanceMember(string paramName, int index)
        {
            return new ArgumentException("Argument must be an instance member", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentMustBeInteger(string paramName, int index)
        {
            return new ArgumentException("Argument must be of an integer type", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentMustBeSingleDimensionalArrayType(string paramName)
        {
            return new ArgumentException("Argument must be single dimensional array type", paramName);
        }

        internal static Exception ArgumentMustNotHaveValueType(string paramName)
        {
            return new ArgumentException("Argument must not have a value type.", paramName);
        }

        internal static Exception ArgumentOutOfRange(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName);
        }

        internal static Exception ArgumentTypeCannotBeVoid()
        {
            return new ArgumentException("Argument type cannot be void");
        }

        internal static Exception ArgumentTypeDoesNotMatchMember(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException($" Argument type '{p0}' does not match the corresponding member type '{p1}'", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentTypesMustMatch()
        {
            return new ArgumentException("Argument types do not match");
        }

        internal static Exception ArgumentTypesMustMatch(string paramName)
        {
            return new ArgumentException("Argument types do not match", paramName);
        }

        internal static Exception BinaryOperatorNotDefined(object p0, object p1, object p2)
        {
            return new InvalidOperationException($"The binary operator {p0} is not defined for the types '{p1}' and '{p2}'.");
        }

        internal static Exception BinderNotCompatibleWithCallSite(object p0, object p1, object p2)
        {
            return new InvalidOperationException($"The result type '{p0}' of the binder '{p1}' is not compatible with the result type '{p2}' expected by the call site.");
        }

        internal static Exception BindingCannotBeNull()
        {
            return new InvalidOperationException("Bind cannot return null.");
        }

        internal static Exception BodyOfCatchMustHaveSameTypeAsBodyOfTry()
        {
            return new ArgumentException("Body of catch must have the same type as body of try.");
        }

        internal static Exception BothAccessorsMustBeStatic(string paramName)
        {
            return new ArgumentException("Both accessors must be static.", paramName);
        }

        internal static Exception BoundsCannotBeLessThanOne(string paramName)
        {
            return new ArgumentException("Bounds count cannot be less than 1", paramName);
        }

        internal static Exception CannotAutoInitializeValueTypeElementThroughProperty(object p0)
        {
            return new InvalidOperationException($"Cannot auto initialize elements of value type through property '{p0}', use assignment instead");
        }

        internal static Exception CannotAutoInitializeValueTypeMemberThroughProperty(object p0)
        {
            return new InvalidOperationException($"Cannot auto initialize members of value type through property '{p0}', use assignment instead");
        }

        internal static Exception CannotCloseOverByRef(object p0, object p1)
        {
            return new InvalidOperationException($"Cannot close over byref parameter '{p0}' referenced in lambda '{p1}'");
        }

        internal static Exception CannotCompileConstant(object p0)
        {
            return new InvalidOperationException($"CompileToMethod cannot compile constant '{p0}' because it is a non-trivial value, such as a live object. Instead, create an expression tree that can construct this value.");
        }

        internal static Exception CannotCompileDynamic()
        {
            return new NotSupportedException("Dynamic expressions are not supported by CompileToMethod. Instead, create an expression tree that uses System.Runtime.CompilerServices.CallSite.");
        }

        internal static Exception CoalesceUsedOnNonNullType()
        {
            return new InvalidOperationException("Coalesce used with type that cannot be null");
        }

        internal static Exception CoercionOperatorNotDefined(object p0, object p1)
        {
            return new InvalidOperationException($"No coercion operator is defined between types '{p0}' and '{p1}'.");
        }

        internal static Exception CollectionModifiedWhileEnumerating()
        {
            return new InvalidOperationException("Collection was modified; enumeration operation may not execute");
        }

        internal static Exception CollectionReadOnly()
        {
            return new NotSupportedException("Collection is read-only.");
        }

        internal static Exception ControlCannotEnterExpression()
        {
            return new InvalidOperationException("Control cannot enter an expression--only statements can be jumped into.");
        }

        internal static Exception ControlCannotEnterTry()
        {
            return new InvalidOperationException("Control cannot enter a try block.");
        }

        internal static Exception ControlCannotLeaveFilterTest()
        {
            return new InvalidOperationException("Control cannot leave a filter test.");
        }

        internal static Exception ControlCannotLeaveFinally()
        {
            return new InvalidOperationException("Control cannot leave a finally block.");
        }

        internal static Exception ConversionIsNotSupportedForArithmeticTypes()
        {
            return new InvalidOperationException("Conversion is not supported for arithmetic types without operator overloading.");
        }

        internal static Exception DefaultBodyMustBeSupplied(string paramName)
        {
            return new ArgumentException("Default body must be supplied if case bodies are not System.Void.", paramName);
        }

        internal static Exception DuplicateVariable(object p0, string paramName, int index)
        {
            return new ArgumentException($"Found duplicate parameter '{p0}'. Each ParameterExpression in the list must be a unique object.", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception DynamicBinderResultNotAssignable(object p0, object p1, object p2)
        {
            return new InvalidCastException($"The result type '{p0}' of the dynamic binding produced by binder '{p1}' is not compatible with the result type '{p2}' expected by the call site.");
        }

        internal static Exception DynamicBindingNeedsRestrictions(object p0, object p1)
        {
            return new InvalidOperationException($"The result of the dynamic binding produced by the object with type '{p0}' for the binder '{p1}' needs at least one restriction.");
        }

        internal static Exception DynamicObjectResultNotAssignable(object p0, object p1, object p2, object p3)
        {
            return new InvalidCastException($"The result type '{p0}' of the dynamic binding produced by the object with type '{p1}' for the binder '{p2}' is not compatible with the result type '{p3}' expected by the call site.");
        }

        internal static Exception ElementInitializerMethodNoRefOutParam(object p0, object p1, string paramName)
        {
            return new ArgumentException($"Parameter '{p0}' of element initializer method '{p1}' must not be a pass by reference parameter", paramName);
        }

        internal static Exception ElementInitializerMethodNotAdd(string paramName)
        {
            return new ArgumentException("Element initializer method must be named 'Add'", paramName);
        }

        internal static Exception ElementInitializerMethodStatic(string paramName)
        {
            return new ArgumentException("Element initializer method must be an instance method", paramName);
        }

        internal static Exception ElementInitializerMethodWithZeroArgs(string paramName)
        {
            return new ArgumentException("Element initializer method must have at least 1 parameter", paramName);
        }

        internal static Exception EnumerationIsDone()
        {
            return new InvalidOperationException("Enumeration has either not started or has already finished.");
        }

        internal static Exception EqualityMustReturnBoolean(object p0, string paramName)
        {
            return new ArgumentException($"The user-defined equality method '{p0}' must return a boolean value.", paramName);
        }

        internal static Exception ExpressionMustBeReadable(string paramName, int index)
        {
            return new ArgumentException("Expression must be readable", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ExpressionMustBeWriteable(string paramName)
        {
            return new ArgumentException("Expression must be writeable", paramName);
        }

        internal static Exception ExpressionTypeCannotInitializeArrayType(object p0, object p1)
        {
            return new InvalidOperationException($"An expression of type '{p0}' cannot be used to initialize an array of type '{p1}'");
        }

        internal static Exception ExpressionTypeDoesNotMatchAssignment(object p0, object p1)
        {
            return new ArgumentException($"Expression of type '{p0}' cannot be used for assignment to type '{p1}'");
        }

        internal static Exception ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException($"Expression of type '{p0}' cannot be used for constructor parameter of type '{p1}'", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ExpressionTypeDoesNotMatchLabel(object p0, object p1)
        {
            return new ArgumentException($"Expression of type '{p0}' cannot be used for label of type '{p1}'");
        }

        internal static Exception ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2, string paramName, int index)
        {
            return new ArgumentException($"Expression of type '{p0}' cannot be used for parameter of type '{p1}' of method '{p2}'", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ExpressionTypeDoesNotMatchParameter(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException($"Expression of type '{p0}' cannot be used for parameter of type '{p1}'", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ExpressionTypeDoesNotMatchReturn(object p0, object p1)
        {
            return new ArgumentException($"Expression of type '{p0}' cannot be used for return type '{p1}'");
        }

        internal static Exception ExpressionTypeNotInvocable(object p0, string paramName)
        {
            return new ArgumentException($"Expression of type '{p0}' cannot be invoked", paramName);
        }

        internal static Exception ExtensionNodeMustOverrideProperty(object p0)
        {
            return new InvalidOperationException($"Extension node must override the property {p0}.");
        }

        internal static Exception FaultCannotHaveCatchOrFinally(string paramName)
        {
            return new ArgumentException("fault cannot be used with catch or finally clauses", paramName);
        }

        internal static Exception FieldInfoNotDefinedForType(object p0, object p1, object p2)
        {
            return new ArgumentException($"Field '{p0}.{p1}' is not defined for type '{p2}'");
        }

        internal static Exception FieldNotDefinedForType(object p0, object p1)
        {
            return new ArgumentException($"Field '{p0}' is not defined for type '{p1}'");
        }

        internal static Exception FirstArgumentMustBeCallSite()
        {
            return new ArgumentException("First argument of delegate must be CallSite");
        }

        internal static Exception GenericMethodWithArgsDoesNotExistOnType(object p0, object p1)
        {
            return new InvalidOperationException($"No generic method '{p0}' on type '{p1}' is compatible with the supplied type arguments and arguments. No type arguments should be provided if the method is non-generic. ");
        }

        internal static Exception IncorrectNumberOfArgumentsForMembers()
        {
            return new ArgumentException("Incorrect number of arguments for the given members ");
        }

        internal static Exception IncorrectNumberOfConstructorArguments()
        {
            return new ArgumentException("Incorrect number of arguments for constructor");
        }

        internal static Exception IncorrectNumberOfIndexes()
        {
            return new ArgumentException("Incorrect number of indexes");
        }

        internal static Exception IncorrectNumberOfLambdaArguments()
        {
            return new InvalidOperationException("Incorrect number of arguments supplied for lambda invocation");
        }

        internal static Exception IncorrectNumberOfLambdaDeclarationParameters()
        {
            return new ArgumentException("Incorrect number of parameters supplied for lambda declaration");
        }

        internal static Exception IncorrectNumberOfMembersForGivenConstructor()
        {
            return new ArgumentException("Incorrect number of members for constructor");
        }

        internal static Exception IncorrectNumberOfMethodCallArguments(object p0, string paramName)
        {
            return new ArgumentException($"Incorrect number of arguments supplied for call to method '{p0}'", paramName);
        }

        internal static Exception IncorrectNumberOfTypeArgsForAction(string paramName)
        {
            return new ArgumentException("An incorrect number of type args were specified for the declaration of an Action type.", paramName);
        }

        internal static Exception IncorrectNumberOfTypeArgsForFunc(string paramName)
        {
            return new ArgumentException("An incorrect number of type args were specified for the declaration of a Func type.", paramName);
        }

        internal static Exception IncorrectTypeForTypeAs(object p0, string paramName)
        {
            return new ArgumentException($"The type used in TypeAs Expression must be of reference or nullable type, {p0} is neither", paramName);
        }

        internal static Exception IndexesOfSetGetMustMatch(string paramName)
        {
            return new ArgumentException("Indexing parameters of getter and setter must match.", paramName);
        }

        internal static Exception InstanceAndMethodTypeMismatch(object p0, object p1, object p2)
        {
            return new ArgumentException($"Method '{p0}' declared on type '{p1}' cannot be called with instance of type '{p2}'");
        }

        internal static Exception InstanceFieldNotDefinedForType(object p0, object p1)
        {
            return new ArgumentException($"Instance field '{p0}' is not defined for type '{p1}'");
        }

        internal static Exception InstancePropertyNotDefinedForType(object p0, object p1, string paramName)
        {
            return new ArgumentException($"Instance property '{p0}' is not defined for type '{p1}'", paramName);
        }

        internal static Exception InstancePropertyWithoutParameterNotDefinedForType(object p0, object p1)
        {
            return new ArgumentException($"Instance property '{p0}' that takes no argument is not defined for type '{p1}'");
        }

        internal static Exception InstancePropertyWithSpecifiedParametersNotDefinedForType(object p0, object p1, object p2, string paramName)
        {
            return new ArgumentException($"Instance property '{p0}{p1}' is not defined for type '{p2}'", paramName);
        }

        internal static Exception InvalidArgumentValue(string paramName)
        {
            return new ArgumentException("Invalid argument value", paramName);
        }

        internal static Exception InvalidLvalue(ExpressionType p0)
        {
            return new InvalidOperationException($"Invalid lvalue for assignment: {p0}.");
        }

        internal static Exception InvalidMetaObjectCreated(object p0)
        {
            return new InvalidOperationException($"An IDynamicMetaObjectProvider {p0} created an invalid DynamicMetaObject instance.");
        }

        internal static Exception InvalidNullValue(Type type, string paramName)
        {
            return new ArgumentException($"The value null is not of type '{type}' and cannot be used in this collection.", paramName);
        }

        internal static Exception InvalidProgram()
        {
            return new InvalidProgramException();
        }

        internal static Exception InvalidTypeException(object value, Type type, string paramName)
        {
            return new ArgumentException($"The value '{value?.GetType() as object ?? "null"}' is not of type '{type}' and cannot be used in this collection.", paramName);
        }

        internal static Exception InvalidUnboxType(string paramName)
        {
            return new ArgumentException("Can only unbox from an object or interface type to a value type.", paramName);
        }

        internal static Exception KeyDoesNotExistInExpando(object p0)
        {
            return new KeyNotFoundException($"The specified key '{p0}' does not exist in the ExpandoObject.");
        }

        internal static Exception LabelMustBeVoidOrHaveExpression(string paramName)
        {
            return new ArgumentException("Label type must be System.Void if an expression is not supplied", paramName);
        }

        internal static Exception LabelTargetAlreadyDefined(object p0)
        {
            return new InvalidOperationException($"Cannot redefine label '{p0}' in an inner block.");
        }

        internal static Exception LabelTargetUndefined(object p0)
        {
            return new InvalidOperationException($"Cannot jump to undefined label '{p0}'.");
        }

        internal static Exception LabelTypeMustBeVoid(string paramName)
        {
            return new ArgumentException("Type must be System.Void for this label argument", paramName);
        }

        internal static Exception LambdaTypeMustBeDerivedFromSystemDelegate(string paramName)
        {
            return new ArgumentException("Lambda type parameter must be derived from System.Delegate", paramName);
        }

        internal static Exception LogicalOperatorMustHaveBooleanOperators(object p0, object p1)
        {
            return new ArgumentException($"The user-defined operator method '{p1}' for operator '{p0}' must have associated boolean True and False operators.");
        }

        internal static Exception MemberNotFieldOrProperty(object p0, string paramName)
        {
            return new ArgumentException($"Member '{p0}' not field or property", paramName);
        }

        internal static Exception MethodBuilderDoesNotHaveTypeBuilder()
        {
            return new ArgumentException("MethodBuilder does not have a valid TypeBuilder");
        }

        internal static Exception MethodContainsGenericParameters(object p0, string paramName)
        {
            return new ArgumentException($"Method {p0} contains generic parameters", paramName);
        }

        internal static Exception MethodIsGeneric(object p0, string paramName)
        {
            return new ArgumentException($"Method {p0} is a generic method definition", paramName);
        }

        internal static Exception MethodNotPropertyAccessor(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException($"The method '{p0}.{p1}' is not a property accessor", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception MethodWithArgsDoesNotExistOnType(object p0, object p1)
        {
            return new InvalidOperationException($"No method '{p0}' on type '{p1}' is compatible with the supplied arguments.");
        }

        internal static Exception MethodWithMoreThanOneMatch(object p0, object p1)
        {
            return new InvalidOperationException($"More than one method '{p0}' on type '{p1}' is compatible with the supplied arguments.");
        }

        internal static Exception MustBeReducible()
        {
            return new ArgumentException("must be reducible node");
        }

        internal static Exception MustReduceToDifferent()
        {
            return new ArgumentException("node cannot reduce to itself or null");
        }

        internal static Exception MustRewriteChildToSameType(object p0, object p1, object p2)
        {
            return new InvalidOperationException($"Rewriting child expression from type '{p0}' to type '{p1}' is not allowed, because it would change the meaning of the operation. If this is intentional, override '{p2}' and change it to allow this rewrite.");
        }

        internal static Exception MustRewriteToSameNode(object p0, object p1, object p2)
        {
            return new InvalidOperationException($"When called from '{p0}', rewriting a node of type '{p1}' must return a non-null value of the same type. Alternatively, override '{p2}' and change it to not visit children of this type.");
        }

        internal static Exception MustRewriteWithoutMethod(object p0, object p1)
        {
            return new InvalidOperationException($"Rewritten expression calls operator method '{p0}', but the original node had no operator method. If this is is intentional, override '{p1}' and change it to allow this rewrite.");
        }

        internal static Exception NonAbstractConstructorRequired()
        {
            return new InvalidOperationException("Can't compile a NewExpression with a constructor declared on an abstract class");
        }

        internal static Exception NonEmptyCollectionRequired(string paramName)
        {
            return new ArgumentException("Non-empty collection required", paramName);
        }

        internal static Exception NonLocalJumpWithValue(object p0)
        {
            return new InvalidOperationException($"Cannot jump to non-local label '{p0}' with a value. Only jumps to labels defined in outer blocks can pass values.");
        }

        internal static Exception NonStaticConstructorRequired(string paramName)
        {
            return new ArgumentException("The constructor should not be static", paramName);
        }

        internal static Exception NoOrInvalidRuleProduced()
        {
            return new InvalidOperationException("No or Invalid rule produced");
        }

        internal static Exception NotAMemberOfAnyType(object p0, string paramName)
        {
            return new ArgumentException($"'{p0}' is not a member of any type", paramName);
        }

        internal static Exception NotAMemberOfType(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException($"{p0}' is not a member of type '{p1}'", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception NotSupported()
        {
            return new NotSupportedException();
        }

        internal static Exception OnlyStaticFieldsHaveNullInstance(string paramName)
        {
            return new ArgumentException("Static field requires null instance, non-static field requires non-null instance.", paramName);
        }

        internal static Exception OnlyStaticMethodsHaveNullInstance()
        {
            return new ArgumentException("Static method requires null instance, non-static method requires non-null instance.");
        }

        internal static Exception OnlyStaticPropertiesHaveNullInstance(string paramName)
        {
            return new ArgumentException("Static property requires null instance, non-static property requires non-null instance.", paramName);
        }

        internal static Exception OperandTypesDoNotMatchParameters(object p0, object p1)
        {
            return new InvalidOperationException($"The operands for operator '{p0}' do not match the parameters of method '{p1}'.");
        }

        internal static Exception OutOfRange(string paramName, object p1)
        {
            return new ArgumentOutOfRangeException(paramName, $"{paramName} must be greater than or equal to {p1}");
        }

        internal static Exception OverloadOperatorTypeDoesNotMatchConversionType(object p0, object p1)
        {
            return new InvalidOperationException($"The return type of overload method for operator '{p0}' does not match the parameter type of conversion method '{p1}'.");
        }

        internal static Exception ParameterExpressionNotValidAsDelegate(object p0, object p1)
        {
            return new ArgumentException($"ParameterExpression of type '{p0}' cannot be used for delegate parameter of type '{p1}'");
        }

        internal static Exception PdbGeneratorNeedsExpressionCompiler()
        {
            return new NotSupportedException("DebugInfoGenerator created by CreatePdbGenerator can only be used with LambdaExpression.CompileToMethod.");
        }

        internal static Exception PropertyCannotHaveRefType(string paramName)
        {
            return new ArgumentException("Property cannot have a managed pointer type.", paramName);
        }

        internal static Exception PropertyDoesNotHaveAccessor(object p0, string paramName)
        {
            return new ArgumentException($"The property '{p0}' has no 'get' or 'set' accessors", paramName);
        }

        internal static Exception PropertyDoesNotHaveGetter(object p0, string paramName, int index)
        {
            return new ArgumentException($"The property '{p0}' has no 'get' accessor", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception PropertyDoesNotHaveSetter(object p0, string paramName)
        {
            return new ArgumentException($"The property '{p0}' has no 'set' accessor", paramName);
        }

        internal static Exception PropertyNotDefinedForType(object p0, object p1, string paramName)
        {
            return new ArgumentException($"Property '{p0}' is not defined for type '{p1}'", paramName);
        }

        internal static Exception PropertyTypeCannotBeVoid(string paramName)
        {
            return new ArgumentException("Property cannot have a void type.", paramName);
        }

        internal static Exception PropertyTypeMustMatchGetter(string paramName)
        {
            return new ArgumentException("Property type must match the value type of getter", paramName);
        }

        internal static Exception PropertyTypeMustMatchSetter(string paramName)
        {
            return new ArgumentException("Property type must match the value type of setter", paramName);
        }

        internal static Exception PropertyWithMoreThanOneMatch(object p0, object p1)
        {
            return new InvalidOperationException($"More than one property '{p0}' on type '{p1}' is compatible with the supplied arguments.");
        }

        internal static Exception QuotedExpressionMustBeLambda(string paramName)
        {
            return new ArgumentException("Quoted expression must be a lambda", paramName);
        }

        internal static Exception ReducedNotCompatible()
        {
            return new ArgumentException("cannot assign from the reduced node type to the original node type");
        }

        internal static Exception ReducibleMustOverrideReduce()
        {
            return new ArgumentException("reducible nodes must override Expression.Reduce()");
        }

        internal static Exception ReferenceEqualityNotDefined(object p0, object p1)
        {
            return new InvalidOperationException($"Reference equality is not defined for the types '{p0}' and '{p1}'.");
        }

        internal static Exception RethrowRequiresCatch()
        {
            return new InvalidOperationException("Rethrow statement is valid only inside a Catch block.");
        }

        internal static Exception SameKeyExistsInExpando(object key)
        {
            return new ArgumentException($"An element with the same key '{key}' already exists in the ExpandoObject.", nameof(key));
        }

        internal static Exception SetterHasNoParams(string paramName)
        {
            return new ArgumentException("Setter must have parameters.", paramName);
        }

        internal static Exception SetterMustBeVoid(string paramName)
        {
            return new ArgumentException("Setter should have void type.", paramName);
        }

        internal static Exception StartEndMustBeOrdered()
        {
            return new ArgumentException("Start and End must be well ordered");
        }

        internal static Exception SwitchValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
        {
            return new ArgumentException($"Switch value of type '{p0}' cannot be used for the comparison method parameter of type '{p1}'");
        }

        internal static Exception TestValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
        {
            return new ArgumentException($"Test value of type '{p0}' cannot be used for the comparison method parameter of type '{p1}'");
        }

        internal static Exception TryMustHaveCatchFinallyOrFault()
        {
            return new ArgumentException("try must have at least one catch, finally, or fault clause");
        }

        internal static Exception TryNotAllowedInFilter()
        {
            return new InvalidOperationException("Try expression is not allowed inside a filter body.");
        }

        internal static Exception TryNotSupportedForMethodsWithRefArgs(object p0)
        {
            return new NotSupportedException($"TryExpression is not supported as an argument to method '{p0}' because it has an argument with by-ref type. Construct the tree so the TryExpression is not nested inside of this expression.");
        }

        internal static Exception TryNotSupportedForValueTypeInstances(object p0)
        {
            return new NotSupportedException($"TryExpression is not supported as a child expression when accessing a member on type '{p0}' because it is a value type. Construct the tree so the TryExpression is not nested inside of this expression.");
        }

        internal static Exception TypeContainsGenericParameters(object p0, string paramName, int index)
        {
            return new ArgumentException($"Type {p0} contains generic parameters", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception TypeIsGeneric(object p0, string paramName, int index)
        {
            return new ArgumentException($"Type {p0} is a generic type definition", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception TypeMissingDefaultConstructor(object p0, string paramName)
        {
            return new ArgumentException($"Type '{p0}' does not have a default constructor", paramName);
        }

        internal static Exception TypeMustBeDerivedFromSystemDelegate()
        {
            return new ArgumentException("Type must be derived from System.Delegate");
        }

        internal static Exception TypeMustNotBeByRef(string paramName)
        {
            return new ArgumentException("type must not be ByRef", paramName);
        }

        internal static Exception TypeMustNotBePointer(string paramName)
        {
            return new ArgumentException("Type must not be a pointer type", paramName);
        }

        internal static Exception TypeNotIEnumerable(object p0, string paramName)
        {
            return new ArgumentException($"Type '{p0}' is not IEnumerable", paramName);
        }

        internal static Exception TypeParameterIsNotDelegate(object p0)
        {
            return new InvalidOperationException($"Type parameter is {p0}. Expected a delegate.");
        }

        internal static Exception UnaryOperatorNotDefined(object p0, object p1)
        {
            return new InvalidOperationException($"The unary operator {p0} is not defined for the type '{p1}'.");
        }

        internal static Exception UndefinedVariable(object p0, object p1, object p2)
        {
            return new InvalidOperationException(
                $"variable '{p0}' of type '{p1}' referenced from scope '{p2}', but it is not defined");
        }

        internal static Exception UnexpectedVarArgsCall(object p0)
        {
            return new InvalidOperationException($"Unexpected VarArgs call to method '{p0}'");
        }

        internal static Exception UnhandledBinary(object p0, string paramName)
        {
            return new ArgumentException($"Unhandled binary: {p0}", paramName);
        }

        internal static Exception UnhandledBinding()
        {
            return new ArgumentException("Unhandled binding ");
        }

        internal static Exception UnhandledBindingType(object p0)
        {
            return new ArgumentException($"Unhandled Binding Type: {p0}");
        }

        internal static Exception UnhandledUnary(object p0, string paramName)
        {
            return new ArgumentException($"Unhandled unary: {p0}", paramName);
        }

        internal static Exception UnknownBindingType(int index)
        {
            return new ArgumentException(SR.UnknownBindingType, $"bindings[{index}]");
        }

        internal static Exception UserDefinedOperatorMustBeStatic(object p0, string paramName)
        {
            return new ArgumentException($"User-defined operator method '{p0}' must be static.", paramName);
        }

        internal static Exception UserDefinedOperatorMustNotBeVoid(object p0, string paramName)
        {
            return new ArgumentException($"User-defined operator method '{p0}' must not be void.", paramName);
        }

        internal static Exception UserDefinedOpMustHaveConsistentTypes(object p0, object p1)
        {
            return new ArgumentException($"The user-defined operator method '{p1}' for operator '{p0}' must have identical parameter and return types.");
        }

        internal static Exception UserDefinedOpMustHaveValidReturnType(object p0, object p1)
        {
            return new ArgumentException($"The user-defined operator method '{p1}' for operator '{p0}' must return the same type as its parameter or a derived type.");
        }

        internal static Exception VariableMustNotBeByRef(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException($"Variable '{p0}' uses unsupported type '{p1}'. Reference types are not supported for variables.", index >= 0 ? $"{paramName}[{index}]" : paramName);
        }
    }
}

#endif