using System;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class FilteredObserver<T> : IObserver<T>
    {
        private IObserver<T> _observer;
        private Predicate<T> _predicate;

        public FilteredObserver(IObserver<T> observer, Predicate<T> predicate)
        {
            _observer = Check.NotNullArgument(observer, "observer");
            _predicate = Check.NotNullArgument(predicate, "predicate");
        }

        public void OnCompleted()
        {
            _observer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _observer.OnError(error);
        }

        public void OnNext(T value)
        {
            if (_predicate(value))
            {
                _observer.OnNext(value);
            }
        }
    }
}