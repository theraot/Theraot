#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;
using Theraot;

namespace System.Threading.Tasks
{
    internal interface ITaskCompletionAction
    {
        void Invoke(Task completingTask);
    }

    public partial class Task
    {
        internal sealed class CompleteOnInvokePromise : Task<Task>, ITaskCompletionAction
        {
            private int _firstTaskAlreadyCompleted;
            private ICollection<Task> _tasks; // must track this for cleanup

            public CompleteOnInvokePromise(ICollection<Task> tasks)
            {
                Contract.Requires(tasks != null, "Expected non-null collection of tasks");
                Contract.Requires(tasks.Count > 0, "Expected a non-zero length task array");
                _tasks = tasks;
            }

            public void Invoke(Task completingTask)
            {
                if (Interlocked.CompareExchange(ref _firstTaskAlreadyCompleted, 1, 0) == 0)
                {
                    var success = TrySetResult(completingTask);
                    Contract.Assert(success, "Only one task should have gotten to this point, and thus this must be successful.");

                    // We need to remove continuations that may be left straggling on other tasks.
                    // Otherwise, repeated calls to WhenAny using the same task could leak actions.
                    // This may also help to avoided unnecessary invocations of this whenComplete delegate.
                    // Note that we may be attempting to remove a continuation from a task that hasn't had it
                    // added yet; while there's overhead there, the operation won't hurt anything.
                    foreach (var task in _tasks)
                    {
                        // if an element was erroneously nulled out concurrently, just skip it; worst case is we don't remove a continuation
                        if (task != null && !task.IsCompleted)
                        {
                            task.RemoveContinuation(this);
                        }
                    }
                    _tasks = null;
                }
            }
        }

        private sealed class WhenAllCore : ITaskCompletionAction, IDisposable
        {
            private int _count;
            private Action _done;
            private int _ready;
            private Task[] _tasks;

            internal WhenAllCore(ICollection<Task> tasks, Action done)
            {
                Contract.Requires(tasks != null, "Expected non-null collection of tasks");
                Contract.Requires(tasks.Count > 0, "Expected a non-zero length task array");
                _done = done;
                _tasks = new Task[tasks.Count];
                // Making sure _done and _tasks are set before we start adding tasks
                Thread.MemoryBarrier();
                // Add all tasks (this should increment _count, and add continuations)
                foreach (var task in tasks)
                {
                    AddTask(task);
                }
                // Report we finished adding all tasks
                Thread.VolatileWrite(ref _ready, 1);
                CheckCount();
            }

            public bool IsDone
            {
                get { return Interlocked.CompareExchange(ref _done, null, null) == null; }
            }

            public void Dispose()
            {
                // Get and erase the tasks
                var tasks = Interlocked.Exchange(ref _tasks, null);
                // If there are no tasks there is nothing to do
                if (tasks == null)
                {
                    return;
                }
                // Figure out if all tasks has completed
                // And remove continuations from those that have not completed
                var complete = true;
                foreach (var task in tasks)
                {
                    if (task == null)
                    {
                        continue;
                    }
                    if (task.IsCompleted)
                    {
                        continue;
                    }
                    task.RemoveContinuation(this);
                    complete = false;
                }
                // If they have, call done
                if (complete)
                {
                    Done();
                }
            }

            public void Invoke(Task completingTask)
            {
                // Continuations call here
                // Get the tasks
                var tasks = Interlocked.CompareExchange(ref _tasks, null, null);
                // If there are no tasks (Disposed) there is nothing to do
                if (tasks == null)
                {
                    return;
                }
                // Find the completing task and set it to null
                // If we do not find it, it measn the continuation executed before the task was added
                // Do not use IndexOf
                for (var index = 0; index < tasks.Length; index++)
                {
                    ref var current = ref tasks[index];
                    if (current == completingTask)
                    {
                        current = null;
                        break;
                    }
                }
                // Decrement count
                Interlocked.Decrement(ref _count);
                CheckCount();
            }

            private void CheckCount()
            {
                var count = Thread.VolatileRead(ref _count);
                // If count reached zero and we have finished adding all tasks, call done
                if (count == 0 && Thread.VolatileRead(ref _ready) == 1)
                {
                    Done();
                }
            }

