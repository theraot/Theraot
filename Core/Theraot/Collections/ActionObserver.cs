// Needed for NET40

using System;
using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class ActionObserver<T> : IObserver<T>
    {
        private readonly Action<T> _action;

        public ActionObserver(Action<T> action)
        {
            _action = Check.NotNullArgument(action, "action");
        }

        public void OnCompleted()
        {
            // Empty
        }

        public void OnError(Exception error)
        {
            GC.KeepAlive(error);
        }

        public void OnNext(T value)
        {
            _action.Invoke(value);
        }
    }
}