// Needed for NET35 (ConditionalWeakTable)

using System;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    public class NeedleReservoir<T, TNeedle>
        where TNeedle : class, IRecyclable, INeedle<T>
    {
        private readonly Func<T, TNeedle> _needleFactory;
        private readonly Pool<TNeedle> _pool;

        public NeedleReservoir(Func<T, TNeedle> needleFactory)
        {
            _needleFactory = needleFactory ?? throw new ArgumentNullException(nameof(needleFactory));
            _pool = new Pool<TNeedle>(64, Recycle);

            static void Recycle(TNeedle obj)
            {
                obj.Free();
            }
        }

        internal void DonateNeedle(TNeedle donation)
        {
            _pool.Donate(donation);
        }

        internal TNeedle GetNeedle(T value)
        {
            if (_pool.TryGet(out var result))
            {
                result.Value = value;
            }
            else
            {
                result = _needleFactory(value);
            }

            return result;
        }
    }
}