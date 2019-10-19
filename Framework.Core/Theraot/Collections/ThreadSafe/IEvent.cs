using System;
using System.Reflection;

namespace Theraot.Collections.ThreadSafe
{
    public interface IEvent<TEventArgs>
        where TEventArgs : EventArgs
    {
        void Add(EventHandler<TEventArgs> value);

        void Add(MethodInfo method, object target);

        void Invoke(Action<Exception> onException, object? sender, TEventArgs eventArgs);

        void Invoke(object? sender, TEventArgs eventArgs);

        void Remove(EventHandler<TEventArgs> value);

        void Remove(MethodInfo method, object target);
    }
}