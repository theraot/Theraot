#if NET20 || NET30 || NET35 || NET40

using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;
using Theraot;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Provides a builder for asynchronous methods that return <see cref="T:System.Threading.Tasks.Task"/>.
    ///             This type is intended for compiler use only.
    ///
    /// </summary>
    ///
    /// <remarks>
    /// AsyncTaskMethodBuilder is a value type, and thus it is copied by value.
    ///             Prior to being copied, one of its Task, SetResult, or SetException members must be accessed,
    ///             or else the copies may end up building distinct Task instances.
    ///
    /// </remarks>
    public struct AsyncTaskMethodBuilder : IAsyncMethodBuilder
    {
        /// <summary>
        /// A cached VoidStruct task used for builders that complete synchronously.
        /// </summary>
        private static readonly TaskCompletionSource<VoidStruct> _cachedCompleted = AsyncTaskMethodBuilder<VoidStruct>.DefaultResultTask;

        /// <summary>
        /// The generic builder object to which this non-generic instance delegates.
        /// </summary>
        // Note: this is a mutable struct, it has starts at the fault value and then it is mutated
        // There is no need to assign it
        // It should not be readonly
#pragma warning disable 649
        private AsyncTaskMethodBuilder<VoidStruct> _builder;
#pragma warning restore 649

        /// <summary>
        /// Gets the <see cref="T:System.Threading.Tasks.Task"/> for this builder.
        /// </summary>
        ///
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task"/> representing the builder's asynchronous operation.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The builder is not initialized.</exception>
        public Task Task => _builder.Task;

        /// <summary>
        /// Initializes a new <see cref="T:System.Runtime.CompilerServices.AsyncTaskMethodBuilder"/>.
        /// </summary>
        ///
        /// <returns>
        /// The initialized <see cref="T:System.Runtime.CompilerServices.AsyncTaskMethodBuilder"/>.
        /// </returns>
        public static AsyncTaskMethodBuilder Create()
        {
            return new AsyncTaskMethodBuilder();
        }

        /// <summary>
        /// Schedules the specified state machine to be pushed forward when the specified awaiter completes.
        ///
        /// </summary>
        /// <typeparam name="TAwaiter">Specifies the type of the awaiter.</typeparam><typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam><param name="awaiter">The awaiter.</param><param name="stateMachine">The state machine.</param>
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        /// <summary>
        /// Schedules the specified state machine to be pushed forward when the specified awaiter completes.
        ///
        /// </summary>
        /// <typeparam name="TAwaiter">Specifies the type of the awaiter.</typeparam><typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam><param name="awaiter">The awaiter.</param><param name="stateMachine">The state machine.</param>
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }

        void IAsyncMethodBuilder.PreBoxInitialization()
        {
            GC.KeepAlive(Task);
        }

        /// <summary>
        /// Completes the <see cref="T:System.Threading.Tasks.Task"/> in the
        ///             <see cref="T:System.Threading.Tasks.TaskStatus">Faulted</see> state with the specified exception.
        ///
        /// </summary>
        /// <param name="exception">The <see cref="T:System.Exception"/> to use to fault the task.</param><exception cref="T:System.ArgumentNullException">The <paramref name="exception"/> argument is null (Nothing in Visual Basic).</exception><exception cref="T:System.InvalidOperationException">The builder is not initialized.</exception><exception cref="T:System.InvalidOperationException">The task has already completed.</exception>
        public void SetException(Exception exception)
        {
            _builder.SetException(exception);
        }

        /// <summary>
        /// Completes the <see cref="T:System.Threading.Tasks.Task"/> in the
        ///             <see cref="T:System.Threading.Tasks.TaskStatus">RanToCompletion</see> state.
        ///
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The builder is not initialized.</exception><exception cref="T:System.InvalidOperationException">The task has already completed.</exception>
        public void SetResult()
        {
            _builder.SetResult(_cachedCompleted);
        }

        /// <summary>
        /// Associates the builder with the state machine it represents.
        /// </summary>
        /// <param name="stateMachine">The heap-allocated state machine object.</param><exception cref="T:System.ArgumentNullException">The <paramref name="stateMachine"/> argument was null (Nothing in Visual Basic).</exception><exception cref="T:System.InvalidOperationException">The builder is incorrectly initialized.</exception>
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _builder.SetStateMachine(stateMachine);
        }

        /// <summary>
        /// Initiates the builder's execution with the associated state machine.
        /// </summary>
        /// <typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam><param name="stateMachine">The state machine instance, passed by reference.</param>
        [DebuggerStepThrough]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _builder.Start(ref stateMachine);
        }

        /// <summary>
        /// Called by the debugger to request notification when the first wait operation
        ///             (await, Wait, Result, etc.) on this builder's task completes.
        ///
        /// </summary>
        /// <param name="enabled">true to enable notification; false to disable a previously set notification.
        ///             </param>
        internal void SetNotificationForWaitCompletion(bool enabled)
        {
            _builder.SetNotificationForWaitCompletion(enabled);
        }
    }

    /// <summary>
    /// Provides a builder for asynchronous methods that return <see cref="T:System.Threading.Tasks.Task`1"/>.
    ///             This type is intended for compiler use only.
    ///
    /// </summary>
    ///
    /// <remarks>
    /// AsyncTaskMethodBuilder{TResult} is a value type, and thus it is copied by value.
    ///             Prior to being copied, one of its Task, SetResult, or SetException members must be accessed,
    ///             or else the copies may end up building distinct Task instances.
    ///
    /// </remarks>
    public struct AsyncTaskMethodBuilder<TResult> : IAsyncMethodBuilder
    {
        /// <summary>
        /// A cached task for default(TResult).
        /// </summary>
        internal static readonly TaskCompletionSource<TResult> DefaultResultTask;

        /// <summary>
        /// State related to the IAsyncStateMachine.
        /// </summary>
        // Note: this is a mutable struct, it has starts at the fault value and then it is mutated
        // There is no need to assign it
        // It should not be readonly
#pragma warning disable 649
        private AsyncMethodBuilderCore _coreState;
#pragma warning restore 649

        /// <summary>
        /// The lazily-initialized task completion source.
        /// </summary>
        private TaskCompletionSource<TResult> _task;

        /// <summary>
        /// Temporary support for disabling crashing if tasks go unobserved.
        /// </summary>
        static AsyncTaskMethodBuilder()
        {
            DefaultResultTask = AsyncMethodTaskCache<TResult>.CreateCompleted(default);
            try
            {
                AsyncVoidMethodBuilder.PreventUnobservedTaskExceptions();
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Threading.Tasks.Task`1"/> for this builder.
        /// </summary>
        ///
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task`1"/> representing the builder's asynchronous operation.
        /// </returns>
        public Task<TResult> Task => CompletionSource.Task;

        /// <summary>
        /// Gets the lazily-initialized TaskCompletionSource.
        /// </summary>
        internal TaskCompletionSource<TResult> CompletionSource
        {
            get
            {
                var completionSource = _task;
                if (completionSource == null)
                {
                    _task = completionSource = new TaskCompletionSource<TResult>();
                }

                return completionSource;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="T:System.Runtime.CompilerServices.AsyncTaskMethodBuilder"/>.
        /// </summary>
        ///
        /// <returns>
        /// The initialized <see cref="T:System.Runtime.CompilerServices.AsyncTaskMethodBuilder"/>.
        /// </returns>
        public static AsyncTaskMethodBuilder<TResult> Create()
        {
            return new AsyncTaskMethodBuilder<TResult>();
        }

        /// <summary>
        /// Schedules the specified state machine to be pushed forward when the specified awaiter completes.
        ///
        /// </summary>
        /// <typeparam name="TAwaiter">Specifies the type of the awaiter.</typeparam><typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam><param name="awaiter">The awaiter.</param><param name="stateMachine">The state machine.</param>
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            try
            {
                var completionAction = _coreState.GetCompletionAction(ref this, ref stateMachine);
                awaiter.OnCompleted(completionAction);
            }
            catch (Exception ex)
            {
                AsyncMethodBuilderCore.ThrowOnContext(ex, null);
            }
        }

        /// <summary>
        /// Schedules the specified state machine to be pushed forward when the specified awaiter completes.
        ///
        /// </summary>
        /// <typeparam name="TAwaiter">Specifies the type of the awaiter.</typeparam><typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam><param name="awaiter">The awaiter.</param><param name="stateMachine">The state machine.</param>
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            try
            {
                var completionAction = _coreState.GetCompletionAction(ref this, ref stateMachine);
                awaiter.UnsafeOnCompleted(completionAction);
            }
            catch (Exception ex)
            {
                AsyncMethodBuilderCore.ThrowOnContext(ex, null);
            }
        }

        void IAsyncMethodBuilder.PreBoxInitialization()
        {
            GC.KeepAlive(Task);
        }

        /// <summary>
        /// Completes the <see cref="T:System.Threading.Tasks.Task`1"/> in the
        ///             <see cref="T:System.Threading.Tasks.TaskStatus">Faulted</see> state with the specified exception.
        ///
        /// </summary>
        /// <param name="exception">The <see cref="T:System.Exception"/> to use to fault the task.</param><exception cref="T:System.ArgumentNullException">The <paramref name="exception"/> argument is null (Nothing in Visual Basic).</exception><exception cref="T:System.InvalidOperationException">The task has already completed.</exception>
        public void SetException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var completionSource = CompletionSource;
            var setException = exception is OperationCanceledException ? completionSource.TrySetCanceled() : completionSource.TrySetException(exception);
            if (!setException)
            {
                throw new InvalidOperationException("The Task was already completed.");
            }
        }

        /// <summary>
        /// Completes the <see cref="T:System.Threading.Tasks.Task`1"/> in the
        ///             <see cref="T:System.Threading.Tasks.TaskStatus">RanToCompletion</see> state with the specified result.
        ///
        /// </summary>
        /// <param name="result">The result to use to complete the task.</param><exception cref="T:System.InvalidOperationException">The task has already completed.</exception>
        public void SetResult(TResult result)
        {
            var completionSource = _task;
            if (completionSource == null)
            {
                _task = GetTaskForResult(result);
            }
            else if (!completionSource.TrySetResult(result))
            {
                throw new InvalidOperationException("The Task was already completed.");
            }
        }

        /// <summary>
        /// Associates the builder with the state machine it represents.
        /// </summary>
        /// <param name="stateMachine">The heap-allocated state machine object.</param><exception cref="T:System.ArgumentNullException">The <paramref name="stateMachine"/> argument was null (Nothing in Visual Basic).</exception><exception cref="T:System.InvalidOperationException">The builder is incorrectly initialized.</exception>
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _coreState.SetStateMachine(stateMachine);
        }

        /// <summary>
        /// Initiates the builder's execution with the associated state machine.
        /// </summary>
        /// <typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam><param name="stateMachine">The state machine instance, passed by reference.</param>
        [DebuggerStepThrough]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            // Should not be static
            AsyncMethodBuilderCore.Start(ref stateMachine);
        }

        /// <summary>
        /// Called by the debugger to request notification when the first wait operation
        ///             (await, Wait, Result, etc.) on this builder's task completes.
        ///
        /// </summary>
        /// <param name="enabled">true to enable notification; false to disable a previously set notification.
        ///             </param>
        /// <remarks>
        /// This should only be invoked from within an asynchronous method,
        ///             and only by the debugger.
        ///
        /// </remarks>
        internal void SetNotificationForWaitCompletion(bool enabled)
        {
            // Should not be static
            GC.KeepAlive(enabled);
        }

        /// <summary>
        /// Completes the builder by using either the supplied completed task, or by completing
        ///             the builder's previously accessed task using default(TResult).
        ///
        /// </summary>
        /// <param name="completedTask">A task already completed with the value default(TResult).</param><exception cref="T:System.InvalidOperationException">The task has already completed.</exception>
        internal void SetResult(TaskCompletionSource<TResult> completedTask)
        {
            if (_task == null)
            {
                _task = completedTask;
            }
            else
            {
                SetResult(default(TResult));
            }
        }

        /// <summary>
        /// Gets a task for the specified result.  This will either
        ///             be a cached or new task, never null.
        ///
        /// </summary>
        /// <param name="result">The result for which we need a task.</param>
        /// <returns>
        /// The completed task containing the result.
        /// </returns>
        private static TaskCompletionSource<TResult> GetTaskForResult(TResult result)
        {
            var asyncMethodTaskCache = AsyncMethodTaskCache<TResult>.Singleton;
            if (asyncMethodTaskCache == null)
            {
                return AsyncMethodTaskCache<TResult>.CreateCompleted(result);
            }

            return asyncMethodTaskCache.FromResult(result);
        }
    }
}

#endif