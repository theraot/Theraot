// Needed for Workaround

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Collections.ThreadSafe;
using Theraot.Reflection;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    /// <summary>
    ///     Represents a context to execute operation without reentry.
    /// </summary>
    [DebuggerNonUserCode]
    public sealed class ReentryGuard
    {
        [ThreadStatic]
        private static HashSet<UniqueId>? _guard;

        private ThreadSafeQueue<Action>? _workQueue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReentryGuard" /> class.
        /// </summary>
        public ReentryGuard()
        {
            Id = RuntimeUniqueIdProvider.GetNextId();
        }

        /// <summary>
        ///     Gets a value indicating whether or not the current thread did enter.
        /// </summary>
        public bool IsTaken => _guard?.Contains(Id) == true;

        internal UniqueId Id { get; }

        private ThreadSafeQueue<Action> WorkQueue => TypeHelper.LazyCreate(ref _workQueue, () => new ThreadSafeQueue<Action>());

        /// <summary>
        ///     Executes an operation-
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>Returns a promise to finish the execution.</returns>
        public IPromise Execute(Action operation)
        {
            var workQueue = WorkQueue;
            var result = AddExecution(operation, workQueue);
            ExecutePending(workQueue, Id);
            return result;
        }

        /// <summary>
        ///     Executes an operation-
        /// </summary>
        /// <typeparam name="T">The return value of the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>Returns a promise to finish the execution.</returns>
        public IPromise<T> Execute<T>(Func<T> operation)
        {
            var workQueue = WorkQueue;
            var result = AddExecution(operation, WorkQueue);
            ExecutePending(workQueue, Id);
            return result;
        }

        public IDisposable TryEnter(out bool didEnter)
        {
            didEnter = Enter(Id);
            return didEnter ? DisposableAkin.Create(() => Leave(Id)) : NoOpDisposable.Instance;
        }

        internal static bool Enter(UniqueId id)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (GCMonitor.FinalizingForUnload)
            {
                return false;
            }

            var guard = _guard;
            if (guard == null)
            {
                _guard = new HashSet<UniqueId>
                {
                    id
                };
                return true;
            }

            if (guard.Contains(id))
            {
                return false;
            }

            guard.Add(id);
            return true;
        }

        internal static void Leave(UniqueId id)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var guard = _guard;
            guard?.Remove(id);
        }

        private static IPromise AddExecution(Action action, ThreadSafeQueue<Action> queue)
        {
            var promised = new Promise(done: false);
            var result = new ReadOnlyPromise(promised);
            queue.Add
            (
                () =>
                {
                    try
                    {
                        action.Invoke();
                        promised.SetCompleted();
                    }
                    catch (Exception exception)
                    {
                        promised.SetError(exception);
                    }
                }
            );
            return result;
        }

        private static IPromise<T> AddExecution<T>(Func<T> action, ThreadSafeQueue<Action> queue)
        {
            var promised = new PromiseNeedle<T>(done: false);
            var result = new ReadOnlyPromiseNeedle<T>(promised);
            queue.Add
            (
                () =>
                {
                    try
                    {
                        promised.Value = action.Invoke();
                    }
                    catch (Exception exception)
                    {
                        promised.SetError(exception);
                    }
                }
            );
            return result;
        }

        private static void ExecutePending(ThreadSafeQueue<Action> queue, UniqueId id)
        {
            var didEnter = false;
            try
            {
                didEnter = Enter(id);
                if (!didEnter)
                {
                    return;
                }

                while (queue.TryTake(out var action))
                {
                    action.Invoke();
                }
            }
            finally
            {
                if (didEnter)
                {
                    Leave(id);
                }
            }
        }
    }
}