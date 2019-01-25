#if FAT

using System;
using System.ComponentModel;

namespace Theraot.Collections.ThreadSafe
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class StrongCancellableEvent<TCancelEventArgs> : StrongEvent<TCancelEventArgs>
        where TCancelEventArgs : CancelEventArgs
    {
        public StrongCancellableEvent(bool freeReentry)
            : base(freeReentry)
        {
            // Empty
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

        public override void InvokeWithException(Action<Exception> onException, object sender, TCancelEventArgs eventArgs)
        {
            foreach (var handler in EventHandlers)
            {
                try
                {
                    handler.DynamicInvoke(eventArgs);
                }
                catch (Exception exception)
                {
                    onException(exception);
                }
                if (eventArgs.Cancel)
                {
                    break;
                }
            }
        }
    }

    [System.Diagnostics.DebuggerNonUserCode]
    public class WeakCancellableEvent<TCancelEventArgs> : WeakEvent<TCancelEventArgs>
        where TCancelEventArgs : CancelEventArgs
    {
        public WeakCancellableEvent()
        {
            // Empty
        }

        public WeakCancellableEvent(bool freeReentry)
            : base(freeReentry)
        {
            // Empty
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

        public override void InvokeWithException(Action<Exception> onException, object sender, TCancelEventArgs eventArgs)
        {
            foreach (var handler in EventHandlers)
            {
                try
                {
                    handler.DynamicInvoke(eventArgs);
                }
                catch (Exception exception)
                {
                    onException(exception);
                }
                if (eventArgs.Cancel)
                {
                    break;
                }
            }
        }
    }
}

#endif