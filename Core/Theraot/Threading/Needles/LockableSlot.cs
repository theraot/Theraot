using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Threading.Needles
{
    internal sealed class LockableSlot : IDisposable
    {
        private LockableContext _context;
        private LockSlot<Thread> _lockSlot;
        private LockableSlot _parent;
        private List<Pin> _pins;

        internal LockableSlot(LockableContext context)
        {
            _context = context;
            _parent = _context.Slot;
            _context.Slot = this;
            // --
            LockSlot<Thread> lockSlot = null;
            ThreadingHelper.SpinWaitUntil(() => _context.Context.ClaimSlot(out lockSlot));
            lockSlot.Value = Thread.CurrentThread;
            _lockSlot = lockSlot;
            _pins = new List<Pin>();
        }

        internal LockSlot<Thread> LockSlot
        {
            get
            {
                return _lockSlot;
            }
        }

        public void Dispose()
        {
            var lockslot = Interlocked.Exchange(ref _lockSlot, null);
            if (lockslot != null)
            {
                _context.Slot = _parent;
                Thread.MemoryBarrier();
                foreach (var pin in _pins)
                {
                    pin.Release(lockslot);
                }
                _pins.Clear();
                lockslot.Free();
                _parent = null;
                _context = null;
                _pins = null;
            }
        }

        internal void Add(Pin pin)
        {
            _pins.Add(pin);
        }
    }
}