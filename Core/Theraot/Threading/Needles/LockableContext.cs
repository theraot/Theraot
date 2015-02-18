#if FAT

using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public sealed class LockableContext
    {
        internal readonly LockContext<Thread> Context;
        private readonly StructNeedle<TrackingThreadLocal<LockableSlot>> _slots;

        public LockableContext(int capacity)
        {
            Context = new LockContext<Thread>(capacity);
            _slots.Value = new TrackingThreadLocal<LockableSlot>();
        }

        internal bool HasSlot
        {
            get
            {
                return ((IThreadLocal<LockableSlot>)_slots.Value).ValueForDebugDisplay != null;
            }
        }

        internal LockableSlot Slot
        {
            get
            {
                return _slots.Value.Value;
            }
            set
            {
                _slots.Value.Value = value;
            }
        }

        public IDisposable Enter()
        {
            return new LockableSlot(this);
        }
    }
}

#endif