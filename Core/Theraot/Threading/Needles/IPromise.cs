// Needed for NET40

using System;

namespace Theraot.Threading.Needles
{
    public interface IPromise
    {
        Exception Exception
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
    }

#if NETCF

    public interface IPromise<T> : IPromise, IReadOnlyNeedle<T>
#else

    public interface IPromise<out T> : IPromise, IReadOnlyNeedle<T>
#endif
    {
        // Empty
    }

    public interface IWaitablePromise : IPromise
    {
        void Wait();
    }

#if NETCF

    public interface IWaitablePromise<T> : IPromise<T>, IWaitablePromise
#else

    public interface IWaitablePromise<out T> : IPromise<T>, IWaitablePromise
#endif
    {
        // Empty
    }
}