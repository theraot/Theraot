#if NET20 || NET30 || NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Theraot.Threading;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Provides data for the event that is raised when a faulted <see cref="System.Threading.Tasks.Task"/>'s
    /// exception goes unobserved.
    /// </summary>
    /// <remarks>
    /// The Exception property is used to examine the exception without marking it
    /// as observed, whereas the <see cref="SetObserved"/> method is used to mark the exception
    /// as observed.  Marking the exception as observed prevents it from triggering exception escalation policy
    /// which, by default, terminates the process.
    /// </remarks>
    public class UnobservedTaskExceptionEventArgs : EventArgs
    {
        private AggregateException m_exception;
        internal bool m_observed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnobservedTaskExceptionEventArgs"/> class
        /// with the unobserved exception.
        /// </summary>
        /// <param name="exception">The Exception that has gone unobserved.</param>
        public UnobservedTaskExceptionEventArgs(AggregateException exception) { m_exception = exception; }

        /// <summary>
        /// Marks the <see cref="Exception"/> as "observed," thus preventing it
        /// from triggering exception escalation policy which, by default, terminates the process.
        /// </summary>
        public void SetObserved() { m_observed = true; }

        /// <summary>
        /// Gets whether this exception has been marked as "observed."
        /// </summary>
        public bool Observed { get { return m_observed; } }

        /// <summary>
        /// The Exception that went unobserved.
        /// </summary>
        public AggregateException Exception { get { return m_exception; } }
    }
}

#endif