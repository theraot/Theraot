#if FAT

using System;
using System.Reflection;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class WeakEvent<TEventArgs>
        where TEventArgs : EventArgs
    {
        private readonly WeakDelegateCollection _eventHandlers;

        public WeakEvent()
        {
            _eventHandlers = new WeakDelegateCollection(true, true);
        }

        public WeakEvent(bool reentryGuard)
        {
            _eventHandlers = new WeakDelegateCollection(true, reentryGuard);
        }

        public int Count
        {
            get { return _eventHandlers.Count; }
        }

        protected WeakDelegateCollection EventHandlers
        {
            get { return _eventHandlers; }
        }

        public void Add(EventHandler<TEventArgs> value)
        {
            try
            {
                _eventHandlers.Add(value);
            }
            catch (NullReferenceException)
            {
                //Empty
            }
        }

        public void Add(MethodInfo method, object target)
        {
            try
            {
                Delegate value = method.CreateDelegate(typeof(EventHandler<TEventArgs>), target);
                _eventHandlers.Add(value);
            }
            catch (NullReferenceException)
            {
                //Empty
            }
        }

        public void Clear()
        {
            _eventHandlers.Clear();
        }

        public virtual void Invoke(object sender, TEventArgs eventArgs)
        {
            _eventHandlers.Invoke(sender, eventArgs);
        }

        public void Remove(EventHandler<TEventArgs> value)
        {
            try
            {
                _eventHandlers.Remove(value);
            }
            catch (NullReferenceException)
            {
                //Empty
            }
        }

        public void Remove(MethodInfo method, object target)
        {
            try
            {
                Delegate value = method.CreateDelegate(typeof(EventHandler<TEventArgs>), target);
                _eventHandlers.Remove(value);
            }
            catch (NullReferenceException)
            {
                //Empty
            }
        }
    }
}

#endif