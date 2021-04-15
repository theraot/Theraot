#if LESSTHAN_NET40

#pragma warning disable AsyncifyVariable // Use Task Async

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;
using Theraot;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        internal sealed class CompleteOnInvokePromise : Task<Task>, ITaskCompletionAction
        {
            private int _firstTaskAlreadyCompleted;
            private ICollection<Task>? _tasks; // must track this for cleanup

            public CompleteOnInvokePromise(ICollection<Task> tasks)
                : base(TaskCreationOptions.None, state: null)
            {
                Contract.Requires(tasks.Count > 0, "Expected a non-zero length task array");
                _tasks = tasks;
            }

            public void Invoke(Task completingTask)
            {
                if (Interlocked.CompareExchange(ref _firstTaskAlreadyCompleted, 1, 0) != 0)
                {
                    return;
                }

#if DEBUG
                var success =
#endif
                    TrySetResult(completingTask);
#if DEBUG
                Contract.Assert(success, "Only one task should have gotten to this point, and thus this must be successful.");
#endif

                // We need to remove continuations that may be left straggling on other tasks.
                // Otherwise, repeated calls to WhenAny using the same task could leak actions.
                // This may also help to avoided unnecessary invocations of this whenComplete delegate.
                // Note that we may be attempting to remove a continuation from a task that hasn't had it
                // added yet; while there's overhead there, the operation won't hurt anything.
                foreach (var task in _tasks!)
                {
                    // if an element was erroneously nulled out concurrently, just skip it; worst case is we don't remove a continuation
                    if (task?.IsCompleted != false)
                    {
                        continue;
                    }

                    task.RemoveContinuation(this);
                }

                _tasks = null;
            }
        }

        private sealed class WhenAllCore : ITaskCompletionAction, IDisposable
        {
            private int _count;
            private Action? _done;
            private int _index;
            private int _ready;
            private Task?[]? _tasks;

            internal WhenAllCore(ICollection<Task> tasks, Action done)
            {
                Contract.Requires(tasks.Count > 0, "Expected a non-zero length task array");
                _done = done;
                _tasks = new Task[tasks.Count];
                _index = -1;
                // Add all tasks (this should increment _count, and add continuations)
                foreach (var task in tasks)
                {
                    AddTask(task);
                }

                // Report we finished adding all tasks
                Volatile.Write(ref _ready, 1);
                CheckCount();
            }

            public bool IsDone => Interlocked.CompareExchange(ref _done, value: null, comparand: null) == null;

            public void Dispose()
            {
                // Get and erase the tasks
                var tasks = Interlocked.Exchange(ref _tasks, value: null);
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
                    if (task?.IsCompleted != false)
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
                var tasks = Interlocked.CompareExchange(ref _tasks, value: null, comparand: null);
                // If there are no tasks (Disposed) there is nothing to do
                if (tasks == null)
                {
                    return;
                }

                // Find the completing task and set it to null
                // If we do not find it, it means the continuation executed before the task was added
                // Do not use IndexOf, this is a micro-optimization
                for (var index = 0; index < tasks.Length; index++)
                {
                    ref var current = ref tasks[index];
                    if (current != completingTask)
                    {
                        continue;
                    }

                    // Set to null so the GC can take it early
                    current = null; // equivalent to tasks[index] = null
                    break;
                }

                // Decrement count
                Interlocked.Decrement(ref _count);
                CheckCount();
            }

            private void AddTask(Task awaitedTask)
            {
                Contract.Requires(Volatile.Read(ref _ready) == 0);
                // Get the tasks
                var tasks = Interlocked.CompareExchange(ref _tasks, value: null, comparand: null);
                // If there are no tasks (Disposed) there is nothing to do
                if (tasks == null)
                {
                    return;
                }

                // Only add tasks that has not completed
                if (awaitedTask.Status == TaskStatus.RanToCompletion)
                {
                    return;
                }

                // Preemptively increment _count
                // So that is has already been incremented when the continuation runs
                Interlocked.Increment(ref _count);
                // Add the continuation
                if (awaitedTask.AddTaskContinuation(this, /*addBeforeOthers:*/ addBeforeOthers: true))
                {
                    // Find a spot in the tasks
                    var index = Interlocked.Increment(ref _index);
                    // Add the task
                    tasks[index] = awaitedTask;
                    // Check again if the task has completed, it may have completed while we were adding it
                    if (awaitedTask.Status == TaskStatus.RanToCompletion)
                    {
                        // Perhaps it did complete:
                        // - Before adding the continuation -> nothing to do
                        // - After adding the continuation before adding the task -> We have an orphan task to remove
                        // - After adding the continuation and the task -> nothing to do
                        // Remove the orphan task if it is there
                        tasks[index] = null;
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

            private void CheckCount()
            {
                var count = Volatile.Read(ref _count);
                // If count reached zero and we have finished adding all tasks, call done
                if (count == 0 && Volatile.Read(ref _ready) == 1)
                {
                    Done();
                }
            }

            private void Done()
            {
                // Get and erase done
                var done = Interlocked.Exchange(ref _done, value: null);
                // If if was there, call it
                done?.Invoke();
            }
        }

        // A Task<VoidStruct> that gets completed when all of its constituent tasks complete.
        // Completion logic will analyze the antecedents in order to choose completion status.
        // Used in InternalWhenAll(Task[])
        private sealed class WhenAllPromise : Task<VoidStruct>, ITaskCompletionAction
        {
            private readonly Task?[] _tasks;
            private int _count;
            private int _done;
            private int _ready;

            internal WhenAllPromise(Task[] tasks)
                : base(TaskCreationOptions.None, state: null)
            {
                Contract.Requires(tasks.Length > 0, "Expected a non-zero length task array");
                _tasks = tasks;
                // Add all tasks (this should increment _count, and add continuations)
                foreach (var task in tasks)
                {
                    AddTask(task);
                }

                // Report we finished adding all tasks
                Volatile.Write(ref _ready, 1);
                CheckCount();
            }

            /// <summary>
            ///     Returns whether we should notify the debugger of a wait completion.  This returns
            ///     true iff at least one constituent task has its bit set.
            /// </summary>
            internal override bool ShouldNotifyDebuggerOfWaitCompletion => base.ShouldNotifyDebuggerOfWaitCompletion && AnyTaskRequiresNotifyDebuggerOfWaitCompletion(_tasks);

            public void Invoke(Task completingTask)
            {
                // Decrement count
                Interlocked.Decrement(ref _count);
                CheckCount();
            }

            private void AddTask(Task awaitedTask)
            {
                Contract.Requires(Volatile.Read(ref _ready) == 0);
                // Only add tasks that has not completed
                if (awaitedTask.Status == TaskStatus.RanToCompletion)
                {
                    return;
                }

                // Preemptively increment _count
                // So that is has already been incremented when the continuation runs
                Interlocked.Increment(ref _count);
                // Add the continuation
                if (!awaitedTask.AddTaskContinuation(this, /*addBeforeOthers:*/ addBeforeOthers: true))
                {
                    // We failed to add the continuation
                    // Decrement the _count
                    Interlocked.Decrement(ref _count);
                }
            }

            private void CheckCount()
            {
                var count = Volatile.Read(ref _count);
                // If count reached zero and we have finished adding all tasks, call done
                if (count == 0 && Volatile.Read(ref _ready) == 1)
                {
                    Done();
                }
            }

            private void Done()
            {
                var done = Interlocked.CompareExchange(ref _done, 1, 0);
                if (done == 0)
                {
                    PrivateDone();
                }
            }

            private void PrivateDone()
            {
                // Set up some accounting variables
                List<ExceptionDispatchInfo>? observedExceptions = null;
                Task? canceledTask = null;
                // Loop through antecedents:
                //   If any one of them faults, the result will be faulted
                //   If none fault, but at least one is canceled, the result will be canceled
                //   If none fault or are canceled, then result will be RanToCompletion
                for (var index = 0; index < _tasks.Length; index++)
                {
                    var task = _tasks[index];
                    if (task == null)
                    {
                        Contract.Assert(condition: false, "Constituent task in WhenAll should never be null");
                        throw new InvalidOperationException("Constituent task in WhenAll should never be null");
                    }

                    if (task.IsFaulted)
                    {
                        (observedExceptions ??= new List<ExceptionDispatchInfo>()).AddRange(task._exceptionsHolder!.GetExceptionDispatchInfos());
                    }
                    else if (task.IsCanceled && canceledTask == null)
                    {
                        canceledTask = task; // use the first task that's canceled
                    }

                    // Regardless of completion state, if the task has its debug bit set, transfer it to the
                    // WhenAll task.  We must do this before we complete the task.
                    if (task.IsWaitNotificationEnabled)
                    {
                        SetNotificationForWaitCompletion( /*enabled:*/ enabled: true);
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
                    TrySetResult(default);
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
            private readonly Task<T>?[] _tasks;
            private int _count;
            private int _done;
            private int _ready;

            internal WhenAllPromise(Task<T>[] tasks)
                : base(TaskCreationOptions.None, state: null)
            {
                Contract.Requires(tasks.Length > 0, "Expected a non-zero length task array");
                _tasks = tasks;
                // Add all tasks (this should increment _count, and add continuations)
                foreach (var task in tasks)
                {
                    AddTask(task);
                }

                // Report we finished adding all tasks
                Volatile.Write(ref _ready, 1);
                CheckCount();
            }

            /// <summary>
            ///     Returns whether we should notify the debugger of a wait completion.  This returns
            ///     true iff at least one constituent task has its bit set.
            /// </summary>
            internal override bool ShouldNotifyDebuggerOfWaitCompletion => base.ShouldNotifyDebuggerOfWaitCompletion && AnyTaskRequiresNotifyDebuggerOfWaitCompletion(_tasks);

            public void Invoke(Task completingTask)
            {
                // Decrement count
                Interlocked.Decrement(ref _count);
                CheckCount();
            }

            private void AddTask(Task awaitedTask)
            {
                Contract.Requires(Volatile.Read(ref _ready) == 0);
                // Only add tasks that has not completed
                if (awaitedTask.Status == TaskStatus.RanToCompletion)
                {
                    return;
                }

                // Preemptively increment _count
                // So that is has already been incremented when the continuation runs
                Interlocked.Increment(ref _count);
                // Add the continuation
                if (!awaitedTask.AddTaskContinuation(this, /*addBeforeOthers:*/ addBeforeOthers: true))
                {
                    // We failed to add the continuation
                    // Decrement the _count
                    Interlocked.Decrement(ref _count);
                }
            }

            private void CheckCount()
            {
                var count = Volatile.Read(ref _count);
                // If count reached zero and we have finished adding all tasks, call done
                if (count == 0 && Volatile.Read(ref _ready) == 1)
                {
                    Done();
                }
            }

            private void Done()
            {
                var done = Interlocked.CompareExchange(ref _done, 1, 0);
                if (done == 0)
                {
                    PrivateDone();
                }
            }

            private void PrivateDone()
            {
                // Set up some accounting variables
                var results = new T[_tasks.Length];
                List<ExceptionDispatchInfo>? observedExceptions = null;
                Task? canceledTask = null;
                // Loop through antecedents:
                //   If any one of them faults, the result will be faulted
                //   If none fault, but at least one is canceled, the result will be canceled
                //   If none fault or are canceled, then result will be RanToCompletion
                for (var index = 0; index < _tasks.Length; index++)
                {
                    var task = _tasks[index];
                    if (task == null)
                    {
                        Contract.Assert(condition: false, "Constituent task in WhenAll should never be null");
                        throw new InvalidOperationException("Constituent task in WhenAll should never be null");
                    }

                    if (task.IsFaulted)
                    {
                        (observedExceptions ??= new List<ExceptionDispatchInfo>()).AddRange(task._exceptionsHolder!.GetExceptionDispatchInfos());
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
                        SetNotificationForWaitCompletion( /*enabled:*/ enabled: true);
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
        }
    }
}

#endif