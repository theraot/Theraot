#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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