#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions
{
    internal static class Utils
    {
        public static readonly object BoxedDefaultByte = default(byte);
        public static readonly object BoxedDefaultChar = default(char);
        public static readonly object BoxedDefaultDateTime = default(DateTime);
        public static readonly object BoxedDefaultDecimal = default(decimal);
        public static readonly object BoxedDefaultDouble = default(double);
        public static readonly object BoxedDefaultInt16 = default(short);
        public static readonly object BoxedDefaultInt64 = default(long);
        public static readonly object BoxedDefaultSByte = default(sbyte);
        public static readonly object BoxedDefaultSingle = default(float);
        public static readonly object BoxedDefaultUInt16 = default(ushort);
        public static readonly object BoxedDefaultUInt32 = default(uint);
        public static readonly object BoxedDefaultUInt64 = default(ulong);
        public static readonly object BoxedFalse = false;
        public static readonly object BoxedInt0 = 0;
        public static readonly object BoxedInt1 = 1;
        public static readonly object BoxedInt2 = 2;
        public static readonly object BoxedInt3 = 3;
        public static readonly object BoxedIntM1 = -1;
        public static readonly object BoxedTrue = true;
        public static readonly DefaultExpression Empty = Expression.Empty();
        public static readonly ConstantExpression Null = Expression.Constant(null);
        private static readonly ConstantExpression _0 = Expression.Constant(BoxedInt0);
        private static readonly ConstantExpression _1 = Expression.Constant(BoxedInt1);
        private static readonly ConstantExpression _2 = Expression.Constant(BoxedInt2);
        private static readonly ConstantExpression _3 = Expression.Constant(BoxedInt3);
        private static readonly ConstantExpression _false = Expression.Constant(BoxedFalse);
        private static readonly ConstantExpression _m1 = Expression.Constant(BoxedIntM1);
        private static readonly ConstantExpression _true = Expression.Constant(BoxedTrue);

        public static ConstantExpression Constant(bool value) => value ? _true : _false;

        public static ConstantExpression Constant(int value)
        {
            switch (value)
            {
                case -1: return _m1;
                case 0: return _0;
                case 1: return _1;
                case 2: return _2;
                case 3: return _3;
                default: return Expression.Constant(value);
            }
        }
    }
}

#endif