#if LESSTHAN_NET40

namespace System.Threading.Tasks
{
    [Flags]
    [Serializable]
    public enum TaskCreationOptions
    {
        /// <summary>
        /// Specifies that the default behavior should be used.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// A hint to a <see cref="TaskScheduler">TaskScheduler</see> to schedule a
        /// task in as fair a manner as possible, meaning that tasks scheduled sooner will be more likely to
        /// be run sooner, and tasks scheduled later will be more likely to be run later.
        /// </summary>
        PreferFairness = 0x01,

        /// <summary>
        /// Specifies that a task will be a long-running, course-grained operation. It provides a hint to the
        /// <see cref="TaskScheduler">TaskScheduler</see> that oversubscription may be
        /// warranted.
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

        // 0x20 is already being used in TaskContinuationOptions

        /// <summary>
        /// Forces continuations added to the current task to be executed asynchronously.
        /// This option has precedence over TaskContinuationOptions.ExecuteSynchronously
        /// </summary>
        RunContinuationsAsynchronously = 0x40
    }
}

#endif