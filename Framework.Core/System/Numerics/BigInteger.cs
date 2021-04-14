#if LESSTHAN_NET40 || NETSTANDARD1_0

using System.Globalization;

namespace System.Numerics
{
    /// <summary>Represents an arbitrarily large signed integer.</summary>
    [Serializable]
    public readonly partial struct BigInteger : IFormattable, IComparable, IComparable<BigInteger>, IEquatable<BigInteger>
    {
        internal readonly uint[]? InternalBits;
        internal readonly int InternalSign;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BigInteger" /> structure using a 32-bit signed
        ///     integer value.
        /// </summary>
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
                this = _bigIntegerMinInt;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BigInteger" /> structure using an unsigned
        ///     32-bit integer value.
        /// </summary>
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

        /// <summary>
        ///     Initializes a new instance of the <see cref="BigInteger" /> structure using a 64-bit signed
        ///     integer value.
        /// </summary>
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
                    this = _bigIntegerMinInt;
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
                num = (ulong)-value;
                InternalSign = -1;
            }

            InternalBits = new[] { (uint)num, (uint)(num >> 32) };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BigInteger" /> structure with an unsigned
        ///     64-bit integer value.
        /// </summary>
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

        /// <summary>
        ///     Initializes a new instance of the <see cref="BigInteger" /> structure using a
        ///     single-precision floating-point value.
        /// </summary>
        /// <param name="value">A single-precision floating-point value.</param>
        /// <exception cref="OverflowException">
        ///     The value of <paramref name="value" /> is
        ///     <see cref="System.Single.NaN" />.-or-The value of <paramref name="value" /> is
        ///     <see cref="System.Single.NegativeInfinity" />.-or-The value of <paramref name="value" /> is
        ///     <see cref="System.Single.PositiveInfinity" />.
        /// </exception>
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

        /// <summary>
        ///     Initializes a new instance of the <see cref="BigInteger" /> structure using a
        ///     double-precision floating-point value.
        /// </summary>
        /// <param name="value">A double-precision floating-point value.</param>
        /// <exception cref="OverflowException">
        ///     The value of <paramref name="value" /> is
        ///     <see cref="System.Double.NaN" />.-or-The value of <paramref name="value" /> is
        ///     <see cref="System.Double.NegativeInfinity" />.-or-The value of <paramref name="value" /> is
        ///     <see cref="System.Double.PositiveInfinity" />.
        /// </exception>
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

        /// <summary>
        ///     Initializes a new instance of the <see cref="BigInteger" /> structure using a
        ///     <see cref="decimal" /> value.
        /// </summary>
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
                this = Zero;
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

