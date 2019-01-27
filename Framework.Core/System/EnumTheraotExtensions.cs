#if LESSTHAN_NET40

using System.Globalization;

namespace System
{
    public static class EnumTheraotExtensions
    {
        public static bool HasFlag(this Enum value, Enum flag)
        {
            //Added in .NET 4.0
            var type = value.GetType();
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (flag == null)
            {
                throw new ArgumentNullException(nameof(flag));
            }

            if (flag.GetType() != type)
            {
                throw new ArgumentException("Enum types don't match");
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                    return (Convert.ToByte(value, CultureInfo.InvariantCulture) & Convert.ToByte(flag, CultureInfo.InvariantCulture)) != 0;

                case TypeCode.Int16:
                    return (Convert.ToInt16(value, CultureInfo.InvariantCulture) & Convert.ToInt16(flag, CultureInfo.InvariantCulture)) != 0;

                case TypeCode.Int32:
                    return (Convert.ToInt32(value, CultureInfo.InvariantCulture) & Convert.ToInt32(flag, CultureInfo.InvariantCulture)) != 0;

                case TypeCode.Int64:
                    return (Convert.ToInt64(value, CultureInfo.InvariantCulture) & Convert.ToInt64(flag, CultureInfo.InvariantCulture)) != 0;

                case TypeCode.SByte:
                    return (Convert.ToSByte(value, CultureInfo.InvariantCulture) & Convert.ToSByte(flag, CultureInfo.InvariantCulture)) != 0;

                case TypeCode.UInt16:
                    return (Convert.ToUInt16(value, CultureInfo.InvariantCulture) & Convert.ToUInt16(flag, CultureInfo.InvariantCulture)) != 0;

                case TypeCode.UInt32:
                    return (Convert.ToUInt32(value, CultureInfo.InvariantCulture) & Convert.ToUInt32(flag, CultureInfo.InvariantCulture)) != 0;

                case TypeCode.UInt64:
                    return (Convert.ToUInt64(value, CultureInfo.InvariantCulture) & Convert.ToUInt64(flag, CultureInfo.InvariantCulture)) != 0;

                default:
                    throw new InvalidOperationException("The underlying type of the Enum is not a recognized primitive integer type.");
            }
        }
    }
}

#endif