using System.Runtime.CompilerServices;
using Theraot;

namespace System.Threading.Tasks
{
    public static partial class TaskExEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task FromCanceled(CancellationToken cancellationToken)
        {
            return FromCanceled<VoidStruct>(cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
        {
#if LESSTHAN_NET40
            // Microsoft says Task.FromCancellation throws ArgumentOutOfRangeException when cancellation has not been requested for cancellationToken
            if (!cancellationToken.IsCancellationRequested)
            {
                throw new ArgumentOutOfRangeException(nameof(cancellationToken));
            }

            var task = new Task<TResult>(TaskCreationOptions.None, null);
            task.TrySetCanceled(cancellationToken);
            return task;
#endif
#if (GREATERTHAN_NET35 && LESSTHAN_NET46) || LESSTHAN_NETSTANDARD13
            // Microsoft says Task.FromCancellation throws ArgumentOutOfRangeException when cancellation has not been requested for cancellationToken
            if (!cancellationToken.IsCancellationRequested)
            {
                throw new ArgumentOutOfRangeException(nameof(cancellationToken));
            }

            var taskCompleteSource = new TaskCompletionSource<TResult>();
            taskCompleteSource.TrySetCanceled(cancellationToken);
            return taskCompleteSource.Task;
#endif
#if GREATERTHAN_NET45 || GREATERTHAN_NETSTANDARD12 || TARGETS_NETCORE
            return Task.FromCanceled<TResult>(cancellationToken);
#endif
        }
    }
}