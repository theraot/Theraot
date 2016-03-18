#if NET20 || NET30 || NET35

// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// TaskScheduler.cs
//
// <OWNER>[....]</OWNER>
//
// This file contains the primary interface and management of tasks and queues.  
//

using System;

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
    private readonly AggregateException _exception;
    private bool _observed;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnobservedTaskExceptionEventArgs"/> class
    /// with the unobserved exception.
    /// </summary>
    /// <param name="exception">The Exception that has gone unobserved.</param>
    public UnobservedTaskExceptionEventArgs(AggregateException exception) { _exception = exception; }

    /// <summary>
    /// Marks the <see cref="Exception"/> as "observed," thus preventing it
    /// from triggering exception escalation policy which, by default, terminates the process.
    /// </summary>
    public void SetObserved() { _observed = true; }

    /// <summary>
    /// Gets whether this exception has been marked as "observed."
    /// </summary>
    public bool Observed { get { return _observed; } }

    /// <summary>
    /// The Exception that went unobserved.
    /// </summary>
    public AggregateException Exception { get { return _exception; } }
}

#endif