            private void AddTask(Task awaitedTask)
            {
                Contract.Requires(Thread.VolatileRead(ref _ready) == 0);
                // Get the tasks
                var tasks = Interlocked.CompareExchange(ref _tasks, null, null);
                // If there are no tasks (Disposed) there is nothing to do
                if (tasks == null)
                {
                    return;
                }
                // Only add tasks taht has not completed
                if (awaitedTask.Status == TaskStatus.RanToCompletion)
                {
                    return;
                }
                // Preemptively increment _count
                // So that is has already been incremented when the continuation runs
                Interlocked.Increment(ref _count);
                // Add the continuation
                if (awaitedTask.AddTaskContinuation(this, /*addBeforeOthers:*/ true))
                {
                    // Find a spot in the tasks
                    var index = Array.IndexOf(tasks, null);
                    // Try to add the task
                    while (Interlocked.CompareExchange(ref tasks[index], awaitedTask, null) != null)
                    {
                        index = (index + 1) % tasks.Length;
                    }
                    // Check again if the task has completed, it may have completed while we were adding it
                    if (awaitedTask.Status == TaskStatus.RanToCompletion)
                    {
                        // Perhaps it did complete:
                        // - Before adding the continuation -> nothing to do
                        // - After adding the continuation before adding the task -> We have an orphan task to remove
                        // - After adding the continuation and the task -> nothing to do
                        // Remove the orphan task if it is there
                        Interlocked.CompareExchange(ref tasks[index], null, awaitedTask);
                        // Note: we let the continuation decrement _count
                    }
                }
                else
                {
                    // We failed to add the continuation
                    // Decrement the _count
                    Interlocked.Decrement(ref _count);
                    CheckCount();
                }
            }

            private void Done()
            {
                // Get and erase done
                var done = Interlocked.Exchange(ref _done, null);
                // If if was there, call it
                if (done != null)
                {
                    done();
                }
            }
        }

        // A Task<VoidTaskResult> that gets completed when all of its constituent tasks complete.
        // Completion logic will analyze the antecedents in order to choose completion status.
        // This type allows us to replace this logic:
        //      Task<VoidTaskResult> promise = new Task<VoidTaskResult>(...);
        //      Action<Task> completionAction = delegate { <completion logic>};
        //      TaskFactory.CommonCWAllLogic(tasksCopy).AddCompletionAction(completionAction);
        //      return promise;
        // which involves several allocations, with this logic:
        //      return new WhenAllPromise(tasksCopy);
        // which saves a couple of allocations and enables debugger notification specialization.
        //
        // Used in InternalWhenAll(Task[])
        private sealed class WhenAllPromise : Task<VoidStruct>, ITaskCompletionAction
        {
            private readonly Task[] _tasks;
            private int _count;
            private int _done;
            private int _ready;

            internal WhenAllPromise(Task[] tasks)
            {
                Contract.Requires(tasks != null, "Expected a non-null task array");
                Contract.Requires(tasks.Length > 0, "Expected a non-zero length task array");
                _tasks = tasks;
                foreach (var task in _tasks)
                {
                    AddTask(task);
                }
                Ready();
            }

            /// <summary>
            /// Returns whether we should notify the debugger of a wait completion.  This returns
            /// true iff at least one constituent task has its bit set.
            /// </summary>
            internal override bool ShouldNotifyDebuggerOfWaitCompletion
            {
                get { return base.ShouldNotifyDebuggerOfWaitCompletion && AnyTaskRequiresNotifyDebuggerOfWaitCompletion(_tasks); }
            }

            public void Invoke(Task completingTask)
            {
                var count = Interlocked.Decrement(ref _count);
                if (count == 0)
                {
                    if (Thread.VolatileRead(ref _ready) == 1)
                    {
                        Done();
                    }
                }
            }

            private void AddTask(Task awaitedTask)
            {
                Contract.Requires(Thread.VolatileRead(ref _ready) == 0);
                Interlocked.Increment(ref _count);
                if (!awaitedTask.AddTaskContinuation(this, /*addBeforeOthers:*/ true))
                {
                    Interlocked.Decrement(ref _count);
                }
            }

            private void Done()
            {
                var done = Interlocked.Exchange(ref _done, 1);
                if (done == 0)
                {
                    PrivateDone();
                }
            }

            private void PrivateDone()
            {
                // Set up some accounting variables
                List<ExceptionDispatchInfo> observedExceptions = null;
                Task canceledTask = null;
                // Loop through antecedents:
                //   If any one of them faults, the result will be faulted
                //   If none fault, but at least one is canceled, the result will be canceled
                //   If none fault or are canceled, then result will be RanToCompletion
                for (var index = 0; index < _tasks.Length; index++)
                {
                    var task = _tasks[index];
                    if (task == null)
                    {
                        Contract.Assert(false, "Constituent task in WhenAll should never be null");
                        throw new InvalidOperationException("Constituent task in WhenAll should never be null");
                    }
                    if (task.IsFaulted)
                    {
                        if (observedExceptions == null)
                        {
                            observedExceptions = new List<ExceptionDispatchInfo>();
                        }
                        observedExceptions.AddRange(task._exceptionsHolder.GetExceptionDispatchInfos());
                    }
                    else if (task.IsCanceled)
                    {
                        if (canceledTask == null)
                        {
                            canceledTask = task; // use the first task that's canceled
                        }
                    }
                    // Regardless of completion state, if the task has its debug bit set, transfer it to the
                    // WhenAll task.  We must do this before we complete the task.
                    if (task.IsWaitNotificationEnabled)
                    {
                        SetNotificationForWaitCompletion(/*enabled:*/ true);
                    }
                    else
                    {
                        _tasks[index] = null; // avoid holding onto tasks unnecessarily
                    }
                }
                if (observedExceptions != null)
                {
                    Contract.Assert(observedExceptions.Count > 0, "Expected at least one exception");
                    //We don't need to TraceOperationCompleted here because TrySetException will call Finish and we'll log it there
                    TrySetException(observedExceptions);
                }
                else if (canceledTask != null)
                {
                    TrySetCanceledPromise(canceledTask.CancellationToken);
                }
                else
                {
                    TrySetResult(default(VoidStruct));
                }
            }

