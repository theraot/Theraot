// Needed for NET40

using System;

namespace Theraot.Core
{
    public static partial class NumericHelper
    {
        public static bool IsZero(double value)
        {
            var test = unchecked((ulong)BitConverter.DoubleToInt64Bits(value));
            return test == 0x0000000000000000ul || test == 0x8000000000000000ul;
        }

        public static bool IsZero(float value)
        {
            var test = unchecked(SingleAsUInt32(value));
            return test == 0x00000000 || test == 0x80000000;
        }

        public static bool IsOne(double value)
        {
            var test = unchecked((ulong)BitConverter.DoubleToInt64Bits(value));
            return test == 0x3ff0000000000000ul;
        }

        public static bool IsOne(float value)
        {
            var test = unchecked(SingleAsUInt32(value));
            return test == 0x3f800000;
        }
    }
}
