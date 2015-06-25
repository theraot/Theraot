using System;

namespace Theraot.Threading.Needles
{
    public interface IPromise
    {
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

        Exception Error { get; }
        
        void Wait();
    }

    public interface IPromise<out T> : IPromise, IReadOnlyNeedle<T>
    {
        //Empty
    }
}