                InternalSign = (bits[3] & int.MinValue) == 0 ? 1 : -1;
            }
            else
            {
                InternalSign = bits[0];
                InternalSign *= (bits[3] & int.MinValue) == 0 ? 1 : -1;
                InternalBits = null;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BigInteger" /> structure using the values in a
        ///     byte array.
        /// </summary>
        /// <param name="value">An array of byte values in little-endian order.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value" /> is null.
        /// </exception>
        [CLSCompliant(false)]
        public BigInteger(byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var valueLength = value.Length;
            var isNegative = valueLength > 0 && (value[valueLength - 1] & 128) == 128;
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
                var unalignedBytes = valueLength % 4;
                var dwordCount = (valueLength / 4) + (unalignedBytes == 0 ? 0 : 1);
                var isZero = true;
                var internalBits = new uint[dwordCount];
                var byteIndex = 3;
                var dwordIndex = 0;
                for (; dwordIndex < dwordCount - (unalignedBytes == 0 ? 0 : 1); dwordIndex++)
                {
                    ref var current = ref internalBits[dwordIndex];
                    for (var byteInDword = 0; byteInDword < 4; byteInDword++)
                    {
                        isZero &= value[byteIndex] == 0;
                        current <<= 8;
                        current |= value[byteIndex];
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
                        ref var current = ref internalBits[dwordIndex];
                        ref var currentValue = ref value[byteIndex];
                        isZero &= currentValue == 0;
                        current <<= 8;
                        current |= currentValue;
                    }
                }

                if (isZero)
                {
                    this = Zero;
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
                                this = MinusOne;
                                break;

                            case unchecked((uint)int.MinValue):
                                this = _bigIntegerMinInt;
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
                InternalSign = isNegative ? -1 : 0;
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
                    this = _bigIntegerMinInt;
                }
            }
        }

        internal BigInteger(int internalSign, uint[]? rgu)
        {
            InternalSign = internalSign;
            InternalBits = rgu;
        }

        internal BigInteger(uint[] value, bool negative)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var length = value.Length;
            while (length > 0 && value[length - 1] == 0)
            {
                length--;
            }

            if (length == 0)
            {
                this = Zero;
            }
            else if (length != 1 || value[0] >= unchecked((uint)int.MinValue))
            {
                InternalSign = !negative ? 1 : -1;
                InternalBits = new uint[length];
                Array.Copy(value, InternalBits, length);
            }
            else
            {
                InternalSign = !negative ? (int)value[0] : (int)-value[0];
                InternalBits = null;
                if (InternalSign == int.MinValue)
                {
                    this = _bigIntegerMinInt;
                }
            }
        }

        private BigInteger(uint[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var dwordCount = value.Length;
            var isNegative = dwordCount > 0 && (value[dwordCount - 1] & unchecked((uint)int.MinValue)) == unchecked((uint)int.MinValue);
            while (dwordCount > 0 && value[dwordCount - 1] == 0)
            {
                dwordCount--;
            }

            switch (dwordCount)
            {
                case 0:
                    this = Zero;
                    return;

                case 1:
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
                        this = _bigIntegerMinInt;
                    }

                    return;

                default:
                    break;
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
                this = MinusOne;
            }
            else if (value[0] != unchecked((uint)int.MinValue))
            {
                InternalSign = (int)(-1 * value[0]);
                InternalBits = null;
            }
            else
            {
                this = _bigIntegerMinInt;
            }
        }

        /// <summary>
        ///     Indicates whether the value of the current <see cref="BigInteger" /> object is an even
        ///     number.
        /// </summary>
        /// <returns>
        ///     true if the value of the <see cref="BigInteger" /> object is an even number; otherwise,
        ///     false.
        /// </returns>
        public bool IsEven => InternalBits != null ? (InternalBits[0] & 1) == 0 : (InternalSign & 1) == 0;

        /// <summary>
        ///     Indicates whether the value of the current <see cref="BigInteger" /> object is
        ///     <see cref="One" />.
        /// </summary>
        /// <returns>
        ///     true if the value of the <see cref="BigInteger" /> object is
        ///     <see cref="One" />; otherwise, false.
        /// </returns>
        public bool IsOne => InternalSign == 1 && InternalBits == null;

        /// <summary>
        ///     Indicates whether the value of the current <see cref="BigInteger" /> object is a power of
        ///     two.
        /// </summary>
        /// <returns>
        ///     true if the value of the <see cref="BigInteger" /> object is a power of two; otherwise,
        ///     false.
        /// </returns>
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
                if ((InternalBits[index] & (InternalBits[index] - 1)) != 0)
                {
                    return false;
                }

                do
                {
                    index--;
                    if (index >= 0)
                    {
                        continue;
                    }

                    return true;
                } while (InternalBits[index] == 0);

                return false;
            }
        }

        /// <summary>
        ///     Indicates whether the value of the current <see cref="BigInteger" /> object is
        ///     <see cref="Zero" />.
        /// </summary>
        /// <returns>
        ///     true if the value of the <see cref="BigInteger" /> object is
        ///     <see cref="Zero" />; otherwise, false.
        /// </returns>
        public bool IsZero => InternalSign == 0;

        /// <summary>
        ///     Gets a number that indicates the sign (negative, positive, or zero) of the current
        ///     <see cref="BigInteger" /> object.
        /// </summary>
        /// <returns>
        ///     A number that indicates the sign of the <see cref="BigInteger" /> object, as shown in the
        ///     following table.NumberDescription-1The value of this object is negative.0The value of this object is 0 (zero).1The
        ///     value of this object is positive.
        /// </returns>
        public int Sign => (InternalSign >> 31) - (-InternalSign >> 31);

        /// <summary>
        ///     Compares this instance to a signed 64-bit integer and returns an integer that indicates whether the value of
        ///     this instance is less than, equal to, or greater than the value of the signed 64-bit integer.
        /// </summary>
        /// <returns>
        ///     A signed integer value that indicates the relationship of this instance to <paramref name="other" />, as shown
        ///     in the following table.Return valueDescriptionLess than zeroThe current instance is less than
        ///     <paramref name="other" />.ZeroThe current instance equals <paramref name="other" />.Greater than zeroThe current
        ///     instance is greater than <paramref name="other" />.
        /// </returns>
        /// <param name="other">The signed 64-bit integer to compare.</param>
        public int CompareTo(long other)
        {
            if (InternalBits == null)
            {
                return ((long)InternalSign).CompareTo(other);
            }

            if ((InternalSign ^ other) < 0)
            {
                return InternalSign;
            }

            var length = Length(InternalBits);
            if (length > 2)
            {
                return InternalSign;
            }

            var magnitude = other >= 0 ? (ulong)other : (ulong)-other;
            var unsigned = ULong(length, InternalBits);
            return InternalSign * unsigned.CompareTo(magnitude);
        }

        /// <summary>
        ///     Compares this instance to an unsigned 64-bit integer and returns an integer that indicates whether the value
        ///     of this instance is less than, equal to, or greater than the value of the unsigned 64-bit integer.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative value of this instance and <paramref name="other" />, as shown in
        ///     the following table.Return valueDescriptionLess than zeroThe current instance is less than
        ///     <paramref name="other" />.ZeroThe current instance equals <paramref name="other" />.Greater than zeroThe current
        ///     instance is greater than <paramref name="other" />.
        /// </returns>
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
            return length > 2 ? 1 : ULong(length, InternalBits).CompareTo(other);
        }

        /// <summary>
        ///     Compares this instance to a second <see cref="BigInteger" /> and returns an integer that
        ///     indicates whether the value of this instance is less than, equal to, or greater than the value of the specified
        ///     object.
        /// </summary>
        /// <returns>
        ///     A signed integer value that indicates the relationship of this instance to <paramref name="other" />, as shown
        ///     in the following table.Return valueDescriptionLess than zeroThe current instance is less than
        ///     <paramref name="other" />.ZeroThe current instance equals <paramref name="other" />.Greater than zeroThe current
        ///     instance is greater than <paramref name="other" />.
        /// </returns>
        /// <param name="other">The object to compare.</param>
        public int CompareTo(BigInteger other)
        {
            if ((InternalSign ^ other.InternalSign) < 0)
            {
                return InternalSign >= 0 ? 1 : -1;
            }

            if (InternalBits == null)
            {
                if (other.InternalBits != null)
                {
                    return -other.InternalSign;
                }

                return GetSignComparison(other);
            }

            if (other.InternalBits == null)
            {
                return InternalSign;
            }

            var length = Length(InternalBits);
            var otherLength = Length(other.InternalBits);
            if (length > otherLength)
            {
                return InternalSign;
            }

            if (length < otherLength)
            {
                return -InternalSign;
            }

            var diffLength = GetDiffLength(InternalBits, other.InternalBits, length);
            if (diffLength == 0)
            {
                return 0;
            }

            return InternalBits[diffLength - 1] >= other.InternalBits[diffLength - 1] ? InternalSign : -InternalSign;
        }

        /// <summary>
        ///     Compares this instance to a specified object and returns an integer that indicates whether the value of this
        ///     instance is less than, equal to, or greater than the value of the specified object.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relationship of the current instance to the <paramref name="obj" />
        ///     parameter, as shown in the following table.Return valueDescriptionLess than zeroThe current instance is less than
        ///     <paramref name="obj" />.ZeroThe current instance equals <paramref name="obj" />.Greater than zeroThe current
        ///     instance is greater than <paramref name="obj" />, or the <paramref name="obj" /> parameter is null.
        /// </returns>
        /// <param name="obj">The object to compare.</param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="obj" /> is not a <see cref="BigInteger" />.
        /// </exception>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is BigInteger other)
            {
                return CompareTo(other);
            }

            throw new ArgumentException("The parameter must be a BigInteger.", nameof(obj));
        }

        /// <summary>Returns a value that indicates whether the current instance and a specified object have the same value.</summary>
        /// <returns>
        ///     true if the <paramref name="obj" /> parameter is a <see cref="BigInteger" /> object or a
        ///     type capable of implicit conversion to a <see cref="BigInteger" /> value, and its value is equal
        ///     to the value of the current <see cref="BigInteger" /> object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare. </param>
        public override bool Equals(object obj)
        {
            return obj is BigInteger bigInteger && Equals(bigInteger);
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

            if ((InternalSign ^ other) < 0)
            {
                return false;
            }

            var length = Length(InternalBits);
            if (length > 2)
            {
                return false;
            }

            var magnitude = other >= 0 ? (ulong)other : (ulong)-other;
            return ULong(length, InternalBits) == magnitude;
        }

        /// <summary>
        ///     Returns a value that indicates whether the current instance and an unsigned 64-bit integer have the same
        ///     value.
        /// </summary>
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
                return InternalSign == unchecked((long)other);
            }

            var length = Length(InternalBits);
            if (length > 2)
            {
                return false;
            }

            return ULong(length, InternalBits) == other;
        }

        /// <summary>
        ///     Returns a value that indicates whether the current instance and a specified
        ///     <see cref="BigInteger" /> object have the same value.
        /// </summary>
        /// <returns>
        ///     true if this <see cref="BigInteger" /> object and <paramref name="other" /> have the same
        ///     value; otherwise, false.
        /// </returns>
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

        /// <summary>Returns the hash code for the current <see cref="BigInteger" /> object.</summary>
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
                index--;
                if (index < 0)
                {
                    break;
                }

                sign = NumericsHelpers.CombineHash(sign, (int)InternalBits[index]);
            }

            return sign;
        }

        /// <summary>Converts a <see cref="BigInteger" /> value to a byte array.</summary>
        /// <returns>The value of the current <see cref="BigInteger" /> object converted to an array of bytes.</returns>
        public byte[] ToByteArray()
        {
            uint[] internalBits;
            byte highByte;
            switch (InternalBits)
            {
                case null when InternalSign == 0:
                    return new byte[1];

                case null:
                    internalBits = new[] { unchecked((uint)InternalSign) };
                    highByte = InternalSign < 0 ? (byte)0xff : (byte)0x00;
                    break;

                default:
                    if (InternalSign != -1)
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

                    break;
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

        /// <summary>
        ///     Converts the numeric value of the current <see cref="BigInteger" /> object to its equivalent
        ///     string representation.
        /// </summary>
        /// <returns>The string representation of the current <see cref="BigInteger" /> value.</returns>
        public override string ToString()
        {
            return FormatBigInteger(this, format: null, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///     Converts the numeric value of the current <see cref="BigInteger" /> object to its equivalent
        ///     string representation by using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        ///     The string representation of the current <see cref="BigInteger" /> value in the format
        ///     specified by the <paramref name="provider" /> parameter.
        /// </returns>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        public string ToString(IFormatProvider provider)
        {
            return FormatBigInteger(this, format: null, NumberFormatInfo.GetInstance(provider));
        }

        /// <summary>
        ///     Converts the numeric value of the current <see cref="BigInteger" /> object to its equivalent
        ///     string representation by using the specified format.
        /// </summary>
        /// <returns>
        ///     The string representation of the current <see cref="BigInteger" /> value in the format
        ///     specified by the <paramref name="format" /> parameter.
        /// </returns>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <exception cref="FormatException">
        ///     <paramref name="format" /> is not a valid format string.
        /// </exception>
        public string ToString(string format)
        {
            return FormatBigInteger(this, format, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///     Converts the numeric value of the current <see cref="BigInteger" /> object to its equivalent
        ///     string representation by using the specified format and culture-specific format information.
        /// </summary>
        /// <returns>
        ///     The string representation of the current <see cref="BigInteger" /> value as specified by the
        ///     <paramref name="format" /> and <paramref name="formatProvider" /> parameters.
        /// </returns>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <exception cref="FormatException">
        ///     <paramref name="format" /> is not a valid format string.
        /// </exception>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return FormatBigInteger(this, format, NumberFormatInfo.GetInstance(formatProvider));
        }

        private readonly int GetSignComparison(in BigInteger other)
        {
            if (InternalSign < other.InternalSign)
            {
                return -1;
            }

            if (InternalSign <= other.InternalSign)
            {
                return 0;
            }

            return 1;
        }

        private uint[] ToUInt32Array()
        {
            uint[] internalBits;
            uint highDword;
            switch (InternalBits)
            {
                case null when InternalSign == 0:
                    return new uint[1];

                case null:
                    internalBits = new[] { unchecked((uint)InternalSign) };
                    highDword = (uint)(InternalSign >= 0 ? 0 : -1);
                    break;

                default:
                    if (InternalSign != -1)
                    {
                        internalBits = InternalBits;
                        highDword = 0;
                    }
                    else
                    {
                        internalBits = (uint[])InternalBits.Clone();
                        NumericsHelpers.DangerousMakeTwosComplement(internalBits);
                        highDword = unchecked((uint)-1);
                    }

                    break;
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
    }
}

#endif