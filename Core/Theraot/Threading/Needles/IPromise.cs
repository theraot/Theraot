using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public interface IPromise
    {
        AggregateException Exception
        {
            get;
        }

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