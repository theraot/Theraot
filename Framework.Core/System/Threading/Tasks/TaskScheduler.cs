#if LESSTHAN_NET40

#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
#pragma warning disable RECS0083 // Shows NotImplementedException throws in the quick task bar

using System.Collections.Generic;
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
                if (currentTask != null)
                {
                    return currentTask.ExecutingTaskScheduler;
                }
                return Default;
            }
        }

        public static TaskScheduler Default { get; } = new ThreadPoolTaskScheduler();

        public int Id { get; }

        internal virtual bool RequiresAtomicStartTransition => true;

        public static TaskScheduler FromCurrentSynchronizationContext()
        {
            // TODO
            throw new NotImplementedException();
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

        protected abstract IEnumerable<Task> GetScheduledTasks();

        protected virtual bool TryDequeue(Task task)
        {
            Theraot.No.Op(task);
            return false;
        }

        protected bool TryExecuteTask(Task task)
        {
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