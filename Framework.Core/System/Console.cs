#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2

using System.Diagnostics;

namespace System
{
    public static class Console
    {
        public static void WriteLine(string value)
        {
            Debug.WriteLine(value);
        }

        [CLSCompliant(false)]
        public static void WriteLine(ulong value)
        {
            Debug.WriteLine(value);
        }

        [CLSCompliant(false)]
        public static void WriteLine(uint value)
        {
            Debug.WriteLine(value);
        }

        public static void WriteLine(float value)
        {
            Debug.WriteLine(value);
        }

        public static void WriteLine(decimal value)
        {
            Debug.WriteLine(value);
        }

        public static void WriteLine(long value)
        {
            Debug.WriteLine(value);
        }

        public static void WriteLine(int value)
        {
            Debug.WriteLine(value);
        }

        public static void WriteLine(char value)
        {
            Debug.WriteLine(value);
        }

        public static void WriteLine(bool value)
        {
            Debug.WriteLine(value);
        }

        public static void WriteLine(char[] value)
        {
            var str = new string(value);
            Debug.WriteLine(str);
        }

        public static void WriteLine(char[] buffer, int index, int count)
        {
            var str = new string(buffer, index, count);
            Debug.WriteLine(str);
        }

        public static void WriteLine(object value)
        {
            Debug.WriteLine(value);
        }

        public static void WriteLine()
        {
            Debug.WriteLine(string.Empty);
        }

        public static void WriteLine(double value)
        {
            Debug.WriteLine(value);
        }

        public static void WriteLine(string format, object arg0)
        {
            Debug.WriteLine(format, arg0);
        }

        public static void WriteLine(string format, object arg0, object arg1)
        {
            Debug.WriteLine(format, arg0, arg1);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Debug.WriteLine(format, arg0, arg1, arg2);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
        {
            Debug.WriteLine(format, arg0, arg1, arg2, arg3);
        }

        public static void WriteLine(string format, params object[] arg)
        {
            Debug.WriteLine(format, arg);
        }
    }
}

#endif