#if NET20 || NET30 || NET35 || NET40

namespace System
{
    internal static class SR
    {
        internal static string AccessorsCannotHaveByRefArgs => "Accessor indexes cannot be passed ByRef.";

        internal static string AccessorsCannotHaveVarArgs => "Accessor method should not have VarArgs.";

        internal static string AllCaseBodiesMustHaveSameType => "All case bodies and the default body must have the same type.";

        internal static string AllTestValuesMustHaveSameType => "All test values must have the same type.";

        internal static string AmbiguousJump => "Cannot jump to ambiguous label '{0}'.";

        internal static string ArgumentCannotBeOfTypeVoid => "Argument type cannot be System.Void.";

        internal static string ArgumentMemberNotDeclOnType => " The member '{0}' is not declared on type '{1}' being created";

        internal static string ArgumentMustBeArray => "Argument must be array";

        internal static string ArgumentMustBeArrayIndexType => "Argument for array index must be of type Int32";

        internal static string ArgumentMustBeBoolean => "Argument must be boolean";

        internal static string ArgumentMustBeInstanceMember => "Argument must be an instance member";

        internal static string ArgumentMustBeInteger => "Argument must be of an integer type";

        internal static string ArgumentMustBeSingleDimensionalArrayType => "Argument must be single dimensional array type";

        internal static string ArgumentMustNotHaveValueType => "Argument must not have a value type.";

        internal static string ArgumentTypeDoesNotMatchMember => " Argument type '{0}' does not match the corresponding member type '{1}'";

        internal static string ArgumentTypesMustMatch => "Argument types do not match";

        internal static string BinaryOperatorNotDefined => "The binary operator {0} is not defined for the types '{1}' and '{2}'.";

        internal static string BodyOfCatchMustHaveSameTypeAsBodyOfTry => "Body of catch must have the same type as body of try.";

        internal static string BothAccessorsMustBeStatic => "Both accessors must be static.";

        internal static string BoundsCannotBeLessThanOne => "Bounds count cannot be less than 1";

        internal static string CannotAutoInitializeValueTypeElementThroughProperty => "Cannot auto initialize elements of value type through property '{0}', use assignment instead";

        internal static string CannotAutoInitializeValueTypeMemberThroughProperty => "Cannot auto initialize members of value type through property '{0}', use assignment instead";

        internal static string CannotCloseOverByRef => "Cannot close over byref parameter '{0}' referenced in lambda '{1}'";

        internal static string CannotCompileConstant => "CompileToMethod cannot compile constant '{0}' because it is a non-trivial value, such as a live object. Instead, create an expression tree that can construct this value.";

        internal static string CannotCompileDynamic => "Dynamic expressions are not supported by CompileToMethod. Instead, create an expression tree that uses System.Runtime.CompilerServices.CallSite.";

        internal static string CoalesceUsedOnNonNullType => "Coalesce used with type that cannot be null";

        internal static string CoercionOperatorNotDefined => "No coercion operator is defined between types '{0}' and '{1}'.";

        internal static string CollectionModifiedWhileEnumerating => "Collection was modified; enumeration operation may not execute";

        internal static string ControlCannotEnterExpression => "Control cannot enter an expression--only statements can be jumped into.";

        internal static string ControlCannotEnterTry => "Control cannot enter a try block.";

        internal static string ControlCannotLeaveFilterTest => "Control cannot leave a filter test.";

        internal static string ControlCannotLeaveFinally => "Control cannot leave a finally block.";

        internal static string ConversionIsNotSupportedForArithmeticTypes => "Conversion is not supported for arithmetic types without operator overloading.";

        internal static string DefaultBodyMustBeSupplied => "Default body must be supplied if case bodies are not System.Void.";

        internal static string DuplicateVariable => "Found duplicate parameter '{0}'. Each ParameterExpression in the list must be a unique object.";

        internal static string ElementInitializerMethodNoRefOutParam => "Parameter '{0}' of element initializer method '{1}' must not be a pass by reference parameter";

        internal static string ElementInitializerMethodNotAdd => "Element initializer method must be named 'Add'";

        internal static string ElementInitializerMethodStatic => "Element initializer method must be an instance method";

        internal static string ElementInitializerMethodWithZeroArgs => "Element initializer method must have at least 1 parameter";

