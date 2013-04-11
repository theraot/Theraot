using System;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class FilteredConvertedObserver<TInput, TOutput> : IObserver<TInput>
    {
        private Converter<TInput, TOutput> _converter;
        private IObserver<TOutput> _observer;
        private Predicate<TInput> _predicate;

        public FilteredConvertedObserver(IObserver<TOutput> observer, Converter<TInput, TOutput> converter, Predicate<TInput> predicate)
        {
            _observer = Check.NotNullArgument(observer, "observer");
            _converter = Check.NotNullArgument(converter, "converter");
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

        public void OnNext(TInput value)
        {
            if (_predicate(value))
            {
                _observer.OnNext(_converter.Invoke(value));
            }
        }
    }
}