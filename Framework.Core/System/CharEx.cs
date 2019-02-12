using System.Globalization;
using System.Runtime.CompilerServices;

namespace System
{
    public static class CharEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static char ToUpper(char c, CultureInfo culture)
        {
#if TARGETS_NET
            return char.ToUpper(c, culture);
#else
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }
            return culture.TextInfo.ToUpper(c);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static char ToLower(char c, CultureInfo culture)
        {
#if TARGETS_NET
            return char.ToLower(c, culture);
#else
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }
            return culture.TextInfo.ToLower(c);
#endif
        }
    }
}
