#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics;
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
                foreach (var task in tasks)
                {
                    AddTask(task);
                }
                Ready();
            }

            public bool IsDone
            {
                get
                {
                    return Interlocked.CompareExchange(ref _done, null, null) == null;
                }
            }

            public void Dispose()
            {
                var tasks = Interlocked.Exchange(ref _tasks, null);
                if (Interlocked.CompareExchange(ref _done, null, null) == null)
                {
                    return;
                }
                var incomplete = false;
                foreach (var task in tasks)
                {
                    if (task.IsCompleted)
                    {
                        continue;
                    }
                    task.RemoveContinuation(this);
                    incomplete = true;
                }
                if (!incomplete)
                {
                    Done();
                }
            }

            public void Invoke(Task completingTask)
            {
                Debug.Print("Running continuation on task: " + completingTask.Id);
                var index = Array.IndexOf(_tasks, completingTask);
                if (index >= 0)
                {
                    _tasks[index] = null;
                }
                var count = Interlocked.Decrement(ref _count);
                if (count == 0 && Thread.VolatileRead(ref _ready) == 1)
                {
                    Done();
                }
            }

            private void AddTask(Task awaitedTask)
            {
                Contract.Requires(Thread.VolatileRead(ref _ready) == 0);
                if (awaitedTask.Status != TaskStatus.RanToCompletion)
                {
                    Interlocked.Increment(ref _count);
                    if (awaitedTask.AddTaskContinuation(this, /*addBeforeOthers:*/ true))
                    {
                        var index = Array.IndexOf(_tasks, null);
                        while (Interlocked.CompareExchange(ref _tasks[index], awaitedTask, null) != null)
                        {
                            index = (index + 1) % _tasks.Length;
                        }
                    }
                    else
                    {
                        Interlocked.Decrement(ref _count);
                    }
                }
            }

            private void Done()
            {
                var done = Interlocked.Exchange(ref _done, null);
                if (done != null)
                {
                    done();
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
                get
                {
                    return base.ShouldNotifyDebuggerOfWaitCompletion && AnyTaskRequiresNotifyDebuggerOfWaitCompletion(_tasks);
                }
            }

            public void Invoke(Task completedTask)
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
                get
                {
                    return base.ShouldNotifyDebuggerOfWaitCompletion && AnyTaskRequiresNotifyDebuggerOfWaitCompletion(_tasks);
                }
            }

            public void Invoke(Task completedTask)
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
                        if (canceledTask == null) canceledTask = task; // use the first task that's canceled
                    }
                    else
                    {
                        Contract.Assert(task.Status == TaskStatus.RanToCompletion);
                        results[index] = task.Result;
                    }
                    // Regardless of completion state, if the task has its debug bit set, transfer it to the
                    // WhenAll task.  We must do this before we complete the task.
                    if (task.IsWaitNotificationEnabled) SetNotificationForWaitCompletion(/*enabled:*/ true);
                    else _tasks[index] = null; // avoid holding onto tasks unnecessarily
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