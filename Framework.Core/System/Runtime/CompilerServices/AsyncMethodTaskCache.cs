#if LESSTHAN_NET45

using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides a cache for Boolean tasks.
    /// </summary>
    internal sealed class AsyncMethodBooleanTaskCache : AsyncMethodTaskCache<bool>
    {
        /// <summary>
        ///     A false task.
        /// </summary>
        private readonly TaskCompletionSource<bool> _false = CreateCompleted(false);

        /// <summary>
        ///     A true task.
        /// </summary>
        private readonly TaskCompletionSource<bool> _true = CreateCompleted(true);

        /// <inheritdoc />
        /// <summary>
        ///     Gets a cached task for the Boolean result.
        /// </summary>
        /// <param name="result">true or false</param>
        /// <returns>
        ///     A cached task for the Boolean result.
        /// </returns>
        internal override TaskCompletionSource<bool> FromResult(bool result)
        {
            return result ? _true : _false;
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides a cache for zero Int32 tasks.
    /// </summary>
    internal sealed class AsyncMethodInt32TaskCache : AsyncMethodTaskCache<int>
    {
        /// <summary>
        ///     The maximum value, exclusive, for which we want a cached task.
        /// </summary>
        private const int _maxInt32ValueExclusive = 9;

        /// <summary>
        ///     The minimum value, inclusive, for which we want a cached task.
        /// </summary>
        private const int _minInt32ValueInclusive = -1;

        /// <summary>
        ///     The cache of Task{Int32}.
        /// </summary>
        private static readonly TaskCompletionSource<int>[] _int32Tasks = CreateInt32Tasks();

        /// <inheritdoc />
        /// <summary>
        ///     Gets a cached task for the zero Int32 result.
        /// </summary>
        /// <param name="result">The integer value</param>
        /// <returns>
        ///     A cached task for the Int32 result or null if not cached.
        /// </returns>
        internal override TaskCompletionSource<int> FromResult(int result)
        {
            if (result < _minInt32ValueInclusive || result >= _maxInt32ValueExclusive)
            {
                return CreateCompleted(result);
            }

            return _int32Tasks[result - -1];
        }

        /// <summary>
        ///     Creates an array of cached tasks for the values in the range [INCLUSIVE_MIN,EXCLUSIVE_MAX).
        /// </summary>
        private static TaskCompletionSource<int>[] CreateInt32Tasks()
        {
            var completionSourceArray = new TaskCompletionSource<int>[10];
            for (var index = 0; index < completionSourceArray.Length; ++index)
            {
                completionSourceArray[index] = CreateCompleted(index - 1);
            }

            return completionSourceArray;
        }
    }

    /// <summary>
    ///     Provides a base class used to cache tasks of a specific return type.
    /// </summary>
    /// <typeparam name="TResult">Specifies the type of results the cached tasks return.</typeparam>
    internal class AsyncMethodTaskCache<TResult>
    {
        /// <summary>
        ///     A singleton cache for this result type.
        ///     This may be null if there are no cached tasks for this TResult.
        /// </summary>
        internal static readonly AsyncMethodTaskCache<TResult> Singleton = CreateCache();

        static AsyncMethodTaskCache()
        {
            // Empty
        }

        /// <summary>
        ///     Creates a non-disposable task.
        /// </summary>
        /// <param name="result">The result for the task.</param>
        /// <returns>
        ///     The cacheable task.
        /// </returns>
        internal static TaskCompletionSource<TResult> CreateCompleted(TResult result)
        {
            var completionSource = new TaskCompletionSource<TResult>();
            completionSource.TrySetResult(result);
            return completionSource;
        }

        /// <summary>
        ///     Gets a cached task if one exists.
        /// </summary>
        /// <param name="result">The result for which we want a cached task.</param>
        /// <returns>
        ///     A cached task if one exists; otherwise, null.
        /// </returns>
        internal virtual TaskCompletionSource<TResult> FromResult(TResult result)
        {
            return CreateCompleted(result);
        }

        /// <summary>
        ///     Creates a cache.
        /// </summary>
        /// <returns>
        ///     A task cache for this result type.
        /// </returns>
        private static AsyncMethodTaskCache<TResult> CreateCache()
        {
            var type = typeof(TResult);
            if (type == typeof(bool))
            {
                return (AsyncMethodTaskCache<TResult>)(object)new AsyncMethodBooleanTaskCache();
            }

            if (type == typeof(int))
            {
                return (AsyncMethodTaskCache<TResult>)(object)new AsyncMethodInt32TaskCache();
            }

            return null;
        }
    }
}

#endif