        internal static string EnumerationIsDone => "Enumeration has either not started or has already finished.";

        internal static string EqualityMustReturnBoolean => "The user-defined equality method '{0}' must return a boolean value.";

        internal static string ExpressionMustBeReadable => "Expression must be readable";

        internal static string ExpressionMustBeWriteable => "Expression must be writeable";

        internal static string ExpressionTypeCannotInitializeArrayType => "An expression of type '{0}' cannot be used to initialize an array of type '{1}'";

        internal static string ExpressionTypeDoesNotMatchAssignment => "Expression of type '{0}' cannot be used for assignment to type '{1}'";

        internal static string ExpressionTypeDoesNotMatchConstructorParameter => "Expression of type '{0}' cannot be used for constructor parameter of type '{1}'";

        internal static string ExpressionTypeDoesNotMatchLabel => "Expression of type '{0}' cannot be used for label of type '{1}'";

        internal static string ExpressionTypeDoesNotMatchMethodParameter => "Expression of type '{0}' cannot be used for parameter of type '{1}' of method '{2}'";

        internal static string ExpressionTypeDoesNotMatchParameter => "Expression of type '{0}' cannot be used for parameter of type '{1}'";

        internal static string ExpressionTypeDoesNotMatchReturn => "Expression of type '{0}' cannot be used for return type '{1}'";

        internal static string ExpressionTypeNotInvocable => "Expression of type '{0}' cannot be invoked";

        internal static string ExtensionNodeMustOverrideProperty => "Extension node must override the property {0}.";

        internal static string FaultCannotHaveCatchOrFinally => "fault cannot be used with catch or finally clauses";

        internal static string FieldInfoNotDefinedForType => "Field '{0}.{1}' is not defined for type '{2}'";

        internal static string FieldNotDefinedForType => "Field '{0}' is not defined for type '{1}'";

        internal static string GenericMethodWithArgsDoesNotExistOnType => "No generic method '{0}' on type '{1}' is compatible with the supplied type arguments and arguments. No type arguments should be provided if the method is non-generic. ";

        internal static string IncorrectNumberOfArgumentsForMembers => "Incorrect number of arguments for the given members ";

        internal static string IncorrectNumberOfConstructorArguments => "Incorrect number of arguments for constructor";

        internal static string IncorrectNumberOfIndexes => "Incorrect number of indexes";

        internal static string IncorrectNumberOfLambdaArguments => "Incorrect number of arguments supplied for lambda invocation";

        internal static string IncorrectNumberOfLambdaDeclarationParameters => "Incorrect number of parameters supplied for lambda declaration";

        internal static string IncorrectNumberOfMembersForGivenConstructor => " Incorrect number of members for constructor";

        internal static string IncorrectNumberOfMethodCallArguments => "Incorrect number of arguments supplied for call to method '{0}'";

        internal static string IncorrectNumberOfTypeArgsForAction => "An incorrect number of type args were specified for the declaration of an Action type.";

        internal static string IncorrectNumberOfTypeArgsForFunc => "An incorrect number of type args were specified for the declaration of a Func type.";

        internal static string IncorrectTypeForTypeAs => "The type used in TypeAs Expression must be of reference or nullable type, {0} is neither";

        internal static string IndexesOfSetGetMustMatch => "Indexing parameters of getter and setter must match.";

        internal static string InstanceAndMethodTypeMismatch => "Method '{0}' declared on type '{1}' cannot be called with instance of type '{2}'";

        internal static string InstanceFieldNotDefinedForType => "Instance field '{0}' is not defined for type '{1}'";

        internal static string InstancePropertyNotDefinedForType => "Instance property '{0}' is not defined for type '{1}'";

        internal static string InstancePropertyWithoutParameterNotDefinedForType => "Instance property '{0}' that takes no argument is not defined for type '{1}'";

        internal static string InstancePropertyWithSpecifiedParametersNotDefinedForType => "Instance property '{0}{1}' is not defined for type '{2}'";

        internal static string InvalidLvalue => "Invalid lvalue for assignment: {0}.";

        internal static string InvalidNullValue => "The value null is not of type '{0}' and cannot be used in this collection.";

        internal static string InvalidObjectType => "The value '{0}' is not of type '{1}' and cannot be used in this collection.";

