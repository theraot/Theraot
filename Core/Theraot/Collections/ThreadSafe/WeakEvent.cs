using System;
using System.Reflection;

namespace Theraot.Collections.ThreadSafe
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class WeakEvent<TEventArgs>
        where TEventArgs : EventArgs
    {
        private readonly WeakDelegateSet _eventHandlers;

        public WeakEvent()
        {
            _eventHandlers = new WeakDelegateSet(true, true);
        }

        public WeakEvent(bool reentryGuard)
        {
            _eventHandlers = new WeakDelegateSet(true, reentryGuard);
        }

        public int Count
        {
            get
            {
                return _eventHandlers.Count;
            }
        }

        protected WeakDelegateSet EventHandlers
        {
            get
            {
                return _eventHandlers;
            }
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
                Delegate value = Delegate.CreateDelegate(typeof(EventHandler<TEventArgs>), target, method);
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
            _eventHandlers.Invoke(new[] { sender, eventArgs });
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
                Delegate value = Delegate.CreateDelegate(typeof(EventHandler<TEventArgs>), target, method);
                _eventHandlers.Remove(value);
            }
            catch (NullReferenceException)
            {
                //Empty
            }
        }
    }
}