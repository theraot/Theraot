using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading
{
    internal class LockNeedleContext<T>
    {
        private readonly QueueBucket<LockNeedleSlot<T>> _freeSlots;
        private readonly LazyBucket<LockNeedleSlot<T>> _slots;
        private readonly VersionProvider _version;
        private int _current;

        public LockNeedleContext()
        {
            _slots = new LazyBucket<LockNeedleSlot<T>>(index => new LockNeedleSlot<T>(this, index), 32);
            _freeSlots = new QueueBucket<LockNeedleSlot<T>>(32);
            _version = new VersionProvider();
        }

        public bool ClaimSlot(out LockNeedleSlot<T> slot)
        {
            if (TryClaimFreeSlot(out slot))
            {
                return true;
            }
            else
            {
                if (_slots.Count < _slots.Capacity)
                {
                    int index = Interlocked.Increment(ref _current) & 31;
                    slot = _slots.Get(index);
                    slot.Claim();
                    return true;
                }
                else
                {
                    slot = null;
                    return false;
                }
            }
        }

        public LockNeedle<T> CreateToken()
        {
            return new LockNeedle<T>(this);
        }

        internal VersionProvider.VersionToken Advance()
        {
            return _version.AdvanceNewToken();
        }

        internal void Free(LockNeedleSlot<T> slot)
        {
            _freeSlots.Add(slot);
        }

        internal bool Read(int id, out T value)
        {
            LockNeedleSlot<T> slot;
            foreach (var item in Theraot.Core.NumericHelper.Bits(id))
            {
                if (_slots.TryGet(id, out slot))
                {
                    value = slot.Value;
                    return true;
                }
            }
            value = default(T);
            return false;
        }

        private bool TryClaimFreeSlot(out LockNeedleSlot<T> slot)
        {
            if (_freeSlots.TryTake(out slot))
            {
                if (slot.Claim())
                {
                    return true;
                }
            }
            return false;
        }
    }
}