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

            return type == typeof(bool) ? TypeCode.Boolean
                : type == typeof(char) ? TypeCode.Char
                : type == typeof(sbyte) ? TypeCode.SByte
                : type == typeof(byte) ? TypeCode.Byte
                : type == typeof(short) ? TypeCode.Int16
                : type == typeof(ushort) ? TypeCode.UInt16
                : type == typeof(int) ? TypeCode.Int32
                : type == typeof(uint) ? TypeCode.UInt32
                : type == typeof(long) ? TypeCode.Int64
                : type == typeof(ulong) ? TypeCode.UInt64
                : type == typeof(float) ? TypeCode.Single
                : type == typeof(double) ? TypeCode.Double
                : type == typeof(decimal) ? TypeCode.Decimal
                : type == typeof(DateTime) ? TypeCode.DateTime
                : type == typeof(string) ? TypeCode.String
                : TypeCode.Object;
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