#if LESSTHAN_NET40

#pragma warning disable CA1717 // Only FlagsAttribute enums should have plural names

namespace System.Threading.Tasks
{
    /// <summary>
    ///     Represents the current stage in the lifecycle of a <see cref="Task" />.
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        ///     The task has been initialized but has not yet been scheduled.
        /// </summary>
        Created = 0,

        /// <summary>
        ///     The task is waiting to be activated and scheduled internally by the .NET Framework infrastructure.
        /// </summary>
        WaitingForActivation = 1,

        /// <summary>
        ///     The task has been scheduled for execution but has not yet begun executing.
        /// </summary>
        WaitingToRun = 2,

        /// <summary>
        ///     The task is running but has not yet completed.
        /// </summary>
        Running = 3,

        /// <summary>
        ///     The task has finished executing and is implicitly waiting for
        ///     attached child tasks to complete.
        /// </summary>
        WaitingForChildrenToComplete = 4,

        /// <summary>
        ///     The task completed execution successfully.
        /// </summary>
        RanToCompletion = 5,

        /// <summary>
        ///     The task acknowledged cancellation by throwing an OperationCanceledException with its own CancellationToken
        ///     while the token was in signaled state, or the task's CancellationToken was already signaled before the
        ///     task started executing.
        /// </summary>
        Canceled = 6,

        /// <summary>
        ///     The task completed due to an unhandled exception.
        /// </summary>
        Faulted = 7
    }
}

#endif