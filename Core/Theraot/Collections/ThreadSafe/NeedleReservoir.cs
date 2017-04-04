// Needed for NET35 (ConditionalWeakTable)

using System;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    public static class NeedleReservoir
    {
        [ThreadStatic]
        internal static int InternalRecycling;

        public static bool Recycling
        {
            get
            {
                return InternalRecycling > 0;
            }
        }
    }

    public class NeedleReservoir<T, TNeedle>
        where TNeedle : class, IRecyclableNeedle<T>
    {
        private readonly Func<T, TNeedle> _needleFactory;
        private readonly Pool<TNeedle> _pool;

        public NeedleReservoir(Func<T, TNeedle> needleFactory)
        {
            if (needleFactory == null)
            {
                throw new ArgumentNullException("needleFactory");
            }
            _needleFactory = needleFactory;
            _pool = new Pool<TNeedle>(64, Recycle);
        }

        internal void DonateNeedle(TNeedle donation)
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

        internal TNeedle GetNeedle(T value)
        {
            TNeedle result;
            if (_pool.TryGet(out result))
            {
                NeedleReservoir.InternalRecycling++;
                ((INeedle<T>)result).Value = value;
                NeedleReservoir.InternalRecycling--;
            }
            else
            {
                result = _needleFactory(value);
            }
            return result;
        }

        private void Recycle(TNeedle obj)
        {
            try
            {
                NeedleReservoir.InternalRecycling++;
                obj.Free();
            }
            finally
            {
                NeedleReservoir.InternalRecycling--;
            }
        }
    }
}