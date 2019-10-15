#if LESSTHAN_NET40

using System.Collections.Generic;
using Theraot;
using Theraot.Core;

namespace System.Threading.Tasks
{
    public abstract partial class TaskScheduler
    {
        private static int _lastId;

        protected TaskScheduler()
        {
            Id = Interlocked.Increment(ref _lastId) - 1;
        }

        public static TaskScheduler Current
        {
            get
            {
                var currentTask = Task.InternalCurrent;
                return currentTask != null ? currentTask.ExecutingTaskScheduler : Default;
            }
        }

        public static TaskScheduler Default { get; } = new ThreadPoolTaskScheduler();

        public int Id { get; }

        internal virtual bool RequiresAtomicStartTransition => true;

        public static TaskScheduler FromCurrentSynchronizationContext()
        {
            return new SynchronizationContextTaskScheduler();
        }

        internal bool InternalTryDequeue(Task task, ref bool special)
        {
            try
            {
                return TryDequeue(task);
            }
            catch (InternalSpecialCancelException)
            {
                // Special path for ThreadPool
                special = true;
                return false;
            }
            catch (ThreadAbortException)
            {
                return false;
            }
            catch (Exception exception)
            {
                throw new TaskSchedulerException(exception);
            }
        }

        internal bool InternalTryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTaskInline(task, taskWasPreviouslyQueued);
        }

        protected internal abstract void QueueTask(Task task);

        protected abstract IEnumerable<Task>? GetScheduledTasks();

        protected virtual bool TryDequeue(Task task)
        {
            No.Op(task);
            return false;
        }

        protected bool TryExecuteTask(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            if (task.ExecutingTaskScheduler != this)
            {
                throw new InvalidOperationException("Wrong Task Scheduler");
            }

            return task.ExecuteEntry(true);
        }

        protected abstract bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued);
    }
}

#endif