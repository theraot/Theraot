#if NET20 || NET30 || NET35

using System.Collections.Generic;
using Theraot.Core;

namespace System.Threading.Tasks
{
    public abstract partial class TaskScheduler
    {
        private static readonly TaskScheduler _default = new ThreadPoolTaskScheduler();
        private static int _lastId;
        private readonly int _id;

        protected TaskScheduler()
        {
            _id = Interlocked.Increment(ref _lastId) - 1;
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

        public static TaskScheduler Default
        {
            get
            {
                return _default;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public int MaximunConcurrencyLevel
        {
            get
            {
                return int.MaxValue;
            }
        }

        internal virtual bool RequiresAtomicStartTransition
        {
            get
            {
                return true;
            }
        }

        public static TaskScheduler FromCurrentSynchronizationContext()
        {
            // TODO
            throw new NotImplementedException();
        }

        internal bool InernalTryDequeue(Task task, ref bool special)
        {
            try
            {
                return TryDequeue(task);
            }
            catch (Exception exception)
            {
                if (exception is InternalSpecialCancelException)
                {
                    // Special path for ThreadPool
                    special = true;
                    return false;
                }
                if (exception is ThreadAbortException)
                {
                    return false;
                }
                throw new TaskSchedulerException(exception);
            }
        }

        internal bool InernalTryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return TryExecuteTaskInline(task, taskWasPreviouslyQueued);
        }

        protected internal abstract void QueueTask(Task task);

        protected abstract IEnumerable<Task> GetScheduledTasks();

        protected virtual bool TryDequeue(Task task)
        {
            GC.KeepAlive(task);
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