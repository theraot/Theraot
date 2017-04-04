using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonoTests.System.Threading.Tasks
{
    internal class MockScheduler : TaskScheduler
    {
        public event Action<Task, bool> TryExecuteTaskInlineHandler;

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        protected override void QueueTask(Task task)
        {
            return;
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (TryExecuteTaskInlineHandler != null)
            {
                TryExecuteTaskInlineHandler(task, taskWasPreviouslyQueued);
            }

            return base.TryExecuteTask(task);
        }
    }
}