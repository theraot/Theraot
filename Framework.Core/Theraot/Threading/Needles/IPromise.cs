// Needed for NET40

using System;

namespace Theraot.Threading.Needles
{
    public interface IPromise
    {
        Exception Exception { get; }

        bool IsCanceled { get; }

        bool IsCompleted { get; }

        bool IsFaulted { get; }
    }

    public interface IPromise<out T> : IPromise, IReadOnlyNeedle<T>
    {
        // Empty
    }

    public interface IWaitablePromise : IPromise
    {
        void Wait();
    }

    public interface IWaitablePromise<out T> : IPromise<T>, IWaitablePromise
    {
        // Empty
    }
}