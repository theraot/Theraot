#if LESSTHAN_NET40 || NETSTANDARD1_0

#pragma warning disable CA2225 // Operator overloads have named alternates

using Theraot.Core;

namespace System.Numerics
{
    public readonly partial struct BigInteger
    {
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
            return checked((byte)(int)value);
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

        public static explicit operator double(BigInteger value)
        {
            if (value.InternalBits == null)
            {
                return value.InternalSign;
            }

            var sign = 1;
            var bigIntegerBuilder = new BigIntegerBuilder(value, ref sign);
            bigIntegerBuilder.GetApproxParts(out var exp, out var man);
            return NumericHelper.BuildDouble(sign, man, exp);
        }

        public static explicit operator float(BigInteger value)
        {
            return (float)(double)value;
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

            if (value.InternalBits[0] > unchecked((uint)int.MinValue))
            {
                throw new OverflowException("Value was either too large or too small for an Int32.");
            }

            return (int)-value.InternalBits[0];
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

            var target = ULong(length, value.InternalBits);
            var result = value.InternalSign <= 0 ? -(long)target : (long)target;
            if ((result <= 0 || value.InternalSign <= 0) && (result >= 0 || value.InternalSign >= 0))
            {
                throw new OverflowException("Value was either too large or too small for an Int64.");
            }

            return result;
        }

        [CLSCompliant(false)]
        public static explicit operator sbyte(BigInteger value)
        {
            return checked((sbyte)(int)value);
        }

        public static explicit operator short(BigInteger value)
        {
            return checked((short)(int)value);
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

            return ULong(length, value.InternalBits);
        }

        [CLSCompliant(false)]
        public static explicit operator ushort(BigInteger value)
        {
            return checked((ushort)(int)value);
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

        /// <summary>
        ///     Subtracts a <see cref="BigInteger" /> value from another
        ///     <see cref="BigInteger" /> value.
        /// </summary>
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

        /// <summary>Decrements a <see cref="BigInteger" /> value by 1.</summary>
        /// <returns>The value of the <paramref name="value" /> parameter decremented by 1.</returns>
        /// <param name="value">The value to decrement.</param>
        public static BigInteger operator --(BigInteger value)
        {
            return value - One;
        }

        /// <summary>
        ///     Returns a value that indicates whether two <see cref="BigInteger" /> objects have different
        ///     values.
        /// </summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator !=(BigInteger left, BigInteger right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value and a 64-bit signed
        ///     integer are not equal.
        /// </summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator !=(BigInteger left, long right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Returns a value that indicates whether a 64-bit signed integer and a
        ///     <see cref="BigInteger" /> value are not equal.
        /// </summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator !=(long left, BigInteger right)
        {
            return !right.Equals(left);
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value and a 64-bit
        ///     unsigned integer are not equal.
        /// </summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator !=(BigInteger left, ulong right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///     Returns a value that indicates whether a 64-bit unsigned integer and a
        ///     <see cref="BigInteger" /> value are not equal.
        /// </summary>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator !=(ulong left, BigInteger right)
        {
            return !right.Equals(left);
        }

        /// <summary>
        ///     Returns the remainder that results from division with two specified
        ///     <see cref="BigInteger" /> values.
        /// </summary>
        /// <returns>The remainder that results from the division.</returns>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <exception cref="DivideByZeroException">
        ///     <paramref name="divisor" /> is 0 (zero).
        /// </exception>
        public static BigInteger operator %(BigInteger dividend, BigInteger divisor)
        {
            var signNUm = 1;
            var signDen = 1;
            var regNum = new BigIntegerBuilder(dividend, ref signNUm);
            var regDen = new BigIntegerBuilder(divisor, ref signDen);
            regNum.Mod(ref regDen);
            return regNum.GetInteger(signNUm);
        }

        /// <summary>Performs a bitwise And operation on two <see cref="BigInteger" /> values.</summary>
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
            var xExtend = (uint)(left.InternalSign >= 0 ? 0 : -1);
            var yExtend = (uint)(right.InternalSign >= 0 ? 0 : -1);
            for (var index = 0; index < z.Length; index++)
            {
                var num2 = index >= x.Length ? xExtend : x[index];
                z[index] = num2 & (index >= y.Length ? yExtend : y[index]);
            }

            return new BigInteger(z);
        }

        /// <summary>Multiplies two specified <see cref="BigInteger" /> values.</summary>
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

        /// <summary>
        ///     Divides a specified <see cref="BigInteger" /> value by another specified
        ///     <see cref="BigInteger" /> value by using integer division.
        /// </summary>
        /// <returns>The integral result of the division.</returns>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <exception cref="DivideByZeroException">
        ///     <paramref name="divisor" /> is 0 (zero).
        /// </exception>
        public static BigInteger operator /(BigInteger dividend, BigInteger divisor)
        {
            var sign = 1;
            var regNum = new BigIntegerBuilder(dividend, ref sign);
            var regDen = new BigIntegerBuilder(divisor, ref sign);
            regNum.Div(ref regDen);
            return regNum.GetInteger(sign);
        }

        /// <summary>Performs a bitwise exclusive Or (XOr) operation on two <see cref="BigInteger" /> values.</summary>
        /// <returns>The result of the bitwise Or operation.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        public static BigInteger operator ^(BigInteger left, BigInteger right)
        {
            var x = left.ToUInt32Array();
            var y = right.ToUInt32Array();
            var z = new uint[Math.Max(x.Length, y.Length)];
            var xExtend = (uint)(left.InternalSign >= 0 ? 0 : -1);
            var yExtend = (uint)(right.InternalSign >= 0 ? 0 : -1);
            for (var index = 0; index < z.Length; index++)
            {
                var num2 = index >= x.Length ? xExtend : x[index];
                z[index] = num2 ^ (index >= y.Length ? yExtend : y[index]);
            }

            return new BigInteger(z);
        }

        /// <summary>Performs a bitwise Or operation on two <see cref="BigInteger" /> values.</summary>
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
            var xExtend = (uint)(left.InternalSign >= 0 ? 0 : -1);
            var yExtend = (uint)(right.InternalSign >= 0 ? 0 : -1);
            for (var index = 0; index < z.Length; index++)
            {
                var num2 = index >= x.Length ? xExtend : x[index];
                z[index] = num2 | (index >= y.Length ? yExtend : y[index]);
            }

            return new BigInteger(z);
        }

        /// <summary>Returns the bitwise one's complement of a <see cref="BigInteger" /> value.</summary>
        /// <returns>The bitwise one's complement of <paramref name="value" />.</returns>
        /// <param name="value">An integer value.</param>
        public static BigInteger operator ~(BigInteger value)
        {
            return -(value + One);
        }

        /// <summary>Adds the values of two specified <see cref="BigInteger" /> objects.</summary>
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

        /// <summary>
        ///     Returns the value of the <see cref="BigInteger" /> operand. (The sign of the operand is
        ///     unchanged.)
        /// </summary>
        /// <returns>The value of the <paramref name="value" /> operand.</returns>
        /// <param name="value">An integer value.</param>
        public static BigInteger operator +(BigInteger value)
        {
            return value;
        }

        /// <summary>Increments a <see cref="BigInteger" /> value by 1.</summary>
        /// <returns>The value of the <paramref name="value" /> parameter incremented by 1.</returns>
        /// <param name="value">The value to increment.</param>
        public static BigInteger operator ++(BigInteger value)
        {
            return value + One;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is less than
        ///     another <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is less than a
        ///     64-bit signed integer.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <(BigInteger left, long right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a 64-bit signed integer is less than a
        ///     <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <(long left, BigInteger right)
        {
            return right.CompareTo(left) > 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is less than a
        ///     64-bit unsigned integer.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator <(BigInteger left, ulong right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a 64-bit unsigned integer is less than a
        ///     <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is less than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator <(ulong left, BigInteger right)
        {
            return right.CompareTo(left) > 0;
        }

        /// <summary>Shifts a <see cref="BigInteger" /> value a specified number of bits to the left.</summary>
        /// <returns>A value that has been shifted to the left by the specified number of bits.</returns>
        /// <param name="value">The value whose bits are to be shifted.</param>
        /// <param name="shift">The number of bits to shift <paramref name="value" /> to the left.</param>
        public static BigInteger operator <<(BigInteger value, int shift)
        {
            switch (shift)
            {
                case 0:
                    return value;

                case int.MinValue:
                    return value >> int.MaxValue >> 1;

                default:
                    break;
            }

            if (shift < 0)
            {
                return value >> -shift;
            }

            var digitShift = shift / 32;
            var smallShift = shift - (digitShift * 32);
            var partsForBitManipulation = GetPartsForBitManipulation(ref value, out var xd, out var xl);
            var zd = new uint[xl + digitShift + 1];
            if (smallShift != 0)
            {
                var carryShift = 32 - smallShift;
                uint carry = 0;
                int index;
                for (index = 0; index < xl; index++)
                {
                    var rot = xd[index];
                    zd[index + digitShift] = (rot << (smallShift & 31)) | carry;
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

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is less than or
        ///     equal to another <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is less than or
        ///     equal to a 64-bit signed integer.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <=(BigInteger left, long right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a 64-bit signed integer is less than or equal to a
        ///     <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator <=(long left, BigInteger right)
        {
            return right.CompareTo(left) >= 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is less than or
        ///     equal to a 64-bit unsigned integer.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator <=(BigInteger left, ulong right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a 64-bit unsigned integer is less than or equal to a
        ///     <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is less than or equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator <=(ulong left, BigInteger right)
        {
            return right.CompareTo(left) >= 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether the values of two <see cref="BigInteger" /> objects
        ///     are equal.
        /// </summary>
        /// <returns>
        ///     true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise,
        ///     false.
        /// </returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator ==(BigInteger left, BigInteger right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value and a signed long
        ///     integer value are equal.
        /// </summary>
        /// <returns>
        ///     true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise,
        ///     false.
        /// </returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator ==(BigInteger left, long right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Returns a value that indicates whether a signed long integer value and a
        ///     <see cref="BigInteger" /> value are equal.
        /// </summary>
        /// <returns>
        ///     true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise,
        ///     false.
        /// </returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator ==(long left, BigInteger right)
        {
            return right.Equals(left);
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value and an unsigned
        ///     long integer value are equal.
        /// </summary>
        /// <returns>
        ///     true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise,
        ///     false.
        /// </returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator ==(BigInteger left, ulong right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Returns a value that indicates whether an unsigned long integer value and a
        ///     <see cref="BigInteger" /> value are equal.
        /// </summary>
        /// <returns>
        ///     true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise,
        ///     false.
        /// </returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator ==(ulong left, BigInteger right)
        {
            return right.Equals(left);
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is greater than
        ///     another <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> is greater than a 64-bit
        ///     signed integer value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >(BigInteger left, long right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a 64-bit signed integer is greater than a
        ///     <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >(long left, BigInteger right)
        {
            return right.CompareTo(left) < 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is greater than a
        ///     64-bit unsigned integer.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator >(BigInteger left, ulong right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is greater than a
        ///     64-bit unsigned integer.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator >(ulong left, BigInteger right)
        {
            return right.CompareTo(left) < 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is greater than or
        ///     equal to another <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >=(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is greater than or
        ///     equal to a 64-bit signed integer value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >=(BigInteger left, long right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a 64-bit signed integer is greater than or equal to a
        ///     <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static bool operator >=(long left, BigInteger right)
        {
            return right.CompareTo(left) <= 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a <see cref="BigInteger" /> value is greater than or
        ///     equal to a 64-bit unsigned integer value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator >=(BigInteger left, ulong right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        ///     Returns a value that indicates whether a 64-bit unsigned integer is greater than or equal to a
        ///     <see cref="BigInteger" /> value.
        /// </summary>
        /// <returns>true if <paramref name="left" /> is greater than <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        [CLSCompliant(false)]
        public static bool operator >=(ulong left, BigInteger right)
        {
            return right.CompareTo(left) <= 0;
        }

        /// <summary>Shifts a <see cref="BigInteger" /> value a specified number of bits to the right.</summary>
        /// <returns>A value that has been shifted to the right by the specified number of bits.</returns>
        /// <param name="value">The value whose bits are to be shifted.</param>
        /// <param name="shift">The number of bits to shift <paramref name="value" /> to the right.</param>
        public static BigInteger operator >>(BigInteger value, int shift)
        {
            switch (shift)
            {
                case 0:
                    return value;

                case int.MinValue:
                    return value << int.MaxValue << 1;

                default:
                    break;
            }

            if (shift < 0)
            {
                return value << -shift;
            }

            var digitShift = shift / 32;
            var smallShift = shift - (digitShift * 32);
            var negative = GetPartsForBitManipulation(ref value, out var xd, out var xl);
            if (negative)
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
                    if (!negative || index != xl - 1)
                    {
                        zd[index - digitShift] = (rot >> (smallShift & 31)) | carry;
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

            if (negative)
            {
                NumericsHelpers.DangerousMakeTwosComplement(zd);
            }

            return new BigInteger(zd, negative);
        }
    }
}

#endif