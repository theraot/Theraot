#if FAT
#if NET20 || NET30 || NET35

// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

namespace System.Threading.Tasks
{
    public abstract partial class TaskScheduler
    {
        private static readonly object _unobservedTaskExceptionLockObject = new object();
        private static EventHandler<UnobservedTaskExceptionEventArgs> _unobservedTaskException;

        /// <summary>
        /// Occurs when a faulted <see cref="System.Threading.Tasks.Task"/>'s unobserved exception is about to trigger exception escalation
        /// policy, which, by default, would terminate the process.
        /// </summary>
        /// <remarks>
        /// This AppDomain-wide event provides a mechanism to prevent exception
        /// escalation policy (which, by default, terminates the process) from triggering. 
        /// Each handler is passed a <see cref="T:System.Threading.Tasks.UnobservedTaskExceptionEventArgs"/>
        /// instance, which may be used to examine the exception and to mark it as observed.
        /// </remarks>
        public static event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException
        {
            [Security.SecurityCritical]
            add
            {
                if (value != null)
                {
                    lock (_unobservedTaskExceptionLockObject)
                    {
                        _unobservedTaskException += value;
                    }
                }
            }
            [Security.SecurityCritical]
            remove
            {
                lock (_unobservedTaskExceptionLockObject)
                {
                    _unobservedTaskException -= value;
                }
            }
        }

        internal static void PublishUnobservedTaskException(Task sender, UnobservedTaskExceptionEventArgs ueea)
        {
            // Lock this logic to prevent just-unregistered handlers from being called.
            lock (_unobservedTaskExceptionLockObject)
            {
                // Since we are under lock, it is technically no longer necessary
                // to make a copy.  It is done here for convenience.
                EventHandler<UnobservedTaskExceptionEventArgs> handler = _unobservedTaskException;
                if (handler != null)
                {
                    handler(sender, ueea);
                }
            }
        }
    }
}

#endif
#endif