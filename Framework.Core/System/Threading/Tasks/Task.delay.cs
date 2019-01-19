#if LESSTHAN_NET40

#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.

using System.Diagnostics.Contracts;
using Theraot.Threading;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        /// <summary>Gets a task that's already been completed successfully.</summary>
        /// <remarks>May not always return the same instance.</remarks>
        public static Task CompletedTask => TaskEx.CompletedTask;

        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="delay">The time span to wait before completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="delay"/> is less than -1 or greater than Int32.MaxValue.
        /// </exception>
        /// <remarks>
        /// After the specified time delay, the Task is completed in RanToCompletion state.
        /// </remarks>
        public static Task Delay(TimeSpan delay)
        {
            return Delay(delay, default);
        }

        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="delay">The time span to wait before completing the returned Task</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="delay"/> is less than -1 or greater than Int32.MaxValue.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The provided <paramref name="cancellationToken"/> has already been disposed.
        /// </exception>
        /// <remarks>
        /// If the cancellation token is signaled before the specified time delay, then the Task is completed in
        /// Canceled state.  Otherwise, the Task is completed in RanToCompletion state once the specified time
        /// delay has expired.
        /// </remarks>
        public static Task Delay(TimeSpan delay, CancellationToken cancellationToken)
        {
            var milliseconds = (long)delay.TotalMilliseconds;
            if (milliseconds < -1 || milliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(delay), "The value needs to translate in milliseconds to - 1(signifying an infinite timeout), 0 or a positive integer less than or equal to Int32.MaxValue");
            }

            return Delay((int)milliseconds, cancellationToken);
        }

        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="millisecondsDelay"/> is less than -1.
        /// </exception>
        /// <remarks>
        /// After the specified time delay, the Task is completed in RanToCompletion state.
        /// </remarks>
        public static Task Delay(int millisecondsDelay)
        {
            return Delay(millisecondsDelay, default);
        }

        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned Task</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="millisecondsDelay"/> is less than -1.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The provided <paramref name="cancellationToken"/> has already been disposed.
        /// </exception>
        /// <remarks>
        /// If the cancellation token is signaled before the specified time delay, then the Task is completed in
        /// Canceled state.  Otherwise, the Task is completed in RanToCompletion state once the specified time
        /// delay has expired.
        /// </remarks>
        public static Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
        {
            // Throw on non-sensical time
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer.");
            }
            Contract.EndContractBlock();
            // some short-cuts in case quick completion is in order
            if (cancellationToken.IsCancellationRequested)
            {
                // return a Task created as already-Canceled
                return TaskEx.FromCanceled(cancellationToken);
            }
            if (millisecondsDelay == 0)
            {
                // return a Task created as already-RanToCompletion
                return TaskEx.CompletedTask;
            }
            var source = new TaskCompletionSource<bool>();
            if (millisecondsDelay > 0)
            {
                var timeout = RootedTimeout.Launch
                    (
                        () =>
                        {
                            try
                            {
                                source.SetResult(true);
                            }
                            catch (InvalidOperationException exception)
                            {
                                // Already canceled
                                Theraot.No.Op(exception);
                            }
                        },
                        millisecondsDelay,
                        cancellationToken
                    );
                source.Task.SetPromiseCheck(() => timeout.CheckRemaining());
            }
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register
                    (
                        () =>
                        {
                            try
                            {
                                source.SetCanceled();
                            }
                            catch (InvalidOperationException exception)
                            {
                                // Already timed out
                                Theraot.No.Op(exception);
                            }
                        }
                    );
            }
            return source.Task;
        }
    }
}

#endif