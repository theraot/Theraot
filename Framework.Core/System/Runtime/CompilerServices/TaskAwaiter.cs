﻿#if LESSTHAN_NET45

#pragma warning disable CA1815 // Override equals and operator equals on value types

using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    ///     Provides an awaiter for awaiting a <see cref="Task" /> .
    /// </summary>
    /// <remarks>
    ///     This type is intended for compiler use only.
    /// </remarks>
    public readonly struct TaskAwaiter : ICriticalNotifyCompletion
    {
        /// <summary>
        ///     An empty array to use with MethodInfo.Invoke.
        /// </summary>
        private static readonly object[] _emptyParams = new object[0];

        /// <summary>
        ///     A MethodInfo for the Exception.PrepForRemoting method.
        /// </summary>
        private static readonly MethodInfo? _prepForRemoting = GetPrepForRemotingMethodInfo();

        /// <summary>
        ///     The task being awaited.
        /// </summary>
        private readonly Task _task;

        /// <summary>
        ///     Initializes the <see cref="TaskAwaiter" /> .
        /// </summary>
        /// <param name="task"> The <see cref="Task" /> to be awaited. </param>
        internal TaskAwaiter(Task task)
        {
            _task = task;
        }

        /// <summary>
        ///     Gets whether the task being awaited is completed.
        /// </summary>
        /// <remarks>
        ///     This property is intended for compiler user rather than use directly in code.
        /// </remarks>
        /// <exception cref="NullReferenceException">The awaiter was not properly initialized.</exception>
        public bool IsCompleted => _task.IsCompleted;

        /// <summary>
        ///     Whether the current thread is appropriate for inlining the await continuation.
        /// </summary>
        private static bool IsValidLocationForInlining
        {
            get
            {
                var current = SynchronizationContext.Current;
                if (current != null && current.GetType() != typeof(SynchronizationContext))
                {
                    return false;
                }

                return TaskScheduler.Current == TaskScheduler.Default;
            }
        }

        /// <summary>
        ///     Ends the await on the completed <see cref="Task" /> .
        /// </summary>
        /// <exception cref="NullReferenceException">The awaiter was not properly initialized.</exception>
        /// <exception cref="InvalidOperationException">The task was not yet completed.</exception>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="Exception">The task completed in a Faulted state.</exception>
        public void GetResult()
        {
            ValidateEnd(_task);
        }

        /// <summary>
        ///     Schedules the continuation onto the <see cref="Task" /> associated with this <see cref="TaskAwaiter" /> .
        /// </summary>
        /// <param name="continuation"> The action to invoke when the await operation completes. </param>
        /// <exception cref="ArgumentNullException">
        ///     The
        ///     <paramref name="continuation" />
        ///     argument is null (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="InvalidOperationException">The awaiter was not properly initialized.</exception>
        /// <remarks>
        ///     This method is intended for compiler user rather than use directly in code.
        /// </remarks>
        public void OnCompleted(Action continuation)
        {
            OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
        }

        /// <summary>
        ///     Schedules the continuation onto the <see cref="Task" /> associated with this <see cref="TaskAwaiter" /> .
        /// </summary>
        /// <param name="continuation"> The action to invoke when the await operation completes. </param>
        /// <exception cref="ArgumentNullException">
        ///     The
        ///     <paramref name="continuation" />
        ///     argument is null (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="InvalidOperationException">The awaiter was not properly initialized.</exception>
        /// <remarks>
        ///     This method is intended for compiler user rather than use directly in code.
        /// </remarks>
        [SecurityCritical]
        public void UnsafeOnCompleted(Action continuation)
        {
            OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
        }

        /// <summary>
        ///     Schedules the continuation onto the <see cref="Task" /> associated with this <see cref="TaskAwaiter" /> .
        /// </summary>
        /// <param name="task"> The awaited task. </param>
        /// <param name="continuation"> The action to invoke when the await operation completes. </param>
        /// <param name="continueOnCapturedContext"> Whether to capture and marshal back to the current context. </param>
        /// <exception cref="ArgumentNullException">
        ///     The
        ///     <paramref name="continuation" />
        ///     argument is null (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="NullReferenceException">The awaiter was not properly initialized.</exception>
        /// <remarks>
        ///     This method is intended for compiler user rather than use directly in code.
        /// </remarks>
        internal static void OnCompletedInternal(Task task, Action continuation, bool continueOnCapturedContext)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            var syncContext = continueOnCapturedContext ? SynchronizationContext.Current : null;
            if (syncContext != null && syncContext.GetType() != typeof(SynchronizationContext))
            {
                _ = task.ContinueWith
                (
                    _ =>
                    {
                        try
                        {
                            syncContext.Post(state => ((Action)state!)(), continuation);
                        }
                        catch (Exception ex)
                        {
                            AsyncMethodBuilderCore.ThrowOnContext(ex, targetContext: null);
                        }
                    },
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default
                );
            }
            else
            {
                OnCompletedWithoutSyncContext(task, continuation, continueOnCapturedContext);
            }
        }

        /// <summary>
        ///     Copies the exception's stack trace so its stack trace isn't overwritten.
        /// </summary>
        /// <param name="exc"> The exception to prepare. </param>
        internal static Exception PrepareExceptionForRethrow(Exception exc)
        {
            if (_prepForRemoting == null)
            {
                return exc;
            }

            try
            {
                _prepForRemoting.Invoke(exc, _emptyParams);
            }
            catch (Exception ex)
            {
                _ = ex;
            }

            return exc;
        }

        /// <summary>
        ///     Fast checks for the end of an await operation to determine whether more needs to be done prior to completing the
        ///     await.
        /// </summary>
        /// <param name="task"> The awaited task. </param>
        internal static void ValidateEnd(Task task)
        {
            if (task.Status == TaskStatus.RanToCompletion)
            {
                return;
            }

            HandleNonSuccess(task);
        }

        /// <summary>
        ///     Gets the MethodInfo for the internal PrepForRemoting method on Exception.
        /// </summary>
        /// <returns> The MethodInfo if it could be retrieved, or else null. </returns>
        private static MethodInfo? GetPrepForRemotingMethodInfo()
        {
            try
            {
                return typeof(Exception).GetMethod("PrepForRemoting", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            catch (Exception ex)
            {
                _ = ex;
                return null;
            }
        }

        /// <summary>
        ///     Handles validations on tasks that aren't successfully completed.
        /// </summary>
        /// <param name="task"> The awaited task. </param>
        private static void HandleNonSuccess(Task task)
        {
            if (!task.IsCompleted)
            {
                try
                {
                    task.Wait();
                }
                catch (Exception ex)
                {
                    _ = ex;
                }
            }

            if (task.Status == TaskStatus.RanToCompletion)
            {
                return;
            }

            ThrowForNonSuccess(task);
        }

        private static void OnCompletedWithoutSyncContext(Task task, Action continuation, bool continueOnCapturedContext)
        {
            var scheduler = continueOnCapturedContext ? TaskScheduler.Current : TaskScheduler.Default;
            if (task.IsCompleted)
            {
                _ = Task.Factory.StartNew
                (
                    state => ((Action?)state)?.Invoke(),
                    continuation,
                    CancellationToken.None,
                    TaskCreationOptions.None, scheduler
                );
            }
            else if (scheduler != TaskScheduler.Default)
            {
                _ = task.ContinueWith
                (
                    _ => RunNoException(continuation),
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, scheduler
                );
            }
            else
            {
                _ = task.ContinueWith
                (
                    ContinuationFunction,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default
                );

                void ContinuationFunction(Task completedTask)
                {
                    _ = completedTask;
                    if (IsValidLocationForInlining)
                    {
                        RunNoException(continuation);
                    }
                    else
                    {
                        _ = Task.Factory.StartNew(state => RunNoException((Action?)state), continuation, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
                    }
                }
            }
        }

        private static void RunNoException(Action? continuation)
        {
            if (continuation == null)
            {
                return;
            }

            try
            {
                continuation();
            }
            catch (Exception ex)
            {
                AsyncMethodBuilderCore.ThrowOnContext(ex, targetContext: null);
            }
        }

        private static void ThrowForNonSuccess(Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.Canceled:
                    throw new TaskCanceledException(task);
                case TaskStatus.Faulted:
                    throw PrepareExceptionForRethrow(task.Exception!.InnerException!);
                default:
                    throw new InvalidOperationException("The task has not yet completed.");
            }
        }
    }

    /// <summary>
    ///     Provides an awaiter for awaiting a <see cref="Task{T}" /> .
    /// </summary>
    /// <remarks>
    ///     This type is intended for compiler use only.
    /// </remarks>
    public readonly struct TaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        /// <summary>
        ///     The task being awaited.
        /// </summary>
        private readonly Task<TResult> _task;

        /// <summary>
        ///     Initializes the <see cref="TaskAwaiter{TResult}" /> .
        /// </summary>
        /// <param name="task"> The <see cref="Task{TResult}" /> to be awaited. </param>
        internal TaskAwaiter(Task<TResult> task)
        {
            _task = task;
        }

        /// <summary>
        ///     Gets whether the task being awaited is completed.
        /// </summary>
        /// <remarks>
        ///     This property is intended for compiler user rather than use directly in code.
        /// </remarks>
        /// <exception cref="NullReferenceException">The awaiter was not properly initialized.</exception>
        public bool IsCompleted => _task.IsCompleted;

        /// <summary>
        ///     Ends the await on the completed <see cref="Task{TResult}" /> .
        /// </summary>
        /// <returns> The result of the completed <see cref="Task{TResult}" /> . </returns>
        /// <exception cref="NullReferenceException">The awaiter was not properly initialized.</exception>
        /// <exception cref="InvalidOperationException">The task was not yet completed.</exception>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="Exception">The task completed in a Faulted state.</exception>
        public TResult GetResult()
        {
            TaskAwaiter.ValidateEnd(_task);
            return _task.Result;
        }

        /// <summary>
        ///     Schedules the continuation onto the <see cref="Task" /> associated with this <see cref="TaskAwaiter" /> .
        /// </summary>
        /// <param name="continuation"> The action to invoke when the await operation completes. </param>
        /// <exception cref="ArgumentNullException">
        ///     The
        ///     <paramref name="continuation" />
        ///     argument is null (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="NullReferenceException">The awaiter was not properly initialized.</exception>
        /// <remarks>
        ///     This method is intended for compiler user rather than use directly in code.
        /// </remarks>
        public void OnCompleted(Action continuation)
        {
            TaskAwaiter.OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
        }

        /// <summary>
        ///     Schedules the continuation onto the <see cref="Task" /> associated with this <see cref="TaskAwaiter" /> .
        /// </summary>
        /// <param name="continuation"> The action to invoke when the await operation completes. </param>
        /// <exception cref="ArgumentNullException">
        ///     The
        ///     <paramref name="continuation" />
        ///     argument is null (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="InvalidOperationException">The awaiter was not properly initialized.</exception>
        /// <remarks>
        ///     This method is intended for compiler user rather than use directly in code.
        /// </remarks>
        [SecurityCritical]
        public void UnsafeOnCompleted(Action continuation)
        {
            TaskAwaiter.OnCompletedInternal(_task, continuation, continueOnCapturedContext: true);
        }
    }
}

#endif