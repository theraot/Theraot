// Needed for NET40

using System;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class FilteredConvertedObserver<TInput, TOutput> : IObserver<TInput>
    {
        private readonly Converter<TInput, TOutput> _converter;
        private readonly IObserver<TOutput> _observer;
        private readonly Predicate<TInput> filter;

        public FilteredConvertedObserver(IObserver<TOutput> observer, Predicate<TInput> filter, Converter<TInput, TOutput> converter)
        {
            _observer = Check.NotNullArgument(observer, "observer");
            _converter = Check.NotNullArgument(converter, "converter");
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

        public void OnNext(TInput value)
        {
            if (filter(value))
            {
                _observer.OnNext(_converter.Invoke(value));
            }
        }
    }
}