using System;
using System.Collections.Generic;
using System.Text;

namespace Theraot.Core
{
    public static partial class NumericHelper
    {
        public static string ToStringBinary(this double value)
        {
            return ToStringBinary(BitConverter.DoubleToInt64Bits(value));
        }

        public static string ToStringBinary(this float value)
        {
            return ToStringBinary(BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this sbyte value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        public static string ToStringBinary(this short value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        public static string ToStringBinary(this int value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        public static string ToStringBinary(this long value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        public static string ToStringBinary(this byte value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this ushort value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this uint value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this ulong value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }
    }
}
