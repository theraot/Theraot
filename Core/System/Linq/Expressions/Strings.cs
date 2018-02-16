// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Linq.Expressions
{
    internal static class Strings
    {
        internal static string ReducibleMustOverrideReduce
        {
            get { return SR.ReducibleMustOverrideReduce; }
        }

        internal static string MustReduceToDifferent
        {
            get { return SR.MustReduceToDifferent; }
        }

        internal static string ReducedNotCompatible
        {
            get { return SR.ReducedNotCompatible; }
        }

        internal static string SetterHasNoParams
        {
            get { return SR.SetterHasNoParams; }
        }

        internal static string PropertyCannotHaveRefType
        {
            get { return SR.PropertyCannotHaveRefType; }
        }

        internal static string IndexesOfSetGetMustMatch
        {
            get { return SR.IndexesOfSetGetMustMatch; }
        }

        internal static string AccessorsCannotHaveVarArgs
        {
            get { return SR.AccessorsCannotHaveVarArgs; }
        }

        internal static string AccessorsCannotHaveByRefArgs
        {
            get { return SR.AccessorsCannotHaveByRefArgs; }
        }

        internal static string BoundsCannotBeLessThanOne
        {
            get { return SR.BoundsCannotBeLessThanOne; }
        }

        internal static string TypeMustNotBeByRef
        {
            get { return SR.TypeMustNotBeByRef; }
        }

        internal static string TypeDoesNotHaveConstructorForTheSignature
        {
            get { return SR.TypeDoesNotHaveConstructorForTheSignature; }
        }

        internal static string CountCannotBeNegative
        {
            get { return SR.CountCannotBeNegative; }
        }

        internal static string ArrayTypeMustBeArray
        {
            get { return SR.ArrayTypeMustBeArray; }
        }

        internal static string SetterMustBeVoid
        {
            get { return SR.SetterMustBeVoid; }
        }

        /// <summary>
        /// A string like "Property type must match the value type of setter"
        /// </summary>
        internal static string PropertyTyepMustMatchSetter
        {
            get { return SR.PropertyTyepMustMatchSetter; }
        }

        /// <summary>
        /// A string like "Both accessors must be static."
        /// </summary>
        internal static string BothAccessorsMustBeStatic
        {
            get { return SR.BothAccessorsMustBeStatic; }
        }

        /// <summary>
        /// A string like "Static field requires null instance, non-static field requires non-null instance."
        /// </summary>
        internal static string OnlyStaticFieldsHaveNullInstance
        {
            get { return SR.OnlyStaticFieldsHaveNullInstance; }
        }

        /// <summary>
        /// A string like "Static property requires null instance, non-static property requires non-null instance."
        /// </summary>
        internal static string OnlyStaticPropertiesHaveNullInstance
        {
            get { return SR.OnlyStaticPropertiesHaveNullInstance; }
        }

        /// <summary>
        /// A string like "Static method requires null instance, non-static method requires non-null instance."
        /// </summary>
        internal static string OnlyStaticMethodsHaveNullInstance
        {
            get { return SR.OnlyStaticMethodsHaveNullInstance; }
        }

        /// <summary>
        /// A string like "Property cannot have a void type."
        /// </summary>
        internal static string PropertyTypeCannotBeVoid
        {
            get { return SR.PropertyTypeCannotBeVoid; }
        }

        /// <summary>
        /// A string like "Can only unbox from an object or interface type to a value type."
        /// </summary>
        internal static string InvalidUnboxType
        {
            get { return SR.InvalidUnboxType; }
        }

        /// <summary>
        /// A string like "Expression must be writeable"
        /// </summary>
        internal static string ExpressionMustBeWriteable
        {
            get { return SR.ExpressionMustBeWriteable; }
        }

        /// <summary>
        /// A string like "Argument must not have a value type."
        /// </summary>
        internal static string ArgumentMustNotHaveValueType
        {
            get { return SR.ArgumentMustNotHaveValueType; }
        }

        /// <summary>
        /// A string like "must be reducible node"
        /// </summary>
        internal static string MustBeReducible
        {
            get { return SR.MustBeReducible; }
        }

        /// <summary>
        /// A string like "All test values must have the same type."
        /// </summary>
        internal static string AllTestValuesMustHaveSameType
        {
            get { return SR.AllTestValuesMustHaveSameType; }
        }

        /// <summary>
        /// A string like "All case bodies and the default body must have the same type."
        /// </summary>
        internal static string AllCaseBodiesMustHaveSameType
        {
            get { return SR.AllCaseBodiesMustHaveSameType; }
        }

        /// <summary>
        /// A string like "Default body must be supplied if case bodies are not System.Void."
        /// </summary>
        internal static string DefaultBodyMustBeSupplied
        {
            get { return SR.DefaultBodyMustBeSupplied; }
        }

        /// <summary>
        /// A string like "MethodBuilder does not have a valid TypeBuilder"
        /// </summary>
        internal static string MethodBuilderDoesNotHaveTypeBuilder
        {
            get { return SR.MethodBuilderDoesNotHaveTypeBuilder; }
        }

        /// <summary>
        /// A string like "Label type must be System.Void if an expression is not supplied"
        /// </summary>
        internal static string LabelMustBeVoidOrHaveExpression
        {
            get { return SR.LabelMustBeVoidOrHaveExpression; }
        }

        /// <summary>
        /// A string like "Type must be System.Void for this label argument"
        /// </summary>
        internal static string LabelTypeMustBeVoid
        {
            get { return SR.LabelTypeMustBeVoid; }
        }

        /// <summary>
        /// A string like "Quoted expression must be a lambda"
        /// </summary>
        internal static string QuotedExpressionMustBeLambda
        {
            get { return SR.QuotedExpressionMustBeLambda; }
        }

        internal static string VariableMustNotBeByRef(object p0, object p1)
        {
            return SR.Format(SR.VariableMustNotBeByRef, p0, p1);
        }

        internal static string DuplicateVariable(object p0)
        {
            return SR.Format(SR.DuplicateVariable, p0);
        }

        /// <summary>
        /// A string like "Start and End must be well ordered"
        /// </summary>
        internal static string StartEndMustBeOrdered
        {
            get { return SR.StartEndMustBeOrdered; }
        }

        /// <summary>
        /// A string like "fault cannot be used with catch or finally clauses"
        /// </summary>
        internal static string FaultCannotHaveCatchOrFinally
        {
            get { return SR.FaultCannotHaveCatchOrFinally; }
        }

        /// <summary>
        /// A string like "try must have at least one catch, finally, or fault clause"
        /// </summary>
        internal static string TryMustHaveCatchFinallyOrFault
        {
            get { return SR.TryMustHaveCatchFinallyOrFault; }
        }

        /// <summary>
        /// A string like "Body of catch must have the same type as body of try."
        /// </summary>
        internal static string BodyOfCatchMustHaveSameTypeAsBodyOfTry
        {
            get { return SR.BodyOfCatchMustHaveSameTypeAsBodyOfTry; }
        }

        internal static string ExtensionNodeMustOverrideProperty(object p0)
        {
            return SR.Format(SR.ExtensionNodeMustOverrideProperty, p0);
        }

        internal static string UserDefinedOperatorMustBeStatic(object p0)
        {
            return SR.Format(SR.UserDefinedOperatorMustBeStatic, p0);
        }

        internal static string UserDefinedOperatorMustNotBeVoid(object p0)
        {
            return SR.Format(SR.UserDefinedOperatorMustNotBeVoid, p0);
        }

        internal static string CoercionOperatorNotDefined(object p0, object p1)
        {
            return SR.Format(SR.CoercionOperatorNotDefined, p0, p1);
        }

        internal static string UnaryOperatorNotDefined(object p0, object p1)
        {
            return SR.Format(SR.UnaryOperatorNotDefined, p0, p1);
        }

        internal static string BinaryOperatorNotDefined(object p0, object p1, object p2)
        {
            return SR.Format(SR.BinaryOperatorNotDefined, p0, p1, p2);
        }

        internal static string ReferenceEqualityNotDefined(object p0, object p1)
        {
            return SR.Format(SR.ReferenceEqualityNotDefined, p0, p1);
        }

        internal static string OperandTypesDoNotMatchParameters(object p0, object p1)
        {
            return SR.Format(SR.OperandTypesDoNotMatchParameters, p0, p1);
        }

        internal static string OverloadOperatorTypeDoesNotMatchConversionType(object p0, object p1)
        {
            return SR.Format(SR.OverloadOperatorTypeDoesNotMatchConversionType, p0, p1);
        }

        internal static string ConversionIsNotSupportedForArithmeticTypes
        {
            get { return SR.ConversionIsNotSupportedForArithmeticTypes; }
        }

        internal static string ArgumentMustBeArray
        {
            get { return SR.ArgumentMustBeArray; }
        }

        internal static string ArgumentMustBeBoolean
        {
            get { return SR.ArgumentMustBeBoolean; }
        }

        internal static string EqualityMustReturnBoolean(object p0)
        {
            return SR.Format(SR.EqualityMustReturnBoolean, p0);
        }

        internal static string ArgumentMustBeFieldInfoOrPropertInfo
        {
            get { return SR.ArgumentMustBeFieldInfoOrPropertInfo; }
        }

        internal static string ArgumentMustBeFieldInfoOrPropertInfoOrMethod
        {
            get { return SR.ArgumentMustBeFieldInfoOrPropertInfoOrMethod; }
        }

        internal static string ArgumentMustBeInstanceMember
        {
            get { return SR.ArgumentMustBeInstanceMember; }
        }

        internal static string ArgumentMustBeInteger
        {
            get { return SR.ArgumentMustBeInteger; }
        }

        internal static string ArgumentMustBeArrayIndexType
        {
            get { return SR.ArgumentMustBeArrayIndexType; }
        }

        internal static string ArgumentMustBeSingleDimensionalArrayType
        {
            get { return SR.ArgumentMustBeSingleDimensionalArrayType; }
        }

        internal static string ArgumentTypesMustMatch
        {
            get { return SR.ArgumentTypesMustMatch; }
        }

        internal static string CannotAutoInitializeValueTypeElementThroughProperty(object p0)
        {
            return SR.Format(SR.CannotAutoInitializeValueTypeElementThroughProperty, p0);
        }

        internal static string CannotAutoInitializeValueTypeMemberThroughProperty(object p0)
        {
            return SR.Format(SR.CannotAutoInitializeValueTypeMemberThroughProperty, p0);
        }

        internal static string IncorrectTypeForTypeAs(object p0)
        {
            return SR.Format(SR.IncorrectTypeForTypeAs, p0);
        }

        internal static string CoalesceUsedOnNonNullType
        {
            get { return SR.CoalesceUsedOnNonNullType; }
        }

        internal static string ExpressionTypeCannotInitializeArrayType(object p0, object p1)
        {
            return SR.Format(SR.ExpressionTypeCannotInitializeArrayType, p0, p1);
        }

        internal static string ArgumentTypeDoesNotMatchMember(object p0, object p1)
        {
            return SR.Format(SR.ArgumentTypeDoesNotMatchMember, p0, p1);
        }

        internal static string ArgumentMemberNotDeclOnType(object p0, object p1)
        {
            return SR.Format(SR.ArgumentMemberNotDeclOnType, p0, p1);
        }

        internal static string ExpressionTypeDoesNotMatchReturn(object p0, object p1)
        {
            return SR.Format(SR.ExpressionTypeDoesNotMatchReturn, p0, p1);
        }

        internal static string ExpressionTypeDoesNotMatchAssignment(object p0, object p1)
        {
            return SR.Format(SR.ExpressionTypeDoesNotMatchAssignment, p0, p1);
        }

        internal static string ExpressionTypeDoesNotMatchLabel(object p0, object p1)
        {
            return SR.Format(SR.ExpressionTypeDoesNotMatchLabel, p0, p1);
        }

        internal static string ExpressionTypeNotInvocable(object p0)
        {
            return SR.Format(SR.ExpressionTypeNotInvocable, p0);
        }

        internal static string FieldNotDefinedForType(object p0, object p1)
        {
            return SR.Format(SR.FieldNotDefinedForType, p0, p1);
        }

        internal static string InstanceFieldNotDefinedForType(object p0, object p1)
        {
            return SR.Format(SR.InstanceFieldNotDefinedForType, p0, p1);
        }

        internal static string FieldInfoNotDefinedForType(object p0, object p1, object p2)
        {
            return SR.Format(SR.FieldInfoNotDefinedForType, p0, p1, p2);
        }

        internal static string IncorrectNumberOfIndexes
        {
            get { return SR.IncorrectNumberOfIndexes; }
        }

        internal static string IncorrectNumberOfLambdaDeclarationParameters
        {
            get { return SR.IncorrectNumberOfLambdaDeclarationParameters; }
        }

        internal static string IncorrectNumberOfMembersForGivenConstructor
        {
            get { return SR.IncorrectNumberOfMembersForGivenConstructor; }
        }

        internal static string IncorrectNumberOfArgumentsForMembers
        {
            get { return SR.IncorrectNumberOfArgumentsForMembers; }
        }

        internal static string LambdaTypeMustBeDerivedFromSystemDelegate
        {
            get { return SR.LambdaTypeMustBeDerivedFromSystemDelegate; }
        }

        internal static string MemberNotFieldOrProperty(object p0)
        {
            return SR.Format(SR.MemberNotFieldOrProperty, p0);
        }

        internal static string MethodContainsGenericParameters(object p0)
        {
            return SR.Format(SR.MethodContainsGenericParameters, p0);
        }

        internal static string MethodIsGeneric(object p0)
        {
            return SR.Format(SR.MethodIsGeneric, p0);
        }

        internal static string MethodNotPropertyAccessor(object p0, object p1)
        {
            return SR.Format(SR.MethodNotPropertyAccessor, p0, p1);
        }

        internal static string PropertyDoesNotHaveGetter(object p0)
        {
            return SR.Format(SR.PropertyDoesNotHaveGetter, p0);
        }

        internal static string PropertyDoesNotHaveSetter(object p0)
        {
            return SR.Format(SR.PropertyDoesNotHaveSetter, p0);
        }

        internal static string PropertyDoesNotHaveAccessor(object p0)
        {
            return SR.Format(SR.PropertyDoesNotHaveAccessor, p0);
        }

        internal static string NotAMemberOfType(object p0, object p1)
        {
            return SR.Format(SR.NotAMemberOfType, p0, p1);
        }

        internal static string ExpressionNotSupportedForType(object p0, object p1)
        {
            return SR.Format(SR.ExpressionNotSupportedForType, p0, p1);
        }

        internal static string ExpressionNotSupportedForNullableType(object p0, object p1)
        {
            return SR.Format(SR.ExpressionNotSupportedForNullableType, p0, p1);
        }

        internal static string ParameterExpressionNotValidAsDelegate(object p0, object p1)
        {
            return SR.Format(SR.ParameterExpressionNotValidAsDelegate, p0, p1);
        }

        internal static string PropertyNotDefinedForType(object p0, object p1)
        {
            return SR.Format(SR.PropertyNotDefinedForType, p0, p1);
        }

        internal static string InstancePropertyNotDefinedForType(object p0, object p1)
        {
            return SR.Format(SR.InstancePropertyNotDefinedForType, p0, p1);
        }

        internal static string InstancePropertyWithoutParameterNotDefinedForType(object p0, object p1)
        {
            return SR.Format(SR.InstancePropertyWithoutParameterNotDefinedForType, p0, p1);
        }

        internal static string InstancePropertyWithSpecifiedParametersNotDefinedForType(object p0, object p1, object p2)
        {
            return SR.Format(SR.InstancePropertyWithSpecifiedParametersNotDefinedForType, p0, p1, p2);
        }

        internal static string InstanceAndMethodTypeMismatch(object p0, object p1, object p2)
        {
            return SR.Format(SR.InstanceAndMethodTypeMismatch, p0, p1, p2);
        }

        internal static string TypeMissingDefaultConstructor(object p0)
        {
            return SR.Format(SR.TypeMissingDefaultConstructor, p0);
        }

        internal static string ListInitializerWithZeroMembers
        {
            get { return SR.ListInitializerWithZeroMembers; }
        }

        internal static string ElementInitializerMethodNotAdd
        {
            get { return SR.ElementInitializerMethodNotAdd; }
        }

        internal static string ElementInitializerMethodNoRefOutParam(object p0, object p1)
        {
            return SR.Format(SR.ElementInitializerMethodNoRefOutParam, p0, p1);
        }

        internal static string ElementInitializerMethodWithZeroArgs
        {
            get { return SR.ElementInitializerMethodWithZeroArgs; }
        }

        internal static string ElementInitializerMethodStatic
        {
            get { return SR.ElementInitializerMethodStatic; }
        }

        internal static string TypeNotIEnumerable(object p0)
        {
            return SR.Format(SR.TypeNotIEnumerable, p0);
        }

        internal static string UnexpectedCoalesceOperator
        {
            get { return SR.UnexpectedCoalesceOperator; }
        }

        internal static string InvalidCast(object p0, object p1)
        {
            return SR.Format(SR.InvalidCast, p0, p1);
        }

        internal static string UnhandledBinary(object p0)
        {
            return SR.Format(SR.UnhandledBinary, p0);
        }

        internal static string UnhandledBinding
        {
            get { return SR.UnhandledBinding; }
        }

        internal static string UnhandledBindingType(object p0)
        {
            return SR.Format(SR.UnhandledBindingType, p0);
        }

        internal static string UnhandledConvert(object p0)
        {
            return SR.Format(SR.UnhandledConvert, p0);
        }

        internal static string UnhandledExpressionType(object p0)
        {
            return SR.Format(SR.UnhandledExpressionType, p0);
        }

        internal static string UnhandledUnary(object p0)
        {
            return SR.Format(SR.UnhandledUnary, p0);
        }

        internal static string UnknownBindingType
        {
            get { return SR.UnknownBindingType; }
        }

        internal static string UserDefinedOpMustHaveConsistentTypes(object p0, object p1)
        {
            return SR.Format(SR.UserDefinedOpMustHaveConsistentTypes, p0, p1);
        }

        internal static string UserDefinedOpMustHaveValidReturnType(object p0, object p1)
        {
            return SR.Format(SR.UserDefinedOpMustHaveValidReturnType, p0, p1);
        }

        internal static string LogicalOperatorMustHaveBooleanOperators(object p0, object p1)
        {
            return SR.Format(SR.LogicalOperatorMustHaveBooleanOperators, p0, p1);
        }

        internal static string MethodDoesNotExistOnType(object p0, object p1)
        {
            return SR.Format(SR.MethodDoesNotExistOnType, p0, p1);
        }

        internal static string MethodWithArgsDoesNotExistOnType(object p0, object p1)
        {
            return SR.Format(SR.MethodWithArgsDoesNotExistOnType, p0, p1);
        }

        internal static string GenericMethodWithArgsDoesNotExistOnType(object p0, object p1)
        {
            return SR.Format(SR.GenericMethodWithArgsDoesNotExistOnType, p0, p1);
        }

        internal static string MethodWithMoreThanOneMatch(object p0, object p1)
        {
            return SR.Format(SR.MethodWithMoreThanOneMatch, p0, p1);
        }

        internal static string PropertyWithMoreThanOneMatch(object p0, object p1)
        {
            return SR.Format(SR.PropertyWithMoreThanOneMatch, p0, p1);
        }

        internal static string IncorrectNumberOfTypeArgsForFunc
        {
            get { return SR.IncorrectNumberOfTypeArgsForFunc; }
        }

        internal static string IncorrectNumberOfTypeArgsForAction
        {
            get { return SR.IncorrectNumberOfTypeArgsForAction; }
        }

        internal static string ArgumentCannotBeOfTypeVoid
        {
            get { return SR.ArgumentCannotBeOfTypeVoid; }
        }

        internal static string InvalidOperation(object p0)
        {
            return SR.Format(SR.InvalidOperation, p0);
        }

        internal static string OutOfRange(object p0, object p1)
        {
            return SR.Format(SR.OutOfRange, p0, p1);
        }

        /// <summary>
        /// A string like "Queue empty."
        /// </summary>
        internal static string QueueEmpty
        {
            get { return SR.QueueEmpty; }
        }

        internal static string LabelTargetAlreadyDefined(object p0)
        {
            return SR.Format(SR.LabelTargetAlreadyDefined, p0);
        }

        internal static string LabelTargetUndefined(object p0)
        {
            return SR.Format(SR.LabelTargetUndefined, p0);
        }

        internal static string ControlCannotLeaveFinally
        {
            get { return SR.ControlCannotLeaveFinally; }
        }

        internal static string ControlCannotLeaveFilterTest
        {
            get { return SR.ControlCannotLeaveFilterTest; }
        }

        internal static string AmbiguousJump(object p0)
        {
            return SR.Format(SR.AmbiguousJump, p0);
        }

        internal static string ControlCannotEnterTry
        {
            get { return SR.ControlCannotEnterTry; }
        }

        internal static string ControlCannotEnterExpression
        {
            get { return SR.ControlCannotEnterExpression; }
        }

        internal static string NonLocalJumpWithValue(object p0)
        {
            return SR.Format(SR.NonLocalJumpWithValue, p0);
        }

        internal static string ExtensionNotReduced
        {
            get { return SR.ExtensionNotReduced; }
        }

        internal static string CannotCompileConstant(object p0)
        {
            return SR.Format(SR.CannotCompileConstant, p0);
        }

        internal static string CannotCompileDynamic
        {
            get { return SR.CannotCompileDynamic; }
        }

        internal static string InvalidLvalue(object p0)
        {
            return SR.Format(SR.InvalidLvalue, p0);
        }

        internal static string InvalidMemberType(object p0)
        {
            return SR.Format(SR.InvalidMemberType, p0);
        }

        internal static string UnknownLiftType(object p0)
        {
            return SR.Format(SR.UnknownLiftType, p0);
        }

        internal static string InvalidOutputDir
        {
            get { return SR.InvalidOutputDir; }
        }

        internal static string InvalidAsmNameOrExtension
        {
            get { return SR.InvalidAsmNameOrExtension; }
        }

        internal static string IllegalNewGenericParams(object p0)
        {
            return SR.Format(SR.IllegalNewGenericParams, p0);
        }

        internal static string UndefinedVariable(object p0, object p1, object p2)
        {
            return SR.Format(SR.UndefinedVariable, p0, p1, p2);
        }

        internal static string CannotCloseOverByRef(object p0, object p1)
        {
            return SR.Format(SR.CannotCloseOverByRef, p0, p1);
        }

        internal static string UnexpectedVarArgsCall(object p0)
        {
            return SR.Format(SR.UnexpectedVarArgsCall, p0);
        }

        internal static string RethrowRequiresCatch
        {
            get { return SR.RethrowRequiresCatch; }
        }

        internal static string TryNotAllowedInFilter
        {
            get { return SR.TryNotAllowedInFilter; }
        }

        internal static string MustRewriteToSameNode(object p0, object p1, object p2)
        {
            return SR.Format(SR.MustRewriteToSameNode, p0, p1, p2);
        }

        internal static string MustRewriteChildToSameType(object p0, object p1, object p2)
        {
            return SR.Format(SR.MustRewriteChildToSameType, p0, p1, p2);
        }

        internal static string MustRewriteWithoutMethod(object p0, object p1)
        {
            return SR.Format(SR.MustRewriteWithoutMethod, p0, p1);
        }

        internal static string TryNotSupportedForMethodsWithRefArgs(object p0)
        {
            return SR.Format(SR.TryNotSupportedForMethodsWithRefArgs, p0);
        }

        internal static string TryNotSupportedForValueTypeInstances(object p0)
        {
            return SR.Format(SR.TryNotSupportedForValueTypeInstances, p0);
        }

        internal static string HomogenousAppDomainRequired
        {
            get { return SR.HomogenousAppDomainRequired; }
        }

        internal static string TestValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
        {
            return SR.Format(SR.TestValueTypeDoesNotMatchComparisonMethodParameter, p0, p1);
        }

        internal static string SwitchValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
        {
            return SR.Format(SR.SwitchValueTypeDoesNotMatchComparisonMethodParameter, p0, p1);
        }

        internal static string PdbGeneratorNeedsExpressionCompiler
        {
            get { return SR.PdbGeneratorNeedsExpressionCompiler; }
        }

        internal static string OperatorNotImplementedForType(object p0, object p1)
        {
            return SR.Format(SR.OperatorNotImplementedForType, p0, p1);
        }
    }
}