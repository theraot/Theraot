#if LESSTHAN_NET40

using System.Diagnostics.Contracts;

namespace System.Threading.Tasks
{
    internal abstract class TaskContinuation
    {
        internal abstract void Run(Task completedTask, bool canInlineContinuationTask);

        protected static void InlineIfPossibleOrElseQueue(Task task)
        {
            var scheduler = task.ExecutingTaskScheduler;
            if (scheduler == null)
            {
                Contract.Assert(condition: false);
                throw new InvalidOperationException();
            }

            task.Start(task.ExecutingTaskScheduler, inline: true, throwSchedulerExceptions: false);
        }
    }
}

#endif