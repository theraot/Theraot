using System;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    /// <summary>
    /// Represents a scheduler to execute operationg without reentry.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class SimpleReentryGuard
    {
        private StructNeedle<NoTrackingThreadLocal<Guard>> _guards;

        /// <summary>
        /// Creates a new instance of <see cref="SimpleReentryGuard"/>.
        /// </summary>
        public SimpleReentryGuard()
        {
            _guards = new StructNeedle<NoTrackingThreadLocal<Guard>>
                (
                    new NoTrackingThreadLocal<Guard>
                    (
                        () => new Guard()
                    )
                );
        }

        /// <summary>
        /// Returns whatever or not the current thread did enter.
        /// </summary>
        public bool IsTaken
        {
            get
            {
                var local = _guards.Value.Value;
                return local.IsTaken;
            }
        }

        /// <summary>
        /// Executes an operation-
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>Returns a promise to finish the execution.</returns>
        public void Execute(Action operation)
        {
            var local = _guards.Value.Value;
            IDisposable engagement;
            if (local.Enter(out engagement))
            {
                using (engagement)
                {
                    operation.Invoke();
                }
            }
            else
            {
                throw new InvalidOperationException("Reentry");
            }
        }

        /// <summary>
        /// Executes an operation-
        /// </summary>
        /// <typeparam name="T">The return value of the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>Returns a promise to finish the execution.</returns>
        public T Execute<T>(Func<T> operation)
        {
            var local = _guards.Value.Value;
            IDisposable engagement;
            if (local.Enter(out engagement))
            {
                using (engagement)
                {
                    return operation();
                }
            }
            throw new InvalidOperationException("Reentry");
        }
    }
}