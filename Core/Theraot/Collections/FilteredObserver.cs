// Needed for NET40

using System;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class FilteredObserver<T> : IObserver<T>
    {
        private readonly IObserver<T> _observer;
        private readonly Predicate<T> _filter;

        public FilteredObserver(IObserver<T> observer, Predicate<T> filter)
        {
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            _observer = observer;
            _filter = filter;
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
            var filter = _filter;
            if (filter(value))
            {
                _observer.OnNext(value);
            }
        }
    }
}