#if FAT

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

        ~LockableSlot()
        {
            Dispose();
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
                var context = Interlocked.Exchange(ref _context, null);
                if (context != null)
                {
                    context.Slot = _parent;
                }
                var pins = Interlocked.Exchange(ref _pins, null);
                if (pins != null)
                {
                    foreach (var pin in pins)
                    {
                        pin.Release(lockslot);
                    }
                    pins.Clear();
                }
                lockslot.Release();
                Thread.MemoryBarrier();
                _parent = null;
            }
        }

        internal void Add(Pin pin)
        {
            _pins.Add(pin);
        }
    }
}

#endif