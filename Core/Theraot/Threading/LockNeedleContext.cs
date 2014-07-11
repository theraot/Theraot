using System.Threading;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Threading
{
    internal static class LockNeedleContext<T>
    {
        private static readonly int _capacity;
        private static readonly QueueBucket<LockNeedleSlot<T>> _freeSlots;
        private static readonly LazyBucket<LockNeedleSlot<T>> _slots;
        private static readonly VersionProvider _version = new VersionProvider();
        private static int _index;

        static LockNeedleContext()
        {
            _capacity = 512;
            _capacity = NumericHelper.PopulationCount(_capacity) == 1 ? _capacity : NumericHelper.NextPowerOf2(_capacity);
            _slots = new LazyBucket<LockNeedleSlot<T>>(index => new LockNeedleSlot<T>(index, _version.AdvanceNewToken()), _capacity);
            _freeSlots = new QueueBucket<LockNeedleSlot<T>>(_capacity);
        }

        internal static int Capacity
        {
            get
            {
                return _capacity;
            }
        }

        public static bool ClaimSlot(out LockNeedleSlot<T> slot)
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

        internal static void Free(LockNeedleSlot<T> slot)
        {
            _freeSlots.Add(slot);
        }

        internal static bool Read(int flag, out T value)
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

        internal static bool Read(FlagArray flags, out T value)
        {
            LockNeedleSlot<T> bestSlot = null;
            foreach (var flag in flags.Flags)
            {
                LockNeedleSlot<T> testSlot;
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

        private static bool TryClaimFreeSlot(out LockNeedleSlot<T> slot)
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