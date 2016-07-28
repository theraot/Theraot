// Needed for Workaround

using System;

namespace Theraot.Core
{
    public static class EnumHelper
    {
#if NET35

        public static bool HasFlag(this Enum value, Enum flag)
        {
            //Added in .NET 4.0
            var type = value.GetType();
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException("value");
            }
            if (ReferenceEquals(flag, null))
            {
                throw new ArgumentNullException("flag");
            }
            if (flag.GetType() != type)
            {
                throw new ArgumentException("Enum types don't match");
            }
            var underlyingType = Type.GetTypeCode(type);
            switch (underlyingType)
            {
                case TypeCode.Byte:
                    return (Convert.ToByte(value) & Convert.ToByte(flag)) != 0;

                case TypeCode.Int16:
                    return (Convert.ToInt16(value) & Convert.ToInt16(flag)) != 0;

                case TypeCode.Int32:
                    return (Convert.ToInt32(value) & Convert.ToInt32(flag)) != 0;

                case TypeCode.Int64:
                    return (Convert.ToInt64(value) & Convert.ToInt64(flag)) != 0;

                case TypeCode.SByte:
                    return (Convert.ToSByte(value) & Convert.ToSByte(flag)) != 0;

                case TypeCode.UInt16:
                    return (Convert.ToUInt16(value) & Convert.ToUInt16(flag)) != 0;

                case TypeCode.UInt32:
                    return (Convert.ToUInt32(value) & Convert.ToUInt32(flag)) != 0;

                case TypeCode.UInt64:
                    return (Convert.ToUInt64(value) & Convert.ToUInt64(flag)) != 0;

                default:
                    throw new InvalidOperationException("The underlying type of the Enum is not a recognized primitive integer type.");
            }
        }

#else
        public static bool HasFlag(Enum value, Enum flag)
        {
            // Added in .NET 4.0
            return value.HasFlag(flag);
        }
#endif
    }
}