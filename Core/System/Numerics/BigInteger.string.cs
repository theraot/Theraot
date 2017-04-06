#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using Theraot.Collections.Specialized;
using Theraot.Core;

namespace System.Numerics
{
    public partial struct BigInteger
    {
        internal static string FormatBigInteger(BigInteger value, string format, NumberFormatInfo info)
        {
            int digits;
            var fmt = ParseFormatSpecifier(format, out digits);
            if (fmt == 'x' || fmt == 'X')
            {
                return FormatBigIntegerToHexString(value, fmt, digits, info);
            }

            if (fmt == 'e' || fmt == 'E')
            {
                var precision = digits != -1 ? digits : 6;

                if (value.InternalBits == null)
                {
                    return value.InternalSign.ToString(format, info);
                }

                var scale = (int)Math.Floor(Log10(value));
                // ---
                if (scale > precision + 10)
                {
                    do
                    {
                        value /= 1000000000;
                    } while (Log10(value) > precision + 10);
                }
                while (Log10(value) > precision + 2)
                {
                    value /= 10;
                }
                if ((Log10(value) > precision + 1))
                {
                    var round = value % 10 >= 5;
                    value = (value / 10) + (round ? One : Zero);
                }

                ReverseStringBuilder builder;

                if (value.InternalBits == null)
                {
                    builder = new ReverseStringBuilder(10);
                    builder.Prepend(value.InternalSign.ToString("D"));
                }
                else
                {
                    builder = CreateBuilder(value, info, false, 0);
                }

                // ---
                var decimalSeparator = info.NumberDecimalSeparator;

                var result = new StringBuilder(builder.Length + 6);

                var extra = 0;

                if (precision >= builder.Length)
                {
                    extra = precision - (builder.Length - 1);
                    precision = builder.Length - 1;
                }
                result.Append(builder.ToString(builder.Length, 1));
                result.Append(decimalSeparator);
                result.Append(builder.ToString(builder.Length - 1, precision));
                result.Append(new string('0', extra));
                result.Append(fmt);
                result.Append(info.PositiveSign);
                if (scale < 10)
                {
                    result.Append("00");
                }
                else if (scale < 100)
                {
                    result.Append('0');
                }
                result.Append(scale);

                return result.ToString();
            }
            else
            {
                var decimalFmt = (fmt == 'g' || fmt == 'G' || fmt == 'd' || fmt == 'D' || fmt == 'r' || fmt == 'R');
                if (value.InternalBits == null)
                {
                    if (fmt == 'g' || fmt == 'G' || fmt == 'r' || fmt == 'R')
                    {
                        if (digits > 0)
                        {
                            format = "D" + digits.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            format = "D";
                        }
                    }
                    return value.InternalSign.ToString(format, info);
                }
                var builder = CreateBuilder(value, info, decimalFmt, digits);
                if (decimalFmt)
                {
                    // Format Round-trip decimal
                    // This format is supported for integral types only. The number is converted to a string of
                    // decimal digits (0-9), prefixed by a minus sign if the number is negative. The precision
                    // specifier indicates the minimum number of digits desired in the resulting string. If required,
                    // the number is padded with zeros to its left to produce the number of digits given by the
                    // precision specifier.
                    while (digits > 0 && digits >= builder.Length)
                    {
                        builder.Prepend('0');
                        digits--;
                    }
                    if (value.InternalSign < 0)
                    {
                        builder.Prepend(info.NegativeSign);
                    }
                    return builder.ToString();
                }
                // 'c', 'C', 'e', 'E', 'f', 'F', 'n', 'N', 'p', 'P', custom
                var precision = -1;
                var groupingSizes = new[] { 3 };
                var groupingSeparator = info.NumberGroupSeparator;
                var decimalSeparator = info.NumberDecimalSeparator;
                var groups = false;
                var type = 0;
                if (fmt == '\0')
                {
                    // parse custom
                }
                else
                {
                    if (fmt == 'c' || fmt == 'C')
                    {
                        decimalSeparator = info.CurrencyDecimalSeparator;
                        precision = digits != -1 ? digits : info.CurrencyDecimalDigits;
                        groupingSeparator = info.CurrencyGroupSeparator;
                        groupingSizes = info.CurrencyGroupSizes;
                        groups = true;
                        type = 1;
                    }
                    else if (fmt == 'f' || fmt == 'F')
                    {
                        precision = digits != -1 ? digits : info.NumberDecimalDigits;
                    }
                    else if (fmt == 'n' || fmt == 'N')
                    {
                        precision = digits != -1 ? digits : info.NumberDecimalDigits;
                        groups = true;
                    }
                    else if (fmt == 'p' || fmt == 'P')
                    {
                        decimalSeparator = info.PercentDecimalSeparator;
                        precision = digits != -1 ? digits : info.PercentDecimalDigits;
                        groups = true;
                        type = 2;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                var result = new StringBuilder(builder.Length + 20);
                var close = SetWrap(value, info, type, result);
                var append = builder;
                if (groups)
                {
                    var extra = groupingSizes.Length - 1;
                    if (groupingSizes[groupingSizes.Length - 1] != 0)
                    {
                        var totalDigits = builder.Length;
                        extra += (int)Math.Ceiling(totalDigits * 1.0 / groupingSizes[groupingSizes.Length - 1]);
                    }
                    var length = extra + builder.Length;
                    if (type == 2)
                    {
                        length += 2;
                        append = StringWithGroups(length, new ExtendedEnumerable<char>(new[] { '0', '0' }, builder), groupingSizes, groupingSeparator);
                    }
                    else
                    {
                        append = StringWithGroups(extra + builder.Length, builder, groupingSizes, groupingSeparator);
                    }
                }
                result.Append(append);
                if (precision > 0)
                {
                    result.Append(decimalSeparator);
                    result.Append(new string('0', precision));
                }
                result.Append(close);
                return result.ToString();
            }
        }

        internal static BigInteger ParseBigInteger(string value, NumberStyles style, NumberFormatInfo info)
        {
            ArgumentException argumentException;
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (!TryValidateParseStyleInteger(style, out argumentException))
            {
                throw argumentException;
            }
            BigInteger zero;
            if (!TryParseBigInteger(value, style, info, out zero))
            {
                throw new FormatException("The value could not be parsed.");
            }
            return zero;
        }

        internal static char ParseFormatSpecifier(string format, out int digits)
        {
            digits = -1;
            if (string.IsNullOrEmpty(format))
            {
                return 'R';
            }
            var index = 0;
            var chr = format[index];
            if (chr >= 'A' && chr <= 'Z' || chr >= 'a' && chr <= 'z')
            {
                index++;
                if (index < format.Length)
                {
                    var tmp = format[index];
                    if (tmp >= '0' && tmp <= '9')
                    {
                        index++;
                        digits = tmp - '0';
                        do
                        {
                            if (index >= format.Length || format[index] < '0' || format[index] > '9')
                            {
                                break;
                            }
                            digits = digits * 10 + (format[index] - '0');
                            index++;
                        } while (digits < 10);
                    }
                }
                if (index >= format.Length || format[index] == 0)
                {
                    return chr;
                }
            }
            return '\0';
        }

        internal static bool ParseNumber(StringProcessor reader, NumberStyles options, BigNumberBuffer number, NumberFormatInfo info)
        {
            // Percent intentionally not supported
            // After testig with .NET the patterns are ignored... all patterns are welcome

            var currencySymbol = info.CurrencySymbol;
            var numberGroupSeparator = info.NumberGroupSeparator;
            var currencyGroupSeparator = info.CurrencyGroupSeparator;
            var positiveSign = info.PositiveSign;
            var negativeSign = info.NegativeSign;
            if ((options & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
            {
                var allowLeadingWhite = (options & NumberStyles.AllowLeadingWhite) != NumberStyles.None;
                var allowTrailingWhite = (options & NumberStyles.AllowTrailingWhite) != NumberStyles.None;
                /*
                // Assume validated
                if (
                    (options & NumberStyles.AllowCurrencySymbol) != NumberStyles.None
                    || (options & NumberStyles.AllowLeadingSign) != NumberStyles.None
                    || (options & NumberStyles.AllowParentheses) != NumberStyles.None
                    || (options & NumberStyles.AllowThousands) != NumberStyles.None
                    || (options & NumberStyles.AllowExponent) != NumberStyles.None
                    || (options & NumberStyles.AllowTrailingSign) != NumberStyles.None
                    )
                {
                    return false;
                }*/
                number.Negative = false;
                if (allowLeadingWhite)
                {
                    reader.SkipWhile(CharHelper.IsClassicWhitespace);
                }
                while (true)
                {
                    var input =
                        reader.ReadWhile(new[]
                        {
                            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C',
                            'D', 'E', 'F'
                        });
                    if (input == string.Empty)
                    {
                        break;
                    }
                    number.Scale += input.Length;
                    number.Digits.Append(input.ToUpperInvariant());
                }
                if (allowTrailingWhite)
                {
                    reader.SkipWhile(CharHelper.IsClassicWhitespace);
                }
                return reader.EndOfString;
            }
            else
            {
                var allowCurrencySymbol = (options & NumberStyles.AllowCurrencySymbol) != NumberStyles.None;
                var allowLeadingWhite = (options & NumberStyles.AllowLeadingWhite) != NumberStyles.None;
                var allowLeadingSign = (options & NumberStyles.AllowLeadingSign) != NumberStyles.None;
                var allowParentheses = (options & NumberStyles.AllowParentheses) != NumberStyles.None;
                var allowThousands = (options & NumberStyles.AllowThousands) != NumberStyles.None;
                var allowExponent = (options & NumberStyles.AllowExponent) != NumberStyles.None;
                var allowTrailingWhite = (options & NumberStyles.AllowTrailingWhite) != NumberStyles.None;
                var allowTrailingSign = (options & NumberStyles.AllowTrailingSign) != NumberStyles.None;
                var allowDecimalPoint = (options & NumberStyles.AllowDecimalPoint) != NumberStyles.None;

                var isCurrency = false;
                number.Negative = false;
                var waitingParentheses = false;
                var positive = false;
                // [ws][$][sign][digits,]digits[E[sign]exponential_digits][ws]
                if (allowLeadingWhite)
                {
                    reader.SkipWhile(CharHelper.IsClassicWhitespace);
                }
                // [$][sign][digits,]digits[E[sign]exponential_digits][ws]
                if (allowCurrencySymbol && reader.Read(currencySymbol))
                {
                    isCurrency = true;
                    reader.SkipWhile(CharHelper.IsClassicWhitespace);
                }
                // [sign][digits,]digits[E[sign]exponential_digits][ws
                if (allowLeadingSign)
                {
                    if (reader.Read(negativeSign))
                    {
                        number.Negative = true;
                    }
                    if (reader.Read(positiveSign))
                    {
                        positive = true;
                    }
                }
                if (!number.Negative && allowParentheses && reader.Read('('))
                {
                    // Testing on .NET show that $(n) is allowed, even tho there is no CurrencyNegativePattern for it
                    number.Negative = true;
                    waitingParentheses = true;
                }
                // ---
                if (!isCurrency && allowCurrencySymbol && reader.Read(currencySymbol)) // If the currency symbol is after the negative sign
                {
                    isCurrency = true;
                    reader.SkipWhile(CharHelper.IsClassicWhitespace);
                }
                // [digits,]digits[E[sign]exponential_digits][ws]
                var failure = true;
                var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                var decimalFound = false;
                while (true)
                {
                    var input = reader.ReadWhile(digits);
                    if (input == string.Empty)
                    {
                        if (allowDecimalPoint && !decimalFound)
                        {
                            if (reader.Read(info.CurrencyDecimalSeparator))
                            {
                                decimalFound = true;
                                continue;
                            }
                            if (reader.Read(info.NumberDecimalSeparator))
                            {
                                if (isCurrency)
                                {
                                    return false;
                                }
                                decimalFound = true;
                                continue;
                            }
                        }
                        break;
                    }
                    failure = false;
                    if (!decimalFound)
                    {
                        number.Scale += input.Length;
                    }
                    number.Digits.Append(input);
                    if (allowThousands)
                    {
                        // Testing on .NET show that combining currency and number group separators is allowed
                        // But not if the currency symbol has already appeared
                        reader.SkipWhile(currencyGroupSeparator);
                        if (!isCurrency)
                        {
                            reader.SkipWhile(numberGroupSeparator);
                        }
                    }
                }
                if (failure)
                {
                    return false;
                }
                // [E[sign]exponential_digits][ws]
                if (allowExponent && (reader.Read('E') || reader.Read('e')))
                {
                    // [sign]exponential_digits
                    // Testing on .NET show that no pattern is used here, also no parentheses nor group separators supported
                    // The exponent can be big - but anything beyond 9999 is ignored
                    var exponentNegative = reader.Read(negativeSign);
                    if (!exponentNegative)
                    {
                        reader.Read(positiveSign);
                    }
                    var input = reader.ReadWhile(digits);
                    var exponentMagnitude = int.Parse(input, CultureInfo.InvariantCulture);
                    number.Scale += (exponentNegative ? -1 : 1) * (input.Length > 4 ? 9999 : exponentMagnitude);
                    if (number.Scale < 0)
                    {
                        return false;
                    }
                }
                // ---
                if (allowTrailingWhite)
                {
                    reader.SkipWhile(CharHelper.IsClassicWhitespace);
                }
                if (!isCurrency && allowCurrencySymbol && reader.Read(currencySymbol))
                {
                    isCurrency = true;
                }
                // ---
                if (!number.Negative && !positive && allowTrailingSign)
                {
                    if (reader.Read(negativeSign))
                    {
                        number.Negative = true;
                    }
                    if (reader.Read(positiveSign))
                    {
                        positive = true; // For completness sake
                    }
                }
                if (waitingParentheses && !reader.Read(')'))
                {
                    return false;
                }
                // ---
                if (!isCurrency && allowCurrencySymbol && reader.Read(currencySymbol)) // If the currency symbol is after the negative sign
                {
                    isCurrency = true; // For completeness sake
                }
                // [ws]
                if (allowTrailingWhite)
                {
                    reader.SkipWhile(CharHelper.IsClassicWhitespace);
                }
                return reader.EndOfString;
            }
        }

        internal static bool TryParseBigInteger(string value, NumberStyles style, NumberFormatInfo info, out BigInteger result)
        {
            result = Zero;
            ArgumentException e;
            if (!TryValidateParseStyleInteger(style, out e))
            {
                throw e; // TryParse still throws ArgumentException on invalid NumberStyles
            }
            if (value == null)
            {
                return false;
            }
            var number = BigNumberBuffer.Create();
            if (!ParseNumber(new StringProcessor(value), style, number, info))
            {
                return false;
            }
            if ((style & NumberStyles.AllowHexSpecifier) != 0)
            {
                if (!HexNumberToBigInteger(number, ref result))
                {
                    return false;
                }
            }
            else
            {
                if (!NumberToBigInteger(number, out result))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool TryValidateParseStyleInteger(NumberStyles style, out ArgumentException e)
        {
            if (((int)style & -1024) != (int)NumberStyles.None)
            {
                e = new ArgumentException("An undefined NumberStyles value is being used.", "style");
                return false;
            }
            if ((style & NumberStyles.AllowHexSpecifier) == NumberStyles.None || (style & (NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowParentheses | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowCurrencySymbol)) == NumberStyles.None)
            {
                e = null;
                return true;
            }
            e = new ArgumentException("With the AllowHexSpecifier bit set in the enum bit field, the only other valid bits that can be combined into the enum value must be a subset of those in HexNumber.");
            return false;
        }

        private static ReverseStringBuilder CreateBuilder(BigInteger value, NumberFormatInfo info, bool decimalFmt, int digits)
        {
            // First convert to base 10^9.
            const uint NumericBase = 1000000000; // 10^9
            const int NumericBaseLog10 = 9;
            var sourceLength = Length(value.InternalBits);
            int maxConvertedLength;
            try
            {
                maxConvertedLength = checked(sourceLength * 10 / 9 + 2);
            }
            catch (OverflowException e)
            {
                throw new FormatException("The value is too large to be represented by this format specifier.", e);
            }
            var converted = new uint[maxConvertedLength];
            var convertedLength = 0;
            for (var sourceIndex = sourceLength; --sourceIndex >= 0;)
            {
                // Take a cipher from the source
                var carry = value.InternalBits[sourceIndex];
                // Add it to converted
                for (var convertedIndex = 0; convertedIndex < convertedLength; convertedIndex++)
                {
                    var cipherBlock = NumericsHelpers.MakeUlong(converted[convertedIndex], carry);
                    converted[convertedIndex] = (uint)(cipherBlock % NumericBase);
                    carry = (uint)(cipherBlock / NumericBase);
                }
                if (carry != 0)
                {
                    converted[convertedLength++] = carry % NumericBase;
                    carry /= NumericBase;
                    if (carry != 0)
                    {
                        converted[convertedLength++] = carry;
                    }
                }
            }
            int stringCapacity;
            try
            {
                // Each uint contributes at most 9 digits to the decimal representation.
                stringCapacity = checked(convertedLength * NumericBaseLog10);
            }
            catch (OverflowException e)
            {
                throw new FormatException("The value is too large to be represented by this format specifier.", e);
            }
            if (decimalFmt)
            {
                if (digits > 0 && stringCapacity < digits)
                {
                    stringCapacity = digits;
                }
                if (value.InternalSign < 0)
                {
                    try
                    {
                        // Leave an extra slot for a minus sign.
                        stringCapacity = checked(stringCapacity + info.NegativeSign.Length);
                    }
                    catch (OverflowException e)
                    {
                        throw new FormatException("The value is too large to be represented by this format specifier.", e);
                    }
                }
            }
            var result = new ReverseStringBuilder(stringCapacity);
            for (var stringIndex = 0; stringIndex < convertedLength - 1; stringIndex++)
            {
                var cipherBlock = converted[stringIndex];
                for (var cch = NumericBaseLog10; --cch >= 0;)
                {
                    result.Prepend((char)('0' + cipherBlock % 10));
                    cipherBlock /= 10;
                }
            }
            for (var cipherBlock = converted[convertedLength - 1]; cipherBlock != 0;)
            {
                result.Prepend((char)('0' + cipherBlock % 10));
                cipherBlock /= 10;
            }
            return result;
        }

        private static string FormatBigIntegerToHexString(BigInteger value, char format, int digits, NumberFormatInfo info)
        {
            var stringBuilder = new StringBuilder();
            var byteArray = value.ToByteArray();
            string str1;
            var length = byteArray.Length - 1;
            if (length > -1)
            {
                var flag = false;
                var num = byteArray[length];
                if (num > 247)
                {
                    num = (byte)(num - 240);
                    flag = true;
                }
                if (num < 8 || flag)
                {
                    str1 = string.Format(CultureInfo.InvariantCulture, "{0}1", format);
                    stringBuilder.Append(num.ToString(str1, info));
                    length--;
                }
            }
            if (length > -1)
            {
                str1 = string.Format(CultureInfo.InvariantCulture, "{0}2", format);
                while (length > -1)
                {
                    var num1 = length;
                    length = num1 - 1;
                    stringBuilder.Append(byteArray[num1].ToString(str1, info));
                }
            }
            if (digits > 0 && digits > stringBuilder.Length)
            {
                var stringBuilder1 = stringBuilder;
                string str;
                if (value.InternalSign < 0)
                {
                    str = (format != 'x' ? "F" : "f");
                }
                else
                {
                    str = "0";
                }
                stringBuilder1.Insert(0, str, digits - stringBuilder.Length);
            }
            return stringBuilder.ToString();
        }

        private static bool HexNumberToBigInteger(BigNumberBuffer number, ref BigInteger value)
        {
            if (number.Digits == null || number.Digits.Length == 0)
                return false;

            var len = number.Digits.Length; // there is no trailing '\0'
            var bits = new byte[(len / 2) + (len % 2)];

            var shift = false;
            var isNegative = false;
            var bitIndex = 0;

            // parse the string into a little-endian two's complement byte array
            // string value     : O F E B 7 \0
            // string index (i) : 0 1 2 3 4 5 <--
            // byte[] (bitIndex): 2 1 1 0 0 <--
            //
            for (var i = len - 1; i > -1; i--)
            {
                var c = number.Digits[i];

                byte b;
                if (c >= '0' && c <= '9')
                {
                    b = (byte)(c - '0');
                }
                else if (c >= 'A' && c <= 'F')
                {
                    b = (byte)((c - 'A') + 10);
                }
                else
                {
                    Contract.Assert(c >= 'a' && c <= 'f');
                    b = (byte)((c - 'a') + 10);
                }
                if (i == 0 && (b & 0x08) == 0x08)
                    isNegative = true;

                if (shift)
                {
                    bits[bitIndex] = (byte)(bits[bitIndex] | (b << 4));
                    bitIndex++;
                }
                else
                {
                    bits[bitIndex] = isNegative ? (byte)(b | 0xF0) : (b);
                }
                shift = !shift;
            }

            value = new BigInteger(bits);
            return true;
        }

        private static bool NumberToBigInteger(BigNumberBuffer number, out BigInteger value)
        {
            var cur = 0;
            if (number.Scale > number.Digits.Length)
            {
                var i = number.Digits.Length;
                value = 0;
                while (--i >= 0)
                {
                    value *= 10;
                    value += (number.Digits[cur++] - '0');
                }
                var adjust = number.Scale - number.Digits.Length;
                while (adjust > 9)
                {
                    value *= 1000000000;
                    adjust -= 9;
                }
                while (adjust > 0)
                {
                    value *= 10;
                    adjust--;
                }
            }
            else
            {
                var i = number.Scale;
                value = 0;
                while (--i >= 0)
                {
                    value *= 10;
                    value += (number.Digits[cur++] - '0');
                }
                for (; cur < number.Digits.Length - 1; cur++)
                {
                    if (number.Digits[cur++] != '0')
                    {
                        return false;
                    }
                }
            }
            if (number.Negative)
            {
                value = -value;
            }
            return true;
        }

        private static string SetWrap(BigInteger value, NumberFormatInfo info, int type, StringBuilder result)
        {
            var close = string.Empty;
            switch (type)
            {
                case 1:
                    if (value.InternalSign < 0)
                    {
                        switch (info.CurrencyNegativePattern)
                        {
                            case 0:
                                result.Append('(');
                                result.Append(info.CurrencySymbol);
                                close = ")";
                                break;

                            case 1:
                                result.Append(info.NegativeSign);
                                result.Append(info.CurrencySymbol);
                                break;

                            case 2:
                                result.Append(info.CurrencySymbol);
                                result.Append(info.NegativeSign);
                                break;

                            case 3:
                                result.Append(info.CurrencySymbol);
                                close = info.NegativeSign;
                                break;

                            case 4:
                                result.Append('(');
                                close = info.CurrencySymbol + ")";
                                break;

                            case 5:
                                result.Append(info.NegativeSign);
                                close = info.CurrencySymbol;
                                break;

                            case 6:
                                close = info.NegativeSign + info.CurrencySymbol;
                                break;

                            case 7:
                                close = info.CurrencySymbol + info.NegativeSign;
                                break;

                            case 8:
                                result.Append(info.NegativeSign);
                                close = " " + info.CurrencySymbol;
                                break;

                            case 9:
                                result.Append(info.NegativeSign);
                                result.Append(info.CurrencySymbol);
                                result.Append(" ");
                                break;

                            case 10:
                                close = " " + info.CurrencySymbol + info.NegativeSign;
                                break;

                            case 11:
                                result.Append(info.CurrencySymbol);
                                result.Append(" ");
                                close = info.NegativeSign;
                                break;

                            case 12:
                                result.Append(info.CurrencySymbol);
                                result.Append(" ");
                                result.Append(info.NegativeSign);
                                break;

                            case 13:
                                result.Append(info.CurrencySymbol);
                                result.Append(info.NegativeSign);
                                result.Append(" ");
                                break;

                            case 14:
                                result.Append('(');
                                result.Append(info.CurrencySymbol);
                                result.Append(" ");
                                close = ")";
                                break;

                            case 15:
                                result.Append('(');
                                close = " " + info.CurrencySymbol + ")";
                                break;
                        }
                    }
                    else
                    {
                        switch (info.CurrencyPositivePattern)
                        {
                            case 0:
                                result.Append(info.CurrencySymbol);
                                break;

                            case 1:
                                close = info.CurrencySymbol;
                                break;

                            case 2:
                                result.Append(info.CurrencySymbol);
                                result.Append(" ");
                                break;

                            case 3:
                                close = " " + info.CurrencySymbol;
                                break;
                        }
                    }
                    break;

                case 2:
                    if (value.InternalSign < 0)
                    {
                        switch (info.PercentNegativePattern)
                        {
                            case 0:
                                result.Append(info.NegativeSign);
                                close = " " + info.PercentSymbol;
                                break;

                            case 1:
                                result.Append(info.NegativeSign);
                                close = info.PercentSymbol;
                                break;

                            case 2:
                                result.Append(info.NegativeSign);
                                result.Append(info.PercentSymbol);
                                break;

                            case 3:
                                result.Append(info.PercentSymbol);
                                result.Append(info.NegativeSign);
                                break;

                            case 4:
                                result.Append(info.PercentSymbol);
                                close = info.NegativeSign;
                                break;

                            case 5:
                                close = info.NegativeSign + info.PercentSymbol;
                                break;

                            case 6:
                                close = info.PercentSymbol + info.NegativeSign;
                                break;

                            case 7:
                                result.Append(info.NegativeSign);
                                result.Append(info.PercentSymbol);
                                result.Append(" ");
                                break;

                            case 8:
                                close = " " + info.PercentSymbol + info.NegativeSign;
                                break;

                            case 9:
                                result.Append(info.PercentSymbol);
                                result.Append(" ");
                                close = info.NegativeSign;
                                break;

                            case 10:
                                result.Append(info.PercentSymbol);
                                result.Append(" ");
                                result.Append(info.NegativeSign);
                                break;

                            case 11:
                                close = info.NegativeSign + " " + info.PercentSymbol;
                                break;
                        }
                    }
                    else
                    {
                        switch (info.CurrencyPositivePattern)
                        {
                            case 0:
                                close = " " + info.PercentSymbol;
                                break;

                            case 1:
                                close = info.PercentSymbol;
                                break;

                            case 2:
                                result.Append(info.PercentSymbol);
                                break;

                            case 3:
                                result.Append(info.PercentSymbol);
                                result.Append(" ");
                                break;
                        }
                    }
                    break;

                default:
                    if (value.InternalSign < 0)
                    {
                        result.Append(info.NegativeSign);
                    }
                    break;
            }
            return close;
        }

        private static ReverseStringBuilder StringWithGroups(int capacity, IEnumerable<char> builder, int[] groupingSizes, string groupingSeparator)
        {
            var newBuffer = new ReverseStringBuilder(capacity);
            using (var enumerator = builder.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return newBuffer;
                }
                foreach (var size in groupingSizes)
                {
                    for (var count = size - 1; count >= 0; count--)
                    {
                        newBuffer.Prepend(enumerator.Current);
                        if (!enumerator.MoveNext())
                        {
                            return newBuffer;
                        }
                    }
                    newBuffer.Prepend(groupingSeparator);
                }
                {
                    var size = groupingSizes[groupingSizes.Length - 1];
                    if (size != 0)
                    {
                        while (true)
                        {
                            for (var count = size - 1; count >= 0; count--)
                            {
                                newBuffer.Prepend(enumerator.Current);
                                if (!enumerator.MoveNext())
                                {
                                    return newBuffer;
                                }
                            }
                            newBuffer.Prepend(groupingSeparator);
                        }
                    }
                    while (true)
                    {
                        newBuffer.Prepend(enumerator.Current);
                        if (!enumerator.MoveNext())
                        {
                            return newBuffer;
                        }
                    }
                }
            }
        }
    }
}

#endif