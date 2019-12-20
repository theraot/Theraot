// Needed for NET40

using System.Runtime.CompilerServices;

namespace Theraot.Threading.Needles
{
    public interface IPromise
    {
        bool IsCompleted { get; }
    }

    public interface IPromise<out T> : IPromise, IReadOnlyNeedle<T>
    {
        // Empty
    }

    public interface IWaitablePromise : IPromise, INotifyCompletion
    {
        void Wait();
    }

    public interface IWaitablePromise<out T> : IPromise<T>, IWaitablePromise
    {
        // Empty
    }
}