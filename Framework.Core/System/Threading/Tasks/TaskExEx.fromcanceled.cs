#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
#pragma warning disable RCS1231 // Make parameter ref read-only.

using System.Runtime.CompilerServices;
using Theraot;

#if NET40

using System.Linq;

#endif

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
            taskCompleteSource.TrySetCanceled();
            return taskCompleteSource.Task;
#endif
#if GREATERTHAN_NET45 || GREATERTHAN_NETSTANDARD12 || TARGETS_NETCORE
            return Task.FromCanceled<TResult>(cancellationToken);
#endif
        }
    }
}