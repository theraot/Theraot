using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonoTests.System.Threading.Tasks
{
    internal class ExceptionScheduler : TaskScheduler
    {
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new ApplicationException("1");
        }

        protected override void QueueTask(Task task)
        {
            throw new ApplicationException("2");
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            throw new ApplicationException("3");
        }
    }
}