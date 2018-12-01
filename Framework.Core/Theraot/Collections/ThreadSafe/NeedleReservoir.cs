// Needed for NET35 (ConditionalWeakTable)

using System;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    public static class NeedleReservoir
    {
        [ThreadStatic]
        internal static int InternalRecycling;

        public static bool Recycling => InternalRecycling > 0;
    }

    public class NeedleReservoir<T, TNeedle>
        where TNeedle : class, IRecyclableNeedle<T>
    {
        private readonly Func<T, TNeedle> _needleFactory;
        private readonly Pool<TNeedle> _pool;

        public NeedleReservoir(Func<T, TNeedle> needleFactory)
        {
            _needleFactory = needleFactory ?? throw new ArgumentNullException(nameof(needleFactory));
            _pool = new Pool<TNeedle>(64, Recycle);

            void Recycle(TNeedle obj)
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

        internal void DonateNeedle(TNeedle donation)
        {
            if (!_pool.Donate(donation))
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                var disposable = donation as IDisposable;
                disposable?.Dispose();
            }
        }

        internal TNeedle GetNeedle(T value)
        {
            if (_pool.TryGet(out var result))
            {
                NeedleReservoir.InternalRecycling++;
                result.Value = value;
                NeedleReservoir.InternalRecycling--;
            }
            else
            {
                result = _needleFactory(value);
            }
            return result;
        }
    }
}