        internal static string InvalidUnboxType => "Can only unbox from an object or interface type to a value type.";

        internal static string LabelMustBeVoidOrHaveExpression => "Label type must be System.Void if an expression is not supplied";

        internal static string LabelTargetAlreadyDefined => "Cannot redefine label '{0}' in an inner block.";

        internal static string LabelTargetUndefined => "Cannot jump to undefined label '{0}'.";

        internal static string LabelTypeMustBeVoid => "Type must be System.Void for this label argument";

        internal static string LambdaTypeMustBeDerivedFromSystemDelegate => "Lambda type parameter must be derived from System.Delegate";

        internal static string LogicalOperatorMustHaveBooleanOperators => "The user-defined operator method '{1}' for operator '{0}' must have associated boolean True and False operators.";

        internal static string MemberNotFieldOrProperty => "Member '{0}' not field or property";

        internal static string MethodBuilderDoesNotHaveTypeBuilder => "MethodBuilder does not have a valid TypeBuilder";

        internal static string MethodContainsGenericParameters => "Method {0} contains generic parameters";

        internal static string MethodIsGeneric => "Method {0} is a generic method definition";

        internal static string MethodNotPropertyAccessor => "The method '{0}.{1}' is not a property accessor";

        internal static string MethodWithArgsDoesNotExistOnType => "No method '{0}' on type '{1}' is compatible with the supplied arguments.";

        internal static string MethodWithMoreThanOneMatch => "More than one method '{0}' on type '{1}' is compatible with the supplied arguments.";

        internal static string MustBeReducible => "must be reducible node";

        internal static string MustReduceToDifferent => "node cannot reduce to itself or null";

        internal static string MustRewriteChildToSameType => "Rewriting child expression from type '{0}' to type '{1}' is not allowed, because it would change the meaning of the operation. If this is intentional, override '{2}' and change it to allow this rewrite.";

        internal static string MustRewriteToSameNode => "When called from '{0}', rewriting a node of type '{1}' must return a non-null value of the same type. Alternatively, override '{2}' and change it to not visit children of this type.";

        internal static string MustRewriteWithoutMethod => "Rewritten expression calls operator method '{0}', but the original node had no operator method. If this is is intentional, override '{1}' and change it to allow this rewrite.";

        internal static string NonEmptyCollectionRequired => "Non-empty collection required";

        internal static string NonLocalJumpWithValue => "Cannot jump to non-local label '{0}' with a value. Only jumps to labels defined in outer blocks can pass values.";

        internal static string NotAMemberOfType => "{0}' is not a member of type '{1}'";

        internal static string OnlyStaticFieldsHaveNullInstance => "Static field requires null instance, non-static field requires non-null instance.";

        internal static string OnlyStaticMethodsHaveNullInstance => "Static method requires null instance, non-static method requires non-null instance.";

        internal static string OnlyStaticPropertiesHaveNullInstance => "Static property requires null instance, non-static property requires non-null instance.";

        internal static string OperandTypesDoNotMatchParameters => "The operands for operator '{0}' do not match the parameters of method '{1}'.";

        internal static string OutOfRange => "{0} must be greater than or equal to {1}";

        internal static string OverloadOperatorTypeDoesNotMatchConversionType => "The return type of overload method for operator '{0}' does not match the parameter type of conversion method '{1}'.";

        internal static string ParameterExpressionNotValidAsDelegate => "ParameterExpression of type '{0}' cannot be used for delegate parameter of type '{1}'";

        internal static string PdbGeneratorNeedsExpressionCompiler => "DebugInfoGenerator created by CreatePdbGenerator can only be used with LambdaExpression.CompileToMethod.";

        internal static string PropertyCannotHaveRefType => "Property cannot have a managed pointer type.";

        internal static string PropertyDoesNotHaveAccessor => "The property '{0}' has no 'get' or 'set' accessors";

        internal static string PropertyDoesNotHaveGetter => "The property '{0}' has no 'get' accessor";

        internal static string PropertyDoesNotHaveSetter => "The property '{0}' has no 'set' accessor";

        internal static string PropertyNotDefinedForType => "Property '{0}' is not defined for type '{1}'";

        internal static string PropertyTypeCannotBeVoid => "Property cannot have a void type.";

