using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonoTests.System.Threading.Tasks
{
    internal class NonInlineableScheduler : TaskScheduler
    {
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        protected override void QueueTask(Task task)
        {
            if (!TryExecuteTask(task))
            {
                throw new ApplicationException();
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
    }
}