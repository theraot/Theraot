#if LESSTHAN_NET40

using System.Collections.Generic;

namespace System.Threading.Tasks
{
    internal class SynchronizationContextTaskScheduler : TaskScheduler
    {
        internal SynchronizationContextTaskScheduler()
        {
            Context = SynchronizationContext.Current ?? throw new InvalidOperationException("The current SynchronizationContext may not be used as a TaskScheduler.");
        }

        public SynchronizationContext Context { get; }

        protected internal override void QueueTask(Task task)
        {
            Context.Post(Callback, task);
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return ArrayEx.Empty<Task>();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return SynchronizationContext.Current == Context && TryExecuteTask(task);
        }

        private static void Callback(object state)
        {
            ((Task)state).ExecuteEntry(true);
        }
    }
}

#endif