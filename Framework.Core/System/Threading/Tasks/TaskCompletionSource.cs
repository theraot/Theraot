#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Security.Permissions;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks
{
    public class TaskCompletionSource
    {
        private readonly StructNeedle<Task> _task;

        public TaskCompletionSource()
            : this(null, TaskCreationOptions.None)
        {
            // Empty
        }

        public TaskCompletionSource(object state)
            : this(state, TaskCreationOptions.None)
        {
            // Empty
        }

        public TaskCompletionSource(TaskCreationOptions creationOptions)
            : this(null, creationOptions)
        {
            // Empty
        }

        public TaskCompletionSource(object? state, TaskCreationOptions creationOptions)
        {
            _task = new StructNeedle<Task>(new Task(creationOptions, state));
        }

        public Task Task => _task.Value;

        public void SetCanceled()
        {
            if (!TrySetCanceled())
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public void SetCanceled(CancellationToken cancellationToken)
        {
            if (!TrySetCanceled(cancellationToken))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public void SetException(IEnumerable<Exception> exceptions)
        {
            if (!TrySetException(exceptions))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public void SetException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (!TrySetException(exception))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public void SetResult()
        {
            if (!TrySetResult())
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public bool TrySetCanceled()
        {
            return TrySetCanceled(CancellationTokenSource.CanceledSource.Token);
        }

        public bool TrySetCanceled(CancellationToken cancellationToken)
        {
            var value = _task.Value.TrySetCanceled(cancellationToken);
            if (!value && !_task.Value.IsCompleted)
            {
                SpinUntilCompleted();
            }

            return value;
        }

        public bool TrySetException(IEnumerable<Exception> exceptions)
        {
            if (exceptions == null)
            {
                throw new ArgumentNullException(nameof(exceptions));
            }

            var defensiveCopy = new List<Exception>();
            foreach (var e in exceptions)
            {
                if (e == null)
                {
                    throw new ArgumentException("The exceptions collection included at least one null element.", nameof(exceptions));
                }

                defensiveCopy.Add(e);
            }

            if (defensiveCopy.Count == 0)
            {
                throw new ArgumentException("The exceptions collection was empty.", nameof(exceptions));
            }

            var value = _task.Value.TrySetException(defensiveCopy);
            if (!value && !_task.Value.IsCompleted)
            {
                SpinUntilCompleted();
            }

            return value;
        }

        public bool TrySetException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var value = _task.Value.TrySetException(exception);
            if (!value && !_task.Value.IsCompleted)
            {
                SpinUntilCompleted();
            }

            return value;
        }

        public bool TrySetResult()
        {
            var value = _task.Value.TrySetResult();
            if (!value && !_task.Value.IsCompleted)
            {
                SpinUntilCompleted();
            }

            return value;
        }

        private void SpinUntilCompleted()
        {
            // Spin wait until the completion is finalized by another thread.
            var sw = new SpinWait();
            while (!_task.Value.IsCompleted)
            {
                sw.SpinOnce();
            }
        }
    }

    /// <summary>
    ///     Represents the producer side of a <see cref="Task{TResult}" /> unbound to a
    ///     delegate, providing access to the consumer side through the <see cref="Task" /> property.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         It is often the case that a <see cref="Task{TResult}" /> is desired to
    ///         represent another asynchronous operation.
    ///         <see cref="TaskCompletionSource{TResult}">TaskCompletionSource</see> is provided for this purpose. It enables
    ///         the creation of a task that can be handed out to consumers, and those consumers can use the members
    ///         of the task as they would any other. However, unlike most tasks, the state of a task created by a
    ///         TaskCompletionSource is controlled explicitly by the methods on TaskCompletionSource. This enables the
    ///         completion of the external asynchronous operation to be propagated to the underlying Task. The
    ///         separation also ensures that consumers are not able to transition the state without access to the
    ///         corresponding TaskCompletionSource.
    ///     </para>
    ///     <para>
    ///         All members of <see cref="TaskCompletionSource{TResult}" /> are thread-safe
    ///         and may be used from multiple threads concurrently.
    ///     </para>
    /// </remarks>
    /// <typeparam name="TResult">
    ///     The type of the result value associated with this
    ///     <see cref="TaskCompletionSource{TResult}" />.
    /// </typeparam>
    [HostProtection(Synchronization = true, ExternalThreading = true)]
    public class TaskCompletionSource<TResult>
    {
        private readonly StructNeedle<Task<TResult>> _task;

        /// <summary>
        ///     Creates a <see cref="TaskCompletionSource{TResult}" />.
        /// </summary>
        public TaskCompletionSource()
            : this(null, TaskCreationOptions.None)
        {
            // Empty
        }

        /// <summary>
        ///     Creates a <see cref="TaskCompletionSource{TResult}" />
        ///     with the specified options.
        /// </summary>
        /// <remarks>
        ///     The <see cref="Task{TResult}" /> created
        ///     by this instance and accessible through its <see cref="TaskCompletionSource{TResult}.Task" />
        ///     property
        ///     will be instantiated using the specified <paramref name="creationOptions" />.
        /// </remarks>
        /// <param name="creationOptions">
        ///     The options to use when creating the underlying
        ///     <see cref="Task{TResult}" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The <paramref name="creationOptions" /> represent options invalid for use
        ///     with a <see cref="TaskCompletionSource{TResult}" />.
        /// </exception>
        public TaskCompletionSource(TaskCreationOptions creationOptions)
            : this(null, creationOptions)
        {
            // Empty
        }

        /// <summary>
        ///     Creates a <see cref="TaskCompletionSource{TResult}" />
        ///     with the specified state.
        /// </summary>
        /// <param name="state">
        ///     The state to use as the underlying
        ///     <see cref="Task{TResult}" />'s AsyncState.
        /// </param>
        public TaskCompletionSource(object state)
            : this(state, TaskCreationOptions.None)
        {
            // Empty
        }

        /// <summary>
        ///     Creates a <see cref="TaskCompletionSource{TResult}" /> with
        ///     the specified state and options.
        /// </summary>
        /// <param name="state">
        ///     The state to use as the underlying
        ///     <see cref="Task{TResult}" />'s AsyncState.
        /// </param>
        /// <param name="creationOptions">
        ///     The options to use when creating the underlying
        ///     <see cref="Task{TResult}" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The <paramref name="creationOptions" /> represent options invalid for use
        ///     with a <see cref="TaskCompletionSource{TResult}" />.
        /// </exception>
        public TaskCompletionSource(object? state, TaskCreationOptions creationOptions)
        {
            _task = new StructNeedle<Task<TResult>>(new Task<TResult>(creationOptions, state));
        }

        /// <summary>
        ///     Gets the <see cref="Task{TResult}" /> created
        ///     by this <see cref="TaskCompletionSource{TResult}" />.
        /// </summary>
        /// <remarks>
        ///     This property enables a consumer access to the <see cref="Task{TResult}" /> that is
        ///     controlled by this instance.
        ///     The <see cref="SetResult" />, <see cref="SetException(Exception)" />,
        ///     <see cref="SetException(IEnumerable{Exception})" />, and
        ///     <see cref="SetCanceled()" />
        ///     methods (and their "Try" variants) on this instance all result in the relevant state
        ///     transitions on this underlying Task.
        /// </remarks>
        public Task<TResult> Task => _task.Value;

        /// <summary>
        ///     Transitions the underlying
        ///     <see cref="Task{TResult}" /> into the
        ///     <see cref="TaskStatus.Canceled">Canceled</see>
        ///     state.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The underlying <see cref="Task{TResult}" /> is already in one
        ///     of the three final states:
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>,
        ///     <see cref="TaskStatus.Faulted">Faulted</see>, or
        ///     <see cref="TaskStatus.Canceled">Canceled</see>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Task" /> was disposed.</exception>
        public void SetCanceled()
        {
            if (!TrySetCanceled())
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public void SetCanceled(CancellationToken cancellationToken)
        {
            if (!TrySetCanceled(cancellationToken))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        /// <summary>
        ///     Transitions the underlying
        ///     <see cref="Task{TResult}" /> into the
        ///     <see cref="TaskStatus.Faulted">Faulted</see>
        ///     state.
        /// </summary>
        /// <param name="exception">The exception to bind to this <see cref="Task{TResult}" />.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> argument is null.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The underlying <see cref="Task{TResult}" /> is already in one
        ///     of the three final states:
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>,
        ///     <see cref="TaskStatus.Faulted">Faulted</see>, or
        ///     <see cref="TaskStatus.Canceled">Canceled</see>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Task" /> was disposed.</exception>
        public void SetException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (!TrySetException(exception))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        /// <summary>
        ///     Transitions the underlying
        ///     <see cref="Task{TResult}" /> into the
        ///     <see cref="TaskStatus.Faulted">Faulted</see>
        ///     state.
        /// </summary>
        /// <param name="exceptions">
        ///     The collection of exceptions to bind to this
        ///     <see cref="Task{TResult}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptions" /> argument is null.</exception>
        /// <exception cref="ArgumentException">There are one or more null elements in <paramref name="exceptions" />.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The underlying <see cref="Task{TResult}" /> is already in one
        ///     of the three final states:
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>,
        ///     <see cref="TaskStatus.Faulted">Faulted</see>, or
        ///     <see cref="TaskStatus.Canceled">Canceled</see>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Task" /> was disposed.</exception>
        public void SetException(IEnumerable<Exception> exceptions)
        {
            if (!TrySetException(exceptions))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        /// <summary>
        ///     Transitions the underlying
        ///     <see cref="Task{TResult}" /> into the
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>
        ///     state.
        /// </summary>
        /// <param name="result">The result value to bind to this <see cref="Task{TResult}" />.</param>
        /// <exception cref="InvalidOperationException">
        ///     The underlying <see cref="Task{TResult}" /> is already in one
        ///     of the three final states:
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>,
        ///     <see cref="TaskStatus.Faulted">Faulted</see>, or
        ///     <see cref="TaskStatus.Canceled">Canceled</see>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Task" /> was disposed.</exception>
        public void SetResult(TResult result)
        {
            if (!TrySetResult(result))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        /// <summary>
        ///     Attempts to transition the underlying
        ///     <see cref="Task{TResult}" /> into the
        ///     <see cref="TaskStatus.Canceled">Canceled</see>
        ///     state.
        /// </summary>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        /// <remarks>
        ///     This operation will return false if the
        ///     <see cref="Task{TResult}" /> is already in one
        ///     of the three final states:
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>,
        ///     <see cref="TaskStatus.Faulted">Faulted</see>, or
        ///     <see cref="TaskStatus.Canceled">Canceled</see>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The <see cref="Task" /> was disposed.</exception>
        public bool TrySetCanceled()
        {
            return TrySetCanceled(CancellationTokenSource.CanceledSource.Token);
        }

        /// <summary>
        ///     Attempts to transition the underlying
        ///     <see cref="Task{TResult}" /> into the
        ///     <see cref="TaskStatus.Canceled">Canceled</see>
        ///     state.
        ///     Enables a token to be stored into the canceled task
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        /// <remarks>
        ///     This operation will return false if the
        ///     <see cref="Task{TResult}" /> is already in one
        ///     of the three final states:
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>,
        ///     <see cref="TaskStatus.Faulted">Faulted</see>, or
        ///     <see cref="TaskStatus.Canceled">Canceled</see>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The <see cref="Task" /> was disposed.</exception>
        public bool TrySetCanceled(CancellationToken cancellationToken)
        {
            var value = _task.Value.TrySetCanceled(cancellationToken);
            if (!value && !_task.Value.IsCompleted)
            {
                SpinUntilCompleted();
            }

            return value;
        }

        /// <summary>
        ///     Attempts to transition the underlying
        ///     <see cref="Task{TResult}" /> into the
        ///     <see cref="TaskStatus.Faulted">Faulted</see>
        ///     state.
        /// </summary>
        /// <param name="exception">The exception to bind to this <see cref="Task{TResult}" />.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        /// <remarks>
        ///     This operation will return false if the
        ///     <see cref="Task{TResult}" /> is already in one
        ///     of the three final states:
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>,
        ///     <see cref="TaskStatus.Faulted">Faulted</see>, or
        ///     <see cref="TaskStatus.Canceled">Canceled</see>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> argument is null.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Task" /> was disposed.</exception>
        public bool TrySetException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var value = _task.Value.TrySetException(exception);
            if (!value && !_task.Value.IsCompleted)
            {
                SpinUntilCompleted();
            }

            return value;
        }

        /// <summary>
        ///     Attempts to transition the underlying
        ///     <see cref="Task{TResult}" /> into the
        ///     <see cref="TaskStatus.Faulted">Faulted</see>
        ///     state.
        /// </summary>
        /// <param name="exceptions">
        ///     The collection of exceptions to bind to this
        ///     <see cref="Task{TResult}" />.
        /// </param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        /// <remarks>
        ///     This operation will return false if the
        ///     <see cref="Task{TResult}" /> is already in one
        ///     of the three final states:
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>,
        ///     <see cref="TaskStatus.Faulted">Faulted</see>, or
        ///     <see cref="TaskStatus.Canceled">Canceled</see>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptions" /> argument is null.</exception>
        /// <exception cref="ArgumentException">There are one or more null elements in <paramref name="exceptions" />.</exception>
        /// <exception cref="ArgumentException">The <paramref name="exceptions" /> collection is empty.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Task" /> was disposed.</exception>
        public bool TrySetException(IEnumerable<Exception> exceptions)
        {
            if (exceptions == null)
            {
                throw new ArgumentNullException(nameof(exceptions));
            }

            var defensiveCopy = new List<Exception>();
            foreach (var e in exceptions)
            {
                if (e == null)
                {
                    throw new ArgumentException("The exceptions collection included at least one null element.", nameof(exceptions));
                }

                defensiveCopy.Add(e);
            }

            if (defensiveCopy.Count == 0)
            {
                throw new ArgumentException("The exceptions collection was empty.", nameof(exceptions));
            }

            var value = _task.Value.TrySetException(defensiveCopy);
            if (!value && !_task.Value.IsCompleted)
            {
                SpinUntilCompleted();
            }

            return value;
        }

        /// <summary>
        ///     Attempts to transition the underlying
        ///     <see cref="Task{TResult}" /> into the
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>
        ///     state.
        /// </summary>
        /// <param name="result">The result value to bind to this <see cref="Task{TResult}" />.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        /// <remarks>
        ///     This operation will return false if the
        ///     <see cref="Task{TResult}" /> is already in one
        ///     of the three final states:
        ///     <see cref="TaskStatus.RanToCompletion">RanToCompletion</see>,
        ///     <see cref="TaskStatus.Faulted">Faulted</see>, or
        ///     <see cref="TaskStatus.Canceled">Canceled</see>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The <see cref="Task" /> was disposed.</exception>
        public bool TrySetResult(TResult result)
        {
            var value = _task.Value.TrySetResult(result!);
            if (!value && !_task.Value.IsCompleted)
            {
                SpinUntilCompleted();
            }

            return value;
        }

        /// <summary>Spins until the underlying task is completed.</summary>
        /// <remarks>This should only be called if the task is in the process of being completed by another thread.</remarks>
        private void SpinUntilCompleted()
        {
            // Spin wait until the completion is finalized by another thread.
            var sw = new SpinWait();
            while (!_task.Value.IsCompleted)
            {
                sw.SpinOnce();
            }
        }
    }
}

#elif TARGETS_NET || LESSTHAN_NET50 || TARGETS_NETSTANDARD

using System.Collections.Generic;
using Theraot;

namespace System.Threading.Tasks
{
    public class TaskCompletionSource
    {
        private readonly TaskCompletionSource<VoidStruct> _wrapped;

        public TaskCompletionSource()
            : this(null, TaskCreationOptions.None)
        {
            // Empty
        }

        public TaskCompletionSource(object state)
            : this(state, TaskCreationOptions.None)
        {
            // Empty
        }

        public TaskCompletionSource(TaskCreationOptions creationOptions)
            : this(null, creationOptions)
        {
            // Empty
        }

        public TaskCompletionSource(object? state, TaskCreationOptions creationOptions)
        {
            _wrapped = new TaskCompletionSource<VoidStruct>(state, creationOptions);
        }

        public Task Task => _wrapped.Task;

        public void SetCanceled()
        {
            if (!TrySetCanceled())
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public void SetCanceled(CancellationToken cancellationToken)
        {
            if (!TrySetCanceled(cancellationToken))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public void SetException(IEnumerable<Exception> exceptions)
        {
            if (!TrySetException(exceptions))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public void SetException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (!TrySetException(exception))
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public void SetResult()
        {
            if (!TrySetResult())
            {
                throw new InvalidOperationException("An attempt was made to transition a task to a final state when it had already completed.");
            }
        }

        public bool TrySetCanceled()
        {
            return _wrapped.TrySetCanceled();
        }

        public bool TrySetCanceled(CancellationToken cancellationToken)
        {
            return _wrapped.TrySetCanceled(cancellationToken);
        }

        public bool TrySetException(IEnumerable<Exception> exceptions)
        {
            return _wrapped.TrySetException(exceptions);
        }

        public bool TrySetException(Exception exception)
        {
            return _wrapped.TrySetException(exception);
        }

        public bool TrySetResult()
        {
            return _wrapped.TrySetResult(default);
        }
    }
}

#endif