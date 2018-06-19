// Needed for NET40

using System;

namespace Theraot.Core
{
    public static partial class NumericHelper
    {
        public static bool AreBinaryEquals(double left, double right)
        {
            var leftBits = unchecked((ulong)BitConverter.DoubleToInt64Bits(left));
            var rightBits = unchecked((ulong)BitConverter.DoubleToInt64Bits(right));
            return leftBits == rightBits;
        }

        public static bool AreBinaryEquals(float left, float right)
        {
            var leftBits = SingleAsUInt32(left);
            var rightBits = SingleAsUInt32(right);
            return leftBits == rightBits;
        }

        public static bool IsOne(double value)
        {
            var test = unchecked((ulong)BitConverter.DoubleToInt64Bits(value));
            return test == 0x3ff0000000000000ul;
        }

        public static bool IsOne(float value)
        {
            var test = SingleAsUInt32(value);
            return test == 0x3f800000;
        }

        public static bool IsZero(double value)
        {
            var test = unchecked((ulong)BitConverter.DoubleToInt64Bits(value));
            return test == 0x0000000000000000ul || test == 0x8000000000000000ul;
        }

        public static bool IsZero(float value)
        {
            var test = SingleAsUInt32(value);
            return test == 0x00000000 || test == 0x80000000;
        }
    }
}