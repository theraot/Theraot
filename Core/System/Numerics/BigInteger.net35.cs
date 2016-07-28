#if NET35

using System.Globalization;
using Theraot.Core;

namespace System.Numerics
{
    /// <summary>Represents an arbitrarily large signed integer.</summary>
    [Serializable]
    public partial struct BigInteger : IFormattable, IComparable, IComparable<BigInteger>, IEquatable<BigInteger>
    {
        internal readonly int InternalSign;

        internal readonly uint[] InternalBits;

        private readonly static BigInteger _sBnMinInt;

        private readonly static BigInteger _sBnOneInt;

        private readonly static BigInteger _sBnZeroInt;

        private readonly static BigInteger _sBnMinusOneInt;

        /// <summary>Indicates whether the value of the current <see cref="T:System.Numerics.BigInteger" /> object is an even number.</summary>
        /// <returns>true if the value of the <see cref="T:System.Numerics.BigInteger" /> object is an even number; otherwise, false.</returns>
        public bool IsEven
        {
            get
            {
                return (InternalBits != null ? (InternalBits[0] & 1) == 0 : (InternalSign & 1) == 0);
            }
        }

        /// <summary>Indicates whether the value of the current <see cref="T:System.Numerics.BigInteger" /> object is <see cref="P:System.Numerics.BigInteger.One" />.</summary>
        /// <returns>true if the value of the <see cref="T:System.Numerics.BigInteger" /> object is <see cref="P:System.Numerics.BigInteger.One" />; otherwise, false.</returns>
        public bool IsOne
        {
            get
            {
                return InternalSign == 1 && InternalBits == null;
            }
        }

        /// <summary>Indicates whether the value of the current <see cref="T:System.Numerics.BigInteger" /> object is a power of two.</summary>
        /// <returns>true if the value of the <see cref="T:System.Numerics.BigInteger" /> object is a power of two; otherwise, false.</returns>
        public bool IsPowerOfTwo
        {
            get
            {
                if (InternalBits == null)
                {
                    return (InternalSign & (InternalSign - 1)) == 0 && InternalSign != 0;
                }
                if (InternalSign != 1)
                {
                    return false;
                }
                var index = Length(InternalBits) - 1;
                if ((InternalBits[index] & InternalBits[index] - 1) != 0)
                {
                    return false;
                }
                do
                {
                    index = index - 1;
                    if (index >= 0)
                    {
                        continue;
                    }
                    return true;
                }
                while (InternalBits[index] == 0);
                return false;
            }
        }

        /// <summary>Indicates whether the value of the current <see cref="T:System.Numerics.BigInteger" /> object is <see cref="P:System.Numerics.BigInteger.Zero" />.</summary>
        /// <returns>true if the value of the <see cref="T:System.Numerics.BigInteger" /> object is <see cref="P:System.Numerics.BigInteger.Zero" />; otherwise, false.</returns>
        public bool IsZero
        {
            get
            {
                return InternalSign == 0;
            }
        }

        /// <summary>Gets a value that represents the number negative one (-1).</summary>
        /// <returns>An integer whose value is negative one (-1).</returns>
        public static BigInteger MinusOne
        {
            get
            {
                return _sBnMinusOneInt;
            }
        }

        /// <summary>Gets a value that represents the number one (1).</summary>
        /// <returns>An object whose value is one (1).</returns>
        public static BigInteger One
        {
            get
            {
                return _sBnOneInt;
            }
        }

        /// <summary>Gets a number that indicates the sign (negative, positive, or zero) of the current <see cref="T:System.Numerics.BigInteger" /> object.</summary>
        /// <returns>A number that indicates the sign of the <see cref="T:System.Numerics.BigInteger" /> object, as shown in the following table.NumberDescription-1The value of this object is negative.0The value of this object is 0 (zero).1The value of this object is positive.</returns>
        public int Sign
        {
            get
            {
                return (InternalSign >> 31) - (-InternalSign >> 31);
            }
        }

        /// <summary>Gets a value that represents the number 0 (zero).</summary>
        /// <returns>An integer whose value is 0 (zero).</returns>
        public static BigInteger Zero
        {
            get
            {
                return _sBnZeroInt;
            }
        }

