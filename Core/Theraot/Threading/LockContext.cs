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
        private readonly FixedSizeQueue<LockSlot<T>> _closedSlots;
        private readonly NeedleBucket<LockSlot<T>, LazyNeedle<LockSlot<T>>> _slots;
        private readonly VersionProvider _version = new VersionProvider();
        private int _index;
        private readonly int _capacity;

        public LockContext(int capacity)
        {
            _capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _slots = new NeedleBucket<LockSlot<T>, LazyNeedle<LockSlot<T>>>
                (
                    index => new LockSlot<T>
                    (
                        this,
                        index,
                        _version.AdvanceNewToken()
                    ),
                    key => new LazyNeedle<LockSlot<T>>(key),
                    _capacity
                );
            _closedSlots = new FixedSizeQueue<LockSlot<T>>(_capacity);
        }

        internal int Capacity
        {
            get { return _capacity; }
        }

        internal bool ClaimSlot(out LockSlot<T> slot)
        {
            if (TryClaimFreeSlot(out slot))
            {
                return true;
            }
            if (_slots.Count < _slots.Capacity)
            {
                var index = Interlocked.Increment(ref _index) & (_capacity - 1);
                slot = _slots.Get(index);
                return true;
            }
            slot = null;
            return false;
        }

        internal void Close(LockSlot<T> slot)
        {
            _closedSlots.Add(slot);
        }

        internal bool Read(FlagArray flags, ref int owner, out LockSlot<T> slot)
        {
            if (Read(ref owner, out slot))
            {
                return true;
            }
            var resultLock = -1;
            foreach (var flag in flags.Flags)
            {
                LockSlot<T> testSlot;
                if (!_slots.TryGet(flag, out testSlot))
                {
                    continue;
                }
                if (slot == null || slot.CompareTo(testSlot) < 0)
                {
                    slot = testSlot;
                    resultLock = flag;
                }
            }
            if (Interlocked.CompareExchange(ref owner, resultLock, -1) != -1)
            {
                return Read(ref owner, out slot);
            }
            if (slot == null)
            {
                return false;
            }
            return true;
        }

        private bool Read(ref int owner, out LockSlot<T> slot)
        {
            slot = null;
            var got = owner;
            if (got == -1)
            {
                return false;
            }
            LockSlot<T> found;
            if (_slots.TryGet(got, out found) && found.IsOpen)
            {
                slot = found;
                return true;
            }
            Interlocked.CompareExchange(ref owner, -1, got);
            return false;
        }

        private bool TryClaimFreeSlot(out LockSlot<T> slot)
        {
            if (_closedSlots.TryTake(out slot))
            {
                slot.Open(_version.AdvanceNewToken());
                return true;
            }
            return false;
        }
    }
}

#endif