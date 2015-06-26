#if FAT

using System;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    internal static class NeedleReservoir<T, TNeedle>
        where TNeedle : class, IRecyclableNeedle<T>
    {
        private static readonly Pool<TNeedle> _pool;

        [ThreadStatic]
        private static int _recycling;

        static NeedleReservoir()
        {
            _pool = new Pool<TNeedle>(64, Recycle);
        }

        public static bool Recycling
        {
            get
            {
                return _recycling != 0;
            }
        }

        internal static void DonateNeedle(TNeedle donation)
        {
            if (!_pool.Donate(donation))
            {
                var disposable = donation as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
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

        private static void Recycle(TNeedle obj)
        {
            try
            {
                _recycling++;
                obj.Free();
            }
            finally
            {
                _recycling--;
            }
        }
    }
}

#endif