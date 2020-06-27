#if LESSTHAN_NET40 || NETSTANDARD1_0

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable EPS05 // Use in-modifier for passing readonly struct
// ReSharper disable UselessBinaryOperation

using System.Globalization;
using Theraot.Core;

namespace System.Numerics
{
    public readonly partial struct BigInteger
    {
        private static readonly BigInteger _bigIntegerMinInt = new BigInteger(-1, new[] { unchecked((uint)int.MinValue) });

        /// <summary>Gets a value that represents the number negative one (-1).</summary>
        /// <returns>An integer whose value is negative one (-1).</returns>
        public static BigInteger MinusOne { get; } = new BigInteger(-1);

        /// <summary>Gets a value that represents the number one (1).</summary>
        /// <returns>An object whose value is one (1).</returns>
        public static BigInteger One { get; } = new BigInteger(1);

        /// <summary>Gets a value that represents the number 0 (zero).</summary>
        /// <returns>An integer whose value is 0 (zero).</returns>
        public static BigInteger Zero { get; } = new BigInteger(0);

        /// <summary>Gets the absolute value of a <see cref="BigInteger" /> object.</summary>
        /// <returns>The absolute value of <paramref name="value" />.</returns>
        /// <param name="value">A number.</param>
        public static BigInteger Abs(BigInteger value)
        {
            return value < Zero ? -value : value;
        }

        /// <summary>Adds two <see cref="BigInteger" /> values and returns the result.</summary>
        /// <returns>The sum of <paramref name="left" /> and <paramref name="right" />.</returns>
        /// <param name="left">The first value to add.</param>
        /// <param name="right">The second value to add.</param>
        public static BigInteger Add(BigInteger left, BigInteger right)
        {
            return left + right;
        }

        /// <summary>
        ///     Compares two <see cref="BigInteger" /> values and returns an integer that indicates whether
        ///     the first value is less than, equal to, or greater than the second value.
        /// </summary>
        /// <returns>
        ///     A signed integer that indicates the relative values of <paramref name="left" /> and <paramref name="right" />,
        ///     as shown in the following table.ValueConditionLess than zero<paramref name="left" /> is less than
        ///     <paramref name="right" />.Zero<paramref name="left" /> equals <paramref name="right" />.Greater than zero
        ///     <paramref name="left" /> is greater than <paramref name="right" />.
        /// </returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static int Compare(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right);
        }

        /// <summary>Divides one <see cref="BigInteger" /> value by another and returns the result.</summary>
        /// <returns>The quotient of the division.</returns>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <exception cref="DivideByZeroException">
        ///     <paramref name="divisor" /> is 0 (zero).
        /// </exception>
        public static BigInteger Divide(BigInteger dividend, BigInteger divisor)
        {
            return dividend / divisor;
        }

        /// <summary>
        ///     Divides one <see cref="BigInteger" /> value by another, returns the result, and returns the
        ///     remainder in an output parameter.
        /// </summary>
        /// <returns>The quotient of the division.</returns>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <param name="remainder">
        ///     When this method returns, contains a <see cref="BigInteger" /> value that
        ///     represents the remainder from the division. This parameter is passed uninitialized.
        /// </param>
        /// <exception cref="DivideByZeroException">
        ///     <paramref name="divisor" /> is 0 (zero).
        /// </exception>
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

        /// <summary>Finds the greatest common divisor of two <see cref="BigInteger" /> values.</summary>
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

        /// <summary>Returns the natural (base e) logarithm of a specified number.</summary>
        /// <returns>The natural (base e) logarithm of <paramref name="value" />, as shown in the table in the Remarks section.</returns>
        /// <param name="value">The number whose logarithm is to be found.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The natural log of <paramref name="value" /> is out of range of
        ///     the <see cref="double" /> data type.
        /// </exception>
        public static double Log(BigInteger value)
        {
            return Log(value, Math.E);
        }

