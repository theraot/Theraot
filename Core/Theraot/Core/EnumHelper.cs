using System;

namespace Theraot.Core
{
    public static class EnumHelper
    {
        public static bool HasFlag(this Enum value, Enum flag)
        {
            //Added in .NET 4.0
            Type type = value.GetType();
            if (!flag.GetType().Equals(type))
            {
                throw new ArgumentException("Enum types don't match");
            }
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException("value");
            }
            else if (ReferenceEquals(flag, null))
            {
                throw new ArgumentNullException("flag");
            }
            else
            {
                var underlyingType = Type.GetTypeCode(type);
                switch (underlyingType)
                {
                    case TypeCode.Byte:
                        return (System.Convert.ToByte(value) & System.Convert.ToByte(flag)) != 0;

                    case TypeCode.Int16:
                        return (System.Convert.ToInt16(value) & System.Convert.ToInt16(flag)) != 0;

                    case TypeCode.Int32:
                        return (System.Convert.ToInt32(value) & System.Convert.ToInt32(flag)) != 0;

                    case TypeCode.Int64:
                        return (System.Convert.ToInt64(value) & System.Convert.ToInt64(flag)) != 0;

                    case TypeCode.SByte:
                        return (System.Convert.ToSByte(value) & System.Convert.ToSByte(flag)) != 0;

                    case TypeCode.UInt16:
                        return (System.Convert.ToUInt16(value) & System.Convert.ToUInt16(flag)) != 0;

                    case TypeCode.UInt32:
                        return (System.Convert.ToUInt32(value) & System.Convert.ToUInt32(flag)) != 0;

                    case TypeCode.UInt64:
                        return (System.Convert.ToUInt64(value) & System.Convert.ToUInt64(flag)) != 0;

                    default:
                        throw new InvalidOperationException("The underlying type of the Enum is not a recognized primitive integer type.");
                }
            }
        }
    }
}