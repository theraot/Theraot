using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using Theraot.Collections;

namespace Theraot.Core
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static class StringExtensions
    {
        public static string Append(this string text, string value)
        {
            return string.Concat(text, value);
        }

        public static string Append(this string text, string value1, string value2)
        {
            return string.Concat(text, value1, value2);
        }

        public static string Append(this string text, params string[] values)
        {
            return string.Concat(text, values);
        }

        public static string End(this string text, int characterCount)
        {
            var _text = Check.NotNullArgument(text, "text");
            int length = _text.Length;
            if (length < characterCount)
            {
                return _text;
            }
            else
            {
                return _text.Substring(length - characterCount);
            }
        }

        public static string EnsureEnd(this string text, string end)
        {
            var _text = Check.NotNullArgument(text, "text");
            if (!_text.EndsWith(end, false, CultureInfo.CurrentCulture))
            {
                return _text.Append(end);
            }
            else
            {
                return _text;
            }
        }

        public static string EnsureEnd(this string text, string end, bool ignoreCase, CultureInfo culture)
        {
            var _text = Check.NotNullArgument(text, "text");
            if (!_text.EndsWith(end, ignoreCase, culture))
            {
                return _text.Append(end);
            }
            else
            {
                return _text;
            }
        }

        public static string EnsureEnd(this string text, string end, StringComparison comparisonType)
        {
            var _text = Check.NotNullArgument(text, "text");
            if (!_text.EndsWith(end, comparisonType))
            {
                return _text.Append(end);
            }
            else
            {
                return _text;
            }
        }

        public static string EnsureStart(this string text, string start)
        {
            var _text = Check.NotNullArgument(text, "text");
            if (!_text.StartsWith(start, false, CultureInfo.CurrentCulture))
            {
                return start.Append(_text);
            }
            else
            {
                return _text;
            }
        }

        public static string EnsureStart(this string text, string start, bool ignoreCase, CultureInfo culture)
        {
            var _text = Check.NotNullArgument(text, "text");
            if (!_text.StartsWith(start, ignoreCase, culture))
            {
                return start.Append(_text);
            }
            else
            {
                return _text;
            }
        }

        public static string EnsureStart(this string text, string start, StringComparison comparisonType)
        {
            var _text = Check.NotNullArgument(text, "text");
            if (!_text.StartsWith(start, comparisonType))
            {
                return start.Append(_text);
            }
            else
            {
                return _text;
            }
        }

        public static string ExceptEnd(this string text, int characterCount)
        {
            var _text = Check.NotNullArgument(text, "text");
            int length = _text.Length;
            if (length < characterCount)
            {
                return string.Empty;
            }
            else
            {
                return _text.Substring(0, length - characterCount);
            }
        }

        public static string ExceptStart(this string text, int characterCount)
        {
            var _text = Check.NotNullArgument(text, "text");
            int length = _text.Length;
            if (length < characterCount)
            {
                return string.Empty;
            }
            else
            {
                return _text.Substring(characterCount);
            }
        }

        public static string Implode<TInput>(IEnumerable<TInput> collection, Converter<TInput, string> converter, string separator, string open, string close)
        {
            return Implode(new ConversionSet<TInput, string>(collection, converter), separator, open, close);
        }

        public static string Implode<TInput>(IEnumerable<TInput> collection, Converter<TInput, string> converter, string separator)
        {
            return Implode(new ConversionSet<TInput, string>(collection, converter), separator);
        }

        public static string Implode(IEnumerable<string> collection, string separator)
        {
            StringBuilder str = ImplodeExtracted(collection, separator);
            return str.ToString();
        }

        public static string Implode(IEnumerable<string> collection, string separator, string open, string close)
        {
            StringBuilder str = ImplodeExtracted(collection, separator);
            if (str.Length > 0)
            {
                return open.Safe() + str.ToString() + close.Safe();
            }
            else
            {
                return str.ToString();
            }
        }

        public static bool Like(this string text, Regex regex, int startAt)
        {
            return regex.IsMatch(text, startAt);
        }

        public static bool Like(this string text, Regex regex)
        {
            return text.Like(regex, 0);
        }

        public static bool Like(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.IsMatch(text, startAt);
        }

        public static bool Like(this string text, string regexPattern, RegexOptions regexOptions)
        {
            return text.Like(regexPattern, regexOptions, 0);
        }

        public static bool Like(this string text, string regexPattern, bool ignoreCase)
        {
            return text.Like(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
        }

        public static bool Like(this string text, string regexPattern)
        {
            return text.Like(regexPattern, RegexOptions.IgnoreCase, 0);
        }

        public static bool Like(this string text, string regexPattern, bool ignoreCase, int startAt)
        {
            return text.Like(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt);
        }

        public static bool Like(this string text, string regexPattern, int startAt)
        {
            return text.Like(regexPattern, RegexOptions.IgnoreCase, startAt);
        }

        public static Match Match(this string text, Regex regex, int startAt, int length)
        {
            return regex.Match(text, startAt, length);
        }

        public static Match Match(this string text, Regex regex, int startAt)
        {
            return regex.Match(text, startAt);
        }

        public static Match Match(this string text, Regex regex)
        {
            return text.Match(regex, 0);
        }

        public static Match Match(this string text, string regexPattern, RegexOptions regexOptions, int startAt, int length)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.Match(text, startAt, length);
        }

        public static Match Match(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.Match(text, startAt);
        }

        public static Match Match(this string text, string regexPattern, RegexOptions regexOptions)
        {
            return text.Match(regexPattern, regexOptions, 0);
        }

        public static Match Match(this string text, string regexPattern, bool ignoreCase, int startAt, int length)
        {
            return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt, length);
        }

        public static Match Match(this string text, string regexPattern, bool ignoreCase, int startAt)
        {
            return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, startAt);
        }

        public static Match Match(this string text, string regexPattern, bool ignoreCase)
        {
            return text.Match(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
        }

        public static Match Match(this string text, string regexPattern, int startAt, int length)
        {
            return text.Match(regexPattern, RegexOptions.IgnoreCase, startAt, length);
        }

        public static Match Match(this string text, string regexPattern, int startAt)
        {
            return text.Match(regexPattern, RegexOptions.IgnoreCase, startAt);
        }

        public static Match Match(this string text, string regexPattern)
        {
            return text.Match(regexPattern, RegexOptions.IgnoreCase, 0);
        }

        public static MatchCollection Matches(this string text, Regex regex, int startAt)
        {
            return regex.Matches(text, startAt);
        }

        public static MatchCollection Matches(this string text, Regex regex)
        {
            return text.Matches(regex, 0);
        }

        public static MatchCollection Matches(this string text, string regexPattern, RegexOptions regexOptions, int startAt)
        {
            var regex = new Regex(regexPattern, regexOptions);
            return regex.Matches(text, startAt);
        }

        public static MatchCollection Matches(this string text, string regexPattern, RegexOptions regexOptions)
        {
            return text.Matches(regexPattern, regexOptions, 0);
        }

        public static MatchCollection Matches(this string text, string regexPattern, bool ignoreCase, int startAt)
        {
            var regex = new Regex(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            return regex.Matches(text, startAt);
        }

        public static MatchCollection Matches(this string text, string regexPattern, bool ignoreCase)
        {
            return text.Matches(regexPattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None, 0);
        }

        public static MatchCollection Matches(this string text, string regexPattern, int startAt)
        {
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            return regex.Matches(text, startAt);
        }

        public static MatchCollection Matches(this string text, string regexPattern)
        {
            return text.Matches(regexPattern, RegexOptions.IgnoreCase, 0);
        }

        public static string NeglectEnd(this string text, string end)
        {
            var _text = Check.NotNullArgument(text, "text");
            var _end = Check.NotNullArgument(end, "end");
            if (_text.EndsWith(_end, false, CultureInfo.CurrentCulture))
            {
                return _text.ExceptEnd(_end.Length);
            }
            else
            {
                return _text;
            }
        }

        public static string NeglectEnd(this string text, string end, bool ignoreCase, CultureInfo culture)
        {
            var _text = Check.NotNullArgument(text, "text");
            var _end = Check.NotNullArgument(end, "end");
            if (_text.EndsWith(_end, ignoreCase, culture))
            {
                return _text.ExceptEnd(_end.Length);
            }
            else
            {
                return _text;
            }
        }

        public static string NeglectEnd(this string text, string end, StringComparison comparisonType)
        {
            var _text = Check.NotNullArgument(text, "text");
            var _end = Check.NotNullArgument(end, "end");
            if (_text.EndsWith(_end, comparisonType))
            {
                return _text.ExceptEnd(_end.Length);
            }
            else
            {
                return _text;
            }
        }

        public static string NeglectStart(this string text, string start)
        {
            var _text = Check.NotNullArgument(text, "text");
            var _start = Check.NotNullArgument(start, "start");
            if (_text.StartsWith(_start, false, CultureInfo.CurrentCulture))
            {
                return _text.ExceptStart(_start.Length);
            }
            else
            {
                return _text;
            }
        }

        public static string NeglectStart(this string text, string start, bool ignoreCase, CultureInfo culture)
        {
            var _text = Check.NotNullArgument(text, "text");
            var _start = Check.NotNullArgument(start, "start");
            if (_text.StartsWith(_start, ignoreCase, culture))
            {
                return _text.ExceptStart(_start.Length);
            }
            else
            {
                return _text;
            }
        }

        public static string NeglectStart(this string text, string start, StringComparison comparisonType)
        {
            var _text = Check.NotNullArgument(text, "text");
            var _start = Check.NotNullArgument(start, "start");
            if (_text.StartsWith(_start, comparisonType))
            {
                return _text.ExceptStart(_start.Length);
            }
            else
            {
                return _text;
            }
        }

        public static string Safe(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            else
            {
                return text;
            }
        }

        public static string Start(this string text, int characterCount)
        {
            var _text = Check.NotNullArgument(text, "text");
            int length = _text.Length;
            if (length < characterCount)
            {
                return _text;
            }
            else
            {
                return _text.Substring(0, characterCount);
            }
        }

        /*[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "By Design")]
        public static string ToString(this object obj, string onNull)
        {
            if (obj == null)
            {
                return onNull;
            }
            else
            {
                return obj.ToString();
            }
        }*/

        private static StringBuilder ImplodeExtracted(IEnumerable<string> collection, string separator)
        {
            var str = new StringBuilder();
            bool first = true;
            foreach (string item in Check.NotNullArgument(collection, "collection"))
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    str.Append(separator);
                }
                str.Append(item);
            }
            return str;
        }
    }
}