#if NET20 || NET30 || NET35

namespace System.Threading.Tasks
{
    /// <summary>
    /// Task creation flags which are only used internally.
    /// </summary>
    [Flags]
    [Serializable]
    internal enum InternalTaskOptions
    {
        /// <summary> Specifies "No internal task options" </summary>
        None,

        /// <summary>Used to filter out internal vs. public task creation options.</summary>
        InternalOptionsMask = 0x0000FF00,

        ChildReplica = 0x0100,
        ContinuationTask = 0x0200,
        PromiseTask = 0x0400,
        SelfReplicating = 0x0800,

        /// <summary>
        /// Store the presence of TaskContinuationOptions.LazyCancellation, since it does not directly
        /// translate into any TaskCreationOptions.
        /// </summary>
        LazyCancellation = 0x1000,

        /// <summary>Specifies that the task will be queued by the runtime before handing it over to the user.
        /// This flag will be used to skip the CancellationToken registration step, which is only meant for unstarted tasks.</summary>
        QueuedByRuntime = 0x2000,

        /// <summary>
        /// Denotes that Dispose should be a complete nop for a Task.  Used when constructing tasks that are meant to be cached/reused.
        /// </summary>
        DoNotDispose = 0x4000
    }
}

#endif