#if GREATERTHAN_NET35 && LESSTHAN_NET46 || LESSTHAN_NETSTANDARD14

using System.Diagnostics;
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
        /// Calls <c>TaskCompletionSource&lt;T&gt;.TrySetCanceled</c> internal method.
        /// </summary>
        private static class TrySetCanceledCachedDelegate<T>
        {
            public static Func<TaskCompletionSource<T>, CancellationToken, bool> TrySetCanceledCached =>
                _trySetCanceledCached ??= CreateTrySetCanceledDelegate();

            private static Func<TaskCompletionSource<T>, CancellationToken, bool>? _trySetCanceledCached;

            private static Func<TaskCompletionSource<T>, CancellationToken, bool> CreateTrySetCanceledDelegate()
            {
                var trySetCanceledMethod = typeof(TaskCompletionSource<T>).GetMethod(
                    nameof(TrySetCanceled),
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    CallingConventions.Any,
                    new[] { typeof(CancellationToken) },
                    modifiers: null
                );

                if (trySetCanceledMethod != null)
                {
                    return (Func<TaskCompletionSource<T>, CancellationToken, bool>)Delegate.CreateDelegate(
                        typeof(Func<TaskCompletionSource<T>, CancellationToken, bool>),
                        trySetCanceledMethod
                    );
                }

                // net40 doesn't have CT overload
                // TODO: fallback to Task.TrySetCanceled

                // TODO: TRACE is not defined
                new TraceSource("Theraot.Core").TraceEvent(TraceEventType.Warning, 1,
                    "TaskCompletionSource<T>.TrySetCanceled(CancellationToken): fallback to overload without CancellationToken.");

                trySetCanceledMethod = typeof(TaskCompletionSource<T>).GetMethod(
                    nameof(TrySetCanceled),
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );
                if (trySetCanceledMethod == null)
                    throw new PlatformNotSupportedException("Method not found: TaskCompletionSource.TrySetCanceled");

                var trySetCanceled = (Func<TaskCompletionSource<T>, bool>)Delegate.CreateDelegate(
                    typeof(Func<TaskCompletionSource<T>, bool>), trySetCanceledMethod);

                return (tcs, ct) => trySetCanceled(tcs);
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