// Needed for NET20 (Contract, Task)

using System;
using System.Reflection;

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6

using Theraot.Core;

#endif

namespace Theraot.Collections.ThreadSafe
{
    [System.Diagnostics.DebuggerNonUserCode]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class StrongEvent<TEventArgs> : IEvent<TEventArgs>
        where TEventArgs : EventArgs
    {
        public StrongEvent()
        {
            EventHandlers = new StrongDelegateCollection(true);
        }

        public StrongEvent(bool freeReentry)
        {
            EventHandlers = new StrongDelegateCollection(freeReentry);
        }

        public int Count => EventHandlers.Count;

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

        public void Clear()
        {
            EventHandlers.Clear();
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