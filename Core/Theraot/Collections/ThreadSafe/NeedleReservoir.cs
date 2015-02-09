using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    internal static class NeedleReservoir<T, TNeedle>
        where TNeedle : INeedle<T>
    {
        internal static void DonateNeedle(TNeedle donation)
        {
            // TODO
        }

        internal static TNeedle GetNeedle(T value)
        {
            return NeedleHelper.CreateNeedle<T, TNeedle>(value);
        }
    }
}