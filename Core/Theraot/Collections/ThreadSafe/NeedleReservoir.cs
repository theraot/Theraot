// Needed for NET35 (ConditionalWeakTable)

using System;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    public class NeedleReservoir<T, TNeedle>
        where TNeedle : class, IRecyclableNeedle<T>
    {
        private readonly Pool<TNeedle> _pool;
        private readonly Func<T, TNeedle> _needleFactory;

        private int _recycling;

        public NeedleReservoir(Func<T, TNeedle> needleFactory)
        {
            if (needleFactory == null)
            {
                throw new ArgumentNullException("needleFactory");
            }
            _needleFactory = needleFactory;
            _pool = new Pool<TNeedle>(64, Recycle);
        }

        public bool Recycling
        {
            get
            {
                return _recycling != 0;
            }
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
                ((INeedle<T>)result).Value = value;
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