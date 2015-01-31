#if FAT

using System.Threading;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public class LockContext<T>
    {
        private readonly int _capacity;
        private readonly QueueBucket<LockSlot<T>> _freeSlots;
        private readonly NeedleBucket<LockSlot<T>, LazyNeedle<LockSlot<T>>> _slots;
        private readonly VersionProvider _version = new VersionProvider();
        private int _index;

        public LockContext(int capacity)
        {
            _capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _slots = new NeedleBucket<LockSlot<T>, LazyNeedle<LockSlot<T>>>(index => new LockSlot<T>(this, index, _version.AdvanceNewToken()), _capacity);
            _freeSlots = new QueueBucket<LockSlot<T>>(_capacity);
        }

        internal int Capacity
        {
            get
            {
                return _capacity;
            }
        }

        internal bool ClaimSlot(out LockSlot<T> slot)
        {
            if (TryClaimFreeSlot(out slot))
            {
                return true;
            }
            if (_slots.Count < _slots.Capacity)
            {
                int index = Interlocked.Increment(ref _index) & (_capacity - 1);
                slot = _slots.Get(index);
                return true;
            }
            slot = null;
            return false;
        }

        internal void Free(LockSlot<T> slot)
        {
            _freeSlots.Add(slot);
        }

        internal bool Read(FlagArray flags, out T value)
        {
            LockSlot<T> bestSlot = null;
            value = default(T);
            foreach (var flag in flags.Flags)
            {
                LockSlot<T> testSlot;
                if (!_slots.TryGet(flag, out testSlot))
                {
                    continue;
                }
                if (ReferenceEquals(bestSlot, null) || bestSlot.CompareTo(testSlot) < 0)
                {
                    bestSlot = SwitchSlot(out value, testSlot);
                }
            }
            return !ReferenceEquals(bestSlot, null);
        }

        private static LockSlot<T> SwitchSlot(out T value, LockSlot<T> testSlot)
        {
            value = testSlot.Value;
            return testSlot;
        }

        private bool TryClaimFreeSlot(out LockSlot<T> slot)
        {
            if (_freeSlots.TryTake(out slot))
            {
                slot.Unfree(_version.AdvanceNewToken());
                return true;
            }
            return false;
        }
    }
}

#endif