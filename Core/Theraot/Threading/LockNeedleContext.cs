using System.Threading;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Threading
{
    internal class LockNeedleContext<T>
    {
        private static readonly VersionProvider _version = new VersionProvider();
        private readonly int _capacity;
        private readonly QueueBucket<LockNeedleSlot<T>> _freeSlots;
        private readonly LazyBucket<LockNeedleSlot<T>> _slots;
        private int _index;
        private VersionProvider.VersionToken _versionToken;

        public LockNeedleContext(int capacity)
        {
            _versionToken = _version.NewToken();
            _capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            _slots = new LazyBucket<LockNeedleSlot<T>>(index => new LockNeedleSlot<T>(this, index), capacity);
            _freeSlots = new QueueBucket<LockNeedleSlot<T>>(capacity);
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

        internal VersionProvider.VersionToken VersionToken
        {
            get
            {
                return _versionToken;
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

        public LockNeedle<T> CreateToken(T value)
        {
            return new LockNeedle<T>(this, value);
        }

        internal void Advance()
        {
            _version.Advance();
            _versionToken.Update();
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
                        if (bestSlot.CompareTo(slot) == 1)
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
            return _freeSlots.TryTake(out slot);
        }
    }
}