            private void Ready()
            {
                Thread.VolatileWrite(ref _ready, 1);
                if (Thread.VolatileRead(ref _count) == 0)
                {
                    Done();
                }
            }
        }

        // A Task<T> that gets completed when all of its constituent tasks complete.
        // Completion logic will analyze the antecedents in order to choose completion status.
        // See comments for non-generic version of WhenAllPromise class.
        //
        // Used in InternalWhenAll<TResult>(Task<TResult>[])
        private sealed class WhenAllPromise<T> : Task<T[]>, ITaskCompletionAction
        {
            private readonly Task<T>[] _tasks;
            private int _count;
            private int _done;
            private int _ready;

            internal WhenAllPromise(Task<T>[] tasks)
            {
                Contract.Requires(tasks != null, "Expected a non-null task array");
                Contract.Requires(tasks.Length > 0, "Expected a non-zero length task array");
                _tasks = tasks;
                foreach (var task in _tasks)
                {
                    AddTask(task);
                }
                Ready();
            }

            /// <summary>
            /// Returns whether we should notify the debugger of a wait completion.  This returns
            /// true iff at least one constituent task has its bit set.
            /// </summary>
            internal override bool ShouldNotifyDebuggerOfWaitCompletion
            {
                get { return base.ShouldNotifyDebuggerOfWaitCompletion && AnyTaskRequiresNotifyDebuggerOfWaitCompletion(_tasks); }
            }

            public void Invoke(Task completingTask)
            {
                var count = Interlocked.Decrement(ref _count);
                if (count == 0)
                {
                    if (Thread.VolatileRead(ref _ready) == 1)
                    {
                        Done();
                    }
                }
            }

            private void AddTask(Task awaitedTask)
            {
                Contract.Requires(Thread.VolatileRead(ref _ready) == 0);
                Interlocked.Increment(ref _count);
                if (!awaitedTask.AddTaskContinuation(this, /*addBeforeOthers:*/ true))
                {
                    Interlocked.Decrement(ref _count);
                }
            }

            private void Done()
            {
                var done = Interlocked.Exchange(ref _done, 1);
                if (done == 0)
                {
                    PrivateDone();
                }
            }

            private void PrivateDone()
            {
                // Set up some accounting variables
                var results = new T[_tasks.Length];
                List<ExceptionDispatchInfo> observedExceptions = null;
                Task canceledTask = null;
                // Loop through antecedents:
                //   If any one of them faults, the result will be faulted
                //   If none fault, but at least one is canceled, the result will be canceled
                //   If none fault or are canceled, then result will be RanToCompletion
                for (var index = 0; index < _tasks.Length; index++)
                {
                    var task = _tasks[index];
                    if (task == null)
                    {
                        Contract.Assert(false, "Constituent task in WhenAll should never be null");
                        throw new InvalidOperationException("Constituent task in WhenAll should never be null");
                    }
                    if (task.IsFaulted)
                    {
                        if (observedExceptions == null)
                        {
                            observedExceptions = new List<ExceptionDispatchInfo>();
                        }
                        observedExceptions.AddRange(task._exceptionsHolder.GetExceptionDispatchInfos());
                    }
                    else if (task.IsCanceled)
                    {
                        if (canceledTask == null)
                        {
                            canceledTask = task; // use the first task that's canceled
                        }
                    }
                    else
                    {
                        Contract.Assert(task.Status == TaskStatus.RanToCompletion);
                        results[index] = task.Result;
                    }
                    // Regardless of completion state, if the task has its debug bit set, transfer it to the
                    // WhenAll task.  We must do this before we complete the task.
                    if (task.IsWaitNotificationEnabled)
                    {
                        SetNotificationForWaitCompletion(/*enabled:*/ true);
                    }
                    else
                    {
                        _tasks[index] = null; // avoid holding onto tasks unnecessarily
                    }
                }
                if (observedExceptions != null)
                {
                    Contract.Assert(observedExceptions.Count > 0, "Expected at least one exception");

                    //We don't need to TraceOperationCompleted here because TrySetException will call Finish and we'll log it there

                    TrySetException(observedExceptions);
                }
                else if (canceledTask != null)
                {
                    TrySetCanceledPromise(canceledTask.CancellationToken);
                }
                else
                {
                    TrySetResult(results);
                }
            }

            private void Ready()
            {
                Thread.VolatileWrite(ref _ready, 1);
                if (Thread.VolatileRead(ref _count) == 0)
                {
                    Done();
                }
            }
        }
    }
}

#endif