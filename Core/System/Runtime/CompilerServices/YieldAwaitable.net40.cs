#if NET20 || NET30 || NET35 || NET40

using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides an awaitable context for switching into a target environment.</summary>
    /// <remarks>This type is intended for compiler use only.</remarks>
    public struct YieldAwaitable
    {
        /// <summary>Gets an awaiter for this <see cref="YieldAwaitable"/>.</summary>
        /// <returns>An awaiter for this awaitable.</returns>
        /// <remarks>This method is intended for compiler user rather than use directly in code.</remarks>
        public YieldAwaiter GetAwaiter()
        {
            return new YieldAwaiter();
        }

        /// <summary>Provides an awaiter that switches into a target environment.</summary>
        /// <remarks>This type is intended for compiler use only.</remarks>
        [HostProtection(Synchronization = true, ExternalThreading = true)]
        public struct YieldAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>WaitCallback that invokes the Action supplied as object state.</summary>
            private static readonly WaitCallback _waitCallbackRunAction = RunAction;

            /// <summary>Gets whether a yield is not required.</summary>
            /// <remarks>This property is intended for compiler user rather than use directly in code.</remarks>
            public bool IsCompleted
            {
                get
                {
                    return false;
                }
            }

            /// <summary>Ends the await operation.</summary>
            public void GetResult()
            {
                // Empty
            }

            /// <summary>Posts the <paramref name="continuation"/> back to the current context.</summary>
            /// <param name="continuation">The action to invoke asynchronously.</param>
            /// <exception cref="System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
            [SecuritySafeCritical]
            public void OnCompleted(Action continuation)
            {
                if (continuation == null)
                {
                    throw new ArgumentNullException("continuation");
                }
                if (TaskScheduler.Current == TaskScheduler.Default)
                {
                    ThreadPool.QueueUserWorkItem(_waitCallbackRunAction, continuation);
                }
                else
                {
                    Task.Factory.StartNew(continuation, default(CancellationToken), TaskCreationOptions.PreferFairness, TaskScheduler.Current);
                }
            }

            /// <summary>Posts the <paramref name="continuation"/> back to the current context.</summary>
            /// <param name="continuation">The action to invoke asynchronously.</param>
            /// <exception cref="System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation)
            {
                if (continuation == null)
                {
                    throw new ArgumentNullException("continuation");
                }
                if (TaskScheduler.Current == TaskScheduler.Default)
                {
                    ThreadPool.UnsafeQueueUserWorkItem(_waitCallbackRunAction, continuation);
                }
                else
                {
                    Task.Factory.StartNew(continuation, default(CancellationToken), TaskCreationOptions.PreferFairness, TaskScheduler.Current);
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