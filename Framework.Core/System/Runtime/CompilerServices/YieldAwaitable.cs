#if LESSTHAN_NET45

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1815 // Override equals and operator equals on value types

using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides an awaitable context for switching into a target environment.</summary>
    /// <remarks>This type is intended for compiler use only.</remarks>
    public readonly struct YieldAwaitable
    {
        /// <summary>Gets an awaiter for this <see cref="YieldAwaitable" />.</summary>
        /// <returns>An awaiter for this awaitable.</returns>
        /// <remarks>This method is intended for compiler user rather than use directly in code.</remarks>
        public YieldAwaiter GetAwaiter()
        {
            // Should not be static
            return new YieldAwaiter();
        }

        /// <summary>Provides an awaiter that switches into a target environment.</summary>
        /// <remarks>This type is intended for compiler use only.</remarks>
        [HostProtection(Synchronization = true, ExternalThreading = true)]
        public readonly struct YieldAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>WaitCallback that invokes the Action supplied as object state.</summary>
            private static readonly WaitCallback _waitCallbackRunAction = RunAction;

            /// <summary>Gets whether a yield is not required.</summary>
            /// <remarks>This property is intended for compiler user rather than use directly in code.</remarks>
            public bool IsCompleted => false;

            /// <summary>Ends the await operation.</summary>
            public void GetResult()
            {
                // Should not be static
                // Empty
            }

            /// <summary>Posts the <paramref name="continuation" /> back to the current context.</summary>
            /// <param name="continuation">The action to invoke asynchronously.</param>
            /// <exception cref="ArgumentNullException">
            ///     The <paramref name="continuation" /> argument is null (Nothing in Visual
            ///     Basic).
            /// </exception>
            [SecuritySafeCritical]
            public void OnCompleted(Action continuation)
            {
                if (continuation == null)
                {
                    throw new ArgumentNullException(nameof(continuation));
                }

                if (TaskScheduler.Current == TaskScheduler.Default)
                {
                    ThreadPool.QueueUserWorkItem(_waitCallbackRunAction, continuation);
                }
                else
                {
                    _ = Task.Factory.StartNew(continuation, default, TaskCreationOptions.PreferFairness, TaskScheduler.Current);
                }
            }

            /// <summary>Posts the <paramref name="continuation" /> back to the current context.</summary>
            /// <param name="continuation">The action to invoke asynchronously.</param>
            /// <exception cref="ArgumentNullException">
            ///     The <paramref name="continuation" /> argument is null (Nothing in Visual
            ///     Basic).
            /// </exception>
            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation)
            {
                if (continuation == null)
                {
                    throw new ArgumentNullException(nameof(continuation));
                }

                if (TaskScheduler.Current == TaskScheduler.Default)
                {
                    ThreadPool.UnsafeQueueUserWorkItem(_waitCallbackRunAction, continuation);
                }
                else
                {
                    _ = Task.Factory.StartNew(continuation, default, TaskCreationOptions.PreferFairness, TaskScheduler.Current);
                }
            }

            /// <summary>Runs an Action delegate provided as state.</summary>
            /// <param name="state">The Action delegate to invoke.</param>
            private static void RunAction(object state)
            {
                ((Action)state)();
            }
        }
    }
}

#endif