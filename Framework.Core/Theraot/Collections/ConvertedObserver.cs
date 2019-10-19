// Needed for NET40

using System;

namespace Theraot.Collections
{
    public sealed class ConvertedObserver<TInput, TOutput> : IObserver<TInput>
    {
        private readonly Func<TInput, TOutput> _converter;

        private readonly IObserver<TOutput> _observer;

        public ConvertedObserver(IObserver<TOutput> observer, Func<TInput, TOutput> converter)
        {
            _observer = observer ?? throw new ArgumentNullException(nameof(observer));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
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