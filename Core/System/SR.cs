namespace System
{
    internal class SR
    {
        internal static string AccessorsCannotHaveByRefArgs
        {
            get
            {
                return "Accessor indexes cannot be passed ByRef.";
            }
        }

        internal static string AccessorsCannotHaveVarArgs
        {
            get
            {
                return "Accessor method should not have VarArgs.";
            }
        }

        internal static string AllCaseBodiesMustHaveSameType
        {
            get
            {
                return "All case bodies and the default body must have the same type.";
            }
        }

        internal static string AllTestValuesMustHaveSameType
        {
            get
            {
                return "All test values must have the same type.";
            }
        }

        internal static string AmbiguousJump
        {
            get
            {
                return "Cannot jump to ambiguous label '{0}'.";
            }
        }

        internal static string ArgumentCannotBeOfTypeVoid
        {
            get
            {
                return "Argument type cannot be System.Void.";
            }
        }

        internal static string ArgumentMemberNotDeclOnType
        {
            get
            {
                return " The member '{0}' is not declared on type '{1}' being created";
            }
        }

        internal static string ArgumentMustBeArray
        {
            get
            {
                return "Argument must be array";
            }
        }

        internal static string ArgumentMustBeArrayIndexType
        {
            get
            {
                return "Argument for array index must be of type Int32";
            }
        }

        internal static string ArgumentMustBeBoolean
        {
            get
            {
                return "Argument must be boolean";
            }
        }

        internal static string ArgumentMustBeFieldInfoOrPropertInfo
        {
            get
            {
                return "Argument must be either a FieldInfo or PropertyInfo";
            }
        }

        internal static string ArgumentMustBeFieldInfoOrPropertInfoOrMethod
        {
            get
            {
                return "Argument must be either a FieldInfo, PropertyInfo or MethodInfo";
            }
        }

        internal static string ArgumentMustBeInstanceMember
        {
            get
            {
                return "Argument must be an instance member";
            }
        }

        internal static string ArgumentMustBeInteger
        {
            get
            {
                return "Argument must be of an integer type";
            }
        }

        internal static string ArgumentMustBeSingleDimensionalArrayType
        {
            get
            {
                return "Argument must be single dimensional array type";
            }
        }

        internal static string ArgumentMustNotHaveValueType
        {
            get
            {
                return "Argument must not have a value type.";
            }
        }

        internal static string ArgumentTypeDoesNotMatchMember
        {
            get
            {
                return " Argument type '{0}' does not match the corresponding member type '{1}'";
            }
        }

        internal static string ArgumentTypesMustMatch
        {
            get
            {
                return "Argument types do not match";
            }
        }

        internal static string ArrayTypeMustBeArray
        {
            get
            {
                return "arrayType must be an array type";
            }
        }

        internal static string BinaryOperatorNotDefined
        {
            get
            {
                return "The binary operator {0} is not defined for the types '{1}' and '{2}'.";
            }
        }

        internal static string BodyOfCatchMustHaveSameTypeAsBodyOfTry
        {
            get
            {
                return "Body of catch must have the same type as body of try.";
            }
        }

        internal static string BothAccessorsMustBeStatic
        {
            get
            {
                return "Both accessors must be static.";
            }
        }

        internal static string BoundsCannotBeLessThanOne
        {
            get
            {
                return "Bounds count cannot be less than 1";
            }
        }

        internal static string CannotAutoInitializeValueTypeElementThroughProperty
        {
            get
            {
                return "Cannot auto initialize elements of value type through property '{0}', use assignment instead";
            }
        }

        internal static string CannotAutoInitializeValueTypeMemberThroughProperty
        {
            get
            {
                return "Cannot auto initialize members of value type through property '{0}', use assignment instead";
            }
        }

        internal static string CannotCloseOverByRef
        {
            get
            {
                return "Cannot close over byref parameter '{0}' referenced in lambda '{1}'";
            }
        }

        internal static string CannotCompileConstant
        {
            get
            {
                return "CompileToMethod cannot compile constant '{0}' because it is a non-trivial value, such as a live object. Instead, create an expression tree that can construct this value.";
            }
        }

        internal static string CannotCompileDynamic
        {
            get
            {
                return "Dynamic expressions are not supported by CompileToMethod. Instead, create an expression tree that uses System.Runtime.CompilerServices.CallSite.";
            }
        }

        internal static string CoalesceUsedOnNonNullType
        {
            get
            {
                return "Coalesce used with type that cannot be null";
            }
        }

        internal static string CoercionOperatorNotDefined
        {
            get
            {
                return "No coercion operator is defined between types '{0}' and '{1}'.";
            }
        }

        internal static string CollectionModifiedWhileEnumerating
        {
            get
            {
                return "Collection was modified; enumeration operation may not execute";
            }
        }

        internal static string ControlCannotEnterExpression
        {
            get
            {
                return "Control cannot enter an expression--only statements can be jumped into.";
            }
        }

        internal static string ControlCannotEnterTry
        {
            get
            {
                return "Control cannot enter a try block.";
            }
        }

        internal static string ControlCannotLeaveFilterTest
        {
            get
            {
                return "Control cannot leave a filter test.";
            }
        }

        internal static string ControlCannotLeaveFinally
        {
            get
            {
                return "Control cannot leave a finally block.";
            }
        }

        internal static string ConversionIsNotSupportedForArithmeticTypes
        {
            get
            {
                return "Conversion is not supported for arithmetic types without operator overloading.";
            }
        }

        internal static string CountCannotBeNegative
        {
            get
            {
                return "Count must be non-negative.";
            }
        }

        internal static string DefaultBodyMustBeSupplied
        {
            get
            {
                return "Default body must be supplied if case bodies are not System.Void.";
            }
        }

        internal static string DuplicateVariable
        {
            get
            {
                return "Found duplicate parameter '{0}'. Each ParameterExpression in the list must be a unique object.";
            }
        }

        internal static string ElementInitializerMethodNoRefOutParam
        {
            get
            {
                return "Parameter '{0}' of element initializer method '{1}' must not be a pass by reference parameter";
            }
        }

        internal static string ElementInitializerMethodNotAdd
        {
            get
            {
                return "Element initializer method must be named 'Add'";
            }
        }

        internal static string ElementInitializerMethodStatic
        {
            get
            {
                return "Element initializer method must be an instance method";
            }
        }

        internal static string ElementInitializerMethodWithZeroArgs
        {
            get
            {
                return "Element initializer method must have at least 1 parameter";
            }
        }

        internal static string EnumerationIsDone
        {
            get
            {
                return "Enumeration has either not started or has already finished.";
            }
        }

        internal static string EqualityMustReturnBoolean
        {
            get
            {
                return "The user-defined equality method '{0}' must return a boolean value.";
            }
        }

        internal static string ExpressionMustBeReadable
        {
            get
            {
                return "Expression must be readable";
            }
        }

        internal static string ExpressionMustBeWriteable
        {
            get
            {
                return "Expression must be writeable";
            }
        }

        internal static string ExpressionNotSupportedForNullableType
        {
            get
            {
                return "The expression '{0}' is not supported for nullable type '{1}'";
            }
        }

        internal static string ExpressionNotSupportedForType
        {
            get
            {
                return "The expression '{0}' is not supported for type '{1}'";
            }
        }

        internal static string ExpressionTypeCannotInitializeArrayType
        {
            get
            {
                return "An expression of type '{0}' cannot be used to initialize an array of type '{1}'";
            }
        }

        internal static string ExpressionTypeDoesNotMatchAssignment
        {
            get
            {
                return "Expression of type '{0}' cannot be used for assignment to type '{1}'";
            }
        }

        internal static string ExpressionTypeDoesNotMatchConstructorParameter
        {
            get
            {
                return "Expression of type '{0}' cannot be used for constructor parameter of type '{1}'";
            }
        }

        internal static string ExpressionTypeDoesNotMatchLabel
        {
            get
            {
                return "Expression of type '{0}' cannot be used for label of type '{1}'";
            }
        }

        internal static string ExpressionTypeDoesNotMatchMethodParameter
        {
            get
            {
                return "Expression of type '{0}' cannot be used for parameter of type '{1}' of method '{2}'";
            }
        }

        internal static string ExpressionTypeDoesNotMatchParameter
        {
            get
            {
                return "Expression of type '{0}' cannot be used for parameter of type '{1}'";
            }
        }

        internal static string ExpressionTypeDoesNotMatchReturn
        {
            get
            {
                return "Expression of type '{0}' cannot be used for return type '{1}'";
            }
        }

        internal static string ExpressionTypeNotInvocable
        {
            get
            {
                return "Expression of type '{0}' cannot be invoked";
            }
        }

        internal static string ExtensionNodeMustOverrideProperty
        {
            get
            {
                return "Extension node must override the property {0}.";
            }
        }

        internal static string ExtensionNotReduced
        {
            get
            {
                return "Extension should have been reduced.";
            }
        }

        internal static string FaultBlockNotSupported
        {
            get
            {
                return "Fault blocks are not supported";
            }
        }

        internal static string FaultCannotHaveCatchOrFinally
        {
            get
            {
                return "fault cannot be used with catch or finally clauses";
            }
        }

        internal static string FieldInfoNotDefinedForType
        {
            get
            {
                return "Field '{0}.{1}' is not defined for type '{2}'";
            }
        }

        internal static string FieldNotDefinedForType
        {
            get
            {
                return "Field '{0}' is not defined for type '{1}'";
            }
        }

        internal static string FilterBlockNotSupported
        {
            get
            {
                return "Filter blocks are not supported";
            }
        }

        internal static string GenericMethodWithArgsDoesNotExistOnType
        {
            get
            {
                return "No generic method '{0}' on type '{1}' is compatible with the supplied type arguments and arguments. No type arguments should be provided if the method is non-generic. ";
            }
        }

        internal static string HomogenousAppDomainRequired
        {
            get
            {
                return "Dynamic operations can only be performed in homogenous AppDomain.";
            }
        }

        internal static string IllegalNewGenericParams
        {
            get
            {
                return "Cannot create instance of {0} because it contains generic parameters";
            }
        }

        internal static string IncorrectNumberOfArgumentsForMembers
        {
            get
            {
                return "Incorrect number of arguments for the given members ";
            }
        }

        internal static string IncorrectNumberOfConstructorArguments
        {
            get
            {
                return "Incorrect number of arguments for constructor";
            }
        }

        internal static string IncorrectNumberOfIndexes
        {
            get
            {
                return "Incorrect number of indexes";
            }
        }

        internal static string IncorrectNumberOfLambdaArguments
        {
            get
            {
                return "Incorrect number of arguments supplied for lambda invocation";
            }
        }

        internal static string IncorrectNumberOfLambdaDeclarationParameters
        {
            get
            {
                return "Incorrect number of parameters supplied for lambda declaration";
            }
        }

        internal static string IncorrectNumberOfMembersForGivenConstructor
        {
            get
            {
                return " Incorrect number of members for constructor";
            }
        }

        internal static string IncorrectNumberOfMethodCallArguments
        {
            get
            {
                return "Incorrect number of arguments supplied for call to method '{0}'";
            }
        }

        internal static string IncorrectNumberOfTypeArgsForAction
        {
            get
            {
                return "An incorrect number of type args were specified for the declaration of an Action type.";
            }
        }

        internal static string IncorrectNumberOfTypeArgsForFunc
        {
            get
            {
                return "An incorrect number of type args were specified for the declaration of a Func type.";
            }
        }

        internal static string IncorrectTypeForTypeAs
        {
            get
            {
                return "The type used in TypeAs Expression must be of reference or nullable type, {0} is neither";
            }
        }

        internal static string IndexesOfSetGetMustMatch
        {
            get
            {
                return "Indexing parameters of getter and setter must match.";
            }
        }

        internal static string InstanceAndMethodTypeMismatch
        {
            get
            {
                return "Method '{0}' declared on type '{1}' cannot be called with instance of type '{2}'";
            }
        }

        internal static string InstanceFieldNotDefinedForType
        {
            get
            {
                return "Instance field '{0}' is not defined for type '{1}'";
            }
        }

        internal static string InstancePropertyNotDefinedForType
        {
            get
            {
                return "Instance property '{0}' is not defined for type '{1}'";
            }
        }

        internal static string InstancePropertyWithoutParameterNotDefinedForType
        {
            get
            {
                return "Instance property '{0}' that takes no argument is not defined for type '{1}'";
            }
        }

        internal static string InstancePropertyWithSpecifiedParametersNotDefinedForType
        {
            get
            {
                return "Instance property '{0}{1}' is not defined for type '{2}'";
            }
        }

        internal static string InvalidArgumentValue
        {
            get
            {
                return "Invalid argument value";
            }
        }

        internal static string InvalidAsmNameOrExtension
        {
            get
            {
                return "Invalid assembly name or file extension.";
            }
        }

        internal static string InvalidCast
        {
            get
            {
                return "Cannot cast from type '{0}' to type '{1}";
            }
        }

        internal static string InvalidLvalue
        {
            get
            {
                return "Invalid lvalue for assignment: {0}.";
            }
        }

        internal static string InvalidMemberType
        {
            get
            {
                return "Invalid member type: {0}.";
            }
        }

        internal static string InvalidNullValue
        {
            get
            {
                return "The value null is not of type '{0}' and cannot be used in this collection.";
            }
        }

        internal static string InvalidObjectType
        {
            get
            {
                return "The value '{0}' is not of type '{1}' and cannot be used in this collection.";
            }
        }

        internal static string InvalidOperation
        {
            get
            {
                return "Invalid operation: '{0}'";
            }
        }

        internal static string InvalidOutputDir
        {
            get
            {
                return "Invalid output directory.";
            }
        }

        internal static string InvalidUnboxType
        {
            get
            {
                return "Can only unbox from an object or interface type to a value type.";
            }
        }

        internal static string LabelMustBeVoidOrHaveExpression
        {
            get
            {
                return "Label type must be System.Void if an expression is not supplied";
            }
        }

        internal static string LabelTargetAlreadyDefined
        {
            get
            {
                return "Cannot redefine label '{0}' in an inner block.";
            }
        }

        internal static string LabelTargetUndefined
        {
            get
            {
                return "Cannot jump to undefined label '{0}'.";
            }
        }

        internal static string LabelTypeMustBeVoid
        {
            get
            {
                return "Type must be System.Void for this label argument";
            }
        }

        internal static string LambdaTypeMustBeDerivedFromSystemDelegate
        {
            get
            {
                return "Lambda type parameter must be derived from System.Delegate";
            }
        }

        internal static string ListInitializerWithZeroMembers
        {
            get
            {
                return "List initializers must contain at least one initializer";
            }
        }

        internal static string LogicalOperatorMustHaveBooleanOperators
        {
            get
            {
                return "The user-defined operator method '{1}' for operator '{0}' must have associated boolean True and False operators.";
            }
        }

        internal static string MemberNotFieldOrProperty
        {
            get
            {
                return "Member '{0}' not field or property";
            }
        }

        internal static string MethodBuilderDoesNotHaveTypeBuilder
        {
            get
            {
                return "MethodBuilder does not have a valid TypeBuilder";
            }
        }

        internal static string MethodContainsGenericParameters
        {
            get
            {
                return "Method {0} contains generic parameters";
            }
        }

        internal static string MethodDoesNotExistOnType
        {
            get
            {
                return "No method '{0}' exists on type '{1}'.";
            }
        }

        internal static string MethodIsGeneric
        {
            get
            {
                return "Method {0} is a generic method definition";
            }
        }

        internal static string MethodNotPropertyAccessor
        {
            get
            {
                return "The method '{0}.{1}' is not a property accessor";
            }
        }

        internal static string MethodWithArgsDoesNotExistOnType
        {
            get
            {
                return "No method '{0}' on type '{1}' is compatible with the supplied arguments.";
            }
        }

        internal static string MethodWithMoreThanOneMatch
        {
            get
            {
                return "More than one method '{0}' on type '{1}' is compatible with the supplied arguments.";
            }
        }

        internal static string MustBeReducible
        {
            get
            {
                return "must be reducible node";
            }
        }

        internal static string MustReduceToDifferent
        {
            get
            {
                return "node cannot reduce to itself or null";
            }
        }

        internal static string MustRewriteChildToSameType
        {
            get
            {
                return "Rewriting child expression from type '{0}' to type '{1}' is not allowed, because it would change the meaning of the operation. If this is intentional, override '{2}' and change it to allow this rewrite.";
            }
        }

        internal static string MustRewriteToSameNode
        {
            get
            {
                return "When called from '{0}', rewriting a node of type '{1}' must return a non-null value of the same type. Alternatively, override '{2}' and change it to not visit children of this type.";
            }
        }

        internal static string MustRewriteWithoutMethod
        {
            get
            {
                return "Rewritten expression calls operator method '{0}', but the original node had no operator method. If this is is intentional, override '{1}' and change it to allow this rewrite.";
            }
        }

        internal static string NonEmptyCollectionRequired
        {
            get
            {
                return "Non-empty collection required";
            }
        }

        internal static string NonLocalJumpWithValue
        {
            get
            {
                return "Cannot jump to non-local label '{0}' with a value. Only jumps to labels defined in outer blocks can pass values.";
            }
        }

        internal static string NonReducibleExpressionExtensionsNotSupported
        {
            get
            {
                return "Non-reducible expression extensions are not supported";
            }
        }

        internal static string NotAMemberOfType
        {
            get
            {
                return "{0}' is not a member of type '{1}'";
            }
        }

        internal static string OnlyStaticFieldsHaveNullInstance
        {
            get
            {
                return "Static field requires null instance, non-static field requires non-null instance.";
            }
        }

        internal static string OnlyStaticMethodsHaveNullInstance
        {
            get
            {
                return "Static method requires null instance, non-static method requires non-null instance.";
            }
        }

        internal static string OnlyStaticPropertiesHaveNullInstance
        {
            get
            {
                return "Static property requires null instance, non-static property requires non-null instance.";
            }
        }

        internal static string OperandTypesDoNotMatchParameters
        {
            get
            {
                return "The operands for operator '{0}' do not match the parameters of method '{1}'.";
            }
        }

        internal static string OperatorNotImplementedForType
        {
            get
            {
                return "The operator '{0}' is not implemented for type '{1}'";
            }
        }

        internal static string OutOfRange
        {
            get
            {
                return "{0} must be greater than or equal to {1}";
            }
        }

        internal static string OverloadOperatorTypeDoesNotMatchConversionType
        {
            get
            {
                return "The return type of overload method for operator '{0}' does not match the parameter type of conversion method '{1}'.";
            }
        }

        internal static string ParameterExpressionNotValidAsDelegate
        {
            get
            {
                return "ParameterExpression of type '{0}' cannot be used for delegate parameter of type '{1}'";
            }
        }

        internal static string PdbGeneratorNeedsExpressionCompiler
        {
            get
            {
                return "DebugInfoGenerator created by CreatePdbGenerator can only be used with LambdaExpression.CompileToMethod.";
            }
        }

        internal static string PropertyCannotHaveRefType
        {
            get
            {
                return "Property cannot have a managed pointer type.";
            }
        }

        internal static string PropertyDoesNotHaveAccessor
        {
            get
            {
                return "The property '{0}' has no 'get' or 'set' accessors";
            }
        }

        internal static string PropertyDoesNotHaveGetter
        {
            get
            {
                return "The property '{0}' has no 'get' accessor";
            }
        }

        internal static string PropertyDoesNotHaveSetter
        {
            get
            {
                return "The property '{0}' has no 'set' accessor";
            }
        }

        internal static string PropertyNotDefinedForType
        {
            get
            {
                return "Property '{0}' is not defined for type '{1}'";
            }
        }

        internal static string PropertyTyepMustMatchSetter
        {
            get
            {
                return "Property type must match the value type of setter";
            }
        }

        internal static string PropertyTypeCannotBeVoid
        {
            get
            {
                return "Property cannot have a void type.";
            }
        }

        internal static string PropertyWithMoreThanOneMatch
        {
            get
            {
                return "More than one property '{0}' on type '{1}' is compatible with the supplied arguments.";
            }
        }

        internal static string QueueEmpty
        {
            get
            {
                return "Queue empty.";
            }
        }

        internal static string QuotedExpressionMustBeLambda
        {
            get
            {
                return "Quoted expression must be a lambda";
            }
        }

        internal static string ReducedNotCompatible
        {
            get
            {
                return "cannot assign from the reduced node type to the original node type";
            }
        }

        internal static string ReducibleMustOverrideReduce
        {
            get
            {
                return "reducible nodes must override Expression.Reduce()";
            }
        }

        internal static string ReferenceEqualityNotDefined
        {
            get
            {
                return "Reference equality is not defined for the types '{0}' and '{1}'.";
            }
        }

        internal static string RethrowRequiresCatch
        {
            get
            {
                return "Rethrow statement is valid only inside a Catch block.";
            }
        }

        internal static string SetterHasNoParams
        {
            get
            {
                return "Setter must have parameters.";
            }
        }

        internal static string SetterMustBeVoid
        {
            get
            {
                return "Setter should have void type.";
            }
        }

        internal static string StartEndMustBeOrdered
        {
            get
            {
                return "Start and End must be well ordered";
            }
        }

        internal static string SwitchValueTypeDoesNotMatchComparisonMethodParameter
        {
            get
            {
                return "Switch value of type '{0}' cannot be used for the comparison method parameter of type '{1}'";
            }
        }

        internal static string TestValueTypeDoesNotMatchComparisonMethodParameter
        {
            get
            {
                return "Test value of type '{0}' cannot be used for the comparison method parameter of type '{1}'";
            }
        }

        internal static string TryMustHaveCatchFinallyOrFault
        {
            get
            {
                return "try must have at least one catch, finally, or fault clause";
            }
        }

        internal static string TryNotAllowedInFilter
        {
            get
            {
                return "Try expression is not allowed inside a filter body.";
            }
        }

        internal static string TryNotSupportedForMethodsWithRefArgs
        {
            get
            {
                return "TryExpression is not supported as an argument to method '{0}' because it has an argument with by-ref type. Construct the tree so the TryExpression is not nested inside of this expression.";
            }
        }

        internal static string TryNotSupportedForValueTypeInstances
        {
            get
            {
                return "TryExpression is not supported as a child expression when accessing a member on type '{0}' because it is a value type. Construct the tree so the TryExpression is not nested inside of this expression.";
            }
        }

        internal static string TypeContainsGenericParameters
        {
            get
            {
                return "Type {0} contains generic parameters";
            }
        }

        internal static string TypeDoesNotHaveConstructorForTheSignature
        {
            get
            {
                return "Type doesn't have constructor with a given signature";
            }
        }

        internal static string TypeIsGeneric
        {
            get
            {
                return "Type {0} is a generic type definition";
            }
        }

        internal static string TypeMissingDefaultConstructor
        {
            get
            {
                return "Type '{0}' does not have a default constructor";
            }
        }

        internal static string TypeMustNotBeByRef
        {
            get
            {
                return "type must not be ByRef";
            }
        }

        internal static string TypeNotIEnumerable
        {
            get
            {
                return "Type '{0}' is not IEnumerable";
            }
        }

        internal static string UnaryOperatorNotDefined
        {
            get
            {
                return "The unary operator {0} is not defined for the type '{1}'.";
            }
        }

        internal static string UndefinedVariable
        {
            get
            {
                return "variable '{0}' of type '{1}' referenced from scope '{2}', but it is not defined";
            }
        }

        internal static string UnexpectedCoalesceOperator
        {
            get
            {
                return "Unexpected coalesce operator.";
            }
        }

        internal static string UnexpectedVarArgsCall
        {
            get
            {
                return "Unexpected VarArgs call to method '{0}'";
            }
        }

        internal static string UnhandledBinary
        {
            get
            {
                return "Unhandled binary: {0}";
            }
        }

        internal static string UnhandledBinding
        {
            get
            {
                return "Unhandled binding ";
            }
        }

        internal static string UnhandledBindingType
        {
            get
            {
                return "Unhandled Binding Type: {0}";
            }
        }

        internal static string UnhandledConvert
        {
            get
            {
                return "Unhandled convert: {0}";
            }
        }

        internal static string UnhandledExpressionType
        {
            get
            {
                return "Unhandled Expression Type: {0}";
            }
        }

        internal static string UnhandledUnary
        {
            get
            {
                return "Unhandled unary: {0}";
            }
        }

        internal static string UnknownBindingType
        {
            get
            {
                return "Unknown binding type";
            }
        }

        internal static string UnknownLiftType
        {
            get
            {
                return "unknown lift type: '{0}'.";
            }
        }

        internal static string UnsupportedExpressionType
        {
            get
            {
                return "The expression type '{0}' is not supported";
            }
        }

        internal static string UserDefinedOperatorMustBeStatic
        {
            get
            {
                return "User-defined operator method '{0}' must be static.";
            }
        }

        internal static string UserDefinedOperatorMustNotBeVoid
        {
            get
            {
                return "User-defined operator method '{0}' must not be void.";
            }
        }

        internal static string UserDefinedOpMustHaveConsistentTypes
        {
            get
            {
                return "The user-defined operator method '{1}' for operator '{0}' must have identical parameter and return types.";
            }
        }

        internal static string UserDefinedOpMustHaveValidReturnType
        {
            get
            {
                return "The user-defined operator method '{1}' for operator '{0}' must return the same type as its parameter or a derived type.";
            }
        }

        internal static string VariableMustNotBeByRef
        {
            get
            {
                return "Variable '{0}' uses unsupported type '{1}'. Reference types are not supported for variables.";
            }
        }

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