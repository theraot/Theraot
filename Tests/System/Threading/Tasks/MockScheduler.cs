#pragma warning disable RCS1079 // Throwing of new NotImplementedException

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
            Theraot.No.Op(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            TryExecuteTaskInlineHandler?.Invoke(task, taskWasPreviouslyQueued);

            return TryExecuteTask(task);
        }
    }
}