#if FAT
#if NET20 || NET30 || NET35

using System.Collections.Generic;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace System.Threading.Tasks
{
    public abstract class TaskScheduler
    {
        private static readonly TaskScheduler _default = new ThreadPoolTaskScheduler();

        private static int _lastId;

        private readonly int _id;

        protected TaskScheduler()
        {
            _id = Interlocked.Increment(ref _lastId) - 1;
        }

        public static TaskScheduler Default
        {
            get
            {
                return _default;
            }
        }

        public static TaskScheduler Current
        {
            get
            {
                var currentTask = Task.Current;
                if (currentTask != null)
                {
                    return currentTask.Scheduler;
                }
                return Default;
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

        public static TaskScheduler FromCurrentSynchronizationContext()
        {
            throw new NotImplementedException();
        }

        public Task AddWork(Action action)
        {
            return GCMonitor.FinalizingForUnload ? null : new Task(action, this);
        }

        internal void RunAndWait(Task task, bool taskWasPreviouslyQueued)
        {
            TryExecuteTaskInline(task, taskWasPreviouslyQueued);
        }

        protected internal abstract void QueueTask(Task task);

        protected internal virtual bool TryDequeue(Task task)
        {
            throw new NotImplementedException();
        }

        protected abstract IEnumerable<Task> GetScheduledTasks();

        protected bool TryExecuteTask(Task task)
        {
            throw new NotImplementedException();
        }

        protected abstract bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued);

        internal virtual void NotifyWorkItemProgress()
        {
            // Empty
        }

        internal virtual bool RequiresAtomicStartTransition
        {
            get
            {
                return true;
            }
        }
    }
}

#endif
#endif