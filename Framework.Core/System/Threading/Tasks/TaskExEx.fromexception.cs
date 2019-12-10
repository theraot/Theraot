using System.Runtime.CompilerServices;
using Theraot;

namespace System.Threading.Tasks
{
    public static partial class TaskExEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> FromException<TResult>(Exception exception)
        {
#if LESSTHAN_NET40
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var task = new Task<TResult>(TaskCreationOptions.None, null);
            var value = task.TrySetException(exception);
            if (value || task.IsCompleted)
            {
                return task;
            }

            var sw = new SpinWait();
            while (!task.IsCompleted)
            {
                sw.SpinOnce();
            }

            return task;
#endif
#if (GREATERTHAN_NET35 && LESSTHAN_NET46) || LESSTHAN_NETSTANDARD13
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            var taskCompleteSource = new TaskCompletionSource<TResult>();
            taskCompleteSource.TrySetException(exception);
            return taskCompleteSource.Task;
#endif
#if GREATERTHAN_NET45 || GREATERTHAN_NETSTANDARD12 || TARGETS_NETCORE
            return Task.FromException<TResult>(exception);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task FromException(Exception exception)
        {
            return FromException<VoidStruct>(exception);
        }
    }
}