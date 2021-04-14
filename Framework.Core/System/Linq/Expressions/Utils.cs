#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions
{
    internal static class Utils
    {
        public static readonly object BoxedDefaultByte = (byte)0;
        public static readonly object BoxedDefaultChar = '\0';
        public static readonly object BoxedDefaultDateTime = new DateTime();
        public static readonly object BoxedDefaultDecimal = 0m;
        public static readonly object BoxedDefaultDouble = 0.0;
        public static readonly object BoxedDefaultInt16 = (short)0;
        public static readonly object BoxedDefaultInt64 = 0L;
        public static readonly object BoxedDefaultSByte = (sbyte)0;
        public static readonly object BoxedDefaultSingle = 0.0f;
        public static readonly object BoxedDefaultUInt16 = (ushort)0;
        public static readonly object BoxedDefaultUInt32 = 0u;
        public static readonly object BoxedDefaultUInt64 = 0ul;
        public static readonly object BoxedFalse = false;
        public static readonly object BoxedInt0 = 0;
        public static readonly object BoxedInt1 = 1;
        public static readonly object BoxedInt2 = 2;
        public static readonly object BoxedInt3 = 3;
        public static readonly object BoxedIntM1 = -1;
        public static readonly object BoxedTrue = true;
        public static readonly DefaultExpression Empty = Expression.Empty();
        public static readonly ConstantExpression Null = Expression.Constant(value: null);
        private static readonly ConstantExpression _0 = Expression.Constant(BoxedInt0);
        private static readonly ConstantExpression _1 = Expression.Constant(BoxedInt1);
        private static readonly ConstantExpression _2 = Expression.Constant(BoxedInt2);
        private static readonly ConstantExpression _3 = Expression.Constant(BoxedInt3);
        private static readonly ConstantExpression _false = Expression.Constant(BoxedFalse);
        private static readonly ConstantExpression _m1 = Expression.Constant(BoxedIntM1);
        private static readonly ConstantExpression _true = Expression.Constant(BoxedTrue);

        public static ConstantExpression Constant(bool value)
        {
            return value ? _true : _false;
        }

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