using System.Threading;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Threading
{
    internal class LockNeedleContext<T>
    {
        private readonly int _capacity;
        private readonly QueueBucket<LockNeedleSlot<T>> _freeSlots;
        private readonly LazyBucket<LockNeedleSlot<T>> _slots;
        private readonly VersionProvider _version;
        private int _index;

        public LockNeedleContext(int capacity)
        {
            _capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _slots = new LazyBucket<LockNeedleSlot<T>>(index => new LockNeedleSlot<T>(this, index), capacity);
            _freeSlots = new QueueBucket<LockNeedleSlot<T>>(capacity);
            _version = new VersionProvider();
        }

        public LockNeedleContext()
            : this(32)
        {
            //Empty
        }

        internal int Capacity
        {
            get
            {
                return _capacity;
            }
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
                    int index = Interlocked.Increment(ref _index) & (_capacity - 1);
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

        internal bool Read(int flag, out T value)
        {
            LockNeedleSlot<T> slot;
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
            LockNeedleSlot<T> slot;
            LockNeedleSlot<T> bestSlot = null;
            foreach (var flag in flags.Flags)
            {
                if (_slots.TryGet(flag, out slot))
                {
                    if (ReferenceEquals(bestSlot, null))
                    {
                        bestSlot = slot;
                    }
                    else
                    {
                        if (slot.CompareTo(bestSlot) == 1)
                        {
                            bestSlot = slot;
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