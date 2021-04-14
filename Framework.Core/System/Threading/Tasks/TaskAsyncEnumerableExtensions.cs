#if LESSTHAN_NET47 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    /// <summary>Provides a set of static methods for configuring <see cref="Task" />-related behaviors on asynchronous enumerables and disposables.</summary>
    public static class TaskAsyncEnumerableExtensions
    {
        /// <summary>Configures how awaits on the tasks returned from an async disposable will be performed.</summary>
        /// <param name="source">The source async disposable.</param>
        /// <param name="continueOnCapturedContext">Whether to capture and marshal back to the current context.</param>
        /// <returns>The configured async disposable.</returns>
        public static ConfiguredAsyncDisposable ConfigureAwait(this IAsyncDisposable source, bool continueOnCapturedContext)
        {
            return new ConfiguredAsyncDisposable(source, continueOnCapturedContext);
        }

        /// <summary>Configures how awaits on the tasks returned from an async iteration will be performed.</summary>
        /// <typeparam name="T">The type of the objects being iterated.</typeparam>
        /// <param name="source">The source enumerable being iterated.</param>
        /// <param name="continueOnCapturedContext">Whether to capture and marshal back to the current context.</param>
        /// <returns>The configured enumerable.</returns>
        public static ConfiguredCancelableAsyncEnumerable<T> ConfigureAwait<T>(this IAsyncEnumerable<T> source, bool continueOnCapturedContext)
        {
            return new ConfiguredCancelableAsyncEnumerable<T>(source, continueOnCapturedContext, new CancellationToken());
        }

        /// <summary>Sets the <see cref="CancellationToken" /> to be passed to <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator" /> when iterating.</summary>
        /// <typeparam name="T">The type of the objects being iterated.</typeparam>
        /// <param name="source">The source enumerable being iterated.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> to use.</param>
        /// <returns>The configured enumerable.</returns>
        public static ConfiguredCancelableAsyncEnumerable<T> WithCancellation<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            return new ConfiguredCancelableAsyncEnumerable<T>(source, continueOnCapturedContext: true, cancellationToken);
        }
    }
}

#endif