using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal class LockNeedleContext<T>
    {
        private int _current;
        private Bucket<LockNeedleSlot<T>> _slots;
        private VersionProvider _version;

        public LockNeedleContext()
        {
            _slots = new Bucket<LockNeedleSlot<T>>(32);
            _version = new VersionProvider();
        }

        public bool ClaimSlot(out LockNeedleSlot<T> slot)
        {
            foreach (var currentSlot in _slots)
            {
                if (currentSlot.Claim())
                {
                    slot = currentSlot;
                    return true;
                }
            }
            if (_slots.Count < _slots.Capacity)
            {
                var newSlot = new LockNeedleSlot<T>(this);
                while (_slots.Count < _slots.Capacity)
                {
                    var index = Interlocked.Increment(ref _current) & 31;
                    if (_slots.Insert(index, newSlot))
                    {
                        newSlot.Id = 1 << index;
                        newSlot.Claim();
                        slot = newSlot;
                        return true;
                    }
                }
            }
            slot = null;
            return false;
        }

        public LockNeedle<T> CreateToken()
        {
            return new LockNeedle<T>(this);
        }

        internal VersionProvider.VersionToken Advance()
        {
            return _version.AdvanceNewToken();
        }

        internal bool Read(int id, out T value)
        {
            LockNeedleSlot<T> selected = null;
            foreach (var currentSlot in _slots)
            {
                if ((currentSlot.Id & id) != 0)
                {
                    if (ReferenceEquals(selected, null))
                    {
                        selected = currentSlot;
                    }
                    else
                    {
                        if (selected.CompareTo(currentSlot) < 0)
                        {
                            selected = currentSlot;
                        }
                    }
                }
            }
            if (ReferenceEquals(selected, null))
            {
                value = default(T);
                return false;
            }
            else
            {
                value = selected.Value;
                return true;
            }
        }
    }
}
