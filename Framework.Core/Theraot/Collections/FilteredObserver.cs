// Needed for NET40

using System;

namespace Theraot.Collections
{
    public sealed class FilteredObserver<T> : IObserver<T>
    {
        private readonly Predicate<T> _filter;
        private readonly IObserver<T> _observer;

        public FilteredObserver(IObserver<T> observer, Predicate<T> filter)
        {
            _observer = observer ?? throw new ArgumentNullException(nameof(observer));
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
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