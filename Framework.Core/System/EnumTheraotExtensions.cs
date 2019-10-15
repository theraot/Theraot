#if LESSTHAN_NET40

#pragma warning disable CA2201 // Do not raise reserved exception types

using System.Globalization;
using System.Runtime.CompilerServices;

namespace System
{
    public static class EnumTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool HasFlag(this Enum value, Enum flag)
        {
            if (value == null)
            {
                throw new NullReferenceException();
            }

            if (flag == null)
            {
                throw new ArgumentNullException(nameof(flag));
            }

            var type = value.GetType();

            if (flag.GetType() != type)
            {
                throw new ArgumentException("Enum types don't match");
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                    var byteFlag = Convert.ToByte(flag, CultureInfo.InvariantCulture);
                    return (Convert.ToByte(value, CultureInfo.InvariantCulture) & byteFlag) == byteFlag;

                case TypeCode.Int16:
                    var int16Flag = Convert.ToInt16(flag, CultureInfo.InvariantCulture);
                    return (Convert.ToInt16(value, CultureInfo.InvariantCulture) & int16Flag) == int16Flag;

                case TypeCode.Int32:
                    var int32Flag = Convert.ToInt32(flag, CultureInfo.InvariantCulture);
                    return (Convert.ToInt32(value, CultureInfo.InvariantCulture) & int32Flag) == int32Flag;

                case TypeCode.Int64:
                    var int64Flag = Convert.ToInt64(flag, CultureInfo.InvariantCulture);
                    return (Convert.ToInt64(value, CultureInfo.InvariantCulture) & int64Flag) == int64Flag;

                case TypeCode.SByte:
                    var sbyteFlag = Convert.ToSByte(flag, CultureInfo.InvariantCulture);
                    return (Convert.ToSByte(value, CultureInfo.InvariantCulture) & sbyteFlag) == sbyteFlag;

                case TypeCode.UInt16:
                    var uint16Flag = Convert.ToUInt16(flag, CultureInfo.InvariantCulture);
                    return (Convert.ToUInt16(value, CultureInfo.InvariantCulture) & uint16Flag) == uint16Flag;

                case TypeCode.UInt32:
                    var uint32Flag = Convert.ToUInt32(flag, CultureInfo.InvariantCulture);
                    return (Convert.ToUInt32(value, CultureInfo.InvariantCulture) & uint32Flag) == uint32Flag;

                case TypeCode.UInt64:
                    var uint64Flag = Convert.ToUInt64(flag, CultureInfo.InvariantCulture);
                    return (Convert.ToUInt64(value, CultureInfo.InvariantCulture) & uint64Flag) == uint64Flag;

                default:
                    throw new InvalidOperationException("The underlying type of the Enum is not a recognized primitive integer type.");
            }
        }
    }
}

#endif