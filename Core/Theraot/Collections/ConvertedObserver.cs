using System;

using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class ConvertedObserver<TInput, TOutput> : IObserver<TInput>
    {
        private Converter<TInput, TOutput> _converter;
        private IObserver<TOutput> _observer;

        public ConvertedObserver(IObserver<TOutput> observer, Converter<TInput, TOutput> converter)
        {
            _observer = Check.NotNullArgument(observer, "observer");
            _converter = Check.NotNullArgument(converter, "converter");
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
            _observer.OnNext(_converter.Invoke(value));
        }
    }
}