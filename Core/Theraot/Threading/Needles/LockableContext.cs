#if FAT

using System.Threading;

namespace Theraot.Threading.Needles
{
    public sealed class LockableContext
    {
        internal readonly LockContext<Thread> Context;
        internal readonly StructNeedle<TrackingThreadLocal<LockSlot<Thread>>> Slot;

        public LockableContext(int capacity)
        {
            Context = new LockContext<Thread>(capacity);
            Slot = new TrackingThreadLocal<LockSlot<Thread>>
                (
                    () =>
                    {
                        LockSlot<Thread> _lockSlot = null;
                        ThreadingHelper.SpinWaitUntil(() => Context.ClaimSlot(out _lockSlot));
                        _lockSlot.Value = Thread.CurrentThread;
                        return _lockSlot;
                    }
                );
        }
    }
}

#endif