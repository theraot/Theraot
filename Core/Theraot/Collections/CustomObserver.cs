// Needed for NET40

using System;
using Theraot.Core;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class CustomObserver<T> : IObserver<T>
    {
        private readonly Action _onCompleted;
        private readonly Action<Exception> _onError;
        private readonly Action<T> _onNext;

        public CustomObserver(Action<T> onNext)
        {
            _onCompleted = ActionHelper.GetNoopAction();
            _onError = ActionHelper.GetNoopAction<Exception>();
            _onNext = onNext ?? ActionHelper.GetNoopAction<T>();
        }

        public CustomObserver(Action onCompleted, Action<Exception> onError, Action<T> onNext)
        {
            _onCompleted = onCompleted ?? ActionHelper.GetNoopAction();
            _onError = onError ?? ActionHelper.GetNoopAction<Exception>();
            _onNext = onNext ?? ActionHelper.GetNoopAction<T>();
        }

        public void OnCompleted()
        {
            _onCompleted();
        }

        public void OnError(Exception error)
        {
            _onError(error);
        }

        public void OnNext(T value)
        {
            _onNext(value);
        }
    }
}