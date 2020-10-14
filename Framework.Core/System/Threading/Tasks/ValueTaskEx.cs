using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public static class ValueTaskEx
    {
        /// <summary>Creates a <see cref="ValueTask"/> that has completed due to cancellation with the specified cancellation token.</summary>
        /// <param name="cancellationToken">The cancellation token with which to complete the task.</param>
        /// <returns>The canceled task.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ValueTask FromCanceled(CancellationToken cancellationToken)
        {
#if NET50 || LESSTHAN_NET45
            return ValueTask.FromCanceled(cancellationToken);
#elif GREATERTHAN_NET45 || GREATERTHAN_NETSTANDARD12 || TARGETS_NETCORE
            return new ValueTask(Task.FromCanceled(cancellationToken));
#else
            return new ValueTask(TaskExEx.FromCanceled(cancellationToken));
#endif
        }

        /// <summary>Creates a <see cref="ValueTask{TResult}"/> that has completed due to cancellation with the specified cancellation token.</summary>
        /// <param name="cancellationToken">The cancellation token with which to complete the task.</param>
        /// <returns>The canceled task.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ValueTask<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
        {
#if NET50 || LESSTHAN_NET45
            return ValueTask.FromCanceled<TResult>(cancellationToken);
#elif GREATERTHAN_NET45 || GREATERTHAN_NETSTANDARD12 || TARGETS_NETCORE
            return new ValueTask<TResult>(Task.FromCanceled<TResult>(cancellationToken));
#else
            return new ValueTask<TResult>(TaskExEx.FromCanceled<TResult>(cancellationToken));
#endif
        }

        /// <summary>Creates a <see cref="ValueTask"/> that has completed with the specified exception.</summary>
        /// <param name="exception">The exception with which to complete the task.</param>
        /// <returns>The faulted task.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ValueTask FromException(Exception exception)
        {
#if NET50 || LESSTHAN_NET45
            return ValueTask.FromException(exception);
#elif GREATERTHAN_NET45 || GREATERTHAN_NETSTANDARD12 || TARGETS_NETCORE
            return new ValueTask(Task.FromException(exception));
#else
            return new ValueTask(TaskExEx.FromException(exception));
#endif
        }

        /// <summary>Creates a <see cref="ValueTask{TResult}"/> that has completed with the specified exception.</summary>
        /// <param name="exception">The exception with which to complete the task.</param>
        /// <returns>The faulted task.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ValueTask<TResult> FromException<TResult>(Exception exception)
        {
#if NET50 || LESSTHAN_NET45
            return ValueTask.FromException<TResult>(exception);
#elif GREATERTHAN_NET45 || GREATERTHAN_NETSTANDARD12 || TARGETS_NETCORE
            return new ValueTask<TResult>(Task.FromException<TResult>(exception));
#else
            return new ValueTask<TResult>(TaskExEx.FromException<TResult>(exception));
#endif
        }

        /// <summary>Creates a <see cref="ValueTask{TResult}"/> that's completed successfully with the specified result.</summary>
        /// <typeparam name="TResult">The type of the result returned by the task.</typeparam>
        /// <param name="result">The result to store into the completed task.</param>
        /// <returns>The successfully completed task.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ValueTask<TResult> FromResult<TResult>(TResult result)
        {
            return new ValueTask<TResult>(result);
        }
    }
}