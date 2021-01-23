#if GREATERTHAN_NET35 && LESSTHAN_NET46

using System.Reflection;

namespace System.Threading.Tasks
{
    public static partial class TaskCompletionSourceTheraotExtensions
    {
        public static bool TrySetCanceled<T>(this TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken)
        {
            if (taskCompletionSource == null)
            {
                throw new ArgumentNullException(nameof(taskCompletionSource));
            }

            return TrySetCanceledCachedDelegate<T>.TrySetCanceledCached(taskCompletionSource, cancellationToken);
        }

        /// <summary>
        /// Calls TaskCompletionSource<typeparamref name="T"/>.TrySetCanceled internal method.
        /// </summary>
        private static class TrySetCanceledCachedDelegate<T>
        {
            public static Func<TaskCompletionSource<T>, CancellationToken, bool> TrySetCanceledCached =>
                _trySetCanceledCached ??= CreateTrySetCanceledDelegate();

            private static Func<TaskCompletionSource<T>, CancellationToken, bool>? _trySetCanceledCached;

            private static Func<TaskCompletionSource<T>, CancellationToken, bool> CreateTrySetCanceledDelegate()
            {
                var trySetCanceledCached = typeof(TaskCompletionSource<T>).GetMethod
                (
                    nameof(TrySetCanceled),
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                    null,
                    CallingConventions.Any,
                    new[] { typeof(CancellationToken) },
                    null
                );
                if (trySetCanceledCached == null)
                {
                    throw new PlatformNotSupportedException();
                }

                return (Func<TaskCompletionSource<T>, CancellationToken, bool>)Delegate.CreateDelegate
                (
                    typeof(Func<TaskCompletionSource<T>, CancellationToken, bool>),
                    trySetCanceledCached
                );
            }
        }
    }
}

#elif LESSTHAN_NETSTANDARD14

namespace System.Threading.Tasks
{
    public static partial class TaskCompletionSourceTheraotExtensions
    {
        public static bool TrySetCanceled<T>(this TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken)
        {
            if (taskCompletionSource == null)
            {
                throw new ArgumentNullException(nameof(taskCompletionSource));
            }

            if (taskCompletionSource.Task.IsCompleted)
            {
                return false;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return taskCompletionSource.TrySetCanceled(cancellationToken);
            }

            cancellationToken.Register(() => taskCompletionSource.TrySetCanceled());
            SpinUntilCompleted(taskCompletionSource.Task);
            return taskCompletionSource.Task.Status == TaskStatus.Canceled;
        }

        private static void SpinUntilCompleted(Task task)
        {
            var sw = new SpinWait();
            while (!task.IsCompleted)
            {
                sw.SpinOnce();
            }
        }
    }
}

#endif

#if GREATERTHAN_NET35 || TARGETS_NETSTANDARD || LESSTHAN_NET50

namespace System.Threading.Tasks
{
    public static
#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD14
        partial
#endif
        class TaskCompletionSourceTheraotExtensions
    {
        public static void SetCanceled<T>(this TaskCompletionSource<T> taskCompletionSource, CancellationToken cancellationToken)
        {
            if (taskCompletionSource == null)
            {
                throw new ArgumentNullException(nameof(taskCompletionSource));
            }

            if (!taskCompletionSource.TrySetCanceled(cancellationToken))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }
    }
}

#endif