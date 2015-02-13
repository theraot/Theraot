using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    internal static class NeedleReservoir<T, TNeedle>
        where TNeedle : class, IRecyclableNeedle<T>
    {
        private static readonly Pool<TNeedle> _pool;

        static NeedleReservoir()
        {
            _pool = new Pool<TNeedle>(64, Recycle);
        }

        private static void Recycle(TNeedle obj)
        {
            obj.Free();
        }

        internal static void DonateNeedle(TNeedle donation)
        {
            _pool.Donate(donation);
        }

        internal static TNeedle GetNeedle(T value)
        {
            TNeedle result;
            if (_pool.TryGet(out result))
            {
                result.Value = value;
            }
            else
            {
                result = NeedleHelper.CreateNeedle<T, TNeedle>(value);
            }
            return result;
        }
    }
}