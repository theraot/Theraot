// Needed for NET40

using System;

namespace Theraot.Threading.Needles
{
    public interface IPromise
    {
        bool IsCompleted { get; }
    }

    public interface IPromise<out T> : IPromise, IReadOnlyNeedle<T>
    {
    }

    public interface IWaitablePromise : IPromise
    {
        void OnCompleted(Action continuation);

        void Wait();
    }

    public interface IWaitablePromise<out T> : IPromise<T>, IWaitablePromise
    {
    }
}