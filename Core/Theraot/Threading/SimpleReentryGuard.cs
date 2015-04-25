using System;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    /// <summary>
    /// Represents a scheduler to execute operationg without reentry.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class SimpleReentryGuard : IDisposable
    {
        private StructNeedle<TrackingThreadLocal<int>> _guards;

        /// <summary>
        /// Creates a new instance of <see cref="SimpleReentryGuard"/>.
        /// </summary>
        public SimpleReentryGuard()
        {
            _guards = new StructNeedle<TrackingThreadLocal<int>>(new TrackingThreadLocal<int>());
        }

        /// <summary>
        /// Returns whatever or not the current thread did enter.
        /// </summary>
        public bool IsTaken
        {
            get
            {
                return _guards.Value.Value > 0;
            }
        }

        public void Dispose()
        {
            if (_guards.Value.Value > 0)
            {
                _guards.Value.Value--;
            }
        }

        public bool Enter()
        {
            var value = ++_guards.Value.Value;
            if (value > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Executes an operation-
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>Returns a promise to finish the execution.</returns>
        public void Execute(Action operation)
        {
            var local = _guards.Value;
            ThrowIfReentrant();
            try
            {
                local.Value++;
                operation();
            }
            finally
            {
                local.Value--;
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
            var local = _guards.Value;
            ThrowIfReentrant();
            try
            {
                local.Value++;
                return operation();
            }
            finally
            {
                local.Value--;
            }
        }

        public void ThrowIfReentrant()
        {
            if (_guards.Value.Value > 0)
            {
                throw new InvalidOperationException("Reentry");
            }
        }

        public bool TryEnter()
        {
            if (_guards.Value.Value == 0)
            {
                _guards.Value.Value++;
                return true;
            }
            return false;
        }
    }
}