#if LESSTHAN_NET45

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CS8653 // A default expression introduces a null value when 'T' is a non-nullable reference type.
#pragma warning disable CS0649 // Field is never assigned

using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;
using Theraot;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    ///     Provides a builder for asynchronous methods that return <see cref="Threading.Tasks.Task" />.
    ///     This type is intended for compiler use only.
    /// </summary>
    /// <remarks>
    ///     AsyncTaskMethodBuilder is a value type, and thus it is copied by value.
    ///     Prior to being copied, one of its Task, SetResult, or SetException members must be accessed,
    ///     or else the copies may end up building distinct Task instances.
    /// </remarks>
    public struct AsyncTaskMethodBuilder : IAsyncMethodBuilder
    {
        /// <summary>
        ///     A cached VoidStruct task used for builders that complete synchronously.
        /// </summary>
        private static readonly TaskCompletionSource<VoidStruct> _cachedCompleted = AsyncTaskMethodBuilder<VoidStruct>.DefaultResultTask;

        /// <summary>
        ///     The generic builder object to which this non-generic instance delegates.
        /// </summary>
        // Note: this is a mutable struct, it has starts at the default value and then it is mutated
        // There is no need to assign it
        // It should not be readonly
        private AsyncTaskMethodBuilder<VoidStruct> _builder;

        /// <summary>
        ///     Gets the <see cref="Threading.Tasks.Task" /> for this builder.
        /// </summary>
        /// <returns>
        ///     The <see cref="Threading.Tasks.Task" /> representing the builder's asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidOperationException">The builder is not initialized.</exception>
        public readonly Task Task => _builder.Task;

        /// <summary>
        ///     Initializes a new <see cref="AsyncTaskMethodBuilder" />.
        /// </summary>
        /// <returns>
        ///     The initialized <see cref="AsyncTaskMethodBuilder" />.
        /// </returns>
        public static AsyncTaskMethodBuilder Create()
        {
            return new AsyncTaskMethodBuilder();
        }

        /// <summary>
        ///     Schedules the specified state machine to be pushed forward when the specified awaiter completes.
        /// </summary>
        /// <typeparam name="TAwaiter">Specifies the type of the awaiter.</typeparam>
        /// <typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam>
        /// <param name="awaiter">The awaiter.</param>
        /// <param name="stateMachine">The state machine.</param>
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        /// <summary>
        ///     Schedules the specified state machine to be pushed forward when the specified awaiter completes.
        /// </summary>
        /// <typeparam name="TAwaiter">Specifies the type of the awaiter.</typeparam>
        /// <typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam>
        /// <param name="awaiter">The awaiter.</param>
        /// <param name="stateMachine">The state machine.</param>
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }

        readonly void IAsyncMethodBuilder.PreBoxInitialization()
        {
            GC.KeepAlive(Task);
        }

        /// <summary>
        ///     Completes the <see cref="Threading.Tasks.Task" /> in the
        ///     <see cref="TaskStatus">Faulted</see> state with the specified exception.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to use to fault the task.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="exception" /> argument is null (Nothing in Visual
        ///     Basic).
        /// </exception>
        /// <exception cref="InvalidOperationException">The builder is not initialized.</exception>
        /// <exception cref="InvalidOperationException">The task has already completed.</exception>
        public void SetException(Exception exception)
        {
            _builder.SetException(exception);
        }

        /// <summary>
        ///     Completes the <see cref="Threading.Tasks.Task" /> in the
        ///     <see cref="TaskStatus">RanToCompletion</see> state.
        /// </summary>
        /// <exception cref="InvalidOperationException">The builder is not initialized.</exception>
        /// <exception cref="InvalidOperationException">The task has already completed.</exception>
        public void SetResult()
        {
            _builder.SetResult(_cachedCompleted);
        }

        /// <summary>
        ///     Associates the builder with the state machine it represents.
        /// </summary>
        /// <param name="stateMachine">The heap-allocated state machine object.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="stateMachine" /> argument was null (Nothing in
        ///     Visual Basic).
        /// </exception>
        /// <exception cref="InvalidOperationException">The builder is incorrectly initialized.</exception>
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _builder.SetStateMachine(stateMachine);
        }

        /// <summary>
        ///     Initiates the builder's execution with the associated state machine.
        /// </summary>
        /// <typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam>
        /// <param name="stateMachine">The state machine instance, passed by reference.</param>
        [DebuggerStepThrough]
        public readonly void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _builder.Start(ref stateMachine);
        }

        /// <summary>
        ///     Called by the debugger to request notification when the first wait operation
        ///     (await, Wait, Result, etc.) on this builder's task completes.
        /// </summary>
        /// <param name="enabled">
        ///     true to enable notification; false to disable a previously set notification.
        /// </param>
        internal readonly void SetNotificationForWaitCompletion(bool enabled)
        {
            _builder.SetNotificationForWaitCompletion(enabled);
        }
    }

    /// <summary>
    ///     Provides a builder for asynchronous methods that return <see cref="Task{TResult}" />.
    ///     This type is intended for compiler use only.
    /// </summary>
    /// <remarks>
    ///     AsyncTaskMethodBuilder{TResult} is a value type, and thus it is copied by value.
    ///     Prior to being copied, one of its Task, SetResult, or SetException members must be accessed,
    ///     or else the copies may end up building distinct Task instances.
    /// </remarks>
    public struct AsyncTaskMethodBuilder<TResult> : IAsyncMethodBuilder
    {
        /// <summary>
        ///     A cached task for default(TResult).
        /// </summary>
        internal static readonly TaskCompletionSource<TResult> DefaultResultTask = AsyncMethodTaskCache.CreateCompleted<TResult>(default!);

        /// <summary>
        ///     State related to the IAsyncStateMachine.
        /// </summary>
        // Note: this is a mutable struct, it has starts at the default value and then it is mutated
        // There is no need to assign it
        // It should not be readonly
        private AsyncMethodBuilderCore _coreState;

        /// <summary>
        ///     The lazily-initialized task completion source.
        /// </summary>
        private TaskCompletionSource<TResult>? _task;

        /// <summary>
        ///     Temporary support for disabling crashing if tasks go unobserved.
        /// </summary>
        static AsyncTaskMethodBuilder()
        {
            AsyncVoidMethodBuilder.PreventUnobservedTaskExceptions();
        }

        /// <summary>
        ///     Gets the <see cref="Task{TResult}" /> for this builder.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task{TResult}" /> representing the builder's asynchronous operation.
        /// </returns>
        public Task<TResult> Task => CompletionSource.Task;

        /// <summary>
        ///     Gets the lazily-initialized TaskCompletionSource.
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
        ///     Initializes a new <see cref="AsyncTaskMethodBuilder" />.
        /// </summary>
        /// <returns>
        ///     The initialized <see cref="AsyncTaskMethodBuilder" />.
        /// </returns>
        public static AsyncTaskMethodBuilder<TResult> Create()
        {
            return new AsyncTaskMethodBuilder<TResult>();
        }

        /// <summary>
        ///     Schedules the specified state machine to be pushed forward when the specified awaiter completes.
        /// </summary>
        /// <typeparam name="TAwaiter">Specifies the type of the awaiter.</typeparam>
        /// <typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam>
        /// <param name="awaiter">The awaiter.</param>
        /// <param name="stateMachine">The state machine.</param>
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
                AsyncMethodBuilderCore.ThrowOnContext(ex, targetContext: null);
            }
        }

        /// <summary>
        ///     Schedules the specified state machine to be pushed forward when the specified awaiter completes.
        /// </summary>
        /// <typeparam name="TAwaiter">Specifies the type of the awaiter.</typeparam>
        /// <typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam>
        /// <param name="awaiter">The awaiter.</param>
        /// <param name="stateMachine">The state machine.</param>
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
                AsyncMethodBuilderCore.ThrowOnContext(ex, targetContext: null);
            }
        }

        void IAsyncMethodBuilder.PreBoxInitialization()
        {
            GC.KeepAlive(Task);
        }

        /// <summary>
        ///     Completes the <see cref="Task{TResult}" /> in the
        ///     <see cref="TaskStatus">Faulted</see> state with the specified exception.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to use to fault the task.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="exception" /> argument is null (Nothing in Visual
        ///     Basic).
        /// </exception>
        /// <exception cref="InvalidOperationException">The task has already completed.</exception>
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
        ///     Completes the <see cref="Task{TResult}" /> in the
        ///     <see cref="TaskStatus">RanToCompletion</see> state with the specified result.
        /// </summary>
        /// <param name="result">The result to use to complete the task.</param>
        /// <exception cref="InvalidOperationException">The task has already completed.</exception>
        public void SetResult(TResult result)
        {
            var completionSource = _task;
            if (completionSource == null)
            {
                _task = AsyncMethodTaskCache.CreateCompleted(result);
            }
            else if (!completionSource.TrySetResult(result))
            {
                throw new InvalidOperationException("The Task was already completed.");
            }
        }

        /// <summary>
        ///     Associates the builder with the state machine it represents.
        /// </summary>
        /// <param name="stateMachine">The heap-allocated state machine object.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="stateMachine" /> argument was null (Nothing in
        ///     Visual Basic).
        /// </exception>
        /// <exception cref="InvalidOperationException">The builder is incorrectly initialized.</exception>
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            _coreState.SetStateMachine(stateMachine);
        }

        /// <summary>
        ///     Initiates the builder's execution with the associated state machine.
        /// </summary>
        /// <typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam>
        /// <param name="stateMachine">The state machine instance, passed by reference.</param>
        [DebuggerStepThrough]
        public readonly void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            // Should not be static
            AsyncMethodBuilderCore.Start(ref stateMachine);
        }

        /// <summary>
        ///     Called by the debugger to request notification when the first wait operation
        ///     (await, Wait, Result, etc.) on this builder's task completes.
        /// </summary>
        /// <param name="enabled">
        ///     true to enable notification; false to disable a previously set notification.
        /// </param>
        /// <remarks>
        ///     This should only be invoked from within an asynchronous method,
        ///     and only by the debugger.
        /// </remarks>
        internal readonly void SetNotificationForWaitCompletion(bool enabled)
        {
            // Should not be static
            _ = enabled;
        }

        /// <summary>
        ///     Completes the builder by using either the supplied completed task, or by completing
        ///     the builder's previously accessed task using default(TResult).
        /// </summary>
        /// <param name="completedTask">A task already completed with the value default(TResult).</param>
        /// <exception cref="InvalidOperationException">The task has already completed.</exception>
        internal void SetResult(TaskCompletionSource<TResult> completedTask)
        {
            if (_task == null)
            {
                _task = completedTask;
            }
            else
            {
                SetResult(default(TResult)!);
            }
        }
    }
}

#endif