#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4

#pragma warning disable CA1305 // Specify IFormatProvider
#pragma warning disable CA2201 // Do not raise reserved exception types

using System;
using System.Globalization;
using System.Text;

namespace TestRunner
{
    public static class Console
    {
        private static readonly StringBuilder _stringBuilder = new StringBuilder();

        public static void ReadKey()
        {
            throw new Exception(_stringBuilder.ToString());
        }

        public static void Write(string value)
        {
            _stringBuilder.Append(value);
        }

        public static void Write(ulong value)
        {
            _stringBuilder.Append(value.ToString());
        }

        public static void Write(uint value)
        {
            _stringBuilder.Append(value.ToString());
        }

        public static void Write(float value)
        {
            _stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
        }

        public static void Write(decimal value)
        {
            _stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
        }

        public static void Write(long value)
        {
            _stringBuilder.Append(value.ToString());
        }

        public static void Write(int value)
        {
            _stringBuilder.Append(value.ToString());
        }

        public static void Write(char value)
        {
            _stringBuilder.Append(value.ToString());
        }

        public static void Write(bool value)
        {
            _stringBuilder.Append(value.ToString());
        }

        public static void Write(char[] value)
        {
            var str = new string(value);
            _stringBuilder.Append(str);
        }

        public static void Write(char[] buffer, int index, int count)
        {
            var str = new string(buffer, index, count);
            _stringBuilder.Append(str);
        }

        public static void Write(object value)
        {
            _stringBuilder.Append(value);
        }

        public static void Write()
        {
            _stringBuilder.Append(string.Empty);
        }

        public static void Write(double value)
        {
            _stringBuilder.Append(value.ToString(CultureInfo.InvariantCulture));
        }

        public static void Write(string format, object arg0)
        {
            _stringBuilder.AppendFormat(format, arg0);
        }

        public static void Write(string format, object arg0, object arg1)
        {
            _stringBuilder.AppendFormat(format, arg0, arg1);
        }

        public static void Write(string format, object arg0, object arg1, object arg2)
        {
            _stringBuilder.AppendFormat(format, arg0, arg1, arg2);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, object arg3)
        {
            _stringBuilder.AppendFormat(format, arg0, arg1, arg2, arg3);
        }

        public static void Write(string format, params object[] arg)
        {
            _stringBuilder.AppendFormat(format, arg).AppendLine();
        }

        public static void WriteLine(string value)
        {
            _stringBuilder.AppendLine(value);
        }

        public static void WriteLine(ulong value)
        {
            _stringBuilder.AppendLine(value.ToString());
        }

        public static void WriteLine(uint value)
        {
            _stringBuilder.AppendLine(value.ToString());
        }

        public static void WriteLine(float value)
        {
            _stringBuilder.AppendLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public static void WriteLine(decimal value)
        {
            _stringBuilder.AppendLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public static void WriteLine(long value)
        {
            _stringBuilder.AppendLine(value.ToString());
        }

        public static void WriteLine(int value)
        {
            _stringBuilder.AppendLine(value.ToString());
        }

        public static void WriteLine(char value)
        {
            _stringBuilder.AppendLine(value.ToString());
        }

        public static void WriteLine(bool value)
        {
            _stringBuilder.AppendLine(value.ToString());
        }

        public static void WriteLine(char[] value)
        {
            var str = new string(value);
            _stringBuilder.AppendLine(str);
        }

        public static void WriteLine(char[] buffer, int index, int count)
        {
            var str = new string(buffer, index, count);
            _stringBuilder.AppendLine(str);
        }

        public static void WriteLine(object value)
        {
            _stringBuilder.AppendLine(value == null ? "<null>" : value.ToString());
        }

        public static void WriteLine()
        {
            _stringBuilder.AppendLine(string.Empty);
        }

        public static void WriteLine(double value)
        {
            _stringBuilder.AppendLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public static void WriteLine(string format, object arg0)
        {
            _stringBuilder.AppendFormat(format, arg0).AppendLine();
        }

        public static void WriteLine(string format, object arg0, object arg1)
        {
            _stringBuilder.AppendFormat(format, arg0, arg1).AppendLine();
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            _stringBuilder.AppendFormat(format, arg0, arg1, arg2).AppendLine();
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
        {
            _stringBuilder.AppendFormat(format, arg0, arg1, arg2, arg3).AppendLine();
        }

        public static void WriteLine(string format, params object[] arg)
        {
            _stringBuilder.AppendFormat(format, arg).AppendLine();
        }
    }
}

#endif