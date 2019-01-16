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
            return AccessorsCannotHaveByRefArgs(GetParamName(paramName, index));
        }

        internal static Exception AccessorsCannotHaveVarArgs(string paramName)
        {
            return new ArgumentException(Strings.AccessorsCannotHaveVarArgs, paramName);
        }

        internal static Exception AllCaseBodiesMustHaveSameType(string paramName)
        {
            return new ArgumentException(Strings.AllCaseBodiesMustHaveSameType, paramName);
        }

        internal static Exception AllTestValuesMustHaveSameType(string paramName)
        {
            return new ArgumentException(Strings.AllTestValuesMustHaveSameType, paramName);
        }

        internal static Exception AmbiguousJump(object p0)
        {
            return new InvalidOperationException(Strings.AmbiguousJump(p0));
        }

        internal static Exception AmbiguousMatchInExpandoObject(object p0)
        {
            return new AmbiguousMatchException(Strings.AmbiguousMatchInExpandoObject(p0));
        }

        internal static Exception ArgCntMustBeGreaterThanNameCnt()
        {
            return new ArgumentException(Strings.ArgCntMustBeGreaterThanNameCnt);
        }

        internal static Exception ArgumentCannotBeOfTypeVoid(string paramName)
        {
            return new ArgumentException(Strings.ArgumentCannotBeOfTypeVoid, paramName);
        }

        internal static Exception ArgumentMemberNotDeclOnType(object p0, object p1, string paramName, int index)
        {
            return ArgumentMemberNotDeclOnType(p0, p1, GetParamName(paramName, index));
        }

        internal static Exception ArgumentMustBeArray(string paramName)
        {
            return new ArgumentException(Strings.ArgumentMustBeArray, paramName);
        }

        internal static Exception ArgumentMustBeArrayIndexType(string paramName)
        {
            return new ArgumentException(Strings.ArgumentMustBeArrayIndexType, paramName);
        }

        internal static Exception ArgumentMustBeArrayIndexType(string paramName, int index)
        {
            return ArgumentMustBeArrayIndexType(GetParamName(paramName, index));
        }

        internal static Exception ArgumentMustBeBoolean(string paramName)
        {
            return new ArgumentException(Strings.ArgumentMustBeBoolean, paramName);
        }

        internal static Exception ArgumentMustBeFieldInfoOrPropertyInfo(string paramName)
        {
            return new ArgumentException(Strings.ArgumentMustBeFieldInfoOrPropertyInfo, paramName);
        }

        internal static Exception ArgumentMustBeFieldInfoOrPropertyInfoOrMethod(string paramName, int index)
        {
            return ArgumentMustBeFieldInfoOrPropertyInfoOrMethod(GetParamName(paramName, index));
        }

        internal static Exception ArgumentMustBeInstanceMember(string paramName, int index)
        {
            return ArgumentMustBeInstanceMember(GetParamName(paramName, index));
        }

        internal static Exception ArgumentMustBeInteger(string paramName, int index)
        {
            return ArgumentMustBeInteger(GetParamName(paramName, index));
        }

        internal static Exception ArgumentMustBeSingleDimensionalArrayType(string paramName)
        {
            return new ArgumentException(Strings.ArgumentMustBeSingleDimensionalArrayType, paramName);
        }

        internal static Exception ArgumentMustNotHaveValueType(string paramName)
        {
            return new ArgumentException(Strings.ArgumentMustNotHaveValueType, paramName);
        }

        internal static Exception ArgumentOutOfRange(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName);
        }

        internal static Exception ArgumentTypeCannotBeVoid()
        {
            return new ArgumentException(Strings.ArgumentTypeCannotBeVoid);
        }

        internal static Exception ArgumentTypeDoesNotMatchMember(object p0, object p1, string paramName, int index)
        {
            return ArgumentTypeDoesNotMatchMember(p0, p1, GetParamName(paramName, index));
        }

        internal static Exception ArgumentTypesMustMatch()
        {
            return new ArgumentException(Strings.ArgumentTypesMustMatch);
        }

        internal static Exception ArgumentTypesMustMatch(string paramName)
        {
            return new ArgumentException(Strings.ArgumentTypesMustMatch, paramName);
        }

        internal static Exception BinaryOperatorNotDefined(object p0, object p1, object p2)
        {
            return new InvalidOperationException(Strings.BinaryOperatorNotDefined(p0, p1, p2));
        }

        internal static Exception BinderNotCompatibleWithCallSite(object p0, object p1, object p2)
        {
            return new InvalidOperationException(Strings.BinderNotCompatibleWithCallSite(p0, p1, p2));
        }

        internal static Exception BindingCannotBeNull()
        {
            return new InvalidOperationException(Strings.BindingCannotBeNull);
        }

        internal static Exception BodyOfCatchMustHaveSameTypeAsBodyOfTry()
        {
            return new ArgumentException(Strings.BodyOfCatchMustHaveSameTypeAsBodyOfTry);
        }

        internal static Exception BothAccessorsMustBeStatic(string paramName)
        {
            return new ArgumentException(Strings.BothAccessorsMustBeStatic, paramName);
        }

        internal static Exception BoundsCannotBeLessThanOne(string paramName)
        {
            return new ArgumentException(Strings.BoundsCannotBeLessThanOne, paramName);
        }

        internal static Exception CannotAutoInitializeValueTypeElementThroughProperty(object p0)
        {
            return new InvalidOperationException(Strings.CannotAutoInitializeValueTypeElementThroughProperty(p0));
        }

        internal static Exception CannotAutoInitializeValueTypeMemberThroughProperty(object p0)
        {
            return new InvalidOperationException(Strings.CannotAutoInitializeValueTypeMemberThroughProperty(p0));
        }

        internal static Exception CannotCloseOverByRef(object p0, object p1)
        {
            return new InvalidOperationException(Strings.CannotCloseOverByRef(p0, p1));
        }

        internal static Exception CannotCompileConstant(object p0)
        {
            return new InvalidOperationException(Strings.CannotCompileConstant(p0));
        }

        internal static Exception CannotCompileDynamic()
        {
            return new NotSupportedException(Strings.CannotCompileDynamic);
        }

        internal static Exception CoalesceUsedOnNonNullType()
        {
            return new InvalidOperationException(Strings.CoalesceUsedOnNonNullType);
        }

        internal static Exception CoercionOperatorNotDefined(object p0, object p1)
        {
            return new InvalidOperationException(Strings.CoercionOperatorNotDefined(p0, p1));
        }

        internal static Exception CollectionModifiedWhileEnumerating()
        {
            return new InvalidOperationException(Strings.CollectionModifiedWhileEnumerating);
        }

        internal static Exception CollectionReadOnly()
        {
            return new NotSupportedException(Strings.CollectionReadOnly);
        }

        internal static Exception ControlCannotEnterExpression()
        {
            return new InvalidOperationException(Strings.ControlCannotEnterExpression);
        }

        internal static Exception ControlCannotEnterTry()
        {
            return new InvalidOperationException(Strings.ControlCannotEnterTry);
        }

        internal static Exception ControlCannotLeaveFilterTest()
        {
            return new InvalidOperationException(Strings.ControlCannotLeaveFilterTest);
        }

        internal static Exception ControlCannotLeaveFinally()
        {
            return new InvalidOperationException(Strings.ControlCannotLeaveFinally);
        }

        internal static Exception ConversionIsNotSupportedForArithmeticTypes()
        {
            return new InvalidOperationException(Strings.ConversionIsNotSupportedForArithmeticTypes);
        }

        internal static Exception DefaultBodyMustBeSupplied(string paramName)
        {
            return new ArgumentException(Strings.DefaultBodyMustBeSupplied, paramName);
        }

        internal static Exception DuplicateVariable(object p0, string paramName, int index)
        {
            return DuplicateVariable(p0, GetParamName(paramName, index));
        }

        internal static Exception DynamicBinderResultNotAssignable(object p0, object p1, object p2)
        {
            return new InvalidCastException(Strings.DynamicBinderResultNotAssignable(p0, p1, p2));
        }

        internal static Exception DynamicBindingNeedsRestrictions(object p0, object p1)
        {
            return new InvalidOperationException(Strings.DynamicBindingNeedsRestrictions(p0, p1));
        }

        internal static Exception DynamicObjectResultNotAssignable(object p0, object p1, object p2, object p3)
        {
            return new InvalidCastException(Strings.DynamicObjectResultNotAssignable(p0, p1, p2, p3));
        }

        internal static Exception ElementInitializerMethodNoRefOutParam(object p0, object p1, string paramName)
        {
            return new ArgumentException(Strings.ElementInitializerMethodNoRefOutParam(p0, p1), paramName);
        }

        internal static Exception ElementInitializerMethodNotAdd(string paramName)
        {
            return new ArgumentException(Strings.ElementInitializerMethodNotAdd, paramName);
        }

        internal static Exception ElementInitializerMethodStatic(string paramName)
        {
            return new ArgumentException(Strings.ElementInitializerMethodStatic, paramName);
        }

        internal static Exception ElementInitializerMethodWithZeroArgs(string paramName)
        {
            return new ArgumentException(Strings.ElementInitializerMethodWithZeroArgs, paramName);
        }

        internal static Exception EnumerationIsDone()
        {
            return new InvalidOperationException(Strings.EnumerationIsDone);
        }

        internal static Exception EqualityMustReturnBoolean(object p0, string paramName)
        {
            return new ArgumentException(Strings.EqualityMustReturnBoolean(p0), paramName);
        }

        internal static Exception ExpressionMustBeReadable(string paramName)
        {
            return new ArgumentException(Strings.ExpressionMustBeReadable, paramName);
        }

        internal static Exception ExpressionMustBeReadable(string paramName, int index)
        {
            return ExpressionMustBeReadable(GetParamName(paramName, index));
        }

        internal static Exception ExpressionMustBeWriteable(string paramName)
        {
            return new ArgumentException(Strings.ExpressionMustBeWriteable, paramName);
        }

        internal static Exception ExpressionTypeCannotInitializeArrayType(object p0, object p1)
        {
            return new InvalidOperationException(Strings.ExpressionTypeCannotInitializeArrayType(p0, p1));
        }

        internal static Exception ExpressionTypeDoesNotMatchAssignment(object p0, object p1)
        {
            return new ArgumentException(Strings.ExpressionTypeDoesNotMatchAssignment(p0, p1));
        }

        internal static Exception ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1, string paramName)
        {
            return new ArgumentException(Strings.ExpressionTypeDoesNotMatchConstructorParameter(p0, p1), paramName);
        }

        internal static Exception ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1, string paramName, int index)
        {
            return ExpressionTypeDoesNotMatchConstructorParameter(p0, p1, GetParamName(paramName, index));
        }

        internal static Exception ExpressionTypeDoesNotMatchLabel(object p0, object p1)
        {
            return new ArgumentException(Strings.ExpressionTypeDoesNotMatchLabel(p0, p1));
        }

        internal static Exception ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2, string paramName)
        {
            return new ArgumentException(Strings.ExpressionTypeDoesNotMatchMethodParameter(p0, p1, p2), paramName);
        }

        internal static Exception ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2, string paramName, int index)
        {
            return ExpressionTypeDoesNotMatchMethodParameter(p0, p1, p2, GetParamName(paramName, index));
        }

        internal static Exception ExpressionTypeDoesNotMatchParameter(object p0, object p1, string paramName)
        {
            return new ArgumentException(Strings.ExpressionTypeDoesNotMatchParameter(p0, p1), paramName);
        }

        internal static Exception ExpressionTypeDoesNotMatchParameter(object p0, object p1, string paramName, int index)
        {
            return ExpressionTypeDoesNotMatchParameter(p0, p1, GetParamName(paramName, index));
        }

        internal static Exception ExpressionTypeDoesNotMatchReturn(object p0, object p1)
        {
            return new ArgumentException(Strings.ExpressionTypeDoesNotMatchReturn(p0, p1));
        }

        internal static Exception ExpressionTypeNotInvocable(object p0, string paramName)
        {
            return new ArgumentException(Strings.ExpressionTypeNotInvocable(p0), paramName);
        }

        internal static Exception ExtensionNodeMustOverrideProperty(object p0)
        {
            return new InvalidOperationException(Strings.ExtensionNodeMustOverrideProperty(p0));
        }

        internal static Exception FaultCannotHaveCatchOrFinally(string paramName)
        {
            return new ArgumentException(Strings.FaultCannotHaveCatchOrFinally, paramName);
        }

        internal static Exception FieldInfoNotDefinedForType(object p0, object p1, object p2)
        {
            return new ArgumentException(Strings.FieldInfoNotDefinedForType(p0, p1, p2));
        }

        internal static Exception FieldNotDefinedForType(object p0, object p1)
        {
            return new ArgumentException(Strings.FieldNotDefinedForType(p0, p1));
        }

        internal static Exception FirstArgumentMustBeCallSite()
        {
            return new ArgumentException(Strings.FirstArgumentMustBeCallSite);
        }

        internal static Exception GenericMethodWithArgsDoesNotExistOnType(object p0, object p1)
        {
            return new InvalidOperationException(Strings.GenericMethodWithArgsDoesNotExistOnType(p0, p1));
        }

        internal static Exception IncorrectNumberOfArgumentsForMembers()
        {
            return new ArgumentException(Strings.IncorrectNumberOfArgumentsForMembers);
        }

        internal static Exception IncorrectNumberOfConstructorArguments()
        {
            return new ArgumentException(Strings.IncorrectNumberOfConstructorArguments);
        }

        internal static Exception IncorrectNumberOfIndexes()
        {
            return new ArgumentException(Strings.IncorrectNumberOfIndexes);
        }

        internal static Exception IncorrectNumberOfLambdaArguments()
        {
            return new InvalidOperationException(Strings.IncorrectNumberOfLambdaArguments);
        }

        internal static Exception IncorrectNumberOfLambdaDeclarationParameters()
        {
            return new ArgumentException(Strings.IncorrectNumberOfLambdaDeclarationParameters);
        }

        internal static Exception IncorrectNumberOfMembersForGivenConstructor()
        {
            return new ArgumentException(Strings.IncorrectNumberOfMembersForGivenConstructor);
        }

        internal static Exception IncorrectNumberOfMethodCallArguments(object p0, string paramName)
        {
            return new ArgumentException(Strings.IncorrectNumberOfMethodCallArguments(p0), paramName);
        }

        internal static Exception IncorrectNumberOfTypeArgsForAction(string paramName)
        {
            return new ArgumentException(Strings.IncorrectNumberOfTypeArgsForAction, paramName);
        }

        internal static Exception IncorrectNumberOfTypeArgsForFunc(string paramName)
        {
            return new ArgumentException(Strings.IncorrectNumberOfTypeArgsForFunc, paramName);
        }

        internal static Exception IncorrectTypeForTypeAs(object p0, string paramName)
        {
            return new ArgumentException(Strings.IncorrectTypeForTypeAs(p0), paramName);
        }

        internal static Exception IndexesOfSetGetMustMatch(string paramName)
        {
            return new ArgumentException(Strings.IndexesOfSetGetMustMatch, paramName);
        }

        internal static Exception InstanceAndMethodTypeMismatch(object p0, object p1, object p2)
        {
            return new ArgumentException(Strings.InstanceAndMethodTypeMismatch(p0, p1, p2));
        }

        internal static Exception InstanceFieldNotDefinedForType(object p0, object p1)
        {
            return new ArgumentException(Strings.InstanceFieldNotDefinedForType(p0, p1));
        }

        internal static Exception InstancePropertyNotDefinedForType(object p0, object p1, string paramName)
        {
            return new ArgumentException(Strings.InstancePropertyNotDefinedForType(p0, p1), paramName);
        }

        internal static Exception InstancePropertyWithoutParameterNotDefinedForType(object p0, object p1)
        {
            return new ArgumentException(Strings.InstancePropertyWithoutParameterNotDefinedForType(p0, p1));
        }

        internal static Exception InstancePropertyWithSpecifiedParametersNotDefinedForType(object p0, object p1, object p2, string paramName)
        {
            return new ArgumentException(Strings.InstancePropertyWithSpecifiedParametersNotDefinedForType(p0, p1, p2), paramName);
        }

        internal static Exception InvalidArgumentValue(string paramName)
        {
            return new ArgumentException(Strings.InvalidArgumentValueParamName, paramName);
        }

        internal static Exception InvalidLvalue(ExpressionType p0)
        {
            return new InvalidOperationException(Strings.InvalidLvalue(p0));
        }

        internal static Exception InvalidMetaObjectCreated(object p0)
        {
            return new InvalidOperationException(Strings.InvalidMetaObjectCreated(p0));
        }

        internal static Exception InvalidNullValue(Type type, string paramName)
        {
            return new ArgumentException(Strings.InvalidNullValue(type), paramName);
        }

        internal static Exception InvalidProgram()
        {
            return new InvalidProgramException();
        }

        internal static Exception InvalidTypeException(object value, Type type, string paramName)
        {
            return new ArgumentException(Strings.InvalidObjectType(value?.GetType() as object ?? "null", type), paramName);
        }

        internal static Exception InvalidUnboxType(string paramName)
        {
            return new ArgumentException(Strings.InvalidUnboxType, paramName);
        }

        internal static Exception KeyDoesNotExistInExpando(object p0)
        {
            return new KeyNotFoundException(Strings.KeyDoesNotExistInExpando(p0));
        }

        internal static Exception LabelMustBeVoidOrHaveExpression(string paramName)
        {
            return new ArgumentException(Strings.LabelMustBeVoidOrHaveExpression, paramName);
        }

        internal static Exception LabelTargetAlreadyDefined(object p0)
        {
            return new InvalidOperationException(Strings.LabelTargetAlreadyDefined(p0));
        }

        internal static Exception LabelTargetUndefined(object p0)
        {
            return new InvalidOperationException(Strings.LabelTargetUndefined(p0));
        }

        internal static Exception LabelTypeMustBeVoid(string paramName)
        {
            return new ArgumentException(Strings.LabelTypeMustBeVoid, paramName);
        }

        internal static Exception LambdaTypeMustBeDerivedFromSystemDelegate(string paramName)
        {
            return new ArgumentException(Strings.LambdaTypeMustBeDerivedFromSystemDelegate, paramName);
        }

        internal static Exception LogicalOperatorMustHaveBooleanOperators(object p0, object p1)
        {
            return new ArgumentException(Strings.LogicalOperatorMustHaveBooleanOperators(p0, p1));
        }

        internal static Exception MemberNotFieldOrProperty(object p0, string paramName)
        {
            return new ArgumentException(Strings.MemberNotFieldOrProperty(p0), paramName);
        }

        internal static Exception MethodBuilderDoesNotHaveTypeBuilder()
        {
            return new ArgumentException(Strings.MethodBuilderDoesNotHaveTypeBuilder);
        }

        internal static Exception MethodContainsGenericParameters(object p0, string paramName)
        {
            return new ArgumentException(Strings.MethodContainsGenericParameters(p0), paramName);
        }

        internal static Exception MethodIsGeneric(object p0, string paramName)
        {
            return new ArgumentException(Strings.MethodIsGeneric(p0), paramName);
        }

        internal static Exception MethodNotPropertyAccessor(object p0, object p1, string paramName, int index)
        {
            return MethodNotPropertyAccessor(p0, p1, GetParamName(paramName, index));
        }

        internal static Exception MethodWithArgsDoesNotExistOnType(object p0, object p1)
        {
            return new InvalidOperationException(Strings.MethodWithArgsDoesNotExistOnType(p0, p1));
        }

        internal static Exception MethodWithMoreThanOneMatch(object p0, object p1)
        {
            return new InvalidOperationException(Strings.MethodWithMoreThanOneMatch(p0, p1));
        }

        internal static Exception MustBeReducible()
        {
            return new ArgumentException(Strings.MustBeReducible);
        }

        internal static Exception MustReduceToDifferent()
        {
            return new ArgumentException(Strings.MustReduceToDifferent);
        }

        internal static Exception MustRewriteChildToSameType(object p0, object p1, object p2)
        {
            return new InvalidOperationException(Strings.MustRewriteChildToSameType(p0, p1, p2));
        }

        internal static Exception MustRewriteToSameNode(object p0, object p1, object p2)
        {
            return new InvalidOperationException(Strings.MustRewriteToSameNode(p0, p1, p2));
        }

        internal static Exception MustRewriteWithoutMethod(object p0, object p1)
        {
            return new InvalidOperationException(Strings.MustRewriteWithoutMethod(p0, p1));
        }

        internal static Exception NonAbstractConstructorRequired()
        {
            return new InvalidOperationException(Strings.NonAbstractConstructorRequired);
        }

        internal static Exception NonEmptyCollectionRequired(string paramName)
        {
            return new ArgumentException(Strings.NonEmptyCollectionRequired, paramName);
        }

        internal static Exception NonLocalJumpWithValue(object p0)
        {
            return new InvalidOperationException(Strings.NonLocalJumpWithValue(p0));
        }

        internal static Exception NonStaticConstructorRequired(string paramName)
        {
            return new ArgumentException(Strings.NonStaticConstructorRequired, paramName);
        }

        internal static Exception NoOrInvalidRuleProduced()
        {
            return new InvalidOperationException(Strings.NoOrInvalidRuleProduced);
        }

        internal static Exception NotAMemberOfAnyType(object p0, string paramName)
        {
            return new ArgumentException(Strings.NotAMemberOfAnyType(p0), paramName);
        }

        internal static Exception NotAMemberOfType(object p0, object p1, string paramName)
        {
            return new ArgumentException(Strings.NotAMemberOfType(p0, p1), paramName);
        }

        internal static Exception NotAMemberOfType(object p0, object p1, string paramName, int index)
        {
            return NotAMemberOfType(p0, p1, GetParamName(paramName, index));
        }

        internal static Exception NotSupported()
        {
            return new NotSupportedException();
        }

        internal static Exception OnlyStaticFieldsHaveNullInstance(string paramName)
        {
            return new ArgumentException(Strings.OnlyStaticFieldsHaveNullInstance, paramName);
        }

        internal static Exception OnlyStaticMethodsHaveNullInstance()
        {
            return new ArgumentException(Strings.OnlyStaticMethodsHaveNullInstance);
        }

        internal static Exception OnlyStaticPropertiesHaveNullInstance(string paramName)
        {
            return new ArgumentException(Strings.OnlyStaticPropertiesHaveNullInstance, paramName);
        }

        internal static Exception OperandTypesDoNotMatchParameters(object p0, object p1)
        {
            return new InvalidOperationException(Strings.OperandTypesDoNotMatchParameters(p0, p1));
        }

        internal static Exception OutOfRange(string paramName, object p1)
        {
            return new ArgumentOutOfRangeException(paramName, Strings.OutOfRange(paramName, p1));
        }

        internal static Exception OverloadOperatorTypeDoesNotMatchConversionType(object p0, object p1)
        {
            return new InvalidOperationException(Strings.OverloadOperatorTypeDoesNotMatchConversionType(p0, p1));
        }

        internal static Exception ParameterExpressionNotValidAsDelegate(object p0, object p1)
        {
            return new ArgumentException(Strings.ParameterExpressionNotValidAsDelegate(p0, p1));
        }

        internal static Exception PdbGeneratorNeedsExpressionCompiler()
        {
            return new NotSupportedException(Strings.PdbGeneratorNeedsExpressionCompiler);
        }

        internal static Exception PropertyCannotHaveRefType(string paramName)
        {
            return new ArgumentException(Strings.PropertyCannotHaveRefType, paramName);
        }

        internal static Exception PropertyDoesNotHaveAccessor(object p0, string paramName)
        {
            return new ArgumentException(Strings.PropertyDoesNotHaveAccessor(p0), paramName);
        }

        internal static Exception PropertyDoesNotHaveGetter(object p0, string paramName)
        {
            return new ArgumentException(Strings.PropertyDoesNotHaveGetter(p0), paramName);
        }

        internal static Exception PropertyDoesNotHaveGetter(object p0, string paramName, int index)
        {
            return PropertyDoesNotHaveGetter(p0, GetParamName(paramName, index));
        }

        internal static Exception PropertyDoesNotHaveSetter(object p0, string paramName)
        {
            return new ArgumentException(Strings.PropertyDoesNotHaveSetter(p0), paramName);
        }

        internal static Exception PropertyNotDefinedForType(object p0, object p1, string paramName)
        {
            return new ArgumentException(Strings.PropertyNotDefinedForType(p0, p1), paramName);
        }

        internal static Exception PropertyTypeCannotBeVoid(string paramName)
        {
            return new ArgumentException(Strings.PropertyTypeCannotBeVoid, paramName);
        }

        internal static Exception PropertyTypeMustMatchGetter(string paramName)
        {
            return new ArgumentException(Strings.PropertyTypeMustMatchGetter, paramName);
        }

        internal static Exception PropertyTypeMustMatchSetter(string paramName)
        {
            return new ArgumentException(Strings.PropertyTypeMustMatchSetter, paramName);
        }

        internal static Exception PropertyWithMoreThanOneMatch(object p0, object p1)
        {
            return new InvalidOperationException(Strings.PropertyWithMoreThanOneMatch(p0, p1));
        }

        internal static Exception QuotedExpressionMustBeLambda(string paramName)
        {
            return new ArgumentException(Strings.QuotedExpressionMustBeLambda, paramName);
        }

        internal static Exception ReducedNotCompatible()
        {
            return new ArgumentException(Strings.ReducedNotCompatible);
        }

        internal static Exception ReducibleMustOverrideReduce()
        {
            return new ArgumentException(Strings.ReducibleMustOverrideReduce);
        }

        internal static Exception ReferenceEqualityNotDefined(object p0, object p1)
        {
            return new InvalidOperationException(Strings.ReferenceEqualityNotDefined(p0, p1));
        }

        internal static Exception RethrowRequiresCatch()
        {
            return new InvalidOperationException(Strings.RethrowRequiresCatch);
        }

        internal static Exception SameKeyExistsInExpando(object key)
        {
            return new ArgumentException(Strings.SameKeyExistsInExpando(key), nameof(key));
        }

        internal static Exception SetterHasNoParams(string paramName)
        {
            return new ArgumentException(Strings.SetterHasNoParams, paramName);
        }

        internal static Exception SetterMustBeVoid(string paramName)
        {
            return new ArgumentException(Strings.SetterMustBeVoid, paramName);
        }

        internal static Exception StartEndMustBeOrdered()
        {
            return new ArgumentException(Strings.StartEndMustBeOrdered);
        }

        internal static Exception SwitchValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
        {
            return new ArgumentException(Strings.SwitchValueTypeDoesNotMatchComparisonMethodParameter(p0, p1));
        }

        internal static Exception TestValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
        {
            return new ArgumentException(Strings.TestValueTypeDoesNotMatchComparisonMethodParameter(p0, p1));
        }

        internal static Exception TryMustHaveCatchFinallyOrFault()
        {
            return new ArgumentException(Strings.TryMustHaveCatchFinallyOrFault);
        }

        internal static Exception TryNotAllowedInFilter()
        {
            return new InvalidOperationException(Strings.TryNotAllowedInFilter);
        }

        internal static Exception TryNotSupportedForMethodsWithRefArgs(object p0)
        {
            return new NotSupportedException(Strings.TryNotSupportedForMethodsWithRefArgs(p0));
        }

        internal static Exception TryNotSupportedForValueTypeInstances(object p0)
        {
            return new NotSupportedException(Strings.TryNotSupportedForValueTypeInstances(p0));
        }

        internal static Exception TypeContainsGenericParameters(object p0, string paramName, int index)
        {
            return TypeContainsGenericParameters(p0, GetParamName(paramName, index));
        }

        internal static Exception TypeIsGeneric(object p0, string paramName)
        {
            return new ArgumentException(Strings.TypeIsGeneric(p0), paramName);
        }

        internal static Exception TypeIsGeneric(object p0, string paramName, int index)
        {
            return TypeIsGeneric(p0, GetParamName(paramName, index));
        }

        internal static Exception TypeMissingDefaultConstructor(object p0, string paramName)
        {
            return new ArgumentException(Strings.TypeMissingDefaultConstructor(p0), paramName);
        }

        internal static Exception TypeMustBeDerivedFromSystemDelegate()
        {
            return new ArgumentException(Strings.TypeMustBeDerivedFromSystemDelegate);
        }

        internal static Exception TypeMustNotBeByRef(string paramName)
        {
            return new ArgumentException(Strings.TypeMustNotBeByRef, paramName);
        }

        internal static Exception TypeMustNotBePointer(string paramName)
        {
            return new ArgumentException(Strings.TypeMustNotBePointer, paramName);
        }

        internal static Exception TypeNotIEnumerable(object p0, string paramName)
        {
            return new ArgumentException(Strings.TypeNotIEnumerable(p0), paramName);
        }

        internal static Exception TypeParameterIsNotDelegate(object p0)
        {
            return new InvalidOperationException(Strings.TypeParameterIsNotDelegate(p0));
        }

        internal static Exception UnaryOperatorNotDefined(object p0, object p1)
        {
            return new InvalidOperationException(Strings.UnaryOperatorNotDefined(p0, p1));
        }

        internal static Exception UndefinedVariable(object p0, object p1, object p2)
        {
            return new InvalidOperationException(Strings.UndefinedVariable(p0, p1, p2));
        }

        internal static Exception UnexpectedVarArgsCall(object p0)
        {
            return new InvalidOperationException(Strings.UnexpectedVarArgsCall(p0));
        }

        internal static Exception UnhandledBinary(object p0, string paramName)
        {
            return new ArgumentException(Strings.UnhandledBinary(p0), paramName);
        }

        internal static Exception UnhandledBinding()
        {
            return new ArgumentException(Strings.UnhandledBinding);
        }

        internal static Exception UnhandledBindingType(object p0)
        {
            return new ArgumentException(Strings.UnhandledBindingType(p0));
        }

        internal static Exception UnhandledUnary(object p0, string paramName)
        {
            return new ArgumentException(Strings.UnhandledUnary(p0), paramName);
        }

        internal static Exception UnknownBindingType(int index)
        {
            return new ArgumentException(Strings.UnknownBindingType, $"bindings[{index}]");
        }

        internal static Exception UserDefinedOperatorMustBeStatic(object p0, string paramName)
        {
            return new ArgumentException(Strings.UserDefinedOperatorMustBeStatic(p0), paramName);
        }

        internal static Exception UserDefinedOperatorMustNotBeVoid(object p0, string paramName)
        {
            return new ArgumentException(Strings.UserDefinedOperatorMustNotBeVoid(p0), paramName);
        }

        internal static Exception UserDefinedOpMustHaveConsistentTypes(object p0, object p1)
        {
            return new ArgumentException(Strings.UserDefinedOpMustHaveConsistentTypes(p0, p1));
        }

        internal static Exception UserDefinedOpMustHaveValidReturnType(object p0, object p1)
        {
            return new ArgumentException(Strings.UserDefinedOpMustHaveValidReturnType(p0, p1));
        }

        internal static Exception VariableMustNotBeByRef(object p0, object p1, string paramName)
        {
            return new ArgumentException(Strings.VariableMustNotBeByRef(p0, p1), paramName);
        }

        internal static Exception VariableMustNotBeByRef(object p0, object p1, string paramName, int index)
        {
            return VariableMustNotBeByRef(p0, p1, GetParamName(paramName, index));
        }

        private static Exception AccessorsCannotHaveByRefArgs(string paramName)
        {
            return new ArgumentException(Strings.AccessorsCannotHaveByRefArgs, paramName);
        }

        private static Exception ArgumentMemberNotDeclOnType(object p0, object p1, string paramName)
        {
            return new ArgumentException(Strings.ArgumentMemberNotDeclOnType(p0, p1), paramName);
        }

        private static Exception ArgumentMustBeFieldInfoOrPropertyInfoOrMethod(string paramName)
        {
            return new ArgumentException(Strings.ArgumentMustBeFieldInfoOrPropertyInfoOrMethod, paramName);
        }

        private static Exception ArgumentMustBeInstanceMember(string paramName)
        {
            return new ArgumentException(Strings.ArgumentMustBeInstanceMember, paramName);
        }

        private static Exception ArgumentMustBeInteger(string paramName)
        {
            return new ArgumentException(Strings.ArgumentMustBeInteger, paramName);
        }

        private static Exception ArgumentTypeDoesNotMatchMember(object p0, object p1, string paramName)
        {
            return new ArgumentException(Strings.ArgumentTypeDoesNotMatchMember(p0, p1), paramName);
        }

        private static Exception DuplicateVariable(object p0, string paramName)
        {
            return new ArgumentException(Strings.DuplicateVariable(p0), paramName);
        }

        private static string GetParamName(string paramName, int index)
        {
            if (index >= 0)
            {
                return $"{paramName}[{index}]";
            }

            return paramName;
        }

        private static Exception MethodNotPropertyAccessor(object p0, object p1, string paramName)
        {
            return new ArgumentException(Strings.MethodNotPropertyAccessor(p0, p1), paramName);
        }

        private static Exception TypeContainsGenericParameters(object p0, string paramName)
        {
            return new ArgumentException(Strings.TypeContainsGenericParameters(p0), paramName);
        }
    }
}

#endif