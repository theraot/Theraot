#if LESSTHAN_NET45 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
#pragma warning disable RCS1231 // Make parameter ref read-only.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if NET40

using System.Linq;

#endif

namespace System.Threading.Tasks
{
    public static partial class TaskEx
    {
        /// <summary>
        ///     Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        ///     A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <remarks>
        ///     If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///     about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///     Task will also be canceled.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task WhenAll(params Task[] tasks)
        {
            return WhenAll((IEnumerable<Task>)tasks);
        }

        /// <summary>
        ///     Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        ///     A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <remarks>
        ///     If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///     about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///     Task will also be canceled.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
        {
            return WhenAll((IEnumerable<Task<TResult>>)tasks);
        }

        /// <summary>
        ///     Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        ///     A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <remarks>
        ///     If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///     about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///     Task will also be canceled.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task WhenAll(IEnumerable<Task> tasks)
        {
#if NET40
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }
            return WhenAllCore(tasks, (Action<Task[], TaskCompletionSource<object>>)((_, tcs) => tcs.TrySetResult(null!)));
#else
            // Missing in .NET 4.0
            return Task.WhenAll(tasks);
#endif
        }

        /// <summary>
        ///     Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        ///     A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <remarks>
        ///     If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///     about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///     Task will also be canceled.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
        {
#if NET40
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }
            return WhenAllCore<TResult[]>(tasks, (completedTasks, tcs) =>
                                                 tcs.TrySetResult(completedTasks
                                                                      .Cast<Task<TResult>>()
                                                                      .Select(t => t.Result)
                                                                      .ToArray()));
#else
            // Missing in .NET 4.0
            return Task.WhenAll(tasks);
#endif
        }
    }

#if NET40
    public static partial class TaskEx
    {
        private const string _argumentOutOfRangeTimeoutNonNegativeOrMinusOne = "The timeout must be non-negative or -1, and it must be less than or equal to Int32.MaxValue.";

        /// <summary>
        /// Adds the target exception to the list, initializing the list if it's null.
        /// </summary>
        /// <param name="targetList">The list to which to add the exception and initialize if the list is null.</param><param name="exception">The exception to add, and unwrap if it's an aggregate.</param>
        private static void AddPotentiallyUnwrappedExceptions(ref List<Exception>? targetList, Exception exception)
        {
            if (targetList == null)
            {
                targetList = new List<Exception>();
            }

            if (exception is AggregateException aggregateException)
            {
                targetList.Add(aggregateException.InnerExceptions.Count == 1 ? exception.InnerException : exception);
            }
            else
            {
                targetList.Add(exception);
            }
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param><param name="setResultAction">A callback invoked when all of the tasks complete successfully in the RanToCompletion state.
        ///             This callback is responsible for storing the results into the TaskCompletionSource.
        ///             </param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        private static Task<TResult> WhenAllCore<TResult>(IEnumerable<Task> tasks, Action<Task[], TaskCompletionSource<TResult>> setResultAction)
        {
            var tcs = new TaskCompletionSource<TResult>();
            var taskArray = tasks as Task[] ?? tasks.ToArray();
            if (taskArray.Length == 0)
            {
                setResultAction!(taskArray, tcs);
            }
            else
            {
                Task.Factory.ContinueWhenAll(taskArray, completedTasks =>
                {
                    List<Exception>? exceptions = null;
                    var canceled = false;
                    foreach (var task in completedTasks)
                    {
                        if (task.IsFaulted)
                        {
                            AddPotentiallyUnwrappedExceptions(ref exceptions, task.Exception);
                        }
                        else
                        {
                            canceled |= task.IsCanceled;
                        }
                    }
                    if (exceptions?.Count > 0)
                    {
                        tcs.TrySetException(exceptions);
                    }
                    else if (canceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        setResultAction!(completedTasks, tcs);
                    }
                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }

            return tcs.Task;
        }
    }
#endif
}

#endif