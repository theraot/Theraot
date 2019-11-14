using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public static partial class TaskExEx
    {
        /// <summary>
        ///     Creates an already completed <see cref="System.Threading.Tasks.Task{TResult}" /> from the specified result.
        /// </summary>
        /// <param name="result">The result from which to create the completed task.</param>
        /// <returns>
        ///     The completed task.
        /// </returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> FromResult<TResult>(TResult result)
        {
#if NET40
            var completionSource = new TaskCompletionSource<TResult>();
            completionSource.TrySetResult(result);
            return completionSource.Task;
#else
            // Missing in .NET 4.0
            return Task.FromResult(result);
#endif
        }
    }
}