        internal static string PropertyWithMoreThanOneMatch => "More than one property '{0}' on type '{1}' is compatible with the supplied arguments.";

        internal static string QuotedExpressionMustBeLambda => "Quoted expression must be a lambda";

        internal static string ReducedNotCompatible => "cannot assign from the reduced node type to the original node type";

        internal static string ReducibleMustOverrideReduce => "reducible nodes must override Expression.Reduce()";

        internal static string ReferenceEqualityNotDefined => "Reference equality is not defined for the types '{0}' and '{1}'.";

        internal static string RethrowRequiresCatch => "Rethrow statement is valid only inside a Catch block.";

        internal static string SetterHasNoParams => "Setter must have parameters.";

        internal static string SetterMustBeVoid => "Setter should have void type.";

        internal static string StartEndMustBeOrdered => "Start and End must be well ordered";

        internal static string SwitchValueTypeDoesNotMatchComparisonMethodParameter => "Switch value of type '{0}' cannot be used for the comparison method parameter of type '{1}'";

        internal static string TestValueTypeDoesNotMatchComparisonMethodParameter => "Test value of type '{0}' cannot be used for the comparison method parameter of type '{1}'";

        internal static string TryMustHaveCatchFinallyOrFault => "try must have at least one catch, finally, or fault clause";

        internal static string TryNotAllowedInFilter => "Try expression is not allowed inside a filter body.";

        internal static string TryNotSupportedForMethodsWithRefArgs => "TryExpression is not supported as an argument to method '{0}' because it has an argument with by-ref type. Construct the tree so the TryExpression is not nested inside of this expression.";

        internal static string TryNotSupportedForValueTypeInstances => "TryExpression is not supported as a child expression when accessing a member on type '{0}' because it is a value type. Construct the tree so the TryExpression is not nested inside of this expression.";

        internal static string TypeContainsGenericParameters => "Type {0} contains generic parameters";

        internal static string TypeIsGeneric => "Type {0} is a generic type definition";

        internal static string TypeMissingDefaultConstructor => "Type '{0}' does not have a default constructor";

        internal static string TypeMustNotBeByRef => "type must not be ByRef";

        internal static string TypeNotIEnumerable => "Type '{0}' is not IEnumerable";

        internal static string UnaryOperatorNotDefined => "The unary operator {0} is not defined for the type '{1}'.";

        internal static string UndefinedVariable => "variable '{0}' of type '{1}' referenced from scope '{2}', but it is not defined";

        internal static string UnexpectedVarArgsCall => "Unexpected VarArgs call to method '{0}'";

        internal static string UnhandledBinary => "Unhandled binary: {0}";

        internal static string UnhandledBinding => "Unhandled binding ";

        internal static string UnhandledBindingType => "Unhandled Binding Type: {0}";

        internal static string UnhandledUnary => "Unhandled unary: {0}";

        internal static string UnknownBindingType => "Unknown binding type";

        internal static string UnsupportedExpressionType => "The expression type '{0}' is not supported";

        internal static string UserDefinedOperatorMustBeStatic => "User-defined operator method '{0}' must be static.";

        internal static string UserDefinedOperatorMustNotBeVoid => "User-defined operator method '{0}' must not be void.";

        internal static string UserDefinedOpMustHaveConsistentTypes => "The user-defined operator method '{1}' for operator '{0}' must have identical parameter and return types.";

        internal static string UserDefinedOpMustHaveValidReturnType => "The user-defined operator method '{1}' for operator '{0}' must return the same type as its parameter or a derived type.";

        internal static string VariableMustNotBeByRef => "Variable '{0}' uses unsupported type '{1}'. Reference types are not supported for variables.";

        internal static string Format(string resourceFormat, params object[] args)
        {
            if (args != null)
            {
                return string.Format(resourceFormat, args);
            }
            return resourceFormat;
        }

        internal static string Format(string resourceFormat, object p1)
        {
            return string.Format(resourceFormat, p1);
        }

        internal static string Format(string resourceFormat, object p1, object p2)
        {
            return string.Format(resourceFormat, p1, p2);
        }

        internal static string Format(string resourceFormat, object p1, object p2, object p3)
        {
            return string.Format(resourceFormat, p1, p2, p3);
        }
    }
}

#endif