        /// <summary>Returns the logarithm of a specified number in a specified base.</summary>
        /// <returns>
        ///     The base <paramref name="baseValue" /> logarithm of <paramref name="value" />, as shown in the table in the
        ///     Remarks section.
        /// </returns>
        /// <param name="value">A number whose logarithm is to be found.</param>
        /// <param name="baseValue">The base of the logarithm.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The log of <paramref name="value" /> is out of range of the
        ///     <see cref="double" /> data type.
        /// </exception>
        public static double Log(BigInteger value, double baseValue)
        {
            if (value.InternalSign < 0 || NumericHelper.IsOne(baseValue))
            {
                return double.NaN;
            }

            if (double.IsPositiveInfinity(baseValue))
            {
                return !value.IsOne ? double.NaN : 0;
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
            var topBits = BitLengthOfUInt(value.InternalBits[length - 1]);
            var bitlen = ((length - 1) * 32) + topBits;
            var currentBitMask = (uint)(1 << ((topBits - 1) & 31));
            for (var index = length - 1; index >= 0; index--)
            {
                while (currentBitMask != 0)
                {
                    if ((value.InternalBits[index] & currentBitMask) != 0)
                    {
                        c += d;
                    }

                    d *= 0.5;
                    currentBitMask >>= 1;
                }

                currentBitMask = unchecked((uint)int.MinValue);
            }

            return (Math.Log(c) + (0.69314718055994529D * bitlen)) / Math.Log(baseValue);
        }

        /// <summary>Returns the base 10 logarithm of a specified number.</summary>
        /// <returns>The base 10 logarithm of <paramref name="value" />, as shown in the table in the Remarks section.</returns>
        /// <param name="value">A number whose logarithm is to be found.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The base 10 log of <paramref name="value" /> is out of range of
        ///     the <see cref="double" /> data type.
        /// </exception>
        public static double Log10(BigInteger value)
        {
            return Log(value, 10);
        }

        /// <summary>Returns the larger of two <see cref="BigInteger" /> values.</summary>
        /// <returns>The <paramref name="left" /> or <paramref name="right" /> parameter, whichever is larger.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static BigInteger Max(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) < 0 ? right : left;
        }

        /// <summary>Returns the smaller of two <see cref="BigInteger" /> values.</summary>
        /// <returns>The <paramref name="left" /> or <paramref name="right" /> parameter, whichever is smaller.</returns>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        public static BigInteger Min(BigInteger left, BigInteger right)
        {
            return left.CompareTo(right) <= 0 ? left : right;
        }

