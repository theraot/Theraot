#if LESSTHAN_NET40

#pragma warning disable RCS1194 // Implement exception constructors.

using System.Runtime.Serialization;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Threading.Tasks
{
    /// <summary>
    /// Represents an exception used to communicate task cancellation.
    /// </summary>
    [Serializable]
    public class TaskCanceledException : OperationCanceledExceptionEx
    {
        [NonSerialized]
        private readonly Task _canceledTask; // The task which has been canceled.

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Threading.Tasks.TaskCanceledException"/> class.
        /// </summary>
        public TaskCanceledException()
            : base("A task was canceled")
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Threading.Tasks.TaskCanceledException"/>
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public TaskCanceledException(string message)
            : base(message)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Threading.Tasks.TaskCanceledException"/>
        /// class with a specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public TaskCanceledException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Threading.Tasks.TaskCanceledException"/> class
        /// with a reference to the <see cref="T:System.Threading.Tasks.Task"/> that has been canceled.
        /// </summary>
        /// <param name="task">A task that has been canceled.</param>
        public TaskCanceledException(Task task) :
            base("A task was canceled", task?.CancellationToken ?? new CancellationToken())
        {
            _canceledTask = task;
        }

        protected TaskCanceledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Empty
        }

        /// <summary>
        /// Gets the task associated with this exception.
        /// </summary>
        /// <remarks>
        /// It is permissible for no Task to be associated with a
        /// <see cref="T:System.Threading.Tasks.TaskCanceledException"/>, in which case
        /// this property will return null.
        /// </remarks>
        public Task Task => _canceledTask;
    }
}

#endif