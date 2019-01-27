#if LESSTHAN_NET40

using Theraot;

#endif

namespace System
{
#if LESSTHAN_NET40
    public static class EnumEx
    {
        public static bool TryParse<TEnum>(string value, out TEnum result)
            where TEnum : struct
        {
            var enumType = typeof(TEnum);
            // Throw if the type is not an Enum
            GC.KeepAlive(Enum.GetUnderlyingType(enumType));
            try
            {
                result = (TEnum)Enum.Parse(enumType, value);
                return true;
            }
            catch (Exception ex)
            {
                No.Op(ex);
                result = default;
                return false;
            }
        }

        public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result)
            where TEnum : struct
        {
            var enumType = typeof(TEnum);
            // Throw if the type is not an Enum
            GC.KeepAlive(Enum.GetUnderlyingType(enumType));
            try
            {
                result = (TEnum)Enum.Parse(enumType, value, ignoreCase);
                return true;
            }
            catch (Exception ex)
            {
                No.Op(ex);
                result = default;
                return false;
            }
        }
    }
#else
    public static class EnumEx
    {
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptionsEx.AggressiveInlining)]
        public static bool TryParse<TEnum>(string value, out TEnum result)
            where TEnum : struct
        {
            // Added in .NET 4.0
            return Enum.TryParse(value, out result);
        }

        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptionsEx.AggressiveInlining)]
        public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result)
            where TEnum : struct
        {
            // Added in .NET 4.0
            return Enum.TryParse(value, ignoreCase, out result);
        }
    }
#endif
}