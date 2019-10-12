#if LESSTHAN_NET40 || NETSTANDARD1_0

#pragma warning disable CA1001 // Types that own disposable fields should be disposable

// BASEDON: https://raw.githubusercontent.com/dotnet/corefx/e0ba7aa8026280ee3571179cc06431baf1dfaaac/src/System.Threading.Tasks.Parallel/src/System/Threading/Tasks/TaskReplicator.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using Theraot.Reflection;

namespace System.Threading.Tasks
{
    //
    // TaskReplicator runs a delegate inside of one or more Tasks, concurrently.  The idea is to exploit "available"
    // parallelism, where "available" is determined by the TaskScheduler.  We always keep one Task queued to
    // the scheduler, and if it starts running we queue another one, etc., up to some (potentially) user-defined
    // limit.
    //
    internal sealed class TaskReplicator
    {
        public delegate void ReplicableUserAction<TState>(ref TState replicaState, int timeout, out bool yieldedBeforeCompletion);

        private const int _cooperativeMultitaskingTaskTimeoutIncrement = 50; // millisecond

        private const int _cooperativeMultitaskingTaskTimeoutMin = 100; // millisecond
        private const int _cooperativeMultitaskingTaskTimeoutRootTask = int.MaxValue / 2;

        private readonly ConcurrentQueue<Replica> _pendingReplicas = new ConcurrentQueue<Replica>();

        private readonly TaskScheduler _scheduler;
        private readonly bool _stopOnFirstFailure;
        private ConcurrentQueue<Exception>? _exceptions;
        private bool _stopReplicating;

        private TaskReplicator(ParallelOptions options, bool stopOnFirstFailure)
        {
            _scheduler = options.TaskScheduler ?? TaskScheduler.Current;
            _stopOnFirstFailure = stopOnFirstFailure;
        }

        public static void Run<TState>(ReplicableUserAction<TState> action, ParallelOptions options, bool stopOnFirstFailure)
        {
            var maxConcurrencyLevel = options.EffectiveMaxConcurrencyLevel > 0 ? options.EffectiveMaxConcurrencyLevel : int.MaxValue;

            var replicator = new TaskReplicator(options, stopOnFirstFailure);
            new Replica<TState>(replicator, maxConcurrencyLevel, _cooperativeMultitaskingTaskTimeoutRootTask, action).Start();

            while (replicator._pendingReplicas.TryDequeue(out var nextReplica))
            {
                nextReplica.Wait();
            }

            if (replicator._exceptions != null)
            {
                throw new AggregateException(replicator._exceptions);
            }
        }

        private static int GenerateCooperativeMultitaskingTaskTimeout()
        {
            // This logic ensures that we have a diversity of timeouts across worker tasks (100, 150, 200, 250, 100, etc)
            // Otherwise all worker will try to timeout at precisely the same point, which is bad if the work is just about to finish.
            var period = Environment.ProcessorCount;
            var pseudoRnd = Environment.TickCount;
            return _cooperativeMultitaskingTaskTimeoutMin + (pseudoRnd % period * _cooperativeMultitaskingTaskTimeoutIncrement);
        }

        private abstract class Replica
        {
            protected readonly TaskReplicator Replicator;
            protected readonly int Timeout;
            protected volatile Task? PendingTask; // the most recently queued Task for this replica, or null if we're done.
            protected int RemainingConcurrency;

            protected Replica(TaskReplicator replicator, int maxConcurrency, int timeout)
            {
                Replicator = replicator;
                Timeout = timeout;
                RemainingConcurrency = maxConcurrency - 1;
                PendingTask = new Task(Execute);
                Replicator._pendingReplicas.Enqueue(this);
            }

            public void Start()
            {
                PendingTask!.RunSynchronously(Replicator._scheduler);
            }

            public void Wait()
            {
                //
                // We wait in a loop because each Task might queue another Task, and so on.
                // It's entirely possible for multiple Tasks to be queued without this loop seeing them,
                // but that's fine, since we really only need to know when all of them have finished.
                //
                // Note that it's *very* important that we use Task.Wait here, rather than waiting on some
                // other synchronization primitive.  Task.Wait can "inline" the Task's execution, on this thread,
                // if it hasn't started running on another thread.  That's essential for preventing deadlocks,
                // in the case where all other threads are blocked for other reasons.
                //
                Task? pendingTask;
                while ((pendingTask = PendingTask) != null)
                {
                    pendingTask.Wait();
                }
            }

            private void Execute()
            {
                try
                {
                    if (!Replicator._stopReplicating && RemainingConcurrency > 0)
                    {
                        CreateNewReplica();
                        RemainingConcurrency = 0; // new replica is responsible for adding concurrency from now on.
                    }

                    ExecuteAction(out var userActionYieldedBeforeCompletion);

                    if (userActionYieldedBeforeCompletion)
                    {
                        PendingTask = new Task(Execute);
                        PendingTask.Start(Replicator._scheduler);
                    }
                    else
                    {
                        Replicator._stopReplicating = true;
                        PendingTask = null;
                    }
                }
                catch (Exception ex)
                {
                    TypeHelper.LazyCreate(ref Replicator._exceptions).Enqueue(ex);
                    if (Replicator._stopOnFirstFailure)
                    {
                        Replicator._stopReplicating = true;
                    }

                    PendingTask = null;
                }
            }

            protected abstract void CreateNewReplica();

            protected abstract void ExecuteAction(out bool yieldedBeforeCompletion);
        }

        private sealed class Replica<TState> : Replica
        {
            private readonly ReplicableUserAction<TState> _action;
            private TState _state = default!;

            public Replica(TaskReplicator replicator, int maxConcurrency, int timeout, ReplicableUserAction<TState> action)
                : base(replicator, maxConcurrency, timeout)
            {
                _action = action;
            }

            protected override void CreateNewReplica()
            {
                var newReplica = new Replica<TState>(Replicator, RemainingConcurrency, GenerateCooperativeMultitaskingTaskTimeout(), _action);
                newReplica.PendingTask!.Start(Replicator._scheduler);
            }

            protected override void ExecuteAction(out bool yieldedBeforeCompletion)
            {
                _action(ref _state, Timeout, out yieldedBeforeCompletion);
            }
        }
    }
}

#endif