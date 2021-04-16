#if LESSTHAN_NET45

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CA1815 // Override equals and operator equals on value types

using System.Diagnostics;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    ///     Provides a builder for asynchronous methods that return void.
    ///     This type is intended for compiler use only.
    /// </summary>
    public struct AsyncVoidMethodBuilder : IAsyncMethodBuilder
    {
        /// <summary>
        ///     Non-zero if PreventUnobservedTaskExceptions has already been invoked.
        /// </summary>
        private static int _preventUnobservedTaskExceptionsInvoked;

        /// <summary>
        ///     The synchronization context associated with this operation.
        /// </summary>
        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>
        ///     State related to the IAsyncStateMachine.
        /// </summary>
        private AsyncMethodBuilderCore _coreState;

        /// <summary>
        ///     Temporary support for disabling crashing if tasks go unobserved.
        /// </summary>
        static AsyncVoidMethodBuilder()
        {
            PreventUnobservedTaskExceptions();
        }

        /// <summary>
        ///     Initializes the <see cref="AsyncVoidMethodBuilder" />.
        /// </summary>
        /// <param name="synchronizationContext">The synchronizationContext associated with this operation. This may be null.</param>
        private AsyncVoidMethodBuilder(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
            synchronizationContext?.OperationStarted();

            _coreState = new AsyncMethodBuilderCore();
        }

        /// <summary>
        ///     Initializes a new <see cref="AsyncVoidMethodBuilder" />.
        /// </summary>
        /// <returns>
        ///     The initialized <see cref="AsyncVoidMethodBuilder" />.
        /// </returns>
        public static AsyncVoidMethodBuilder Create()
        {
            return new AsyncVoidMethodBuilder(SynchronizationContext.Current);
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

        readonly void IAsyncMethodBuilder.PreBoxInitialization()
        {
            // Empty
        }

        /// <summary>
        ///     Faults the method builder with an exception.
        /// </summary>
        /// <param name="exception">The exception that is the cause of this fault.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="exception" /> argument is null (Nothing in Visual
        ///     Basic).
        /// </exception>
        /// <exception cref="InvalidOperationException">The builder is not initialized.</exception>
        public readonly void SetException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (_synchronizationContext != null)
            {
                try
                {
                    AsyncMethodBuilderCore.ThrowOnContext(exception, _synchronizationContext);
                }
                finally
                {
                    NotifySynchronizationContextOfCompletion();
                }
            }
            else
            {
                AsyncMethodBuilderCore.ThrowOnContext(exception, targetContext: null);
            }
        }

        /// <summary>
        ///     Completes the method builder successfully.
        /// </summary>
        public readonly void SetResult()
        {
            if (_synchronizationContext == null)
            {
                return;
            }

            NotifySynchronizationContextOfCompletion();
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
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="stateMachine" /> argument was null (Nothing in
        ///     Visual Basic).
        /// </exception>
        [DebuggerStepThrough]
        public readonly void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            // Should not be static
            AsyncMethodBuilderCore.Start(ref stateMachine);
        }

        /// <summary>
        ///     Registers with UnobservedTaskException to suppress exception crashing.
        /// </summary>
        internal static void PreventUnobservedTaskExceptions()
        {
            try
            {
                if (Interlocked.CompareExchange(ref _preventUnobservedTaskExceptionsInvoked, 1, 0) != 0)
                {
                    return;
                }

                TaskScheduler.UnobservedTaskException += (_, e) => e.SetObserved();
            }
            catch (Exception ex)
            {
                _ = ex;
            }
        }

        /// <summary>
        ///     Notifies the current synchronization context that the operation completed.
        /// </summary>
        private readonly void NotifySynchronizationContextOfCompletion()
        {
            try
            {
                _synchronizationContext.OperationCompleted();
            }
            catch (Exception ex)
            {
                AsyncMethodBuilderCore.ThrowOnContext(ex, targetContext: null);
            }
        }
    }
}

#endif