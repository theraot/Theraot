// Needed for NET20 (Contract, Task)

using System;
using System.Diagnostics;
using System.Reflection;

namespace Theraot.Collections.ThreadSafe
{
    [DebuggerNonUserCode]
    public class StrongEvent<TEventArgs> : IEvent<TEventArgs>
        where TEventArgs : EventArgs
    {
        public StrongEvent(bool freeReentry)
        {
            EventHandlers = new StrongDelegateCollection(freeReentry);
        }

        protected StrongDelegateCollection EventHandlers { get; }

        public void Add(EventHandler<TEventArgs> value)
        {
            EventHandlers.Add(value);
        }

        public void Add(MethodInfo method, object target)
        {
            var value = method.CreateDelegate(typeof(EventHandler<TEventArgs>), target);
            EventHandlers.Add(value);
        }

        public virtual void Invoke(object sender, TEventArgs eventArgs)
        {
            EventHandlers.Invoke(sender, eventArgs);
        }

        public virtual void InvokeWithException(Action<Exception> onException, object sender, TEventArgs eventArgs)
        {
            EventHandlers.InvokeWithException(onException, sender, eventArgs);
        }

        public void Remove(EventHandler<TEventArgs> value)
        {
            EventHandlers.Remove(value);
        }

        public void Remove(MethodInfo method, object target)
        {
            var value = method.CreateDelegate(typeof(EventHandler<TEventArgs>), target);
            EventHandlers.Remove(value);
        }
    }
}