#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions
{
    /// <summary>
    /// Describes the node types for the nodes of an expression tree.
    /// </summary>
    public enum ExpressionType
    {
        /// <summary>
        /// A node that represents arithmetic addition without overflow checking.
        /// </summary>
        Add = 0,

        /// <summary>
        /// A node that represents arithmetic addition with overflow checking.
        /// </summary>
        AddChecked = 1,

        /// <summary>
        /// A node that represents a bitwise AND operation.
        /// </summary>
        And = 2,

        /// <summary>
        /// A node that represents a short-circuiting conditional AND operation.
        /// </summary>
        AndAlso = 3,

        /// <summary>
        /// A node that represents getting the length of a one-dimensional array.
        /// </summary>
        ArrayLength = 4,

        /// <summary>
        /// A node that represents indexing into a one-dimensional array.
        /// </summary>
        ArrayIndex = 5,

        /// <summary>
        /// A node that represents a method call.
        /// </summary>
        Call = 6,

        /// <summary>
        /// A node that represents a null coalescing operation.
        /// </summary>
        Coalesce = 7,

        /// <summary>
        /// A node that represents a conditional operation.
        /// </summary>
        Conditional = 8,

        /// <summary>
        /// A node that represents an expression that has a constant value.
        /// </summary>
        Constant = 9,

        /// <summary>
        /// A node that represents a cast or conversion operation. If the operation is a numeric conversion, it overflows silently if the converted value does not fit the target type.
        /// </summary>
        Convert = 10,

        /// <summary>
        /// A node that represents a cast or conversion operation. If the operation is a numeric conversion, an exception is thrown if the converted value does not fit the target type.
        /// </summary>
        ConvertChecked = 11,

        /// <summary>
        /// A node that represents arithmetic division.
        /// </summary>
        Divide = 12,

        /// <summary>
        /// A node that represents an equality comparison.
        /// </summary>
        Equal = 13,

        /// <summary>
        /// A node that represents a bitwise XOR operation.
        /// </summary>
        ExclusiveOr = 14,

        /// <summary>
        /// A node that represents a "greater than" numeric comparison.
        /// </summary>
        GreaterThan = 15,

        /// <summary>
        /// A node that represents a "greater than or equal" numeric comparison.
        /// </summary>
        GreaterThanOrEqual = 16,

        /// <summary>
        /// A node that represents applying a delegate or lambda expression to a list of argument expressions.
        /// </summary>
        Invoke = 17,

        /// <summary>
        /// A node that represents a lambda expression.
        /// </summary>
        Lambda = 18,

        /// <summary>
        /// A node that represents a bitwise left-shift operation.
        /// </summary>
        LeftShift = 19,

        /// <summary>
        /// A node that represents a "less than" numeric comparison.
        /// </summary>
        LessThan = 20,

        /// <summary>
        /// A node that represents a "less than or equal" numeric comparison.
        /// </summary>
        LessThanOrEqual = 21,

        /// <summary>
        /// A node that represents creating a new IEnumerable object and initializing it from a list of elements.
        /// </summary>
        ListInit = 22,

        /// <summary>
        /// A node that represents reading from a field or property.
        /// </summary>
        MemberAccess = 23,

        /// <summary>
        /// A node that represents creating a new object and initializing one or more of its members.
        /// </summary>
        MemberInit = 24,

        /// <summary>
        /// A node that represents an arithmetic remainder operation.
        /// </summary>
        Modulo = 25,

        /// <summary>
        /// A node that represents arithmetic multiplication without overflow checking.
        /// </summary>
        Multiply = 26,

        /// <summary>
        /// A node that represents arithmetic multiplication with overflow checking.
        /// </summary>
        MultiplyChecked = 27,

        /// <summary>
        /// A node that represents an arithmetic negation operation.
        /// </summary>
        Negate = 28,

        /// <summary>
        /// A node that represents a unary plus operation. The result of a predefined unary plus operation is simply the value of the operand, but user-defined implementations may have non-trivial results.
        /// </summary>
        UnaryPlus = 29,

        /// <summary>
        /// A node that represents an arithmetic negation operation that has overflow checking.
        /// </summary>
        NegateChecked = 30,

        /// <summary>
        /// A node that represents calling a constructor to create a new object.
        /// </summary>
        New = 31,

        /// <summary>
        /// A node that represents creating a new one-dimensional array and initializing it from a list of elements.
        /// </summary>
        NewArrayInit = 32,

        /// <summary>
        /// A node that represents creating a new array where the bounds for each dimension are specified.
        /// </summary>
        NewArrayBounds = 33,

        /// <summary>
        /// A node that represents a bitwise complement operation.
        /// </summary>
        Not = 34,

        /// <summary>
        /// A node that represents an inequality comparison.
        /// </summary>
        NotEqual = 35,

        /// <summary>
        /// A node that represents a bitwise OR operation.
        /// </summary>
        Or = 36,

        /// <summary>
        /// A node that represents a short-circuiting conditional OR operation.
        /// </summary>
        OrElse = 37,

        /// <summary>
        /// A node that represents a reference to a parameter or variable defined in the context of the expression.
        /// </summary>
        Parameter = 38,

        /// <summary>
        /// A node that represents raising a number to a power.
        /// </summary>
        Power = 39,

        /// <summary>
        /// A node that represents an expression that has a constant value of type Expression. A Quote node can contain references to parameters defined in the context of the expression it represents.
        /// </summary>
        Quote = 40,

        /// <summary>
        /// A node that represents a bitwise right-shift operation.
        /// </summary>
        RightShift = 41,

        /// <summary>
        /// A node that represents arithmetic subtraction without overflow checking.
        /// </summary>
        Subtract = 42,

        /// <summary>
        /// A node that represents arithmetic subtraction with overflow checking.
        /// </summary>
        SubtractChecked = 43,

        /// <summary>
        /// A node that represents an explicit reference or boxing conversion where null reference (Nothing in Visual Basic) is supplied if the conversion fails.
        /// </summary>
        TypeAs = 44,

        /// <summary>
        /// A node that represents a type test.
        /// </summary>
        TypeIs = 45,

        /// <summary>
        /// A node that represents an assignment.
        /// </summary>
        Assign = 46,

        /// <summary>
        /// A node that represents a block of expressions.
        /// </summary>
        Block = 47,

        /// <summary>
        /// A node that represents a debugging information.
        /// </summary>
        DebugInfo = 48,

        /// <summary>
        /// A node that represents a unary decrement.
        /// </summary>
        Decrement = 49,

        /// <summary>
        /// A node that represents a dynamic operation.
        /// </summary>
        Dynamic = 50,

        /// <summary>
        /// A node that represents a default value.
        /// </summary>
        Default = 51,

        /// <summary>
        /// A node that represents an extension expression.
        /// </summary>
        Extension = 52,

        /// <summary>
        /// A node that represents a goto.
        /// </summary>
        Goto = 53,

        /// <summary>
        /// A node that represents a unary increment.
        /// </summary>
        Increment = 54,

        /// <summary>
        /// A node that represents an index operation.
        /// </summary>
        Index = 55,

        /// <summary>
        /// A node that represents a label.
        /// </summary>
        Label = 56,

        /// <summary>
        /// A node that represents a list of runtime variables.
        /// </summary>
        RuntimeVariables = 57,

        /// <summary>
        /// A node that represents a loop.
        /// </summary>
        Loop = 58,

        /// <summary>
        /// A node that represents a switch operation.
        /// </summary>
        Switch = 59,

        /// <summary>
        /// A node that represents a throwing of an exception.
        /// </summary>
        Throw = 60,

        /// <summary>
        /// A node that represents a try-catch expression.
        /// </summary>
        Try = 61,

        /// <summary>
        /// A node that represents an unbox value type operation.
        /// </summary>
        Unbox = 62,

        /// <summary>
        /// A node that represents an arithmetic addition compound assignment without overflow checking.
        /// </summary>
        AddAssign = 63,

        /// <summary>
        /// A node that represents a bitwise AND compound assignment.
        /// </summary>
        AndAssign = 64,

        /// <summary>
        /// A node that represents an arithmetic division compound assignment .
        /// </summary>
        DivideAssign = 65,

        /// <summary>
        /// A node that represents a bitwise XOR compound assignment.
        /// </summary>
        ExclusiveOrAssign = 66,

        /// <summary>
        /// A node that represents a bitwise left-shift compound assignment.
        /// </summary>
        LeftShiftAssign = 67,

        /// <summary>
        /// A node that represents an arithmetic remainder compound assignment.
        /// </summary>
        ModuloAssign = 68,

        /// <summary>
        /// A node that represents arithmetic multiplication compound assignment without overflow checking.
        /// </summary>
        MultiplyAssign = 69,

        /// <summary>
        /// A node that represents a bitwise OR compound assignment.
        /// </summary>
        OrAssign = 70,

        /// <summary>
        /// A node that represents raising a number to a power compound assignment.
        /// </summary>
        PowerAssign = 71,

        /// <summary>
        /// A node that represents a bitwise right-shift compound assignment.
        /// </summary>
        RightShiftAssign = 72,

        /// <summary>
        /// A node that represents arithmetic subtraction compound assignment without overflow checking.
        /// </summary>
        SubtractAssign = 73,

        /// <summary>
        /// A node that represents an arithmetic addition compound assignment with overflow checking.
        /// </summary>
        AddAssignChecked = 74,

        /// <summary>
        /// A node that represents arithmetic multiplication compound assignment with overflow checking.
        /// </summary>
        MultiplyAssignChecked = 75,

        /// <summary>
        /// A node that represents arithmetic subtraction compound assignment with overflow checking.
        /// </summary>
        SubtractAssignChecked = 76,

        /// <summary>
        /// A node that represents an unary prefix increment.
        /// </summary>
        PreIncrementAssign = 77,

        /// <summary>
        /// A node that represents an unary prefix decrement.
        /// </summary>
        PreDecrementAssign = 78,

        /// <summary>
        /// A node that represents an unary postfix increment.
        /// </summary>
        PostIncrementAssign = 79,

        /// <summary>
        /// A node that represents an unary postfix decrement.
        /// </summary>
        PostDecrementAssign = 80,

        /// <summary>
        /// A node that represents an exact type test.
        /// </summary>
        TypeEqual = 81,

        /// <summary>
        /// A node that represents a ones complement.
        /// </summary>
        OnesComplement = 82,

        /// <summary>
        /// A node that represents a true condition value.
        /// </summary>
        IsTrue = 83,

        /// <summary>
        /// A node that represents a false condition value.
        /// </summary>
        IsFalse = 84
    }
}

#endif