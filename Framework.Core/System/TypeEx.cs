using System.Runtime.CompilerServices;

#if LESSTHAN_NETSTANDARD15

using System.Reflection;

#endif

namespace System
{
    public static partial class TypeEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static TypeCode GetTypeCode(Type type)
        {
#if LESSTHAN_NETSTANDARD15
            if (type == null)
            {
                return TypeCode.Empty;
            }

            while (true)
            {
                var info = type.GetTypeInfo();
                if (info.IsEnum)
                {
                    type = Enum.GetUnderlyingType(type);
                }
                else
                {
                    break;
                }
            }

            if (type == typeof(bool))
            {
                return TypeCode.Boolean;
            }

            if (type == typeof(char))
            {
                return TypeCode.Char;
            }

            if (type == typeof(sbyte))
            {
                return TypeCode.SByte;
            }

            if (type == typeof(byte))
            {
                return TypeCode.Byte;
            }

            if (type == typeof(short))
            {
                return TypeCode.Int16;
            }

            if (type == typeof(ushort))
            {
                return TypeCode.UInt16;
            }

            if (type == typeof(int))
            {
                return TypeCode.Int32;
            }

            if (type == typeof(uint))
            {
                return TypeCode.UInt32;
            }

            if (type == typeof(long))
            {
                return TypeCode.Int64;
            }

            if (type == typeof(ulong))
            {
                return TypeCode.UInt64;
            }

            if (type == typeof(float))
            {
                return TypeCode.Single;
            }

            if (type == typeof(double))
            {
                return TypeCode.Double;
            }

            if (type == typeof(decimal))
            {
                return TypeCode.Decimal;
            }

            if (type == typeof(DateTime))
            {
                return TypeCode.DateTime;
            }

            if (type == typeof(string))
            {
                return TypeCode.String;
            }

            return TypeCode.Object;
#else
            return Type.GetTypeCode(type);
#endif
        }
    }

    public static partial class TypeEx
    {
#if LESSTHAN_NETSTANDARD13
        public static readonly Type[] EmptyTypes = new Type[0];
#else
        public static readonly Type[] EmptyTypes = Type.EmptyTypes;
#endif
    }
}