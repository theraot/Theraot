using System;
using System.Diagnostics;
using System.Reflection;

namespace Theraot.Collections.ThreadSafe
{
    [DebuggerNonUserCode]
    public class WeakEvent<TEventArgs> : IEvent<TEventArgs>
        where TEventArgs : EventArgs
    {
        public WeakEvent()
        {
            EventHandlers = new WeakDelegateCollection(true, true);
        }

        public WeakEvent(bool freeReentry)
        {
            EventHandlers = new WeakDelegateCollection(true, freeReentry);
        }

        protected WeakDelegateCollection EventHandlers { get; }

        public void Add(EventHandler<TEventArgs> value)
        {
            EventHandlers.Add(value);
        }

        public void Add(MethodInfo method, object target)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            var value = method.CreateDelegate(typeof(EventHandler<TEventArgs>), target);
            EventHandlers.Add(value);
        }

        public virtual void Invoke(object? sender, TEventArgs eventArgs)
        {
            EventHandlers.Invoke(DelegateCollectionInvokeOptions.None, sender, eventArgs);
        }

        public virtual void Invoke(Action<Exception> onException, object? sender, TEventArgs eventArgs)
        {
            EventHandlers.Invoke(onException, DelegateCollectionInvokeOptions.None, sender, eventArgs);
        }

        public void Remove(EventHandler<TEventArgs> value)
        {
            EventHandlers.Remove(value);
        }

        public void Remove(MethodInfo method, object target)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            var value = method.CreateDelegate(typeof(EventHandler<TEventArgs>), target);
            EventHandlers.Remove(value);
        }
    }
}