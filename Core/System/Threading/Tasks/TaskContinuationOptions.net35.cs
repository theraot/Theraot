#if NET35

// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

namespace System.Threading.Tasks
{
    /// <summary>
    /// Specifies flags that control optional behavior for the creation and execution of continuation tasks.
    /// </summary>
    [Flags]
    [Serializable]
    public enum TaskContinuationOptions
    {
        /// <summary>
        /// Default = "Continue on any, no task options, run asynchronously"
        /// Specifies that the default behavior should be used.  Continuations, by default, will
        /// be scheduled when the antecedent task completes, regardless of the task's final <see
        /// cref="System.Threading.Tasks.TaskStatus">TaskStatus</see>.
        /// </summary>
        None = 0,

        // These are identical to their meanings and values in TaskCreationOptions

        /// <summary>
        /// A hint to a <see cref="System.Threading.Tasks.TaskScheduler">TaskScheduler</see> to schedule a
        /// task in as fair a manner as possible, meaning that tasks scheduled sooner will be more likely to
        /// be run sooner, and tasks scheduled later will be more likely to be run later.
        /// </summary>
        PreferFairness = 0x01,

        /// <summary>
        /// Specifies that a task will be a long-running, course-grained operation.  It provides
        /// a hint to the <see cref="System.Threading.Tasks.TaskScheduler">TaskScheduler</see> that
        /// oversubscription may be warranted.
        /// </summary>
        LongRunning = 0x02,
        /// <summary>
        /// Specifies that a task is attached to a parent in the task hierarchy.
        /// </summary>
        AttachedToParent = 0x04,

        /// <summary>
        /// Specifies that an InvalidOperationException will be thrown if an attempt is made to attach a child task to the created task.
        /// </summary>
        DenyChildAttach = 0x08,
        /// <summary>
        /// Prevents the ambient scheduler from being seen as the current scheduler in the created task.  This means that operations
        /// like StartNew or ContinueWith that are performed in the created task will see TaskScheduler.Default as the current scheduler.
        /// </summary>
        HideScheduler = 0x10,

        /// <summary>
        /// In the case of continuation cancellation, prevents completion of the continuation until the antecedent has completed.
        /// </summary>
        LazyCancellation = 0x20,

        /// <summary>
        /// Forces continuations added to the current task to be executed asynchronously.
        /// This option has precedence over TaskContinuationOptions.ExecuteSynchronously
        /// </summary>
        RunContinuationsAsynchronously = 0x40,

        // These are specific to continuations

        /// <summary>
        /// Specifies that the continuation task should not be scheduled if its antecedent ran to completion.
        /// This option is not valid for multi-task continuations.
        /// </summary>
        NotOnRanToCompletion = 0x10000,
        /// <summary>
        /// Specifies that the continuation task should not be scheduled if its antecedent threw an unhandled
        /// exception. This option is not valid for multi-task continuations.
        /// </summary>
        NotOnFaulted = 0x20000,
        /// <summary>
        /// Specifies that the continuation task should not be scheduled if its antecedent was canceled. This
        /// option is not valid for multi-task continuations.
        /// </summary>
        NotOnCanceled = 0x40000,
        /// <summary>
        /// Specifies that the continuation task should be scheduled only if its antecedent ran to
        /// completion. This option is not valid for multi-task continuations.
        /// </summary>
        OnlyOnRanToCompletion = NotOnFaulted | NotOnCanceled,
        /// <summary>
        /// Specifies that the continuation task should be scheduled only if its antecedent threw an
        /// unhandled exception. This option is not valid for multi-task continuations.
        /// </summary>
        OnlyOnFaulted = NotOnRanToCompletion | NotOnCanceled,
        /// <summary>
        /// Specifies that the continuation task should be scheduled only if its antecedent was canceled.
        /// This option is not valid for multi-task continuations.
        /// </summary>
        OnlyOnCanceled = NotOnRanToCompletion | NotOnFaulted,
        /// <summary>
        /// Specifies that the continuation task should be executed synchronously. With this option
        /// specified, the continuation will be run on the same thread that causes the antecedent task to
        /// transition into its final state. If the antecedent is already complete when the continuation is
        /// created, the continuation will run on the thread creating the continuation.  Only very
        /// short-running continuations should be executed synchronously.
        /// </summary>
        ExecuteSynchronously = 0x80000
    }

}

#endif