#if LESSTHAN_NET40

#pragma warning disable RCS1191 // Declare enum value as combination of names.

namespace System.Threading.Tasks
{
    /// <summary>Specifies the behavior for a task that is created by using the <see cref="T:System.Threading.Tasks.Task.ContinueWith" /> or <see cref="T:System.Threading.Tasks.Task`1.ContinueWith" /> method.</summary>
    [Flags]
    [Serializable]
    public enum TaskContinuationOptions
    {
        /// <summary>Default = "Continue on any, no task options, run asynchronously" Specifies that the default behavior should be used. Continuations, by default, will be scheduled when the antecedent task completes, regardless of the task's final <see cref="T:System.Threading.Tasks.TaskStatus" />.</summary>
        None = 0,

        /// <summary>A hint to a <see cref="T:System.Threading.Tasks.TaskScheduler" /> to schedule a task in as fair a manner as possible, meaning that tasks scheduled sooner will be more likely to be run sooner, and tasks scheduled later will be more likely to be run later.</summary>
        PreferFairness = 1,

        /// <summary>Specifies that a task will be a long-running, course-grained operation. It provides a hint to the <see cref="T:System.Threading.Tasks.TaskScheduler" /> that oversubscription may be warranted.</summary>
        LongRunning = 2,

        /// <summary>Specifies that a task is attached to a parent in the task hierarchy.</summary>
        AttachedToParent = 4,

        DenyChildAttach = 8,
        HideScheduler = 16,
        LazyCancellation = 32,
        RunContinuationsAsynchronously = 64,

        /// <summary>Specifies that the continuation task should not be scheduled if its antecedent ran to completion. This option is not valid for multi-task continuations.</summary>
        NotOnRanToCompletion = 65536,

        /// <summary>Specifies that the continuation task should not be scheduled if its antecedent threw an unhandled exception. This option is not valid for multi-task continuations.</summary>
        NotOnFaulted = 131072,

        /// <summary>Specifies that the continuation task should be scheduled only if its antecedent was canceled.  This option is not valid for multi-task continuations.</summary>
        OnlyOnCanceled = 196608,

        /// <summary>Specifies that the continuation task should not be scheduled if its antecedent was canceled. This option is not valid for multi-task continuations.</summary>
        NotOnCanceled = 262144,

        /// <summary>Specifies that the continuation task should be scheduled only if its antecedent threw an unhandled exception. This option is not valid for multi-task continuations.</summary>
        OnlyOnFaulted = 327680,

        /// <summary>Specifies that the continuation task should be scheduled only if its antecedent ran to completion. This option is not valid for multi-task continuations.</summary>
        OnlyOnRanToCompletion = 393216,

        /// <summary>Specifies that the continuation task should be executed synchronously. With this option specified, the continuation will be run on the same thread that causes the antecedent task to transition into its final state. If the antecedent is already complete when the continuation is created, the continuation will run on the thread creating the continuation. Only very short-running continuations should be executed synchronously.</summary>
        ExecuteSynchronously = 524288
    }
}

#endif