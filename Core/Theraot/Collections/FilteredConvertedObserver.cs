// Needed for NET40

using System;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class FilteredConvertedObserver<TInput, TOutput> : IObserver<TInput>
    {
        private readonly Func<TInput, TOutput> _converter;
        private readonly IObserver<TOutput> _observer;
        private readonly Predicate<TInput> _filter;

        public FilteredConvertedObserver(IObserver<TOutput> observer, Predicate<TInput> filter, Func<TInput, TOutput> converter)
        {
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            _observer = observer;
            _converter = converter;
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

        public void OnNext(TInput value)
        {
            var filter = _filter;
            if (filter(value))
            {
                _observer.OnNext(_converter.Invoke(value));
            }
        }
    }
}