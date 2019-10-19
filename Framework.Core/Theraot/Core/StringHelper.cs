// Needed for NET40

#pragma warning disable RECS0017 // Possible compare of value type with 'null'
#pragma warning disable RECS0063 // Warns when a culture-aware 'StartsWith' call is used by default.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

#if LESSTHAN_NET45 || GREATERTHAN_NETCOREAPP11

using System.Globalization;

#endif

namespace Theraot.Core
{
    [DebuggerNonUserCode]
    public static partial class StringHelper
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Append(this string text, params string[] values)
        {
            return string.Concat(text, values);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Append(this string text, string value)
        {
            return string.Concat(text, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Append(this string text, string value1, string value2)
        {
            return string.Concat(text, value1, value2);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Append(this string text, string value1, string value2, string value3)
        {
            return string.Concat(text, value1, value2, value3);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string End(this string text, int characterCount)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var length = text.Length;
            return length < characterCount ? text : text.Substring(length - characterCount);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string EnsureEnd(this string text, string end, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return !text.EndsWith(end, comparisonType) ? text.Append(end) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string EnsureStart(this string text, string start)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return !text.StartsWith(start, StringComparison.CurrentCulture) ? start.Append(text) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string EnsureStart(this string text, string start, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return !text.StartsWith(start, comparisonType) ? start.Append(text) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string ExceptEnd(this string text, int characterCount)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var length = text.Length;
            return length < characterCount ? string.Empty : text.Substring(0, length - characterCount);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string ExceptStart(this string text, int characterCount)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var length = text.Length;
            return length < characterCount ? string.Empty : text.Substring(characterCount);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool Like(this string text, Regex regex)
        {
            return text.Like(regex, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool Like(this string text, Regex regex, int startAt)
        {
            if (regex == null)
            {
                throw new ArgumentNullException(nameof(regex));
            }
            return regex.IsMatch(text, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool Like(this string text, string regexPattern)
        {
            return text.Like(regexPattern, RegexOptions.IgnoreCase, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool Like(this string text, string regexPattern, bool ignoreCase)
        {
            return text.Like(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool Like(this string text, string regexPattern, bool ignoreCase, int startAt)
        {
            return text.Like(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool Like(this string text, string regexPattern, int startAt)
        {
            return text.Like(regexPattern, RegexOptions.IgnoreCase, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool Like(this string text, string regexPattern, RegexOptions regexOptions)
        {
            return text.Like(regexPattern, regexOptions, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool Like(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.IsMatch(text, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, Regex regex)
        {
            return text.Match(regex, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, Regex regex, int startAt)
        {
            if (regex == null)
            {
                throw new ArgumentNullException(nameof(regex));
            }
            return regex.Match(text, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, Regex regex, int startAt, int length)
        {
            if (regex == null)
            {
                throw new ArgumentNullException(nameof(regex));
            }
            return regex.Match(text, startAt, length);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, string regexPattern)
        {
            return text.Match(regexPattern, RegexOptions.IgnoreCase, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, string regexPattern, bool ignoreCase)
        {
            return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, string regexPattern, bool ignoreCase, int startAt)
        {
            return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, string regexPattern, bool ignoreCase, int startAt, int length)
        {
            return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt, length);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, string regexPattern, int startAt)
        {
            return text.Match(regexPattern, RegexOptions.IgnoreCase, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, string regexPattern, int startAt, int length)
        {
            return text.Match(regexPattern, RegexOptions.IgnoreCase, startAt, length);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, string regexPattern, RegexOptions regexOptions)
        {
            return text.Match(regexPattern, regexOptions, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.Match(text, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Match Match(this string text, string regexPattern, RegexOptions regexOptions, int startAt, int length)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.Match(text, startAt, length);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MatchCollection Matches(this string text, Regex regex)
        {
            return text.Matches(regex, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MatchCollection Matches(this string text, Regex regex, int startAt)
        {
            if (regex == null)
            {
                throw new ArgumentNullException(nameof(regex));
            }
            return regex.Matches(text, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MatchCollection Matches(this string text, string regexPattern)
        {
            return text.Matches(regexPattern, RegexOptions.IgnoreCase, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MatchCollection Matches(this string text, string regexPattern, bool ignoreCase)
        {
            return text.Matches(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MatchCollection Matches(this string text, string regexPattern, bool ignoreCase, int startAt)
        {
            var regex = new Regex(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            return regex.Matches(text, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MatchCollection Matches(this string text, string regexPattern, int startAt)
        {
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            return regex.Matches(text, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MatchCollection Matches(this string text, string regexPattern, RegexOptions regexOptions)
        {
            return text.Matches(regexPattern, regexOptions, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MatchCollection Matches(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.Matches(text, startAt);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string NeglectEnd(this string text, string end, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (end == null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            return text.EndsWith(end, comparisonType) ? text.ExceptEnd(end.Length) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string NeglectStart(this string text, string start)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            return text.StartsWith(start, StringComparison.CurrentCulture) ? text.ExceptStart(start.Length) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string NeglectStart(this string text, string start, StringComparison comparisonType)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            return text.StartsWith(start, comparisonType) ? text.ExceptStart(start.Length) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Safe(this string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Start(this string text, int characterCount)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var length = text.Length;
            return length < characterCount ? text : text.Substring(0, characterCount);
        }
    }

    public static partial class StringHelper
    {
#if LESSTHAN_NET45 || GREATERTHAN_NETCOREAPP11

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string EnsureEnd(this string text, string end)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return !text.EndsWith(end, false, CultureInfo.CurrentCulture) ? text.Append(end) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string EnsureEnd(this string text, string end, bool ignoreCase, CultureInfo culture)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return !text.EndsWith(end, ignoreCase, culture) ? text.Append(end) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string EnsureStart(this string text, string start, bool ignoreCase, CultureInfo culture)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return !text.StartsWith(start, ignoreCase, culture) ? start.Append(text) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string NeglectEnd(this string text, string end)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (end == null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            return text.EndsWith(end, false, CultureInfo.CurrentCulture) ? text.ExceptEnd(end.Length) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string NeglectEnd(this string text, string end, bool ignoreCase, CultureInfo culture)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (end == null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            return text.EndsWith(end, ignoreCase, culture) ? text.ExceptEnd(end.Length) : text;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string NeglectStart(this string text, string start, bool ignoreCase, CultureInfo culture)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            return text.StartsWith(start, ignoreCase, culture) ? text.ExceptStart(start.Length) : text;
        }

#endif
    }
}