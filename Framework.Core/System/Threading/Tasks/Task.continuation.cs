#if LESSTHAN_NET40

#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
#pragma warning disable CA1068 // CancellationToken parameters must come last

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        private const int _continuationsInitialization = 1;
        private const int _continuationsNotInitialized = 0;
        private const int _runningContinuations = 3;
        private const int _takingContinuations = 2;
        private List<object> _continuations;
        private Thread _continuationsOwner;
        private int _continuationsStatus;

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        public Task ContinueWith(Action<Task> continuationAction)
        {
            return ContinueWith(continuationAction, TaskScheduler.Current, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="Threading.CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        public Task ContinueWith(Action<Task> continuationAction, CancellationToken cancellationToken)
        {
            return ContinueWith(continuationAction, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task" /> completes.  When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        public Task ContinueWith(Action<Task> continuationAction, TaskScheduler scheduler)
        {
            return ContinueWith(continuationAction, scheduler, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed. If the continuation criteria specified through the <paramref name="continuationOptions" /> parameter are
        ///     not met, the continuation task will be canceled
        ///     instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        public Task ContinueWith(Action<Task> continuationAction, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith(continuationAction, TaskScheduler.Current, default, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its
        ///     execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed. If the criteria specified through the <paramref name="continuationOptions" /> parameter
        ///     are not met, the continuation task will be canceled instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="Threading.CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        public Task ContinueWith(Action<Task> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return ContinueWith(continuationAction, scheduler, cancellationToken, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task as and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        public Task ContinueWith(Action<Task, object> continuationAction, object state)
        {
            return ContinueWith(continuationAction, state, TaskScheduler.Current, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="Threading.CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        public Task ContinueWith(Action<Task, object> continuationAction, object state, CancellationToken cancellationToken)
        {
            return ContinueWith(continuationAction, state, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task" /> completes.  When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        public Task ContinueWith(Action<Task, object> continuationAction, object state, TaskScheduler scheduler)
        {
            return ContinueWith(continuationAction, state, scheduler, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed. If the continuation criteria specified through the <paramref name="continuationOptions" /> parameter are
        ///     not met, the continuation task will be canceled
        ///     instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        public Task ContinueWith(Action<Task, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith(continuationAction, state, TaskScheduler.Current, default, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its
        ///     execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed. If the criteria specified through the <paramref name="continuationOptions" /> parameter
        ///     are not met, the continuation task will be canceled instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="Threading.CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        public Task ContinueWith(Action<Task, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return ContinueWith(continuationAction, state, scheduler, cancellationToken, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TResult}" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction)
        {
            return ContinueWith(continuationFunction, TaskScheduler.Current, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TResult}" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="Threading.CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction, CancellationToken cancellationToken)
        {
            return ContinueWith(continuationFunction, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task" /> completes.  When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TResult}" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction, TaskScheduler scheduler)
        {
            return ContinueWith(continuationFunction, scheduler, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TResult}" /> will not be scheduled for execution until the current task has
        ///     completed. If the continuation criteria specified through the <paramref name="continuationOptions" /> parameter are
        ///     not met, the continuation task will be canceled
        ///     instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith(continuationFunction, TaskScheduler.Current, default, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its
        ///     execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TResult}" /> will not be scheduled for execution until the current task has
        ///     completed. If the criteria specified through the <paramref name="continuationOptions" /> parameter
        ///     are not met, the continuation task will be canceled instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="Threading.CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return ContinueWith(continuationFunction, scheduler, cancellationToken, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation function.</param>
        /// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TResult}" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        public Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state)
        {
            return ContinueWith(continuationFunction, state, TaskScheduler.Current, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation function.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TResult}" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="Threading.CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        public Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
        {
            return ContinueWith(continuationFunction, state, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task" /> completes.  When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation function.</param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TResult}" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        public Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
        {
            return ContinueWith(continuationFunction, state, scheduler, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation function.</param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TResult}" /> will not be scheduled for execution until the current task has
        ///     completed. If the continuation criteria specified through the <paramref name="continuationOptions" /> parameter are
        ///     not met, the continuation task will be canceled
        ///     instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        public Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith(continuationFunction, state, TaskScheduler.Current, default, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task" /> completes.
        /// </summary>
        /// <typeparam name="TResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation function.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its
        ///     execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TResult}" /> will not be scheduled for execution until the current task has
        ///     completed. If the criteria specified through the <paramref name="continuationOptions" /> parameter
        ///     are not met, the continuation task will be canceled instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="Threading.CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        public Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return ContinueWith(continuationFunction, state, scheduler, cancellationToken, continuationOptions);
        }

        /// <summary>
        ///     Converts TaskContinuationOptions to TaskCreationOptions, and also does
        ///     some validity checking along the way.
        /// </summary>
        /// <param name="continuationOptions">Incoming TaskContinuationOptions</param>
        /// <param name="creationOptions">Outgoing TaskCreationOptions</param>
        /// <param name="internalOptions">Outgoing InternalTaskOptions</param>
        internal static void CreationOptionsFromContinuationOptions(TaskContinuationOptions continuationOptions, out TaskCreationOptions creationOptions, out InternalTaskOptions internalOptions)
        {
            // This is used a couple of times below
            const TaskContinuationOptions NotOnAnything = TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnRanToCompletion;
            const TaskContinuationOptions CreationOptionsMask = TaskContinuationOptions.PreferFairness | TaskContinuationOptions.LongRunning | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.HideScheduler | TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.RunContinuationsAsynchronously;
            // Check that LongRunning and ExecuteSynchronously are not specified together
            const TaskContinuationOptions IllegalMask = TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.LongRunning;

            if ((continuationOptions & IllegalMask) == IllegalMask)
            {
                throw new ArgumentOutOfRangeException(nameof(continuationOptions), "The specified TaskContinuationOptions combined LongRunning and ExecuteSynchronously.  Synchronous continuations should not be long running.");
            }

            // Check that no illegal options were specified
            if ((continuationOptions & ~(CreationOptionsMask | NotOnAnything | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.ExecuteSynchronously)) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(continuationOptions));
            }

            // Check that we didn't specify "not on anything"
            if ((continuationOptions & NotOnAnything) == NotOnAnything)
            {
                throw new ArgumentOutOfRangeException(nameof(continuationOptions), "The specified TaskContinuationOptions excluded all continuation kinds.");
            }

            // This passes over all but LazyCancellation, which has no representation in TaskCreationOptions
            creationOptions = (TaskCreationOptions)(continuationOptions & CreationOptionsMask);
            // internalOptions has at least ContinuationTask ...
            internalOptions = InternalTaskOptions.ContinuationTask;
            // ... and possibly LazyCancellation
            if ((continuationOptions & TaskContinuationOptions.LazyCancellation) != 0)
            {
                internalOptions |= InternalTaskOptions.LazyCancellation;
            }
        }

        internal void ContinueWithCore(Task continuationTask, TaskScheduler scheduler, CancellationToken cancellationToken, TaskContinuationOptions options)
        {
            Contract.Requires(continuationTask != null, "Task.ContinueWithCore(): null continuationTask");
            Contract.Requires(scheduler != null, "Task.ContinueWithCore(): null scheduler");
            Contract.Requires(!continuationTask.IsCompleted, "Did not expect continuationTask to be completed");
            // Create a TaskContinuation
            TaskContinuation continuation = new StandardTaskContinuation(continuationTask, options, scheduler);
            // If cancellationToken is cancellable, then assign it.
            if (cancellationToken.CanBeCanceled)
            {
                if (IsCompleted || cancellationToken.IsCancellationRequested)
                {
                    // If the antecedent has completed, then we will not be queuing up
                    // the continuation in the antecedent's continuation list.  Likewise,
                    // if the cancellationToken has been canceled, continuationTask will
                    // be completed in the AssignCancellationToken call below, and there
                    // is no need to queue the continuation to the antecedent's continuation
                    // list.  In either of these two cases, we will pass "null" for the antecedent,
                    // meaning "the cancellation callback should not attempt to remove the
                    // continuation from its antecedent's continuation list".
                    continuationTask.AssignCancellationToken(cancellationToken, null, null);
                }
                else
                {
                    // The antecedent is not yet complete, so there is a pretty good chance
                    // that the continuation will be queued up in the antecedent.  Assign the
                    // cancellation token with information about the antecedent, so that the
                    // continuation can be dequeued upon the signalling of the token.
                    //
                    // It's possible that the antecedent completes before the call to AddTaskContinuation,
                    // and that is a benign ----.  It just means that the cancellation will result in
                    // a futile search of the antecedent's continuation list.
                    continuationTask.AssignCancellationToken(cancellationToken, this, continuation);
                }
            }

            // In the case of a pre-canceled token, continuationTask will have been completed
            // in a Canceled state by now.  If such is the case, there is no need to go through
            // the motions of queuing up the continuation for eventual execution.
            if (continuationTask.IsCompleted)
            {
                return;
            }

            // Attempt to enqueue the continuation
            var continuationQueued = AddTaskContinuation(continuation, /*addBeforeOthers:*/ false);

            // If the continuation was not queued (because the task completed), then run it now.
            if (!continuationQueued)
            {
                continuation.Run(this, true);
            }
        }

        /// <summary>
        ///     Runs all of the continuations, as appropriate.
        /// </summary>
        internal void FinishContinuations()
        {
            if (Interlocked.CompareExchange(ref _continuationsStatus, _takingContinuations, _continuationsNotInitialized) == _continuationsNotInitialized)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _continuationsStatus, _takingContinuations, _continuationsInitialization) != _continuationsInitialization)
            {
                return;
            }

            var continuations = Interlocked.CompareExchange(ref _continuations, null, null);
            if (continuations == null)
            {
                return;
            }

            // Wait for any concurrent adds or removes to be retired
            try
            {
                var spinWait = new SpinWait();
                LockEnter(spinWait);
                Interlocked.CompareExchange(ref _continuations, null, continuations);
                Volatile.Write(ref _continuationsStatus, _runningContinuations);
            }
            finally
            {
                LockExit();
            }

            // Skip synchronous execution of continuations if this task's thread was aborted
            var canInlineContinuations =
                Volatile.Read(ref _threadAbortedManaged) == 0
                && Thread.CurrentThread.ThreadState != ThreadState.AbortRequested
                && (CreationOptions & TaskCreationOptions.RunContinuationsAsynchronously) == 0;

            //
            // Begin processing of continuation list
            //

            var continuationCount = continuations.Count;

            // Fire the asynchronous continuations first ...
            for (var index = 0; index < continuationCount; index++)
            {
                // Synchronous continuation tasks will have the ExecuteSynchronously option,
                // and we're looking for asynchronous tasks...
                if (!(continuations[index] is StandardTaskContinuation tc) || (tc.Options & TaskContinuationOptions.ExecuteSynchronously) != 0)
                {
                    continue;
                }

                continuations[index] = null; // so that we can skip this later
                tc.Run(this, canInlineContinuations);
            }

            // ... and then fire the synchronous continuations (if there are any).
            // This includes ITaskCompletionAction, AwaitTaskContinuations, and
            // Action delegates, which are all by default implicitly synchronous.
            for (var index = 0; index < continuationCount; index++)
            {
                var currentContinuation = continuations[index];
                if (currentContinuation == null)
                {
                    continue;
                }

                continuations[index] = null; // to enable freeing up memory earlier
                switch (currentContinuation)
                {
                    // If the continuation is an Action delegate, it came from an await continuation,
                    // and we should use AwaitTaskContinuation to run it.
                    case Action ad:
                        AwaitTaskContinuation.RunOrScheduleAction(ad, canInlineContinuations, ref InternalCurrent);
                        break;
                    // If it's a TaskContinuation object of some kind, invoke it.
                    // Otherwise, it must be an ITaskCompletionAction, so invoke it.
                    case TaskContinuation tc:
                        // We know that this is a synchronous continuation because the
                        // asynchronous ones have been weeded out
                        tc.Run(this, canInlineContinuations);
                        break;
                    default:
                    {
                        Contract.Assert(currentContinuation is ITaskCompletionAction, "Expected continuation element to be Action, TaskContinuation, or ITaskContinuationAction");
                        var action = (ITaskCompletionAction)currentContinuation;
                        action.Invoke(this);
                        break;
                    }
                }
            }
        }

        internal void RemoveContinuation(object continuationObject) // could be TaskContinuation or Action<Task>
        {
            try
            {
                var continuations = GetContinuations();
                if (continuations == null || Status == TaskStatus.RanToCompletion)
                {
                    return;
                }

                var index = continuations.IndexOf(continuationObject);
                if (index != -1)
                {
                    continuations[index] = null;
                }
            }
            finally
            {
                LockExit();
            }
        }

        // Record a continuation task or action.
        // Return true if and only if we successfully queued a continuation.
        private bool AddTaskContinuation(object continuationObject, bool addBeforeOthers)
        {
            Contract.Requires(continuationObject != null);
            try
            {
                var continuations = RetrieveContinuations();
                if (continuations == null || Status == TaskStatus.RanToCompletion)
                {
                    return false;
                }

                if (addBeforeOthers)
                {
                    continuations.Insert(0, continuationObject);
                }
                else
                {
                    continuations.Add(continuationObject);
                }

                return true;
            }
            finally
            {
                LockExit();
            }
        }

        // Same as the above overload, just with a stack mark parameter.
        private Task ContinueWith(Action<Task> continuationAction, TaskScheduler scheduler, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions)
        {
            // Throw on continuation with null action
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            // Throw on continuation with null TaskScheduler
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            Contract.EndContractBlock();
            CreationOptionsFromContinuationOptions(continuationOptions, out var creationOptions, out var internalOptions);
            Task continuationTask = new ContinuationTaskFromTask
            (
                this,
                continuationAction,
                null,
                creationOptions,
                internalOptions
            );
            // Register the continuation.  If synchronous execution is requested, this may
            // actually invoke the continuation before returning.
            ContinueWithCore(continuationTask, scheduler, cancellationToken, continuationOptions);
            return continuationTask;
        }

        // Same as the above overload, just with a stack mark parameter.
        private Task ContinueWith(Action<Task, object> continuationAction, object state, TaskScheduler scheduler, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions)
        {
            // Throw on continuation with null action
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            // Throw on continuation with null TaskScheduler
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            Contract.EndContractBlock();
            CreationOptionsFromContinuationOptions(continuationOptions, out var creationOptions, out var internalOptions);
            Task continuationTask = new ContinuationTaskFromTask
            (
                this,
                continuationAction,
                state,
                creationOptions,
                internalOptions
            );
            // Register the continuation.  If synchronous execution is requested, this may
            // actually invoke the continuation before returning.
            ContinueWithCore(continuationTask, scheduler, cancellationToken, continuationOptions);
            return continuationTask;
        }

        // Same as the above overload, just with a stack mark parameter.
        private Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction, TaskScheduler scheduler, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions)
        {
            // Throw on continuation with null function
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            // Throw on continuation with null task scheduler
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            Contract.EndContractBlock();
            CreationOptionsFromContinuationOptions(continuationOptions, out var creationOptions, out var internalOptions);
            Task<TResult> continuationTask = new ContinuationResultTaskFromTask<TResult>
            (
                this, continuationFunction, null,
                creationOptions,
                internalOptions
            );
            // Register the continuation.  If synchronous execution is requested, this may
            // actually invoke the continuation before returning.
            ContinueWithCore(continuationTask, scheduler, cancellationToken, continuationOptions);
            return continuationTask;
        }

        // Same as the above overload, just with a stack mark parameter.
        private Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state, TaskScheduler scheduler, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions)
        {
            // Throw on continuation with null function
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            // Throw on continuation with null task scheduler
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            Contract.EndContractBlock();
            CreationOptionsFromContinuationOptions(continuationOptions, out var creationOptions, out var internalOptions);
            Task<TResult> continuationTask = new ContinuationResultTaskFromTask<TResult>
            (
                this, continuationFunction, state,
                creationOptions, internalOptions
            );
            // Register the continuation.  If synchronous execution is requested, this may
            // actually invoke the continuation before returning.
            ContinueWithCore(continuationTask, scheduler, cancellationToken, continuationOptions);
            return continuationTask;
        }

        private List<object> GetContinuations()
        {
            if (IsCompleted)
            {
                return null;
            }

            if (Volatile.Read(ref _continuationsStatus) != _continuationsInitialization)
            {
                return null;
            }

            // Initializing or initialized
            var spinWait = new SpinWait();
            List<object> continuations;
            while ((continuations = Interlocked.CompareExchange(ref _continuations, null, null)) == null)
            {
                spinWait.SpinOnce();
            }

            LockEnter(spinWait);
            return Volatile.Read(ref _continuationsStatus) == _continuationsInitialization ? continuations : null;
        }

        private void LockEnter(SpinWait spinWait)
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref _continuations, null, null) == null)
                {
                    return;
                }

                var thread = Interlocked.CompareExchange(ref _continuationsOwner, Thread.CurrentThread, null);
                if (thread == null)
                {
                    return;
                }

                spinWait.SpinOnce();
            }
        }

        private void LockExit()
        {
            Interlocked.CompareExchange(ref _continuationsOwner, null, Thread.CurrentThread);
        }

        private List<object> RetrieveContinuations()
        {
            if (IsCompleted)
            {
                return null;
            }

            List<object> continuations = null;
            var found = Volatile.Read(ref _continuationsStatus);
            var spinWait = new SpinWait();
            switch (found)
            {
                case _continuationsNotInitialized:
                    // Not initialized
                    if (Interlocked.CompareExchange(ref _continuationsStatus, _continuationsInitialization, _continuationsNotInitialized) == _continuationsNotInitialized)
                    {
                        var created = new List<object>();
                        continuations = Interlocked.CompareExchange(ref _continuations, created, null) ?? created;
                        goto default;
                    }

                    goto case _continuationsInitialization;
                case _continuationsInitialization:
                    // Initializing or initialized
                    while (Volatile.Read(ref _continuationsStatus) == _continuationsInitialization && (continuations = Interlocked.CompareExchange(ref _continuations, null, null)) == null)
                    {
                        spinWait.SpinOnce();
                    }

                    if (Volatile.Read(ref _continuationsStatus) == _takingContinuations)
                    {
                        return null;
                    }

                    goto default;
                case _takingContinuations:
                case _runningContinuations:
                    // Being taken for execution, or
                    // Already taken for execution
                    return null;

                default:
                    // The continuations may have already executed at this point
                    LockEnter(spinWait);
                    if (Volatile.Read(ref _continuationsStatus) == _continuationsInitialization)
                    {
                        return continuations;
                    }

                    // It is being taken or has already been taken for execution
                    return null;
            }
        }
    }

    public partial class Task<TResult>
    {
        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task ContinueWith(Action<Task<TResult>> continuationAction)
        {
            return ContinueWithInternal(continuationAction, TaskScheduler.Current, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task ContinueWith(Action<Task<TResult>> continuationAction, CancellationToken cancellationToken)
        {
            return ContinueWithInternal(continuationAction, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task ContinueWith(Action<Task<TResult>> continuationAction, TaskScheduler scheduler)
        {
            return ContinueWithInternal(continuationAction, scheduler, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed. If the continuation criteria specified through the <paramref name="continuationOptions" /> parameter are
        ///     not met, the continuation task will be canceled
        ///     instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task ContinueWith(Action<Task<TResult>> continuationAction, TaskContinuationOptions continuationOptions)
        {
            return ContinueWithInternal(continuationAction, TaskScheduler.Current, default, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its
        ///     execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed. If the criteria specified through the <paramref name="continuationOptions" /> parameter
        ///     are not met, the continuation task will be canceled instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task ContinueWith(Action<Task<TResult>> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return ContinueWithInternal(continuationAction, scheduler, cancellationToken, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task ContinueWith(Action<Task<TResult>, object> continuationAction, object state)
        {
            return ContinueWithInternal(continuationAction, state, TaskScheduler.Current, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task ContinueWith(Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken)
        {
            return ContinueWithInternal(continuationAction, state, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task ContinueWith(Action<Task<TResult>, object> continuationAction, object state, TaskScheduler scheduler)
        {
            return ContinueWithInternal(continuationAction, state, scheduler, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed. If the continuation criteria specified through the <paramref name="continuationOptions" /> parameter are
        ///     not met, the continuation task will be canceled
        ///     instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task ContinueWith(Action<Task<TResult>, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWithInternal(continuationAction, state, TaskScheduler.Current, default, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <param name="continuationAction">
        ///     An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken" /> that will be assigned to the new continuation
        ///     task.
        /// </param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its
        ///     execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task" /> will not be scheduled for execution until the current task has
        ///     completed. If the criteria specified through the <paramref name="continuationOptions" /> parameter
        ///     are not met, the continuation task will be canceled instead of scheduled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationAction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task ContinueWith(Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return ContinueWithInternal(continuationAction, state, scheduler, cancellationToken, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <typeparam name="TNewResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TNewResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TNewResult}" /> will not be scheduled for execution until the current
        ///     task has completed, whether it completes due to running to completion successfully, faulting due
        ///     to an unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction)
        {
            return ContinueWithInternal(continuationFunction, TaskScheduler.Current, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <typeparam name="TNewResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> that will be assigned to the new task.</param>
        /// <returns>A new continuation <see cref="Task{TNewResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TNewResult}" /> will not be scheduled for execution until the current
        ///     task has completed, whether it completes due to running to completion successfully, faulting due
        ///     to an unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction, CancellationToken cancellationToken)
        {
            return ContinueWithInternal(continuationFunction, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <typeparam name="TNewResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task{TResult}" /> completes.  When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TNewResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TNewResult}" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction, TaskScheduler scheduler)
        {
            return ContinueWithInternal(continuationFunction, scheduler, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <typeparam name="TNewResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task as an argument.
        /// </param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TNewResult}" />.</returns>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="Task{TNewResult}" /> will not be scheduled for execution until the current
        ///         task has completed, whether it completes due to running to completion successfully, faulting due
        ///         to an unhandled exception, or exiting out early due to being canceled.
        ///     </para>
        ///     <para>
        ///         The <paramref name="continuationFunction" />, when executed, should return a <see cref="Task{TNewResult}" />.
        ///         This task's completion state will be transferred to the task returned
        ///         from the ContinueWith call.
        ///     </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            return ContinueWithInternal(continuationFunction, TaskScheduler.Current, default, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <typeparam name="TNewResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be passed as
        ///     an argument this completed task.
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> that will be assigned to the new task.</param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its
        ///     execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TNewResult}" />.</returns>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="Task{TNewResult}" /> will not be scheduled for execution until the current task has
        ///         completed, whether it completes due to running to completion successfully, faulting due to an
        ///         unhandled exception, or exiting out early due to being canceled.
        ///     </para>
        ///     <para>
        ///         The <paramref name="continuationFunction" />, when executed, should return a <see cref="Task{TNewResult}" />.
        ///         This task's completion state will be transferred to the task returned from the
        ///         ContinueWith call.
        ///     </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return ContinueWithInternal(continuationFunction, scheduler, cancellationToken, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <typeparam name="TNewResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation function.</param>
        /// <returns>A new continuation <see cref="Task{TNewResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TNewResult}" /> will not be scheduled for execution until the current
        ///     task has completed, whether it completes due to running to completion successfully, faulting due
        ///     to an unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state)
        {
            return ContinueWithInternal(continuationFunction, state, TaskScheduler.Current, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <typeparam name="TNewResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation function.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> that will be assigned to the new task.</param>
        /// <returns>A new continuation <see cref="Task{TNewResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TNewResult}" /> will not be scheduled for execution until the current
        ///     task has completed, whether it completes due to running to completion successfully, faulting due
        ///     to an unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state, CancellationToken cancellationToken)
        {
            return ContinueWithInternal(continuationFunction, state, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <typeparam name="TNewResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task{TResult}" /> completes.  When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation function.</param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TNewResult}" />.</returns>
        /// <remarks>
        ///     The returned <see cref="Task{TNewResult}" /> will not be scheduled for execution until the current task has
        ///     completed, whether it completes due to running to completion successfully, faulting due to an
        ///     unhandled exception, or exiting out early due to being canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state, TaskScheduler scheduler)
        {
            return ContinueWithInternal(continuationFunction, state, scheduler, default, TaskContinuationOptions.None);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <typeparam name="TNewResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation function.</param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TNewResult}" />.</returns>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="Task{TNewResult}" /> will not be scheduled for execution until the current
        ///         task has completed, whether it completes due to running to completion successfully, faulting due
        ///         to an unhandled exception, or exiting out early due to being canceled.
        ///     </para>
        ///     <para>
        ///         The <paramref name="continuationFunction" />, when executed, should return a <see cref="Task{TNewResult}" />.
        ///         This task's completion state will be transferred to the task returned
        ///         from the ContinueWith call.
        ///     </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWithInternal(continuationFunction, state, TaskScheduler.Current, default, continuationOptions);
        }

        /// <summary>
        ///     Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.
        /// </summary>
        /// <typeparam name="TNewResult">
        ///     The type of the result produced by the continuation.
        /// </typeparam>
        /// <param name="continuationFunction">
        ///     A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be
        ///     passed the completed task and the caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation function.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" /> that will be assigned to the new task.</param>
        /// <param name="continuationOptions">
        ///     Options for when the continuation is scheduled and how it behaves. This includes criteria, such
        ///     as <see cref="TaskContinuationOptions.OnlyOnCanceled">OnlyOnCanceled</see>, as
        ///     well as execution options, such as
        ///     <see cref="TaskContinuationOptions.ExecuteSynchronously">ExecuteSynchronously</see>.
        /// </param>
        /// <param name="scheduler">
        ///     The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its
        ///     execution.
        /// </param>
        /// <returns>A new continuation <see cref="Task{TNewResult}" />.</returns>
        /// <remarks>
        ///     <para>
        ///         The returned <see cref="Task{TNewResult}" /> will not be scheduled for execution until the current task has
        ///         completed, whether it completes due to running to completion successfully, faulting due to an
        ///         unhandled exception, or exiting out early due to being canceled.
        ///     </para>
        ///     <para>
        ///         The <paramref name="continuationFunction" />, when executed, should return a <see cref="Task{TNewResult}" />.
        ///         This task's completion state will be transferred to the task returned from the
        ///         ContinueWith call.
        ///     </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="continuationFunction" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="continuationOptions" /> argument specifies an invalid value for
        ///     <see cref="T:System.Threading.Tasks.TaskContinuationOptions">TaskContinuationOptions</see>.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="scheduler" /> argument is null.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="CancellationToken">CancellationToken</see>
        ///     has already been disposed.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoInlining)] // Methods containing StackCrawlMark local var have to be marked non-inlineable
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return ContinueWithInternal(continuationFunction, state, scheduler, cancellationToken, continuationOptions);
        }

        // Same as the above overload, only with a stack mark.
        internal Task ContinueWithInternal(Action<Task<TResult>> continuationAction, TaskScheduler scheduler, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            CreationOptionsFromContinuationOptions
            (
                continuationOptions,
                out var creationOptions,
                out var internalOptions
            );
            Task continuationTask = new ContinuationTaskFromResultTask<TResult>
            (
                this,
                continuationAction,
                null,
                creationOptions,
                internalOptions
            );
            // Register the continuation.  If synchronous execution is requested, this may
            // actually invoke the continuation before returning.
            ContinueWithCore(continuationTask, scheduler, cancellationToken, continuationOptions);
            return continuationTask;
        }

        // Same as the above overload, only with a stack mark.
        internal Task ContinueWithInternal(Action<Task<TResult>, object> continuationAction, object state, TaskScheduler scheduler, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            CreationOptionsFromContinuationOptions
            (
                continuationOptions,
                out var creationOptions,
                out var internalOptions
            );
            Task continuationTask = new ContinuationTaskFromResultTask<TResult>
            (
                this,
                continuationAction,
                state,
                creationOptions,
                internalOptions
            );
            // Register the continuation.  If synchronous execution is requested, this may
            // actually invoke the continuation before returning.
            ContinueWithCore(continuationTask, scheduler, cancellationToken, continuationOptions);
            return continuationTask;
        }

        // Same as the above overload, just with a stack mark.
        internal Task<TNewResult> ContinueWithInternal<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction, TaskScheduler scheduler, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            CreationOptionsFromContinuationOptions
            (
                continuationOptions,
                out var creationOptions,
                out var internalOptions
            );

            Task<TNewResult> continuationFuture = new ContinuationResultTaskFromResultTask<TResult, TNewResult>
            (
                this, continuationFunction, null,
                creationOptions, internalOptions
            );

            // Register the continuation.  If synchronous execution is requested, this may
            // actually invoke the continuation before returning.
            ContinueWithCore(continuationFuture, scheduler, cancellationToken, continuationOptions);

            return continuationFuture;
        }

        // Same as the above overload, just with a stack mark.
        internal Task<TNewResult> ContinueWithInternal<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state, TaskScheduler scheduler, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            CreationOptionsFromContinuationOptions
            (
                continuationOptions,
                out var creationOptions,
                out var internalOptions
            );

            Task<TNewResult> continuationFuture = new ContinuationResultTaskFromResultTask<TResult, TNewResult>
            (
                this, continuationFunction, state,
                creationOptions, internalOptions
            );

            // Register the continuation.  If synchronous execution is requested, this may
            // actually invoke the continuation before returning.
            ContinueWithCore(continuationFuture, scheduler, cancellationToken, continuationOptions);

            return continuationFuture;
        }
    }
}

#endif