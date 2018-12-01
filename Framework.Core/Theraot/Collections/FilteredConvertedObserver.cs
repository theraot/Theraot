// Needed for NET40

using System;

namespace Theraot.Collections
{
    public sealed class FilteredConvertedObserver<TInput, TOutput> : IObserver<TInput>
    {
        private readonly Func<TInput, TOutput> _converter;
        private readonly Predicate<TInput> _filter;
        private readonly IObserver<TOutput> _observer;

        public FilteredConvertedObserver(IObserver<TOutput> observer, Predicate<TInput> filter, Func<TInput, TOutput> converter)
        {
            _observer = observer ?? throw new ArgumentNullException(nameof(observer));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
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