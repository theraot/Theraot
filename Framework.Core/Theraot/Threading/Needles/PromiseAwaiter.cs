using System;
using System.Runtime.CompilerServices;

namespace Theraot.Threading.Needles
{
    public static class PromiseExtensions
    {
        public static PromiseAwaiter GetAwaiter(this IWaitablePromise promise)
        {
            return new PromiseAwaiter(promise);
        }

        public static PromiseAwaiter<T> GetAwaiter<T>(this IWaitablePromise<T> promise)
            where T : class
        {
            return new PromiseAwaiter<T>(promise);
        }
    }

    public sealed class PromiseAwaiter : INotifyCompletion
    {
        private readonly IWaitablePromise _promise;

        internal PromiseAwaiter(IWaitablePromise promise)
        {
            _promise = promise;
        }

        public bool IsCompleted => _promise.IsCompleted;

        public void GetResult()
        {
            _promise.Wait();
        }

        public void OnCompleted(Action continuation)
        {
            _promise.OnCompleted(continuation);
        }
    }

    public sealed class PromiseAwaiter<T> : INotifyCompletion
        where T : class
    {
        private readonly IWaitablePromise<T> _promise;

        internal PromiseAwaiter(IWaitablePromise<T> promise)
        {
            _promise = promise;
        }

        public bool IsCompleted => _promise.IsCompleted;

        public T GetResult()
        {
            return _promise.Value;
        }

        public void OnCompleted(Action continuation)
        {
            _promise.OnCompleted(continuation);
        }
    }
}