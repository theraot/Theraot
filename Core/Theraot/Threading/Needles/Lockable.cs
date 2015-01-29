#if FAT

using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public sealed class Lockable : ILockable
    {
        private readonly LockableContext _context;
        private readonly NeedleLock<Thread> _lock;

        public Lockable(LockableContext context)
        {
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            else
            {
                _context = context;
                _lock = new NeedleLock<Thread>(context.Context);
            }
        }

        public bool HasOwner
        {
            get
            {
                return !ReferenceEquals(_lock.Value, null);
            }
        }

        public bool Capture()
        {
            _context.Slot.Value.Value.Capture(_lock);
            return ReferenceEquals(_lock.Value, Thread.CurrentThread);
        }

        public bool CheckAccess(Thread thread)
        {
            var value = _lock.Value;
            return ReferenceEquals(value, thread) || ReferenceEquals(value, null);
        }

        public void Uncapture()
        {
            LockSlot<Thread> slot;
            if (_context.Slot.Value.TryGetValue(out slot))
            {
                slot.Uncapture(_lock);
            }
            _lock.Free();
        }
    }
}

#endif