#if LESSTHAN_NET40 || LESSTHAN_NETSTANDARD15

using System.Reflection;

namespace System
{
    public static class TypeTheraotExtensions
    {
        public static TypeCode GetTypeCode(this Type type)
        {
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
        }
    }
}


#endif