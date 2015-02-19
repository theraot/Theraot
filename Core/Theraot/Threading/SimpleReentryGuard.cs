using System;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    /// <summary>
    /// Represents a context to execute operationg without reentry.
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

        ~SimpleReentryGuard()
        {
            var guards = _guards.Value;
            if (!ReferenceEquals(guards, null))
            {
                guards.Dispose();
            }
            _guards.Value = null;
        }

        /// <summary>
        /// Returns whatever or not the current thread did enter.
        /// </summary>
        public bool IsTaken
        {
            get
            {
                var guards = _guards.Value;
                if (guards != null)
                {
                    var local = guards.Value;
                    return local.IsTaken;
                }
                return false;
            }
        }

        /// <summary>
        /// Executes an operation-
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>Returns a promise to finish the execution.</returns>
        public void Execute(Action operation)
        {
            var guards = _guards.Value;
            if (guards != null)
            {
                var local = guards.Value;
                IDisposable engagement;
                if (local.Enter(out engagement))
                {
                    operation.Invoke();
                }
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
            var guards = _guards.Value;
            if (guards != null)
            {
                var local = guards.Value;
                T result = default(T);
                IDisposable engagement;
                if (local.Enter(out engagement))
                {
                    using (engagement)
                    {
                        result = operation();
                    }
                }
                return result;
            }
            // This happens after finalization
            return default(T);
        }
    }
}