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
            return new ArgumentException(SR.AccessorsCannotHaveByRefArgs, index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception AccessorsCannotHaveVarArgs(string paramName)
        {
            return new ArgumentException(SR.AccessorsCannotHaveVarArgs, paramName);
        }

        internal static Exception AllCaseBodiesMustHaveSameType(string paramName)
        {
            return new ArgumentException(SR.AllCaseBodiesMustHaveSameType, paramName);
        }

        internal static Exception AllTestValuesMustHaveSameType(string paramName)
        {
            return new ArgumentException(SR.AllTestValuesMustHaveSameType, paramName);
        }

        internal static Exception AmbiguousJump(object p0)
        {
            return new InvalidOperationException(SR.Format(SR.AmbiguousJump, p0));
        }

        internal static Exception AmbiguousMatchInExpandoObject(object p0)
        {
            return new AmbiguousMatchException(SR.Format("More than one key matching '{0}' was found in the ExpandoObject.", p0));
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
            return new ArgumentException(SR.Format(SR.ArgumentMemberNotDeclOnType, p0, p1), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentMustBeArray(string paramName)
        {
            return new ArgumentException(SR.ArgumentMustBeArray, paramName);
        }

        internal static Exception ArgumentMustBeArrayIndexType(string paramName, int index)
        {
            return new ArgumentException(SR.ArgumentMustBeArrayIndexType, index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentMustBeBoolean(string paramName)
        {
            return new ArgumentException(SR.ArgumentMustBeBoolean, paramName);
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
            return new ArgumentException(SR.ArgumentMustBeInstanceMember, index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentMustBeInteger(string paramName, int index)
        {
            return new ArgumentException(SR.ArgumentMustBeInteger, index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentMustBeSingleDimensionalArrayType(string paramName)
        {
            return new ArgumentException(SR.ArgumentMustBeSingleDimensionalArrayType, paramName);
        }

        internal static Exception ArgumentMustNotHaveValueType(string paramName)
        {
            return new ArgumentException(SR.ArgumentMustNotHaveValueType, paramName);
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
            return new ArgumentException(SR.Format(SR.ArgumentTypeDoesNotMatchMember, p0, p1), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ArgumentTypesMustMatch()
        {
            return new ArgumentException(SR.ArgumentTypesMustMatch);
        }

        internal static Exception ArgumentTypesMustMatch(string paramName)
        {
            return new ArgumentException(SR.ArgumentTypesMustMatch, paramName);
        }

        internal static Exception BinaryOperatorNotDefined(object p0, object p1, object p2)
        {
            return new InvalidOperationException(SR.Format(SR.BinaryOperatorNotDefined, p0, p1, p2));
        }

        internal static Exception BinderNotCompatibleWithCallSite(object p0, object p1, object p2)
        {
            return new InvalidOperationException(SR.Format("The result type '{0}' of the binder '{1}' is not compatible with the result type '{2}' expected by the call site.", p0, p1, p2));
        }

        internal static Exception BindingCannotBeNull()
        {
            return new InvalidOperationException("Bind cannot return null.");
        }

        internal static Exception BodyOfCatchMustHaveSameTypeAsBodyOfTry()
        {
            return new ArgumentException(SR.BodyOfCatchMustHaveSameTypeAsBodyOfTry);
        }

        internal static Exception BothAccessorsMustBeStatic(string paramName)
        {
            return new ArgumentException(SR.BothAccessorsMustBeStatic, paramName);
        }

        internal static Exception BoundsCannotBeLessThanOne(string paramName)
        {
            return new ArgumentException(SR.BoundsCannotBeLessThanOne, paramName);
        }

        internal static Exception CannotAutoInitializeValueTypeElementThroughProperty(object p0)
        {
            return new InvalidOperationException(SR.Format(SR.CannotAutoInitializeValueTypeElementThroughProperty, p0));
        }

        internal static Exception CannotAutoInitializeValueTypeMemberThroughProperty(object p0)
        {
            return new InvalidOperationException(SR.Format(SR.CannotAutoInitializeValueTypeMemberThroughProperty, p0));
        }

        internal static Exception CannotCloseOverByRef(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format(SR.CannotCloseOverByRef, p0, p1));
        }

        internal static Exception CannotCompileConstant(object p0)
        {
            return new InvalidOperationException(SR.Format(SR.CannotCompileConstant, p0));
        }

        internal static Exception CannotCompileDynamic()
        {
            return new NotSupportedException(SR.CannotCompileDynamic);
        }

        internal static Exception CoalesceUsedOnNonNullType()
        {
            return new InvalidOperationException(SR.CoalesceUsedOnNonNullType);
        }

        internal static Exception CoercionOperatorNotDefined(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format(SR.CoercionOperatorNotDefined, p0, p1));
        }

        internal static Exception CollectionModifiedWhileEnumerating()
        {
            return new InvalidOperationException(SR.CollectionModifiedWhileEnumerating);
        }

        internal static Exception CollectionReadOnly()
        {
            return new NotSupportedException("Collection is read-only.");
        }

        internal static Exception ControlCannotEnterExpression()
        {
            return new InvalidOperationException(SR.ControlCannotEnterExpression);
        }

        internal static Exception ControlCannotEnterTry()
        {
            return new InvalidOperationException(SR.ControlCannotEnterTry);
        }

        internal static Exception ControlCannotLeaveFilterTest()
        {
            return new InvalidOperationException(SR.ControlCannotLeaveFilterTest);
        }

        internal static Exception ControlCannotLeaveFinally()
        {
            return new InvalidOperationException(SR.ControlCannotLeaveFinally);
        }

        internal static Exception ConversionIsNotSupportedForArithmeticTypes()
        {
            return new InvalidOperationException(SR.ConversionIsNotSupportedForArithmeticTypes);
        }

        internal static Exception DefaultBodyMustBeSupplied(string paramName)
        {
            return new ArgumentException(SR.DefaultBodyMustBeSupplied, paramName);
        }

        internal static Exception DuplicateVariable(object p0, string paramName, int index)
        {
            return new ArgumentException(SR.Format(SR.DuplicateVariable, p0), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception DynamicBinderResultNotAssignable(object p0, object p1, object p2)
        {
            return new InvalidCastException(SR.Format("The result type '{0}' of the dynamic binding produced by binder '{1}' is not compatible with the result type '{2}' expected by the call site.", p0, p1, p2));
        }

        internal static Exception DynamicBindingNeedsRestrictions(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format("The result of the dynamic binding produced by the object with type '{0}' for the binder '{1}' needs at least one restriction.", p0, p1));
        }

        internal static Exception DynamicObjectResultNotAssignable(object p0, object p1, object p2, object p3)
        {
            return new InvalidCastException(SR.Format("The result type '{0}' of the dynamic binding produced by the object with type '{1}' for the binder '{2}' is not compatible with the result type '{3}' expected by the call site.", p0, p1, p2, p3));
        }

        internal static Exception ElementInitializerMethodNoRefOutParam(object p0, object p1, string paramName)
        {
            return new ArgumentException(SR.Format(SR.ElementInitializerMethodNoRefOutParam, p0, p1), paramName);
        }

        internal static Exception ElementInitializerMethodNotAdd(string paramName)
        {
            return new ArgumentException(SR.ElementInitializerMethodNotAdd, paramName);
        }

        internal static Exception ElementInitializerMethodStatic(string paramName)
        {
            return new ArgumentException(SR.ElementInitializerMethodStatic, paramName);
        }

        internal static Exception ElementInitializerMethodWithZeroArgs(string paramName)
        {
            return new ArgumentException(SR.ElementInitializerMethodWithZeroArgs, paramName);
        }

        internal static Exception EnumerationIsDone()
        {
            return new InvalidOperationException(SR.EnumerationIsDone);
        }

        internal static Exception EqualityMustReturnBoolean(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.EqualityMustReturnBoolean, p0), paramName);
        }

        internal static Exception ExpressionMustBeReadable(string paramName, int index)
        {
            return new ArgumentException(SR.ExpressionMustBeReadable, index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ExpressionMustBeWriteable(string paramName)
        {
            return new ArgumentException(SR.ExpressionMustBeWriteable, paramName);
        }

        internal static Exception ExpressionTypeCannotInitializeArrayType(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format(SR.ExpressionTypeCannotInitializeArrayType, p0, p1));
        }

        internal static Exception ExpressionTypeDoesNotMatchAssignment(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.ExpressionTypeDoesNotMatchAssignment, p0, p1));
        }

        internal static Exception ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException(SR.Format(SR.ExpressionTypeDoesNotMatchConstructorParameter, p0, p1), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ExpressionTypeDoesNotMatchLabel(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.ExpressionTypeDoesNotMatchLabel, p0, p1));
        }

        internal static Exception ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2, string paramName, int index)
        {
            return new ArgumentException(SR.Format(SR.ExpressionTypeDoesNotMatchMethodParameter, p0, p1, p2), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ExpressionTypeDoesNotMatchParameter(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException(SR.Format(SR.ExpressionTypeDoesNotMatchParameter, p0, p1), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception ExpressionTypeDoesNotMatchReturn(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.ExpressionTypeDoesNotMatchReturn, p0, p1));
        }

        internal static Exception ExpressionTypeNotInvocable(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.ExpressionTypeNotInvocable, p0), paramName);
        }

        internal static Exception ExtensionNodeMustOverrideProperty(object p0)
        {
            return new InvalidOperationException(SR.Format(SR.ExtensionNodeMustOverrideProperty, p0));
        }

        internal static Exception FaultCannotHaveCatchOrFinally(string paramName)
        {
            return new ArgumentException(SR.FaultCannotHaveCatchOrFinally, paramName);
        }

        internal static Exception FieldInfoNotDefinedForType(object p0, object p1, object p2)
        {
            return new ArgumentException(SR.Format(SR.FieldInfoNotDefinedForType, p0, p1, p2));
        }

        internal static Exception FieldNotDefinedForType(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.FieldNotDefinedForType, p0, p1));
        }

        internal static Exception FirstArgumentMustBeCallSite()
        {
            return new ArgumentException("First argument of delegate must be CallSite");
        }

        internal static Exception GenericMethodWithArgsDoesNotExistOnType(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format(SR.GenericMethodWithArgsDoesNotExistOnType, p0, p1));
        }

        internal static Exception IncorrectNumberOfArgumentsForMembers()
        {
            return new ArgumentException(SR.IncorrectNumberOfArgumentsForMembers);
        }

        internal static Exception IncorrectNumberOfConstructorArguments()
        {
            return new ArgumentException(SR.IncorrectNumberOfConstructorArguments);
        }

        internal static Exception IncorrectNumberOfIndexes()
        {
            return new ArgumentException(SR.IncorrectNumberOfIndexes);
        }

        internal static Exception IncorrectNumberOfLambdaArguments()
        {
            return new InvalidOperationException(SR.IncorrectNumberOfLambdaArguments);
        }

        internal static Exception IncorrectNumberOfLambdaDeclarationParameters()
        {
            return new ArgumentException(SR.IncorrectNumberOfLambdaDeclarationParameters);
        }

        internal static Exception IncorrectNumberOfMembersForGivenConstructor()
        {
            return new ArgumentException(SR.IncorrectNumberOfMembersForGivenConstructor);
        }

        internal static Exception IncorrectNumberOfMethodCallArguments(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.IncorrectNumberOfMethodCallArguments, p0), paramName);
        }

        internal static Exception IncorrectNumberOfTypeArgsForAction(string paramName)
        {
            return new ArgumentException(SR.IncorrectNumberOfTypeArgsForAction, paramName);
        }

        internal static Exception IncorrectNumberOfTypeArgsForFunc(string paramName)
        {
            return new ArgumentException(SR.IncorrectNumberOfTypeArgsForFunc, paramName);
        }

        internal static Exception IncorrectTypeForTypeAs(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.IncorrectTypeForTypeAs, p0), paramName);
        }

        internal static Exception IndexesOfSetGetMustMatch(string paramName)
        {
            return new ArgumentException(SR.IndexesOfSetGetMustMatch, paramName);
        }

        internal static Exception InstanceAndMethodTypeMismatch(object p0, object p1, object p2)
        {
            return new ArgumentException(SR.Format(SR.InstanceAndMethodTypeMismatch, p0, p1, p2));
        }

        internal static Exception InstanceFieldNotDefinedForType(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.InstanceFieldNotDefinedForType, p0, p1));
        }

        internal static Exception InstancePropertyNotDefinedForType(object p0, object p1, string paramName)
        {
            return new ArgumentException(SR.Format(SR.InstancePropertyNotDefinedForType, p0, p1), paramName);
        }

        internal static Exception InstancePropertyWithoutParameterNotDefinedForType(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.InstancePropertyWithoutParameterNotDefinedForType, p0, p1));
        }

        internal static Exception InstancePropertyWithSpecifiedParametersNotDefinedForType(object p0, object p1, object p2, string paramName)
        {
            return new ArgumentException(SR.Format(SR.InstancePropertyWithSpecifiedParametersNotDefinedForType, p0, p1, p2), paramName);
        }

        internal static Exception InvalidArgumentValue(string paramName)
        {
            return new ArgumentException("Invalid argument value", paramName);
        }

        internal static Exception InvalidLvalue(ExpressionType p0)
        {
            return new InvalidOperationException(SR.Format(SR.InvalidLvalue, p0));
        }

        internal static Exception InvalidMetaObjectCreated(object p0)
        {
            return new InvalidOperationException(SR.Format("An IDynamicMetaObjectProvider {0} created an invalid DynamicMetaObject instance.", p0));
        }

        internal static Exception InvalidNullValue(Type type, string paramName)
        {
            return new ArgumentException(SR.Format(SR.InvalidNullValue, type), paramName);
        }

        internal static Exception InvalidProgram()
        {
            return new InvalidProgramException();
        }

        internal static Exception InvalidTypeException(object value, Type type, string paramName)
        {
            return new ArgumentException(SR.Format(SR.InvalidObjectType, value?.GetType() as object ?? "null", type), paramName);
        }

        internal static Exception InvalidUnboxType(string paramName)
        {
            return new ArgumentException(SR.InvalidUnboxType, paramName);
        }

        internal static Exception KeyDoesNotExistInExpando(object p0)
        {
            return new KeyNotFoundException(SR.Format("The specified key '{0}' does not exist in the ExpandoObject.", p0));
        }

        internal static Exception LabelMustBeVoidOrHaveExpression(string paramName)
        {
            return new ArgumentException(SR.LabelMustBeVoidOrHaveExpression, paramName);
        }

        internal static Exception LabelTargetAlreadyDefined(object p0)
        {
            return new InvalidOperationException(SR.Format(SR.LabelTargetAlreadyDefined, p0));
        }

        internal static Exception LabelTargetUndefined(object p0)
        {
            return new InvalidOperationException(SR.Format(SR.LabelTargetUndefined, p0));
        }

        internal static Exception LabelTypeMustBeVoid(string paramName)
        {
            return new ArgumentException(SR.LabelTypeMustBeVoid, paramName);
        }

        internal static Exception LambdaTypeMustBeDerivedFromSystemDelegate(string paramName)
        {
            return new ArgumentException(SR.LambdaTypeMustBeDerivedFromSystemDelegate, paramName);
        }

        internal static Exception LogicalOperatorMustHaveBooleanOperators(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.LogicalOperatorMustHaveBooleanOperators, p0, p1));
        }

        internal static Exception MemberNotFieldOrProperty(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.MemberNotFieldOrProperty, p0), paramName);
        }

        internal static Exception MethodBuilderDoesNotHaveTypeBuilder()
        {
            return new ArgumentException(SR.MethodBuilderDoesNotHaveTypeBuilder);
        }

        internal static Exception MethodContainsGenericParameters(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.MethodContainsGenericParameters, p0), paramName);
        }

        internal static Exception MethodIsGeneric(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.MethodIsGeneric, p0), paramName);
        }

        internal static Exception MethodNotPropertyAccessor(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException(SR.Format(SR.MethodNotPropertyAccessor, p0, p1), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception MethodWithArgsDoesNotExistOnType(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format(SR.MethodWithArgsDoesNotExistOnType, p0, p1));
        }

        internal static Exception MethodWithMoreThanOneMatch(object p0, object p1)
        {
            return new InvalidOperationException(Strings.MethodWithMoreThanOneMatch(p0, p1));
        }

        internal static Exception MustBeReducible()
        {
            return new ArgumentException(SR.MustBeReducible);
        }

        internal static Exception MustReduceToDifferent()
        {
            return new ArgumentException(SR.MustReduceToDifferent);
        }

        internal static Exception MustRewriteChildToSameType(object p0, object p1, object p2)
        {
            return new InvalidOperationException(SR.Format(SR.MustRewriteChildToSameType, p0, p1, p2));
        }

        internal static Exception MustRewriteToSameNode(object p0, object p1, object p2)
        {
            return new InvalidOperationException(SR.Format(SR.MustRewriteToSameNode, p0, p1, p2));
        }

        internal static Exception MustRewriteWithoutMethod(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format(SR.MustRewriteWithoutMethod, p0, p1));
        }

        internal static Exception NonAbstractConstructorRequired()
        {
            return new InvalidOperationException("Can't compile a NewExpression with a constructor declared on an abstract class");
        }

        internal static Exception NonEmptyCollectionRequired(string paramName)
        {
            return new ArgumentException(SR.NonEmptyCollectionRequired, paramName);
        }

        internal static Exception NonLocalJumpWithValue(object p0)
        {
            return new InvalidOperationException(SR.Format(SR.NonLocalJumpWithValue, p0));
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
            return new ArgumentException(SR.Format("'{0}' is not a member of any type", p0), paramName);
        }

        internal static Exception NotAMemberOfType(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException(SR.Format(SR.NotAMemberOfType, p0, p1), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception NotSupported()
        {
            return new NotSupportedException();
        }

        internal static Exception OnlyStaticFieldsHaveNullInstance(string paramName)
        {
            return new ArgumentException(SR.OnlyStaticFieldsHaveNullInstance, paramName);
        }

        internal static Exception OnlyStaticMethodsHaveNullInstance()
        {
            return new ArgumentException(SR.OnlyStaticMethodsHaveNullInstance);
        }

        internal static Exception OnlyStaticPropertiesHaveNullInstance(string paramName)
        {
            return new ArgumentException(SR.OnlyStaticPropertiesHaveNullInstance, paramName);
        }

        internal static Exception OperandTypesDoNotMatchParameters(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format(SR.OperandTypesDoNotMatchParameters, p0, p1));
        }

        internal static Exception OutOfRange(string paramName, object p1)
        {
            return new ArgumentOutOfRangeException(paramName, Strings.OutOfRange(paramName, p1));
        }

        internal static Exception OverloadOperatorTypeDoesNotMatchConversionType(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format(SR.OverloadOperatorTypeDoesNotMatchConversionType, p0, p1));
        }

        internal static Exception ParameterExpressionNotValidAsDelegate(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.ParameterExpressionNotValidAsDelegate, p0, p1));
        }

        internal static Exception PdbGeneratorNeedsExpressionCompiler()
        {
            return new NotSupportedException(SR.PdbGeneratorNeedsExpressionCompiler);
        }

        internal static Exception PropertyCannotHaveRefType(string paramName)
        {
            return new ArgumentException(SR.PropertyCannotHaveRefType, paramName);
        }

        internal static Exception PropertyDoesNotHaveAccessor(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.PropertyDoesNotHaveAccessor, p0), paramName);
        }

        internal static Exception PropertyDoesNotHaveGetter(object p0, string paramName, int index)
        {
            return new ArgumentException(SR.Format(SR.PropertyDoesNotHaveGetter, p0), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception PropertyDoesNotHaveSetter(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.PropertyDoesNotHaveSetter, p0), paramName);
        }

        internal static Exception PropertyNotDefinedForType(object p0, object p1, string paramName)
        {
            return new ArgumentException(SR.Format(SR.PropertyNotDefinedForType, p0, p1), paramName);
        }

        internal static Exception PropertyTypeCannotBeVoid(string paramName)
        {
            return new ArgumentException(SR.PropertyTypeCannotBeVoid, paramName);
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
            return new InvalidOperationException(SR.Format(SR.PropertyWithMoreThanOneMatch, p0, p1));
        }

        internal static Exception QuotedExpressionMustBeLambda(string paramName)
        {
            return new ArgumentException(SR.QuotedExpressionMustBeLambda, paramName);
        }

        internal static Exception ReducedNotCompatible()
        {
            return new ArgumentException(SR.ReducedNotCompatible);
        }

        internal static Exception ReducibleMustOverrideReduce()
        {
            return new ArgumentException(SR.ReducibleMustOverrideReduce);
        }

        internal static Exception ReferenceEqualityNotDefined(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format(SR.ReferenceEqualityNotDefined, p0, p1));
        }

        internal static Exception RethrowRequiresCatch()
        {
            return new InvalidOperationException(SR.RethrowRequiresCatch);
        }

        internal static Exception SameKeyExistsInExpando(object key)
        {
            return new ArgumentException(SR.Format("An element with the same key '{0}' already exists in the ExpandoObject.", key), nameof(key));
        }

        internal static Exception SetterHasNoParams(string paramName)
        {
            return new ArgumentException(SR.SetterHasNoParams, paramName);
        }

        internal static Exception SetterMustBeVoid(string paramName)
        {
            return new ArgumentException(SR.SetterMustBeVoid, paramName);
        }

        internal static Exception StartEndMustBeOrdered()
        {
            return new ArgumentException(SR.StartEndMustBeOrdered);
        }

        internal static Exception SwitchValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.SwitchValueTypeDoesNotMatchComparisonMethodParameter, p0, p1));
        }

        internal static Exception TestValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.TestValueTypeDoesNotMatchComparisonMethodParameter, p0, p1));
        }

        internal static Exception TryMustHaveCatchFinallyOrFault()
        {
            return new ArgumentException(SR.TryMustHaveCatchFinallyOrFault);
        }

        internal static Exception TryNotAllowedInFilter()
        {
            return new InvalidOperationException(SR.TryNotAllowedInFilter);
        }

        internal static Exception TryNotSupportedForMethodsWithRefArgs(object p0)
        {
            return new NotSupportedException(SR.Format(SR.TryNotSupportedForMethodsWithRefArgs, p0));
        }

        internal static Exception TryNotSupportedForValueTypeInstances(object p0)
        {
            return new NotSupportedException(SR.Format(SR.TryNotSupportedForValueTypeInstances, p0));
        }

        internal static Exception TypeContainsGenericParameters(object p0, string paramName, int index)
        {
            return new ArgumentException(SR.Format(SR.TypeContainsGenericParameters, p0), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception TypeIsGeneric(object p0, string paramName, int index)
        {
            return new ArgumentException(SR.Format(SR.TypeIsGeneric, p0), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }

        internal static Exception TypeMissingDefaultConstructor(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.TypeMissingDefaultConstructor, p0), paramName);
        }

        internal static Exception TypeMustBeDerivedFromSystemDelegate()
        {
            return new ArgumentException("Type must be derived from System.Delegate");
        }

        internal static Exception TypeMustNotBeByRef(string paramName)
        {
            return new ArgumentException(SR.TypeMustNotBeByRef, paramName);
        }

        internal static Exception TypeMustNotBePointer(string paramName)
        {
            return new ArgumentException("Type must not be a pointer type", paramName);
        }

        internal static Exception TypeNotIEnumerable(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.TypeNotIEnumerable, p0), paramName);
        }

        internal static Exception TypeParameterIsNotDelegate(object p0)
        {
            return new InvalidOperationException(SR.Format("Type parameter is {0}. Expected a delegate.", p0));
        }

        internal static Exception UnaryOperatorNotDefined(object p0, object p1)
        {
            return new InvalidOperationException(SR.Format(SR.UnaryOperatorNotDefined, p0, p1));
        }

        internal static Exception UndefinedVariable(object p0, object p1, object p2)
        {
            return new InvalidOperationException(SR.Format(SR.UndefinedVariable, p0, p1, p2));
        }

        internal static Exception UnexpectedVarArgsCall(object p0)
        {
            return new InvalidOperationException(SR.Format(SR.UnexpectedVarArgsCall, p0));
        }

        internal static Exception UnhandledBinary(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.UnhandledBinary, p0), paramName);
        }

        internal static Exception UnhandledBinding()
        {
            return new ArgumentException(SR.UnhandledBinding);
        }

        internal static Exception UnhandledBindingType(object p0)
        {
            return new ArgumentException(SR.Format(SR.UnhandledBindingType, p0));
        }

        internal static Exception UnhandledUnary(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.UnhandledUnary, p0), paramName);
        }

        internal static Exception UnknownBindingType(int index)
        {
            return new ArgumentException(SR.UnknownBindingType, $"bindings[{index}]");
        }

        internal static Exception UserDefinedOperatorMustBeStatic(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.UserDefinedOperatorMustBeStatic, p0), paramName);
        }

        internal static Exception UserDefinedOperatorMustNotBeVoid(object p0, string paramName)
        {
            return new ArgumentException(SR.Format(SR.UserDefinedOperatorMustNotBeVoid, p0), paramName);
        }

        internal static Exception UserDefinedOpMustHaveConsistentTypes(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.UserDefinedOpMustHaveConsistentTypes, p0, p1));
        }

        internal static Exception UserDefinedOpMustHaveValidReturnType(object p0, object p1)
        {
            return new ArgumentException(SR.Format(SR.UserDefinedOpMustHaveValidReturnType, p0, p1));
        }

        internal static Exception VariableMustNotBeByRef(object p0, object p1, string paramName, int index)
        {
            return new ArgumentException(SR.Format(SR.VariableMustNotBeByRef, p0, p1), index >= 0 ? $"{paramName}[{index}]" : paramName);
        }
    }
}

#endif