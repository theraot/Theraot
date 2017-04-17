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
        private List<NeedleLock<Thread>> _needleLocks;
        private LockableSlot _parent;

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
            _needleLocks = new List<NeedleLock<Thread>>();
        }

        ~LockableSlot()
        {
            Dispose();
        }

        internal LockSlot<Thread> LockSlot
        {
            get { return _lockSlot; }
        }

        public void Dispose()
        {
            var lockslot = Interlocked.Exchange(ref _lockSlot, null);
            if (ReferenceEquals(lockslot, null))
            {
                return;
            }
            var context = Interlocked.Exchange(ref _context, null);
            if (context != null)
            {
                context.Slot = _parent;
            }
            var needleLocks = Interlocked.Exchange(ref _needleLocks, null);
            if (needleLocks != null)
            {
                foreach (var needleLock in needleLocks)
                {
                    needleLock.Uncapture(lockslot);
                    needleLock.Release();
                }
                needleLocks.Clear();
            }
            lockslot.Close();
            ThreadingHelper.MemoryBarrier();
            _parent = null;
            GC.SuppressFinalize(this);
        }

        internal void Add(NeedleLock<Thread> pin)
        {
            _needleLocks.Add(pin);
        }
    }
}

#endif