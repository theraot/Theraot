using System.Threading;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Threading
{
    internal class LockContext<T>
    {
        private readonly int _capacity;
        private readonly QueueBucket<LockSlot<T>> _freeSlots;
        private readonly LazyBucket<LockSlot<T>> _slots;
        private readonly VersionProvider _version = new VersionProvider();
        private int _index;

        public LockContext(int capacity)
        {
            _capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _slots = new LazyBucket<LockSlot<T>>(index => new LockSlot<T>(this, index, _version.AdvanceNewToken()), _capacity);
            _freeSlots = new QueueBucket<LockSlot<T>>(_capacity);
        }

        internal int Capacity
        {
            get
            {
                return _capacity;
            }
        }

        public bool ClaimSlot(out LockSlot<T> slot)
        {
            if (TryClaimFreeSlot(out slot))
            {
                return true;
            }
            else
            {
                if (_slots.Count < _slots.Capacity)
                {
                    int index = Interlocked.Increment(ref _index) & (_capacity - 1);
                    slot = _slots.Get(index);
                    return true;
                }
                else
                {
                    slot = null;
                    return false;
                }
            }
        }

        internal void Free(LockSlot<T> slot)
        {
            _freeSlots.Add(slot);
        }

        internal bool Read(int flag, out T value)
        {
            LockSlot<T> slot;
            if (_slots.TryGet(flag, out slot))
            {
                value = slot.Value;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        internal bool Read(FlagArray flags, out T value)
        {
            LockSlot<T> bestSlot = null;
            foreach (var flag in flags.Flags)
            {
                LockSlot<T> testSlot;
                if (_slots.TryGet(flag, out testSlot))
                {
                    if (ReferenceEquals(bestSlot, null))
                    {
                        bestSlot = testSlot;
                    }
                    else
                    {
                        if (bestSlot.CompareTo(testSlot) > 0)
                        {
                            bestSlot = testSlot;
                        }
                    }
                }
            }
            if (ReferenceEquals(bestSlot, null))
            {
                value = default(T);
                return false;
            }
            else
            {
                value = bestSlot.Value;
                return true;
            }
        }

        private bool TryClaimFreeSlot(out LockSlot<T> slot)
        {
            if (_freeSlots.TryTake(out slot))
            {
                slot.Unfree();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}