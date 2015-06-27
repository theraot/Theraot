// Needed for NET40

using System;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class FilteredObserver<T> : IObserver<T>
    {
        private readonly IObserver<T> _observer;
        private readonly Predicate<T> filter;

        public FilteredObserver(IObserver<T> observer, Predicate<T> filter)
        {
            _observer = Check.NotNullArgument(observer, "observer");
            this.filter = Check.NotNullArgument(filter, "filter");
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
            if (filter(value))
            {
                _observer.OnNext(value);
            }
        }
    }
}