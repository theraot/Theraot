// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal enum AnalyzeTypeIsResult
    {
        KnownFalse,
        KnownTrue,
        KnownAssignable, // need null check only
        Unknown,         // need full runtime check
    }

    internal static class ConstantCheck
    {
        internal static bool IsNull(Expression e)
        {
            if (e.NodeType == ExpressionType.Constant)
            {
                return ((ConstantExpression)e).Value == null;
            }
            return false;
        }

        internal static AnalyzeTypeIsResult AnalyzeTypeIs(TypeBinaryExpression typeIs)
        {
            return AnalyzeTypeIs(typeIs.Expression, typeIs.TypeOperand);
        }

        private static AnalyzeTypeIsResult AnalyzeTypeIs(Expression operand, Type testType)
        {
            var operandType = operand.Type;

            // Oddly, we allow void operands
            // This is LinqV1 behavior of TypeIs
            if (operandType == typeof(void))
            {
                return AnalyzeTypeIsResult.KnownFalse;
            }

            //
            // Type comparisons treat nullable types as if they were the
            // underlying type. The reason is when you box a nullable it
            // becomes a boxed value of the underlying type, or null.
            //
            var nnOperandType = operandType.GetNonNullableType();
            var nnTestType = testType.GetNonNullableType();

            //
            // See if we can determine the answer based on the static types
            //
            // Extensive testing showed that Type.IsAssignableFrom,
            // Type.IsInstanceOfType, and the isinst instruction were all
            // equivalent when used against a live object
            //
            if (nnTestType.IsAssignableFrom(nnOperandType))
            {
                // If the operand is a value type (other than nullable), we
                // know the result is always true.
                if (operandType.IsValueType && !operandType.IsNullableType())
                {
                    return AnalyzeTypeIsResult.KnownTrue;
                }

                // For reference/nullable types, we need to compare to null at runtime
                return AnalyzeTypeIsResult.KnownAssignable;
            }

            // We used to have an if IsSealed, return KnownFalse check here.
            // but that doesn't handle generic types & co/contravariance correctly.
            // So just use IsInst, which we know always gives us the right answer.

            // Otherwise we need a full runtime check
            return AnalyzeTypeIsResult.Unknown;
        }
    }
}