        /// <summary>Performs modulus division on a number raised to the power of another number.</summary>
        /// <returns>The remainder after dividing <paramref name="value" />exponent by <paramref name="modulus" />.</returns>
        /// <param name="value">The number to raise to the <paramref name="exponent" /> power.</param>
        /// <param name="exponent">The exponent to raise <paramref name="value" /> by.</param>
        /// <param name="modulus">
        ///     The number by which to divide <paramref name="value" /> raised to the
        ///     <paramref name="exponent" /> power.
        /// </param>
        /// <exception cref="DivideByZeroException">
        ///     <paramref name="modulus" /> is zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="exponent" /> is negative.
        /// </exception>
        public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            if (exponent.Sign < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exponent), "The number must be greater than or equal to zero.");
            }

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

            return regRes.GetInteger(GetSign(value, isEven));
        }

        /// <summary>Returns the product of two <see cref="BigInteger" /> values.</summary>
        /// <returns>The product of the <paramref name="left" /> and <paramref name="right" /> parameters.</returns>
        /// <param name="left">The first number to multiply.</param>
        /// <param name="right">The second number to multiply.</param>
        public static BigInteger Multiply(BigInteger left, BigInteger right)
        {
            return left * right;
        }

        /// <summary>Negates a specified <see cref="BigInteger" /> value.</summary>
        /// <returns>The result of the <paramref name="value" /> parameter multiplied by negative one (-1).</returns>
        /// <param name="value">The value to negate.</param>
        public static BigInteger Negate(BigInteger value)
        {
            return -value;
        }

        /// <summary>Converts the string representation of a number to its <see cref="BigInteger" /> equivalent.</summary>
        /// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
        /// <param name="value">A string that contains the number to convert.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value" /> is null.
        /// </exception>
        /// <exception cref="FormatException">
        ///     <paramref name="value" /> is not in the correct format.
        /// </exception>
        public static BigInteger Parse(string value)
        {
            return ParseBigInteger(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///     Converts the string representation of a number in a specified style to its
        ///     <see cref="BigInteger" /> equivalent.
        /// </summary>
        /// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
        /// <param name="value">A string that contains a number to convert. </param>
        /// <param name="style">
        ///     A bitwise combination of the enumeration values that specify the permitted format of
        ///     <paramref name="value" />.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="style" /> is not a <see cref="NumberStyles" /> value.-or-
        ///     <paramref name="style" /> includes the <see cref="NumberStyles.AllowHexSpecifier" /> or
        ///     <see cref="NumberStyles.HexNumber" /> flag along with another value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value" /> is null.
        /// </exception>
        /// <exception cref="FormatException">
        ///     <paramref name="value" /> does not comply with the input pattern specified by
        ///     <see cref="NumberStyles" />.
        /// </exception>
        public static BigInteger Parse(string value, NumberStyles style)
        {
            return ParseBigInteger(value, style, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///     Converts the string representation of a number in a specified culture-specific format to its
        ///     <see cref="BigInteger" /> equivalent.
        /// </summary>
        /// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="value" />.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value" /> is null.
        /// </exception>
        /// <exception cref="FormatException">
        ///     <paramref name="value" /> is not in the correct format.
        /// </exception>
        public static BigInteger Parse(string value, IFormatProvider provider)
        {
            return ParseBigInteger(value, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
        }

        /// <summary>
        ///     Converts the string representation of a number in a specified style and culture-specific format to its
        ///     <see cref="BigInteger" /> equivalent.
        /// </summary>
        /// <returns>A value that is equivalent to the number specified in the <paramref name="value" /> parameter.</returns>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <param name="style">
        ///     A bitwise combination of the enumeration values that specify the permitted format of
        ///     <paramref name="value" />.
        /// </param>
        /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="value" />.</param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="style" /> is not a <see cref="NumberStyles" /> value.-or-
        ///     <paramref name="style" /> includes the <see cref="NumberStyles.AllowHexSpecifier" /> or
        ///     <see cref="NumberStyles.HexNumber" /> flag along with another value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value" /> is null.
        /// </exception>
        /// <exception cref="FormatException">
        ///     <paramref name="value" /> does not comply with the input pattern specified by <paramref name="style" />.
        /// </exception>
        public static BigInteger Parse(string value, NumberStyles style, IFormatProvider provider)
        {
            return ParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider));
        }

        /// <summary>Raises a <see cref="BigInteger" /> value to the power of a specified value.</summary>
        /// <returns>The result of raising <paramref name="value" /> to the <paramref name="exponent" /> power.</returns>
        /// <param name="value">The number to raise to the <paramref name="exponent" /> power.</param>
        /// <param name="exponent">The exponent to raise <paramref name="value" /> by.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The value of the <paramref name="exponent" /> parameter is
        ///     negative.
        /// </exception>
        public static BigInteger Pow(BigInteger value, int exponent)
        {
            if (exponent < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exponent), "The number must be greater than or equal to zero.");
            }

            switch (exponent)
            {
                case 0:
                    return One;

                case 1:
                    return value;

                default:
                    break;
            }

            if (value.InternalBits == null)
            {
                switch (value.InternalSign)
                {
                    case 1:
                        return value;

                    case -1:
                        return (exponent & 1) == 0 ? 1 : value;

                    case 0:
                        return value;

                    default:
                        break;
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
            for (var expTmp = exponent; ;)
            {
                if ((expTmp & 1) != 0)
                {
                    MulUpper(ref resultMax, ref maxResultSize, maxSquare, maxSquareSize);
                    MulLower(ref resultMin, ref minResultSize, minSquare, minSquareSize);
                }

                expTmp >>= 1;
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

            for (var expTmp = exponent; ;)
            {
                if ((expTmp & 1) != 0)
                {
                    NumericHelper.Swap(ref regResult, ref regTmp);
                    regResult.Mul(ref regSquare, ref regTmp);
                }

                expTmp >>= 1;
                if (expTmp == 0)
                {
                    break;
                }

                NumericHelper.Swap(ref regSquare, ref regTmp);
                regSquare.Mul(ref regTmp, ref regTmp);
            }

            return regResult.GetInteger(sign);
        }

        /// <summary>Performs integer division on two <see cref="BigInteger" /> values and returns the remainder.</summary>
        /// <returns>The remainder after dividing <paramref name="dividend" /> by <paramref name="divisor" />.</returns>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <exception cref="DivideByZeroException">
        ///     <paramref name="divisor" /> is 0 (zero).
        /// </exception>
        public static BigInteger Remainder(BigInteger dividend, BigInteger divisor)
        {
            return dividend % divisor;
        }

        /// <summary>Subtracts one <see cref="BigInteger" /> value from another and returns the result.</summary>
        /// <returns>The result of subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend).</param>
        public static BigInteger Subtract(BigInteger left, BigInteger right)
        {
            return left - right;
        }

        /// <summary>
        ///     Tries to convert the string representation of a number to its <see cref="BigInteger" />
        ///     equivalent, and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <returns>true if <paramref name="value" /> was converted successfully; otherwise, false.</returns>
        /// <param name="value">The string representation of a number.</param>
        /// <param name="result">
        ///     When this method returns, contains the <see cref="BigInteger" /> equivalent to
        ///     the number that is contained in <paramref name="value" />, or zero (0) if the conversion fails. The conversion
        ///     fails if the <paramref name="value" /> parameter is null or is not of the correct format. This parameter is passed
        ///     uninitialized.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value" /> is null.
        /// </exception>
        public static bool TryParse(string value, out BigInteger result)
        {
            return TryParseBigInteger(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        /// <summary>
        ///     Tries to convert the string representation of a number in a specified style and culture-specific format to its
        ///     <see cref="BigInteger" /> equivalent, and returns a value that indicates whether the conversion
        ///     succeeded.
        /// </summary>
        /// <returns>true if the <paramref name="value" /> parameter was converted successfully; otherwise, false.</returns>
        /// <param name="value">
        ///     The string representation of a number. The string is interpreted using the style specified by
        ///     <paramref name="style" />.
        /// </param>
        /// <param name="style">
        ///     A bitwise combination of enumeration values that indicates the style elements that can be present
        ///     in <paramref name="value" />. A typical value to specify is
        ///     <see cref="NumberStyles.Integer" />.
        /// </param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="value" />.</param>
        /// <param name="result">
        ///     When this method returns, contains the <see cref="BigInteger" /> equivalent to
        ///     the number that is contained in <paramref name="value" />, or <see cref="Zero" /> if
        ///     the conversion failed. The conversion fails if the <paramref name="value" /> parameter is null or is not in a
        ///     format that is compliant with <paramref name="style" />. This parameter is passed uninitialized.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="style" /> is not a <see cref="NumberStyles" /> value.-or-
        ///     <paramref name="style" /> includes the <see cref="NumberStyles.AllowHexSpecifier" /> or
        ///     <see cref="NumberStyles.HexNumber" /> flag along with another value.
        /// </exception>
        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out BigInteger result)
        {
            return TryParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider), out result);
        }

        internal static int BitLengthOfUInt(uint x)
        {
            var numBits = 0;
            while (x > 0)
            {
                x >>= 1;
                numBits++;
            }

            return numBits;
        }

        internal static int GetDiffLength(uint[] internalBits, uint[] otherInternalBits, int length)
        {
            var index = length;
            do
            {
                index--;
                if (index >= 0)
                {
                    continue;
                }

                return 0;
            } while (internalBits[index] == otherInternalBits[index]);

            return index + 1;
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

        private static bool GetPartsForBitManipulation(ref BigInteger x, out uint[] xd, out int xl)
        {
            xd = x.InternalBits ?? (x.InternalSign >= 0 ? new[] { unchecked((uint)x.InternalSign) } : new[] { unchecked((uint)-x.InternalSign) });
            xl = x.InternalBits?.Length ?? 1;
            return x.InternalSign < 0;
        }

        private static int GetSign(in BigInteger value, bool isEven)
        {
            if (value.InternalSign > 0)
            {
                return 1;
            }

            if (!isEven)
            {
                return -1;
            }

            return 1;
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
                    exp >>= 1;
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
                exp >>= 1;
            }
        }

        private static void ModPowSquareModValue(ref BigIntegerBuilder regVal, ref BigIntegerBuilder regMod, ref BigIntegerBuilder regTmp)
        {
            NumericHelper.Swap(ref regVal, ref regTmp);
            regVal.Mul(ref regTmp, ref regTmp);
            regVal.Mod(ref regMod);
        }

        private static void ModPowUpdateResult(ref BigIntegerBuilder regRes, ref BigIntegerBuilder regVal, ref BigIntegerBuilder regMod, ref BigIntegerBuilder regTmp)
        {
            NumericHelper.Swap(ref regRes, ref regTmp);
            regRes.Mul(ref regTmp, ref regVal);
            regRes.Mod(ref regMod);
        }

        private static void MulLower(ref uint uHiRes, ref int cuRes, uint uHiMul, int cuMul)
        {
            var num = uHiRes * (ulong)uHiMul;
            var hi = NumericHelper.GetHi(num);
            if (hi == 0)
            {
                uHiRes = NumericHelper.GetLo(num);
                cuRes += cuMul - 1;
            }
            else
            {
                uHiRes = hi;
                cuRes += cuMul;
            }
        }

        private static void MulUpper(ref uint uHiRes, ref int cuRes, uint uHiMul, int cuMul)
        {
            var num = uHiRes * (ulong)uHiMul;
            var hi = NumericHelper.GetHi(num);
            if (hi == 0)
            {
                uHiRes = NumericHelper.GetLo(num);
                cuRes += cuMul - 1;
            }
            else
            {
                if (NumericHelper.GetLo(num) != 0)
                {
                    var num1 = hi + 1;
                    hi = num1;
                    if (num1 == 0)
                    {
                        hi = 1;
                        cuRes++;
                    }
                }

                uHiRes = hi;
                cuRes += cuMul;
            }
        }

        private static void SetBitsFromDouble(double value, out uint[]? bits, out int sign)
        {
            sign = 0;
            bits = null;
            NumericHelper.GetDoubleParts(value, out var valueSign, out var valueExp, out var valueMan, out _);
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
                valueMan <<= 11;
                valueExp -= 11;
                var significantDword = ((valueExp - 1) / 32) + 1;
                var extraDword = (significantDword * 32) - valueExp;
                bits = new uint[significantDword + 2];
                bits[significantDword + 1] = (uint)(valueMan >> ((extraDword + 32) & 63));
                bits[significantDword] = (uint)(valueMan >> (extraDword & 63));
                if (extraDword > 0)
                {
                    bits[significantDword - 1] = (uint)valueMan << ((32 - extraDword) & 31);
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

        private static ulong ULong(int length, uint[] internalBits)
        {
            return length <= 1 ? internalBits[0] : NumericHelper.BuildUInt64(internalBits[1], internalBits[0]);
        }
    }
}

#endif