#if FAT

using System.ComponentModel;

namespace Theraot.Collections.ThreadSafe
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class WeakCancellableEvent<TCancelEventArgs> : WeakEvent<TCancelEventArgs>
        where TCancelEventArgs : CancelEventArgs
    {
        public WeakCancellableEvent()
        {
            //Empty
        }

        public WeakCancellableEvent(bool reentryGuard)
            : base(reentryGuard)
        {
            //Empty
        }

        public override void Invoke(object sender, TCancelEventArgs eventArgs)
        {
            foreach (var handler in EventHandlers)
            {
                handler.DynamicInvoke(eventArgs);
                if (eventArgs.Cancel)
                {
                    break;
                }
            }
        }
    }
}

#endif