        static BigInteger()
        {
            _sBnMinInt = new BigInteger(-1, new [] { unchecked ((uint)int.MinValue) });
            _sBnOneInt = new BigInteger(1);
            _sBnZeroInt = new BigInteger(0);
            _sBnMinusOneInt = new BigInteger(-1);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using a 32-bit signed integer value.</summary>
        /// <param name="value">A 32-bit signed integer.</param>
        public BigInteger(int value)
        {
            if (value != int.MinValue)
            {
                InternalSign = value;
                InternalBits = null;
            }
            else
            {
                this = _sBnMinInt;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using an unsigned 32-bit integer value.</summary>
        /// <param name="value">An unsigned 32-bit integer value.</param>
        [CLSCompliant(false)]
        public BigInteger(uint value)
        {
            if (value > int.MaxValue)
            {
                InternalSign = 1;
                InternalBits = new[] { value };
            }
            else
            {
                InternalSign = (int)value;
                InternalBits = null;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using a 64-bit signed integer value.</summary>
        /// <param name="value">A 64-bit signed integer.</param>
        public BigInteger(long value)
        {
            if (int.MinValue <= value && value <= int.MaxValue)
            {
                if (value != int.MinValue)
                {
                    InternalSign = (int)value;
                    InternalBits = null;
                }
                else
                {
                    this = _sBnMinInt;
                }
                return;
            }
            ulong num;
            if (value >= 0)
            {
                num = (ulong)value;
                InternalSign = 1;
            }
            else
            {
                num = (ulong)(-value);
                InternalSign = -1;
            }
            InternalBits = new[] { (uint)num, (uint)(num >> 32) };
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure with an unsigned 64-bit integer value.</summary>
        /// <param name="value">An unsigned 64-bit integer.</param>
        [CLSCompliant(false)]
        public BigInteger(ulong value)
        {
            if (value > int.MaxValue)
            {
                InternalSign = 1;
                InternalBits = new[] { (uint)value, (uint)(value >> 32) };
            }
            else
            {
                InternalSign = (int)value;
                InternalBits = null;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using a single-precision floating-point value.</summary>
        /// <param name="value">A single-precision floating-point value.</param>
        /// <exception cref="T:System.OverflowException">The value of <paramref name="value" /> is <see cref="F:System.Single.NaN" />.-or-The value of <paramref name="value" /> is <see cref="F:System.Single.NegativeInfinity" />.-or-The value of <paramref name="value" /> is <see cref="F:System.Single.PositiveInfinity" />.</exception>
        public BigInteger(float value)
        {
            if (float.IsInfinity(value))
            {
                throw new OverflowException("BigInteger cannot represent infinity.");
            }
            if (float.IsNaN(value))
            {
                throw new OverflowException("The value is not a number.");
            }
            SetBitsFromDouble(value, out InternalBits, out InternalSign);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using a double-precision floating-point value.</summary>
        /// <param name="value">A double-precision floating-point value.</param>
        /// <exception cref="T:System.OverflowException">The value of <paramref name="value" /> is <see cref="F:System.Double.NaN" />.-or-The value of <paramref name="value" /> is <see cref="F:System.Double.NegativeInfinity" />.-or-The value of <paramref name="value" /> is <see cref="F:System.Double.PositiveInfinity" />.</exception>
        public BigInteger(double value)
        {
            if (double.IsInfinity(value))
            {
                throw new OverflowException("BigInteger cannot represent infinity.");
            }
            if (double.IsNaN(value))
            {
                throw new OverflowException("The value is not a number.");
            }
            SetBitsFromDouble(value, out InternalBits, out InternalSign);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using a <see cref="T:System.Decimal" /> value.</summary>
        /// <param name="value">A decimal number.</param>
        public BigInteger(decimal value)
        {
            var bits = decimal.GetBits(decimal.Truncate(value));
            var size = 3;
            while (size > 0 && bits[size - 1] == 0)
            {
                size--;
            }
            if (size == 0)
            {
                this = _sBnZeroInt;
            }
            else if (size != 1 || bits[0] <= 0)
            {
                InternalBits = new uint[size];
                InternalBits[0] = (uint)bits[0];
                if (size > 1)
                {
                    InternalBits[1] = (uint)bits[1];
                }
                if (size > 2)
                {
                    InternalBits[2] = (uint)bits[2];
                }
                InternalSign = ((bits[3] & int.MinValue) == 0 ? 1 : -1);
            }
            else
            {
                InternalSign = bits[0];
                InternalSign = InternalSign * ((bits[3] & int.MinValue) == 0 ? 1 : -1);
                InternalBits = null;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Numerics.BigInteger" /> structure using the values in a byte array.</summary>
        /// <param name="value">An array of byte values in little-endian order.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        [CLSCompliant(false)]
        public BigInteger(byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            var valueLength = value.Length;
            var isNegative = (valueLength > 0 && (value[valueLength - 1] & 128) == 128);
            while (valueLength > 0 && value[valueLength - 1] == 0)
            {
                valueLength--;
            }
            if (valueLength == 0)
            {
                InternalSign = 0;
                InternalBits = null;
                return;
            }
            if (valueLength > 4)
            {
                var unalignedBytes = valueLength%4;
                var dwordCount = valueLength/4 + (unalignedBytes == 0 ? 0 : 1);
                var isZero = true;
                var internalBits = new uint[dwordCount];
                var byteIndex = 3;
                var dwordIndex = 0;
                for (; dwordIndex < dwordCount - (unalignedBytes == 0 ? 0 : 1); dwordIndex++)
                {
                    for (var byteInDword = 0; byteInDword < 4; byteInDword++)
                    {
                        if (value[byteIndex] != 0)
                        {
                            isZero = false;
                        }
                        internalBits[dwordIndex] <<= 8;
                        internalBits[dwordIndex] |= value[byteIndex];
                        byteIndex--;
                    }
                    byteIndex += 8;
                }
                if (unalignedBytes != 0)
                {
                    if (isNegative)
                    {
                        internalBits[dwordCount - 1] = 0xffffffff;
                    }
                    for (byteIndex = valueLength - 1; byteIndex >= valueLength - unalignedBytes; byteIndex--)
                    {
                        if (value[byteIndex] != 0)
                        {
                            isZero = false;
                        }
                        internalBits[dwordIndex] <<= 8;
                        internalBits[dwordIndex] |= value[byteIndex];
                    }
                }
                if (isZero)
                {
                    this = _sBnZeroInt;
                }
                else if (isNegative)
                {
                    NumericsHelpers.DangerousMakeTwosComplement(internalBits);
                    dwordCount = internalBits.Length;
                    while (dwordCount > 0 && internalBits[dwordCount - 1] == 0)
                    {
                        dwordCount--;
                    }
                    if (dwordCount == 1 && internalBits[0] > 0)
                    {
                        switch (internalBits[0])
                        {
                            case 1:
                                this = _sBnMinusOneInt;
                                break;
                            case unchecked((uint) int.MinValue):
                                this = _sBnMinInt;
                                break;
                            default:
                                InternalSign = -1;
                                InternalBits = internalBits;
                                break;
                        }
                    }
                    else
                    {
                        if (dwordCount == internalBits.Length)
                        {
                            InternalSign = -1;
                            InternalBits = internalBits;
                        }
                        else
                        {
                            InternalSign = -1;
                            InternalBits = new uint[dwordCount];
                            Array.Copy(internalBits, InternalBits, dwordCount);
                        }
                    }
                }
                else
                {
                    InternalSign = 1;
                    InternalBits = internalBits;
                }
            }
            else
            {
                if (isNegative)
                {
                    InternalSign = -1;
                }
                else
                {
                    InternalSign = 0;
                }
                for (var index = valueLength - 1; index >= 0; index--)
                {
                    InternalSign <<= 8;
                    InternalSign |= value[index];
                }
                InternalBits = null;
                if (InternalSign < 0 && !isNegative)
                {
                    InternalBits = new uint[1];
                    InternalBits[0] = (uint)InternalSign;
                    InternalSign = 1;
                }
                if (InternalSign == int.MinValue)
                {
                    this = _sBnMinInt;
                }
            }
        }

        internal BigInteger(int internalSign, uint[] rgu)
        {
            InternalSign = internalSign;
            InternalBits = rgu;
        }

        internal BigInteger(uint[] value, bool negative)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            var length = value.Length;
            while (length > 0 && value[length - 1] == 0)
            {
                length--;
            }
            if (length == 0)
            {
                this = _sBnZeroInt;
            }
            else if (length != 1 || value[0] >= unchecked((uint)int.MinValue))
            {
                InternalSign = (!negative ? 1 : -1);
                InternalBits = new uint[length];
                Array.Copy(value, InternalBits, length);
            }
            else
            {
                InternalSign = (!negative ? (int)value[0] : (int)(-value[0]));
                InternalBits = null;
                if (InternalSign == int.MinValue)
                {
                    this = _sBnMinInt;
                }
            }
        }

        private BigInteger(uint[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            var dwordCount = value.Length;
            var isNegative = (dwordCount > 0 && (value[dwordCount - 1] & unchecked((uint)int.MinValue)) == unchecked((uint)int.MinValue));
            while (dwordCount > 0 && value[dwordCount - 1] == 0)
            {
                dwordCount--;
            }
            if (dwordCount == 0)
            {
                this = _sBnZeroInt;
                return;
            }
            if (dwordCount == 1)
            {
                if ((int)value[0] < 0 && !isNegative)
                {
                    InternalBits = new[] { value[0] };
                    InternalSign = 1;
                }
                else if (value[0] != unchecked((uint)int.MinValue))
                {
                    InternalSign = (int)value[0];
                    InternalBits = null;
                }
                else
                {
                    this = _sBnMinInt;
                }
                return;
            }
            if (!isNegative)
            {
                if (dwordCount == value.Length)
                {
                    InternalSign = 1;
                    InternalBits = value;
                }
                else
                {
                    InternalSign = 1;
                    InternalBits = new uint[dwordCount];
                    Array.Copy(value, InternalBits, dwordCount);
                }
                return;
            }
            NumericsHelpers.DangerousMakeTwosComplement(value);
            var length = value.Length;
            while (length > 0 && value[length - 1] == 0)
            {
                length--;
            }
            if (length != 1 || value[0] <= 0)
            {
                if (length == value.Length)
                {
                    InternalSign = -1;
                    InternalBits = value;
                }
                else
                {
                    InternalSign = -1;
                    InternalBits = new uint[length];
                    Array.Copy(value, InternalBits, length);
                }
            }
            else if (value[0] == 1)
            {
                this = _sBnMinusOneInt;
            }
            else if (value[0] != unchecked((uint)int.MinValue))
            {
                InternalSign = (int)(-1 * value[0]);
                InternalBits = null;
            }
            else
            {
                this = _sBnMinInt;
            }
        }

        /// <summary>Gets the absolute value of a <see cref="T:System.Numerics.BigInteger" /> object.</summary>
        /// <returns>The absolute value of <paramref name="value" />.</returns>
        /// <param name="value">A number.</param>
        public static BigInteger Abs(BigInteger value)
        {
            return (value < Zero ? -value : value);
        }

        /// <summary>Adds two <see cref="T:System.Numerics.BigInteger" /> values and returns the result.</summary>
        /// <returns>The sum of <paramref name="left" /> and <paramref name="right" />.</returns>
        /// <param name="left">The first value to add.</param>
        /// <param name="right">The second value to add.</param>
        public static BigInteger Add(BigInteger left, BigInteger right)
        {
            return left + right;
        }

        internal static int BitLengthOfUInt(uint x)
        {
            var numBits = 0;
            while (x > 0)
            {
                x = x >> 1;
                numBits++;
            }
            return numBits;
        }

        /// <summary>Compares two <see cref="T:System.Numerics.BigInteger" /> values and returns an integer that indicates whether the first value is less than, equal to, or greater than the second value.</summary>
        /// <returns>A signed integer that indicates the relative values of <paramref name="left" /> and <paramref name="right" />, as shown in the following table.ValueConditionLess than zero<paramref name="left" /> is less than <paramref name="right" />.Zero<paramref name="left" /> equals <paramref name="right" />.Greater than zero<paramref name="left" /> is greater than <paramref name="right" />.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static int Compare(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right);
        }

        /// <summary>Compares this instance to a signed 64-bit integer and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the signed 64-bit integer.</summary>
        /// <returns>A signed integer value that indicates the relationship of this instance to <paramref name="other" />, as shown in the following table.Return valueDescriptionLess than zeroThe current instance is less than <paramref name="other" />.ZeroThe current instance equals <paramref name="other" />.Greater than zeroThe current instance is greater than <paramref name="other" />.</returns>
        /// <param name="other">The signed 64-bit integer to compare.</param>
        public int CompareTo(long other)
        {
            if (InternalBits == null)
            {
                return ((long)InternalSign).CompareTo(other);
            }
            if ((InternalSign ^ other) >= 0)
            {
                var length = Length(InternalBits);
                if (length <= 2)
                {
                    var magnitude = (other >= 0 ? (ulong)other : (ulong)(-other));
                    var unsigned = (length != 2 ? InternalBits[0] : NumericsHelpers.MakeUlong(InternalBits[1], InternalBits[0]));
                    return InternalSign * unsigned.CompareTo(magnitude);
                }
            }
            return InternalSign;
        }

        /// <summary>Compares this instance to an unsigned 64-bit integer and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the unsigned 64-bit integer.</summary>
        /// <returns>A signed integer that indicates the relative value of this instance and <paramref name="other" />, as shown in the following table.Return valueDescriptionLess than zeroThe current instance is less than <paramref name="other" />.ZeroThe current instance equals <paramref name="other" />.Greater than zeroThe current instance is greater than <paramref name="other" />.</returns>
        /// <param name="other">The unsigned 64-bit integer to compare.</param>
        [CLSCompliant(false)]
        public int CompareTo(ulong other)
        {
            if (InternalSign < 0)
            {
                return -1;
            }
            if (InternalBits == null)
            {
                return ((ulong)InternalSign).CompareTo(other);
            }
            var length = Length(InternalBits);
            if (length > 2)
            {
                return 1;
            }
            return ((length != 2 ? InternalBits[0] : NumericsHelpers.MakeUlong(InternalBits[1], InternalBits[0]))).CompareTo(other);
        }

        /// <summary>Compares this instance to a second <see cref="T:System.Numerics.BigInteger" /> and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified object.</summary>
        /// <returns>A signed integer value that indicates the relationship of this instance to <paramref name="other" />, as shown in the following table.Return valueDescriptionLess than zeroThe current instance is less than <paramref name="other" />.ZeroThe current instance equals <paramref name="other" />.Greater than zeroThe current instance is greater than <paramref name="other" />.</returns>
        /// <param name="other">The object to compare.</param>
        public int CompareTo(BigInteger other)
        {
            if ((InternalSign ^ other.InternalSign) < 0)
            {
                return (InternalSign >= 0 ? 1 : -1);
            }
            if (InternalBits == null)
            {
                if (other.InternalBits != null)
                {
                    return -other.InternalSign;
                }
                int result;
                if (InternalSign >= other.InternalSign)
                {
                    result = (InternalSign <= other.InternalSign ? 0 : 1);
                }
                else
                {
                    result = -1;
                }
                return result;
            }
            if (other.InternalBits != null)
            {
                var length = Length(InternalBits);
                var otherLength = Length(other.InternalBits);
                if (Length(InternalBits) <= otherLength)
                {
                    if (length < otherLength)
                    {
                        return -InternalSign;
                    }
                    var diffLength = GetDiffLength(InternalBits, other.InternalBits, length);
                    if (diffLength == 0)
                    {
                        return 0;
                    }
                    return (InternalBits[diffLength - 1] >= other.InternalBits[diffLength - 1] ? InternalSign : -InternalSign);
                }
            }
            return InternalSign;
        }

        /// <summary>Compares this instance to a specified object and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified object.</summary>
        /// <returns>A signed integer that indicates the relationship of the current instance to the <paramref name="obj" /> parameter, as shown in the following table.Return valueDescriptionLess than zeroThe current instance is less than <paramref name="obj" />.ZeroThe current instance equals <paramref name="obj" />.Greater than zeroThe current instance is greater than <paramref name="obj" />, or the <paramref name="obj" /> parameter is null. </returns>
        /// <param name="obj">The object to compare.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="obj" /> is not a <see cref="T:System.Numerics.BigInteger" />. </exception>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (!(obj is BigInteger))
            {
                throw new ArgumentException("The parameter must be a BigInteger.");
            }
            return CompareTo((BigInteger)obj);
        }

        /// <summary>Divides one <see cref="T:System.Numerics.BigInteger" /> value by another and returns the result.</summary>
        /// <returns>The quotient of the division.</returns>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <exception cref="T:System.DivideByZeroException">
        ///   <paramref name="divisor" /> is 0 (zero).</exception>
        public static BigInteger Divide(BigInteger dividend, BigInteger divisor)
        {
            return dividend / divisor;
        }

        /// <summary>Divides one <see cref="T:System.Numerics.BigInteger" /> value by another, returns the result, and returns the remainder in an output parameter.</summary>
        /// <returns>The quotient of the division.</returns>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <param name="remainder">When this method returns, contains a <see cref="T:System.Numerics.BigInteger" /> value that represents the remainder from the division. This parameter is passed uninitialized.</param>
        /// <exception cref="T:System.DivideByZeroException">
        ///   <paramref name="divisor" /> is 0 (zero).</exception>
        public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder)
        {
            var signNum = 1;
            var signDen = 1;
            var regNum = new BigIntegerBuilder(dividend, ref signNum);
            var regDen = new BigIntegerBuilder(divisor, ref signDen);
            var regQuo = new BigIntegerBuilder();
            regNum.ModDiv(ref regDen, ref regQuo);
            remainder = regNum.GetInteger(signNum);
            return regQuo.GetInteger(signNum * signDen);
        }

        /// <summary>Returns a value that indicates whether the current instance and a specified object have the same value.</summary>
        /// <returns>true if the <paramref name="obj" /> parameter is a <see cref="T:System.Numerics.BigInteger" /> object or a type capable of implicit conversion to a <see cref="T:System.Numerics.BigInteger" /> value, and its value is equal to the value of the current <see cref="T:System.Numerics.BigInteger" /> object; otherwise, false.</returns>
        /// <param name="obj">The object to compare. </param>
        public override bool Equals(object obj)
        {
            if (!(obj is BigInteger))
            {
                return false;
            }
            return Equals((BigInteger)obj);
        }

        /// <summary>Returns a value that indicates whether the current instance and a signed 64-bit integer have the same value.</summary>
        /// <returns>true if the signed 64-bit integer and the current instance have the same value; otherwise, false.</returns>
        /// <param name="other">The signed 64-bit integer value to compare.</param>
        public bool Equals(long other)
        {
            if (InternalBits == null)
            {
                return InternalSign == other;
            }
            if ((InternalSign ^ other) >= 0)
            {
                var length = Length(InternalBits);
                if (length <= 2)
                {
                    var magnitude = (other >= 0 ? (ulong)other : (ulong)(-other));
                    if (length == 1)
                    {
                        return InternalBits[0] == magnitude;
                    }
                    return NumericsHelpers.MakeUlong(InternalBits[1], InternalBits[0]) == magnitude;
                }
            }
            return false;
        }

        /// <summary>Returns a value that indicates whether the current instance and an unsigned 64-bit integer have the same value.</summary>
        /// <returns>true if the current instance and the unsigned 64-bit integer have the same value; otherwise, false.</returns>
        /// <param name="other">The unsigned 64-bit integer to compare.</param>
        [CLSCompliant(false)]
        public bool Equals(ulong other)
        {
            if (InternalSign < 0)
            {
                return false;
            }
            if (InternalBits == null)
            {
                return InternalSign == unchecked((long) other);
            }
            var length = Length(InternalBits);
            if (length > 2)
            {
                return false;
            }
            if (length == 1)
            {
                return InternalBits[0] == other;
            }
            return NumericsHelpers.MakeUlong(InternalBits[1], InternalBits[0]) == other;
        }

        /// <summary>Returns a value that indicates whether the current instance and a specified <see cref="T:System.Numerics.BigInteger" /> object have the same value.</summary>
        /// <returns>true if this <see cref="T:System.Numerics.BigInteger" /> object and <paramref name="other" /> have the same value; otherwise, false.</returns>
        /// <param name="other">The object to compare.</param>
        public bool Equals(BigInteger other)
        {
            if (InternalSign != other.InternalSign)
            {
                return false;
            }
            if (InternalBits == other.InternalBits)
            {
                return true;
            }
            if (InternalBits == null || other.InternalBits == null)
            {
                return false;
            }
            var length = Length(InternalBits);
            if (length != Length(other.InternalBits))
            {
                return false;
            }
            var diffLength = GetDiffLength(InternalBits, other.InternalBits, length);
            return diffLength == 0;
        }

        internal static int GetDiffLength(uint[] internalBits, uint[] otherInternalBits, int length)
        {
            var index = length;
            do
            {
                index = index - 1;
                if (index >= 0)
                {
                    continue;
                }
                return 0;
            }
            while (internalBits[index] == otherInternalBits[index]);
            return index + 1;
        }

        /// <summary>Returns the hash code for the current <see cref="T:System.Numerics.BigInteger" /> object.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            if (InternalBits == null)
            {
                return InternalSign;
            }
            var sign = InternalSign;
            var index = Length(InternalBits);
            while (true)
            {
                index = index - 1;
                if (index < 0)
                {
                    break;
                }
                sign = NumericsHelpers.CombineHash(sign, (int)InternalBits[index]);
            }
            return sign;
        }

        private static bool GetPartsForBitManipulation(ref BigInteger x, out uint[] xd, out int xl)
        {
            if (x.InternalBits != null)
            {
                xd = x.InternalBits;
            }
            else if (x.InternalSign >= 0)
            {
                xd = new[] { unchecked  ((uint)x.InternalSign) };
            }
            else
            {
                xd = new[] { unchecked((uint)-x.InternalSign)  };
            }
            xl = (x.InternalBits != null ? x.InternalBits.Length : 1);
            return x.InternalSign < 0;
        }

        /// <summary>Finds the greatest common divisor of two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
        /// <returns>The greatest common divisor of <paramref name="left" /> and <paramref name="right" />.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        public static BigInteger GreatestCommonDivisor(BigInteger left, BigInteger right)
        {
            if (left.InternalSign == 0)
            {
                return Abs(right);
            }
            if (right.InternalSign == 0)
            {
                return Abs(left);
            }
            var bigIntegerBuilder = new BigIntegerBuilder(left);
            var bigIntegerBuilder1 = new BigIntegerBuilder(right);
            BigIntegerBuilder.Gcd(ref bigIntegerBuilder, ref bigIntegerBuilder1);
            return bigIntegerBuilder.GetInteger(1);
        }

        internal static int Length(uint[] internalBits)
        {
            var length = internalBits.Length;
            if (internalBits[length - 1] != 0)
            {
                return length;
            }
            return length - 1;
        }

        /// <summary>Returns the natural (base e) logarithm of a specified number.</summary>
        /// <returns>The natural (base e) logarithm of <paramref name="value" />, as shown in the table in the Remarks section.</returns>
        /// <param name="value">The number whose logarithm is to be found.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The natural log of <paramref name="value" /> is out of range of the <see cref="T:System.Double" /> data type.</exception>
        public static double Log(BigInteger value)
        {
            return Log(value, Math.E);
        }

        /// <summary>Returns the logarithm of a specified number in a specified base.</summary>
        /// <returns>The base <paramref name="baseValue" /> logarithm of <paramref name="value" />, as shown in the table in the Remarks section.</returns>
        /// <param name="value">A number whose logarithm is to be found.</param>
        /// <param name="baseValue">The base of the logarithm.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The log of <paramref name="value" /> is out of range of the <see cref="T:System.Double" /> data type.</exception>
        public static double Log(BigInteger value, double baseValue)
        {
            if (value.InternalSign < 0 || NumericHelper.IsOne(baseValue))
            {
                return double.NaN;
            }
            if (double.IsPositiveInfinity(baseValue))
            {
                return (!value.IsOne ? double.NaN : 0);
            }
            if (NumericHelper.IsZero(baseValue) && !value.IsOne)
            {
                return double.NaN;
            }
            if (value.InternalBits == null)
            {
                return Math.Log(value.InternalSign, baseValue);
            }
            double c = 0;
            var d = 0.5;
            var length = Length(value.InternalBits);
            var topbits = BitLengthOfUInt(value.InternalBits[length - 1]);
            var bitlen = (length - 1) * 32 + topbits;
            var indbit = (uint)(1 << (topbits - 1 & 31));
            for (var index = length - 1; index >= 0; index--)
            {
                while (indbit != 0)
                {
                    if ((value.InternalBits[index] & indbit) != 0)
                    {
                        c = c + d;
                    }
                    d = d * 0.5;
                    indbit = indbit >> 1;
                }
                indbit = unchecked ((uint)int.MinValue);
            }
            return (Math.Log(c) + 0.69314718055994529D * bitlen) / Math.Log(baseValue);
        }

        /// <summary>Returns the base 10 logarithm of a specified number.</summary>
        /// <returns>The base 10 logarithm of <paramref name="value" />, as shown in the table in the Remarks section.</returns>
        /// <param name="value">A number whose logarithm is to be found.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The base 10 log of <paramref name="value" /> is out of range of the <see cref="T:System.Double" /> data type.</exception>
        public static double Log10(BigInteger value)
        {
            return Log(value, 10);
        }

        /// <summary>Returns the larger of two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
        /// <returns>The <paramref name="left" /> or <paramref name="right" /> parameter, whichever is larger.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static BigInteger Max(BigInteger left, BigInteger right)
        {
            if (left.CompareTo(right) < 0)
            {
                return right;
            }
            return left;
        }

        /// <summary>Returns the smaller of two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
        /// <returns>The <paramref name="left" /> or <paramref name="right" /> parameter, whichever is smaller.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static BigInteger Min(BigInteger left, BigInteger right)
        {
            if (left.CompareTo(right) <= 0)
            {
                return left;
            }
            return right;
        }

        /// <summary>Performs modulus division on a number raised to the power of another number.</summary>
        /// <returns>The remainder after dividing <paramref name="value" />exponent by <paramref name="modulus" />.</returns>
        /// <param name="value">The number to raise to the <paramref name="exponent" /> power.</param>
        /// <param name="exponent">The exponent to raise <paramref name="value" /> by.</param>
        /// <param name="modulus">The number by which to divide <paramref name="value" /> raised to the <paramref name="exponent" /> power.</param>
        /// <exception cref="T:System.DivideByZeroException">
        ///   <paramref name="modulus" /> is zero.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="exponent" /> is negative.</exception>
        public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            if (exponent.Sign < 0)
            {
                throw new ArgumentOutOfRangeException("exponent", "The number must be greater than or equal to zero.");}
            var signRes = 1;
            var signVal = 1;
            var signMod = 1;
            var isEven = exponent.IsEven;
            var regRes = new BigIntegerBuilder(One, ref signRes);
            var regVal = new BigIntegerBuilder(value, ref signVal);
            var regMod = new BigIntegerBuilder(modulus, ref signMod);
            var regTmp = new BigIntegerBuilder(regVal.Size);
            regRes.Mod(ref regMod);
            if (exponent.InternalBits != null)
            {
                var length = Length(exponent.InternalBits);
                for (var index = 0; index < length - 1; index++)
                {
                    var num5 = exponent.InternalBits[index];
                    ModPowInner32(num5, ref regRes, ref regVal, ref regMod, ref regTmp);
                }
                ModPowInner(exponent.InternalBits[length - 1], ref regRes, ref regVal, ref regMod, ref regTmp);
            }
            else
            {
                ModPowInner((uint)exponent.InternalSign, ref regRes, ref regVal, ref regMod, ref regTmp);
            }
            return regRes.GetInteger(value.InternalSign <= 0 ? (!isEven ? -1 : 1) : 1);
        }

        private static void ModPowInner(uint exp, ref BigIntegerBuilder regRes, ref BigIntegerBuilder regVal, ref BigIntegerBuilder regMod, ref BigIntegerBuilder regTmp)
        {
            while (exp != 0)
            {
                if ((exp & 1) == 1)
                {
                    ModPowUpdateResult(ref regRes, ref regVal, ref regMod, ref regTmp);
                }
                if (exp != 1)
                {
                    ModPowSquareModValue(ref regVal, ref regMod, ref regTmp);
                    exp = exp >> 1;
                }
                else
                {
                    break;
                }
            }
        }

        private static void ModPowInner32(uint exp, ref BigIntegerBuilder regRes, ref BigIntegerBuilder regVal, ref BigIntegerBuilder regMod, ref BigIntegerBuilder regTmp)
        {
            for (var index = 0; index < 32; index++)
            {
                if ((exp & 1) == 1)
                {
                    ModPowUpdateResult(ref regRes, ref regVal, ref regMod, ref regTmp);
                }
                ModPowSquareModValue(ref regVal, ref regMod, ref regTmp);
                exp = exp >> 1;
            }
        }

        private static void ModPowSquareModValue(ref BigIntegerBuilder regVal, ref BigIntegerBuilder regMod, ref BigIntegerBuilder regTmp)
        {
            NumericsHelpers.Swap(ref regVal, ref regTmp);
            regVal.Mul(ref regTmp, ref regTmp);
            regVal.Mod(ref regMod);
        }

        private static void ModPowUpdateResult(ref BigIntegerBuilder regRes, ref BigIntegerBuilder regVal, ref BigIntegerBuilder regMod, ref BigIntegerBuilder regTmp)
        {
            NumericsHelpers.Swap(ref regRes, ref regTmp);
            regRes.Mul(ref regTmp, ref regVal);
            regRes.Mod(ref regMod);
        }

        private static void MulLower(ref uint uHiRes, ref int cuRes, uint uHiMul, int cuMul)
        {
            var num = uHiRes * (ulong)uHiMul;
            var hi = NumericsHelpers.GetHi(num);
            if (hi == 0)
            {
                uHiRes = NumericsHelpers.GetLo(num);
                cuRes = cuRes + (cuMul - 1);
            }
            else
            {
                uHiRes = hi;
                cuRes = cuRes + cuMul;
            }
        }

        /// <summary>Returns the product of two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
        /// <returns>The product of the <paramref name="left" /> and <paramref name="right" /> parameters.</returns>
        /// <param name="left">The first number to multiply.</param>
        /// <param name="right">The second number to multiply.</param>
        public static BigInteger Multiply(BigInteger left, BigInteger right)
        {
            return left * right;
        }

        private static void MulUpper(ref uint uHiRes, ref int cuRes, uint uHiMul, int cuMul)
        {
            var num = uHiRes * (ulong)uHiMul;
            var hi = NumericsHelpers.GetHi(num);
            if (hi == 0)
            {
                uHiRes = NumericsHelpers.GetLo(num);
                cuRes = cuRes + (cuMul - 1);
            }
            else
            {
                if (NumericsHelpers.GetLo(num) != 0)
                {
                    var num1 = hi + 1;
                    hi = num1;
                    if (num1 == 0)
                    {
                        hi = 1;
                        cuRes = cuRes + 1;
                    }
                }
                uHiRes = hi;
                cuRes = cuRes + cuMul;
            }
        }

        /// <summary>Negates a specified <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>The result of the <paramref name="value" /> parameter multiplied by negative one (-1).</returns>
        /// <param name="value">The value to negate.</param>
        public static BigInteger Negate(BigInteger value)
        {
            return -value;
        }

        /// <summary>Adds the values of two specified <see cref="T:System.Numerics.BigInteger" /> objects.</summary>
        /// <returns>The sum of <paramref name="left" /> and <paramref name="right" />.</returns>
        /// <param name="left">The first value to add.</param>
        /// <param name="right">The second value to add.</param>
        public static BigInteger operator +(BigInteger left, BigInteger right)
        {
            if (right.IsZero)
            {
                return left;
            }
            if (left.IsZero)
            {
                return right;
            }
            var signLeft = 1;
            var signRight = 1;
            var regLeft = new BigIntegerBuilder(left, ref signLeft);
            var regRight = new BigIntegerBuilder(right, ref signRight);
            if (signLeft != signRight)
            {
                regLeft.Sub(ref signLeft, ref regRight);
            }
            else
            {
                regLeft.Add(ref regRight);
            }
            return regLeft.GetInteger(signLeft);
        }

        /// <summary>Performs a bitwise And operation on two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
        /// <returns>The result of the bitwise And operation.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        public static BigInteger operator &(BigInteger left, BigInteger right)
        {
            if (left.IsZero || right.IsZero)
            {
                return Zero;
            }
            var x = left.ToUInt32Array();
            var y = right.ToUInt32Array();
            var z = new uint[Math.Max(x.Length, y.Length)];
            var xExtend = (uint)((left.InternalSign >= 0 ? 0 : -1));
            var yExtend = (uint)((right.InternalSign >= 0 ? 0 : -1));
            for (var index = 0; index < z.Length; index++)
            {
                var num2 = (index >= x.Length ? xExtend : x[index]);
                z[index] = num2 & (index >= y.Length ? yExtend : y[index]);
            }
            return new BigInteger(z);
        }

        /// <summary>Performs a bitwise Or operation on two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
        /// <returns>The result of the bitwise Or operation.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        public static BigInteger operator |(BigInteger left, BigInteger right)
        {
            if (left.IsZero)
            {
                return right;
            }
            if (right.IsZero)
            {
                return left;
            }
            var x = left.ToUInt32Array();
            var y = right.ToUInt32Array();
            var z = new uint[Math.Max(x.Length, y.Length)];
            var xExtend = (uint)((left.InternalSign >= 0 ? 0 : -1));
            var yExtend = (uint)((right.InternalSign >= 0 ? 0 : -1));
            for (var index = 0; index < z.Length; index++)
            {
                var num2 = (index >= x.Length ? xExtend : x[index]);
                z[index] = num2 | (index >= y.Length ? yExtend : y[index]);
            }
            return new BigInteger(z);
        }

        /// <summary>Decrements a <see cref="T:System.Numerics.BigInteger" /> value by 1.</summary>
        /// <returns>The value of the <paramref name="value" /> parameter decremented by 1.</returns>
        /// <param name="value">The value to decrement.</param>
        public static BigInteger operator --(BigInteger value)
        {
            return value - One;
        }

        /// <summary>Divides a specified <see cref="T:System.Numerics.BigInteger" /> value by another specified <see cref="T:System.Numerics.BigInteger" /> value by using integer division.</summary>
        /// <returns>The integral result of the division.</returns>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <exception cref="T:System.DivideByZeroException">
        ///   <paramref name="divisor" /> is 0 (zero).</exception>
        public static BigInteger operator /(BigInteger dividend, BigInteger divisor)
        {
            var sign = 1;
            var regNum = new BigIntegerBuilder(dividend, ref sign);
            var regDen = new BigIntegerBuilder(divisor, ref sign);
            regNum.Div(ref regDen);
            return regNum.GetInteger(sign);
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:System.Numerics.BigInteger" /> objects are equal.</summary>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator ==(BigInteger left, BigInteger right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value and a signed long integer value are equal.</summary>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator ==(BigInteger left, long right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether a signed long integer value and a <see cref="T:System.Numerics.BigInteger" /> value are equal.</summary>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator ==(long left, BigInteger right)
        {
            return right.Equals(left);
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value and an unsigned long integer value are equal.</summary>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator ==(BigInteger left, ulong right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether an unsigned long integer value and a <see cref="T:System.Numerics.BigInteger" /> value are equal.</summary>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator ==(ulong left, BigInteger right)
        {
            return right.Equals(left);
        }

        /// <summary>Performs a bitwise exclusive Or (XOr) operation on two <see cref="T:System.Numerics.BigInteger" /> values.</summary>
        /// <returns>The result of the bitwise Or operation.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        public static BigInteger operator ^(BigInteger left, BigInteger right)
        {
            var x = left.ToUInt32Array();
            var y = right.ToUInt32Array();
            var z = new uint[Math.Max(x.Length, y.Length)];
            var xEntend = (uint)((left.InternalSign >= 0 ? 0 : -1));
            var yExtend = (uint)((right.InternalSign >= 0 ? 0 : -1));
            for (var index = 0; index < z.Length; index++)
            {
                var num2 = (index >= x.Length ? xEntend : x[index]);
                z[index] = num2 ^ (index >= y.Length ? yExtend : y[index]);
            }
            return new BigInteger(z);
        }

        public static explicit operator BigInteger(float value)
        {
            return new BigInteger(value);
        }

        public static explicit operator BigInteger(double value)
        {
            return new BigInteger(value);
        }

        public static explicit operator BigInteger(decimal value)
        {
            return new BigInteger(value);
        }

        public static explicit operator byte(BigInteger value)
        {
            return checked((byte)((int)value));
        }

        [CLSCompliant(false)]
        public static explicit operator sbyte(BigInteger value)
        {
            return checked((sbyte)((int)value));
        }

        public static explicit operator short(BigInteger value)
        {
            return checked((short)((int)value));
        }

        [CLSCompliant(false)]
        public static explicit operator ushort(BigInteger value)
        {
            return checked((ushort)((int)value));
        }

        public static explicit operator int(BigInteger value)
        {
            if (value.InternalBits == null)
            {
                return value.InternalSign;
            }
            if (Length(value.InternalBits) > 1)
            {
                throw new OverflowException("Value was either too large or too small for an Int32.");
            }
            if (value.InternalSign > 0)
            {
                return checked((int)value.InternalBits[0]);
            }
            if (value.InternalBits[0] > unchecked ((uint)int.MinValue))
            {
                throw new OverflowException("Value was either too large or too small for an Int32.");
            }
            return (int)(-value.InternalBits[0]);
        }

        [CLSCompliant(false)]
        public static explicit operator uint(BigInteger value)
        {
            if (value.InternalBits == null)
            {
                return checked((uint)value.InternalSign);
            }
            if (Length(value.InternalBits) > 1 || value.InternalSign < 0)
            {
                throw new OverflowException("Value was either too large or too small for a UInt32.");
            }
            return value.InternalBits[0];
        }

        public static explicit operator long(BigInteger value)
        {
            if (value.InternalBits == null)
            {
                return value.InternalSign;
            }
            var length = Length(value.InternalBits);
            if (length > 2)
            {
                throw new OverflowException("Value was either too large or too small for an Int64.");
            }
            var target = (length <= 1 ? value.InternalBits[0] : NumericsHelpers.MakeUlong(value.InternalBits[1], value.InternalBits[0]));
            var result = (value.InternalSign <= 0 ? -(long)(target) : (long)target);
            if ((result <= 0 || value.InternalSign <= 0) && (result >= 0 || value.InternalSign >= 0))
            {
                throw new OverflowException("Value was either too large or too small for an Int64.");
            }
            return result;
        }

        [CLSCompliant(false)]
        public static explicit operator ulong(BigInteger value)
        {
            if (value.InternalBits == null)
            {
                return checked((ulong)value.InternalSign);
            }
            var length = Length(value.InternalBits);
            if (length > 2 || value.InternalSign < 0)
            {
                throw new OverflowException("Value was either too large or too small for a UInt64.");
            }
            if (length <= 1)
            {
                return value.InternalBits[0];
            }
            return NumericsHelpers.MakeUlong(value.InternalBits[1], value.InternalBits[0]);
        }

        public static explicit operator float(BigInteger value)
        {
            return (float)((double)value);
        }

        public static explicit operator double(BigInteger value)
        {
            ulong man;
            int exp;
            if (value.InternalBits == null)
            {
                return value.InternalSign;
            }
            var sign = 1;
            var bigIntegerBuilder = new BigIntegerBuilder(value, ref sign);
            bigIntegerBuilder.GetApproxParts(out exp, out man);
            return NumericsHelpers.GetDoubleFromParts(sign, exp, man);
        }

        public static explicit operator decimal(BigInteger value)
        {
            if (value.InternalBits == null)
            {
                return value.InternalSign;
            }
            var length = Length(value.InternalBits);
            if (length > 3)
            {
                throw new OverflowException("Value was either too large or too small for a Decimal.");
            }
            var lo = 0;
            var mi = 0;
            var hi = 0;
            if (length > 2)
            {
                hi = (int)value.InternalBits[2];
            }
            if (length > 1)
            {
                mi = (int)value.InternalBits[1];
            }
            if (length > 0)
            {
                lo = (int)value.InternalBits[0];
            }
            return new decimal(lo, mi, hi, value.InternalSign < 0, 0);
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than another <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> is greater than a 64-bit signed integer value.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >(BigInteger left, long right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>Returns a value that indicates whether a 64-bit signed integer is greater than a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >(long left, BigInteger right)
        {
            return right.CompareTo(left) < 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than a 64-bit unsigned integer.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator >(BigInteger left, ulong right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than a 64-bit unsigned integer.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator >(ulong left, BigInteger right)
        {
            return right.CompareTo(left) < 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than or equal to another <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than or equal to a 64-bit signed integer value.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >=(BigInteger left, long right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>Returns a value that indicates whether a 64-bit signed integer is greater than or equal to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >=(long left, BigInteger right)
        {
            return right.CompareTo(left) <= 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is greater than or equal to a 64-bit unsigned integer value.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator >=(BigInteger left, ulong right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>Returns a value that indicates whether a 64-bit unsigned integer is greater than or equal to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator >=(ulong left, BigInteger right)
        {
            return right.CompareTo(left) <= 0;
        }

        public static implicit operator BigInteger(byte value)
        {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(sbyte value)
        {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(short value)
        {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(ushort value)
        {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(int value)
        {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(uint value)
        {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(long value)
        {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(ulong value)
        {
            return new BigInteger(value);
        }

        /// <summary>Increments a <see cref="T:System.Numerics.BigInteger" /> value by 1.</summary>
        /// <returns>The value of the <paramref name="value" /> parameter incremented by 1.</returns>
        /// <param name="value">The value to increment.</param>
        public static BigInteger operator ++(BigInteger value)
        {
            return value + One;
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:System.Numerics.BigInteger" /> objects have different values.</summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator !=(BigInteger left, BigInteger right)
        {
            return !left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value and a 64-bit signed integer are not equal.</summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator !=(BigInteger left, long right)
        {
            return !left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether a 64-bit signed integer and a <see cref="T:System.Numerics.BigInteger" /> value are not equal.</summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator !=(long left, BigInteger right)
        {
            return !right.Equals(left);
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value and a 64-bit unsigned integer are not equal.</summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator !=(BigInteger left, ulong right)
        {
            return !left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether a 64-bit unsigned integer and a <see cref="T:System.Numerics.BigInteger" /> value are not equal.</summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator !=(ulong left, BigInteger right)
        {
            return !right.Equals(left);
        }

        /// <summary>Shifts a <see cref="T:System.Numerics.BigInteger" /> value a specified number of bits to the left.</summary>
        /// <returns>A value that has been shifted to the left by the specified number of bits.</returns>
        /// <param name="value">The value whose bits are to be shifted.</param>
        /// <param name="shift">The number of bits to shift <paramref name="value" /> to the left.</param>
        public static BigInteger operator <<(BigInteger value, int shift)
        {
            uint[] xd;
            int xl;
            if (shift == 0)
            {
                return value;
            }
            if (shift == int.MinValue)
            {
                return (value >> int.MaxValue) >> 1;
            }
            if (shift < 0)
            {
                return value >> -shift;
            }
            var digitShift = shift / 32;
            var smallShift = shift - digitShift * 32;
            var partsForBitManipulation = GetPartsForBitManipulation(ref value, out xd, out xl);
            var zd = new uint[xl + digitShift + 1];
            if (smallShift != 0)
            {
                var carryShift = 32 - smallShift;
                uint carry = 0;
                int index;
                for (index = 0; index < xl; index++)
                {
                    var rot = xd[index];
                    zd[index + digitShift] = rot << (smallShift & 31) | carry;
                    carry = rot >> (carryShift & 31);
                }
                zd[index + digitShift] = carry;
            }
            else
            {
                for (var index = 0; index < xl; index++)
                {
                    zd[index + digitShift] = xd[index];
                }
            }
            return new BigInteger(zd, partsForBitManipulation);
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than another <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than a 64-bit signed integer.</summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <(BigInteger left, long right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>Returns a value that indicates whether a 64-bit signed integer is less than a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <(long left, BigInteger right)
        {
            return right.CompareTo(left) > 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than a 64-bit unsigned integer.</summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator <(BigInteger left, ulong right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>Returns a value that indicates whether a 64-bit unsigned integer is less than a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator <(ulong left, BigInteger right)
        {
            return right.CompareTo(left) > 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than or equal to another <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than or equal to a 64-bit signed integer.</summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <=(BigInteger left, long right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>Returns a value that indicates whether a 64-bit signed integer is less than or equal to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <=(long left, BigInteger right)
        {
            return right.CompareTo(left) >= 0;
        }

        /// <summary>Returns a value that indicates whether a <see cref="T:System.Numerics.BigInteger" /> value is less than or equal to a 64-bit unsigned integer.</summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator <=(BigInteger left, ulong right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>Returns a value that indicates whether a 64-bit unsigned integer is less than or equal to a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator <=(ulong left, BigInteger right)
        {
            return right.CompareTo(left) >= 0;
        }

        /// <summary>Returns the remainder that results from division with two specified <see cref="T:System.Numerics.BigInteger" /> values.</summary>
        /// <returns>The remainder that results from the division.</returns>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <exception cref="T:System.DivideByZeroException">
        ///   <paramref name="divisor" /> is 0 (zero).</exception>
        public static BigInteger operator %(BigInteger dividend, BigInteger divisor)
        {
            var signNUm = 1;
            var signDen = 1;
            var regNum = new BigIntegerBuilder(dividend, ref signNUm);
            var regDen = new BigIntegerBuilder(divisor, ref signDen);
            regNum.Mod(ref regDen);
            return regNum.GetInteger(signNUm);
        }

        /// <summary>Multiplies two specified <see cref="T:System.Numerics.BigInteger" /> values.</summary>
        /// <returns>The product of <paramref name="left" /> and <paramref name="right" />.</returns>
        /// <param name="left">The first value to multiply.</param>
        /// <param name="right">The second value to multiply.</param>
        public static BigInteger operator *(BigInteger left, BigInteger right)
        {
            var sign = 1;
            var regLeft = new BigIntegerBuilder(left, ref sign);
            var regRight = new BigIntegerBuilder(right, ref sign);
            regLeft.Mul(ref regRight);
            return regLeft.GetInteger(sign);
        }

        /// <summary>Returns the bitwise one's complement of a <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>The bitwise one's complement of <paramref name="value" />.</returns>
        /// <param name="value">An integer value.</param>
        public static BigInteger operator ~(BigInteger value)
        {
            return -(value + One);
        }

        /// <summary>Shifts a <see cref="T:System.Numerics.BigInteger" /> value a specified number of bits to the right.</summary>
        /// <returns>A value that has been shifted to the right by the specified number of bits.</returns>
        /// <param name="value">The value whose bits are to be shifted.</param>
        /// <param name="shift">The number of bits to shift <paramref name="value" /> to the right.</param>
        public static BigInteger operator >>(BigInteger value, int shift)
        {
            uint[] xd;
            int xl;
            if (shift == 0)
            {
                return value;
            }
            if (shift == int.MinValue)
            {
                return (value << int.MaxValue) << 1;
            }
            if (shift < 0)
            {
                return value << -shift;
            }
            var digitShift = shift / 32;
            var smallShift = shift - digitShift * 32;
            var negx = GetPartsForBitManipulation(ref value, out xd, out xl);
            if (negx)
            {
                if (shift >= 32 * xl)
                {
                    return MinusOne;
                }
                var temp = new uint[xl];
                Array.Copy(xd, temp, xl);
                xd = temp;
                NumericsHelpers.DangerousMakeTwosComplement(xd);
            }
            var zl = xl - digitShift;
            if (zl < 0)
            {
                zl = 0;
            }
            var zd = new uint[zl];
            if (smallShift != 0)
            {
                var carryShift = 32 - smallShift;
                uint carry = 0;
                for (var index = xl - 1; index >= digitShift; index--)
                {
                    var rot = xd[index];
                    if (!negx || index != xl - 1)
                    {
                        zd[index - digitShift] = rot >> (smallShift & 31) | carry;
                    }
                    else
                    {
                        zd[index - digitShift] = (rot >> (smallShift & 31)) | (0xFFFFFFFF << (carryShift & 31));
                    }
                    carry = rot << (carryShift & 31);
                }
            }
            else
            {
                for (var index = xl - 1; index >= digitShift; index--)
                {
                    zd[index - digitShift] = xd[index];
                }
            }
            if (negx)
            {
                NumericsHelpers.DangerousMakeTwosComplement(zd);
            }
            return new BigInteger(zd, negx);
        }

        /// <summary>Subtracts a <see cref="T:System.Numerics.BigInteger" /> value from another <see cref="T:System.Numerics.BigInteger" /> value.</summary>
        /// <returns>The result of subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend).</param>
        public static BigInteger operator -(BigInteger left, BigInteger right)
        {
            if (right.IsZero)
            {
                return left;
            }
            if (left.IsZero)
            {
                return -right;
            }
            var leftSign = 1;
            var rightSign = -1;
            var regLeft = new BigIntegerBuilder(left, ref leftSign);
            var regRight = new BigIntegerBuilder(right, ref rightSign);
            if (leftSign != rightSign)
            {
                regLeft.Sub(ref leftSign, ref regRight);
            }
            else
            {
                regLeft.Add(ref regRight);
            }
            return regLeft.GetInteger(leftSign);
        }

        /// <summary>Negates a specified BigInteger value. </summary>
        /// <returns>The result of the <paramref name="value" /> parameter multiplied by negative one (-1).</returns>
        /// <param name="value">The value to negate.</param>
        public static BigInteger operator -(BigInteger value)
        {
            return new BigInteger(-value.InternalSign, value.InternalBits);
        }

        /// <summary>Returns the value of the <see cref="T:System.Numerics.BigInteger" /> operand. (The sign of the operand is unchanged.)</summary>
        /// <returns>The value of the <paramref name="value" /> operand.</returns>
        /// <param name="value">An integer value.</param>
        public static BigInteger operator +(BigInteger value)
        {
            return value;
        }

        /// <summary>Converts the string representation of a number to its <see cref="T:System.Numerics.BigInteger" /> equivalent.</summary>
        /// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
        /// <param name="value">A string that contains the number to convert.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="value" /> is not in the correct format.</exception>
        public static BigInteger Parse(string value)
        {
            return ParseBigInteger(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>Converts the string representation of a number in a specified style to its <see cref="T:System.Numerics.BigInteger" /> equivalent.</summary>
        /// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
        /// <param name="value">A string that contains a number to convert. </param>
        /// <param name="style">A bitwise combination of the enumeration values that specify the permitted format of <paramref name="value" />.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="style" /> is not a <see cref="T:System.Globalization.NumberStyles" /> value.-or-<paramref name="style" /> includes the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier" /> or <see cref="F:System.Globalization.NumberStyles.HexNumber" /> flag along with another value.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="value" /> does not comply with the input pattern specified by <see cref="T:System.Globalization.NumberStyles" />.</exception>
        public static BigInteger Parse(string value, NumberStyles style)
        {
            return ParseBigInteger(value, style, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>Converts the string representation of a number in a specified culture-specific format to its <see cref="T:System.Numerics.BigInteger" /> equivalent.</summary>
        /// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="value" />.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="value" /> is not in the correct format.</exception>
        public static BigInteger Parse(string value, IFormatProvider provider)
        {
            return ParseBigInteger(value, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
        }

        /// <summary>Converts the string representation of a number in a specified style and culture-specific format to its <see cref="T:System.Numerics.BigInteger" /> equivalent.</summary>
        /// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <param name="style">A bitwise combination of the enumeration values that specify the permitted format of <paramref name="value" />.</param>
        /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="value" />.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="style" /> is not a <see cref="T:System.Globalization.NumberStyles" /> value.-or-<paramref name="style" /> includes the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier" /> or <see cref="F:System.Globalization.NumberStyles.HexNumber" /> flag along with another value.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="value" /> does not comply with the input pattern specified by <paramref name="style" />.</exception>
        public static BigInteger Parse(string value, NumberStyles style, IFormatProvider provider)
        {
            return ParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider));
        }

        /// <summary>Raises a <see cref="T:System.Numerics.BigInteger" /> value to the power of a specified value.</summary>
        /// <returns>The result of raising <paramref name="value" /> to the <paramref name="exponent" /> power.</returns>
        /// <param name="value">The number to raise to the <paramref name="exponent" /> power.</param>
        /// <param name="exponent">The exponent to raise <paramref name="value" /> by.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The value of the <paramref name="exponent" /> parameter is negative.</exception>
        public static BigInteger Pow(BigInteger value, int exponent)
        {
            if (exponent < 0)
            {
                throw new ArgumentOutOfRangeException("exponent", "The number must be greater than or equal to zero.");
            }
            if (exponent == 0)
            {
                return One;
            }
            if (exponent == 1)
            {
                return value;
            }
            if (value.InternalBits == null)
            {
                if (value.InternalSign == 1)
                {
                    return value;
                }
                if (value.InternalSign == -1)
                {
                    return (exponent & 1) == 0 ? 1 : value;
                }
                if (value.InternalSign == 0)
                {
                    return value;
                }
            }
            var sign = 1;
            var regSquare = new BigIntegerBuilder(value, ref sign);
            var minSquareSize = regSquare.Size;
            var maxSquareSize = minSquareSize;
            var minSquare = regSquare.High;
            var maxSquare = minSquare + 1;
            if (maxSquare == 0)
            {
                maxSquareSize++;
                maxSquare = 1;
            }
            var minResultSize = 1;
            var maxResultSize = 1;
            uint resultMin = 1;
            uint resultMax = 1;
            for(var expTmp = exponent; ;)
            {
                if ((expTmp & 1) != 0)
                {
                    MulUpper(ref resultMax, ref maxResultSize, maxSquare, maxSquareSize);
                    MulLower(ref resultMin, ref minResultSize, minSquare, minSquareSize);
                }
                expTmp = expTmp >> 1;
                if (expTmp == 0)
                {
                    break;
                }
                MulUpper(ref maxSquare, ref maxSquareSize, maxSquare, maxSquareSize);
                MulLower(ref minSquare, ref minSquareSize, minSquare, minSquareSize);
            }
            if (maxResultSize > 1)
            {
                regSquare.EnsureWritable(maxResultSize, 0);
            }
            var regTmp = new BigIntegerBuilder(maxResultSize);
            var regResult = new BigIntegerBuilder(maxResultSize);
            regResult.Set(1);
            if ((exponent & 1) == 0)
            {
                sign = 1;
            }
            for(var expTmp = exponent; ;)
            {
                if ((expTmp & 1) != 0)
                {
                    NumericsHelpers.Swap(ref regResult, ref regTmp);
                    regResult.Mul(ref regSquare, ref regTmp);
                }
                expTmp = expTmp >> 1;
                if (expTmp == 0)
                {
                    break;
                }
                NumericsHelpers.Swap(ref regSquare, ref regTmp);
                regSquare.Mul(ref regTmp, ref regTmp);
            }
            return regResult.GetInteger(sign);
        }

        /// <summary>Performs integer division on two <see cref="T:System.Numerics.BigInteger" /> values and returns the remainder.</summary>
        /// <returns>The remainder after dividing <paramref name="dividend" /> by <paramref name="divisor" />.</returns>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <exception cref="T:System.DivideByZeroException">
        ///   <paramref name="divisor" /> is 0 (zero).</exception>
        public static BigInteger Remainder(BigInteger dividend, BigInteger divisor)
        {
            return dividend % divisor;
        }

        private static void SetBitsFromDouble(double value, out uint[] bits, out int sign)
        {
            int valueSign;
            int valueExp;
            ulong valueMan;
            bool valueFinite;
            sign = 0;
            bits = null;
            NumericsHelpers.GetDoubleParts(value, out valueSign, out valueExp, out valueMan, out valueFinite);
            if (valueMan == 0)
            {
                return;
            }
            if (valueExp <= 0)
            {
                if (valueExp <= -64)
                {
                    return;
                }
                var tmp = valueMan >> (-valueExp & 63);
                if (tmp > int.MaxValue)
                {
                    sign = 1;
                    bits = new[] { (uint)tmp, (uint)(tmp >> 32) };
                }
                else
                {
                    sign = (int)tmp;
                }
                if (valueSign < 0)
                {
                    sign = -sign;
                }
            }
            else if (valueExp > 11)
            {
                valueMan = valueMan << 11;
                valueExp = valueExp - 11;
                var significantDword = (valueExp - 1) / 32 + 1;
                var extraDword = significantDword * 32 - valueExp;
                bits = new uint[significantDword + 2];
                bits[significantDword + 1] = (uint)(valueMan >> (extraDword + 32 & 63));
                bits[significantDword] = (uint)(valueMan >> (extraDword & 63));
                if (extraDword > 0)
                {
                    bits[significantDword - 1] = (uint)valueMan << (32 - extraDword & 31);
                }
                sign = valueSign;
            }
            else
            {
                var tmp = valueMan << (valueExp & 63);
                if (tmp > int.MaxValue)
                {
                    sign = 1;
                    bits = new[] { (uint)tmp, (uint)(tmp >> 32) };
                }
                else
                {
                    sign = (int)tmp;
                }
                if (valueSign < 0)
                {
                    sign = -sign;
                }
            }
        }

        /// <summary>Subtracts one <see cref="T:System.Numerics.BigInteger" /> value from another and returns the result.</summary>
        /// <returns>The result of subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend).</param>
        public static BigInteger Subtract(BigInteger left, BigInteger right)
        {
            return left - right;
        }

        /// <summary>Converts a <see cref="T:System.Numerics.BigInteger" /> value to a byte array.</summary>
        /// <returns>The value of the current <see cref="T:System.Numerics.BigInteger" /> object converted to an array of bytes.</returns>
        public byte[] ToByteArray()
        {
            uint[] internalBits;
            byte highByte;
            if (InternalBits == null && InternalSign == 0)
            {
                return new byte[1];
            }
            if (InternalBits == null)
            {
                internalBits = new[] { unchecked((uint) InternalSign) };
                highByte = InternalSign < 0 ? (byte)0xff : (byte)0x00;
            }
            else if (InternalSign != -1)
            {
                internalBits = InternalBits;
                highByte = 0;
            }
            else
            {
                internalBits = (uint[])InternalBits.Clone();
                NumericsHelpers.DangerousMakeTwosComplement(internalBits);
                highByte = 255;
            }
            var bytes = new byte[checked(4 * internalBits.Length)];
            var index = 0;
            foreach (var value in internalBits)
            {
                var current = value;
                for (var j = 0; j < 4; j++)
                {
                    bytes[index] = (byte)(current & 255);
                    current >>= 8;
                    index++;
                }
            }
            var length = bytes.Length - 1;
            while (length > 0)
            {
                if (bytes[length] == highByte)
                {
                    length--;
                }
                else
                {
                    break;
                }
            }
            var extra = (bytes[length] & 128) != (highByte & 128);
            var result = new byte[length + 1 + (extra ? 1 : 0)];
            Array.Copy(bytes, result, length + 1);
            if (extra)
            {
                result[result.Length - 1] = highByte;
            }
            return result;
        }

        /// <summary>Converts the numeric value of the current <see cref="T:System.Numerics.BigInteger" /> object to its equivalent string representation.</summary>
        /// <returns>The string representation of the current <see cref="T:System.Numerics.BigInteger" /> value.</returns>
        public override string ToString()
        {
            return FormatBigInteger(this, null, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>Converts the numeric value of the current <see cref="T:System.Numerics.BigInteger" /> object to its equivalent string representation by using the specified culture-specific formatting information.</summary>
        /// <returns>The string representation of the current <see cref="T:System.Numerics.BigInteger" /> value in the format specified by the <paramref name="provider" /> parameter.</returns>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        public string ToString(IFormatProvider provider)
        {
            return FormatBigInteger(this, null, NumberFormatInfo.GetInstance(provider));
        }

        /// <summary>Converts the numeric value of the current <see cref="T:System.Numerics.BigInteger" /> object to its equivalent string representation by using the specified format.</summary>
        /// <returns>The string representation of the current <see cref="T:System.Numerics.BigInteger" /> value in the format specified by the <paramref name="format" /> parameter.</returns>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format" /> is not a valid format string.</exception>
        public string ToString(string format)
        {
            return FormatBigInteger(this, format, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>Converts the numeric value of the current <see cref="T:System.Numerics.BigInteger" /> object to its equivalent string representation by using the specified format and culture-specific format information.</summary>
        /// <returns>The string representation of the current <see cref="T:System.Numerics.BigInteger" /> value as specified by the <paramref name="format" /> and <paramref name="provider" /> parameters.</returns>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format" /> is not a valid format string.</exception>
        public string ToString(string format, IFormatProvider provider)
        {
            return FormatBigInteger(this, format, NumberFormatInfo.GetInstance(provider));
        }

        private uint[] ToUInt32Array()
        {
            uint[] internalBits;
            uint highDword;
            if (InternalBits == null && InternalSign == 0)
            {
                return new uint[1];
            }
            if (InternalBits == null)
            {
                internalBits = new[] { unchecked ((uint)InternalSign) };
                highDword = (uint)((InternalSign >= 0 ? 0 : -1));
            }
            else if (InternalSign != -1)
            {
                internalBits = InternalBits;
                highDword = 0;
            }
            else
            {
                internalBits = (uint[])InternalBits.Clone();
                NumericsHelpers.DangerousMakeTwosComplement(internalBits);
                highDword = unchecked ((uint)-1);
            }
            var length = internalBits.Length - 1;
            while (length > 0)
            {
                if (internalBits[length] == highDword)
                {
                    length--;
                }
                else
                {
                    break;
                }
            }
            var needExtraByte = (internalBits[length] & int.MinValue) != (highDword & int.MinValue);
            var trimmed = new uint[length + 1 + (!needExtraByte ? 0 : 1)];
            Array.Copy(internalBits, trimmed, length + 1);
            if (needExtraByte)
            {
                trimmed[trimmed.Length - 1] = highDword;
            }
            return trimmed;
        }

        /// <summary>Tries to convert the string representation of a number to its <see cref="T:System.Numerics.BigInteger" /> equivalent, and returns a value that indicates whether the conversion succeeded.</summary>
        /// <returns>true if <paramref name="value" /> was converted successfully; otherwise, false.</returns>
        /// <param name="value">The string representation of a number.</param>
        /// <param name="result">When this method returns, contains the <see cref="T:System.Numerics.BigInteger" /> equivalent to the number that is contained in <paramref name="value" />, or zero (0) if the conversion fails. The conversion fails if the <paramref name="value" /> parameter is null or is not of the correct format. This parameter is passed uninitialized.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="value" /> is null.</exception>
        public static bool TryParse(string value, out BigInteger result)
        {
            return TryParseBigInteger(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        /// <summary>Tries to convert the string representation of a number in a specified style and culture-specific format to its <see cref="T:System.Numerics.BigInteger" /> equivalent, and returns a value that indicates whether the conversion succeeded.</summary>
        /// <returns>true if the <paramref name="value" /> parameter was converted successfully; otherwise, false.</returns>
        /// <param name="value">The string representation of a number. The string is interpreted using the style specified by <paramref name="style" />.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="value" />. A typical value to specify is <see cref="F:System.Globalization.NumberStyles.Integer" />.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="value" />.</param>
        /// <param name="result">When this method returns, contains the <see cref="T:System.Numerics.BigInteger" /> equivalent to the number that is contained in <paramref name="value" />, or <see cref="P:System.Numerics.BigInteger.Zero" /> if the conversion failed. The conversion fails if the <paramref name="value" /> parameter is null or is not in a format that is compliant with <paramref name="style" />. This parameter is passed uninitialized.</param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="style" /> is not a <see cref="T:System.Globalization.NumberStyles" /> value.-or-<paramref name="style" /> includes the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier" /> or <see cref="F:System.Globalization.NumberStyles.HexNumber" /> flag along with another value. </exception>
        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out BigInteger result)
        {
            return TryParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider), out result);
        }
    }
}

#endif