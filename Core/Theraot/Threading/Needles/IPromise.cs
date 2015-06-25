using System;

namespace Theraot.Threading.Needles
{
    public interface IPromise
    {
        Exception Error { get; }

        bool IsCanceled
        {
            get;
        }

        bool IsCompleted
        {
            get;
        }

        bool IsFaulted
        {
            get;
        }

        void Wait();
    }

    public interface IPromise<out T> : IPromise, IReadOnlyNeedle<T>
    {
        // Empty
    }
}