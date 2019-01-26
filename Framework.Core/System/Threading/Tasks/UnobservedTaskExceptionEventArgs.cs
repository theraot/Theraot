#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Threading.Tasks
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides data for the event that is raised when a faulted <see cref="T:System.Threading.Tasks.Task" />'s
    ///     exception goes unobserved.
    /// </summary>
    /// <remarks>
    ///     The Exception property is used to examine the exception without marking it
    ///     as observed, whereas the <see cref="M:System.Threading.Tasks.UnobservedTaskExceptionEventArgs.SetObserved" />
    ///     method is used to mark the exception
    ///     as observed.  Marking the exception as observed prevents it from triggering exception escalation policy
    ///     which, by default, terminates the process.
    /// </remarks>
    public class UnobservedTaskExceptionEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Threading.Tasks.UnobservedTaskExceptionEventArgs" /> class
        ///     with the unobserved exception.
        /// </summary>
        /// <param name="exception">The Exception that has gone unobserved.</param>
        public UnobservedTaskExceptionEventArgs(AggregateException exception)
        {
            Exception = exception;
        }

        /// <summary>
        ///     The Exception that went unobserved.
        /// </summary>
        public AggregateException Exception { get; }

        /// <summary>
        ///     Gets whether this exception has been marked as "observed."
        /// </summary>
        public bool Observed { get; set; }

        /// <summary>
        ///     Marks the <see cref="Exception" /> as "observed," thus preventing it
        ///     from triggering exception escalation policy which, by default, terminates the process.
        /// </summary>
        public void SetObserved()
        {
            Observed = true;
        }
    }
}

#endif