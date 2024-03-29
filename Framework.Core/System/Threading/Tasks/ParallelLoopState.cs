﻿#if LESSTHAN_NET40 || NETSTANDARD1_0

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable MA0008 // Add StructLayoutAttribute
#pragma warning disable MA0048 // File name must match type name

// BASEDON: https://raw.githubusercontent.com/dotnet/corefx/e0ba7aa8026280ee3571179cc06431baf1dfaaac/src/System.Threading.Tasks.Parallel/src/System/Threading/Tasks/ParallelLoopState.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// A non-generic and generic parallel state class, used by the Parallel helper class
// for parallel loop management.
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System.Diagnostics;

// Prevents compiler warnings/errors regarding the use of ref params in Interlocked methods

namespace System.Threading.Tasks
{
    /// <summary>
    ///     Provides completion status on the execution of a <see cref="Parallel" /> loop.
    /// </summary>
    /// <remarks>
    ///     If <see cref="IsCompleted" /> returns true, then the loop ran to completion, such that all iterations
    ///     of the loop were executed. If <see cref="IsCompleted" /> returns false and
    ///     <see cref="LowestBreakIteration" /> returns null, a call to
    ///     <see cref="ParallelLoopState.Stop" /> was used to end the loop prematurely. If
    ///     <see cref="IsCompleted" /> returns false and <see cref="LowestBreakIteration" /> returns a non-null integral
    ///     value, <see cref="ParallelLoopState.Break()" /> was used to end the loop prematurely.
    /// </remarks>
    public struct ParallelLoopResult
    {
        /// <summary>
        ///     Gets whether the loop ran to completion, such that all iterations of the loop were executed
        ///     and the loop didn't receive a request to end prematurely.
        /// </summary>
        public bool IsCompleted
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets the index of the lowest iteration from which
        ///     <see cref="ParallelLoopState.Break()" />
        ///     was called.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ParallelLoopState.Break()" /> was not employed, this property will
        ///     return null.
        /// </remarks>
        public long? LowestBreakIteration
        {
            get;
            internal set;
        }
    }

    /// <summary>
    ///     Enables iterations of <see cref="Parallel" /> loops to interact with
    ///     other iterations.
    /// </summary>
    [DebuggerDisplay("ShouldExitCurrentIteration = {" + nameof(ShouldExitCurrentIteration) + "}")]
    public class ParallelLoopState
    {
        // Derived classes will track a ParallelStateFlags32 or ParallelStateFlags64.
        // So this is slightly redundant, but it enables us to implement some
        // methods in this base class.
        private readonly ParallelLoopStateFlags _flagsBase;

        internal ParallelLoopState(ParallelLoopStateFlags flagsBase)
        {
            _flagsBase = flagsBase;
        }

        /// <summary>
        ///     Gets whether any iteration of the loop has thrown an exception that went unhandled by that
        ///     iteration.
        /// </summary>
        public bool IsExceptional => (_flagsBase.LoopStateFlags & ParallelLoopStateFlags.ParallelLoopStateExceptional) != 0;

        /// <summary>
        ///     Gets whether any iteration of the loop has called <see cref="Stop()" />.
        /// </summary>
        public bool IsStopped => (_flagsBase.LoopStateFlags & ParallelLoopStateFlags.ParallelLoopStateStopped) != 0;

        /// <summary>
        ///     Gets the lowest iteration of the loop from which <see cref="Break()" /> was called.
        /// </summary>
        /// <remarks>
        ///     If no iteration of the loop called <see cref="Break()" />, this property will return null.
        /// </remarks>
        public long? LowestBreakIteration => InternalLowestBreakIteration;

        /// <summary>
        ///     Gets whether the current iteration of the loop should exit based
        ///     on requests made by this or other iterations.
        /// </summary>
        /// <remarks>
        ///     When an iteration of a loop calls <see cref="Break()" /> or <see cref="Stop()" />, or
        ///     when one throws an exception, or when the loop is canceled, the <see cref="Parallel" /> class will proactively
        ///     attempt to prohibit additional iterations of the loop from starting execution.
        ///     However, there may be cases where it is unable to prevent additional iterations from starting.
        ///     It may also be the case that a long-running iteration has already begun execution.  In such
        ///     cases, iterations may explicitly check the <see cref="ShouldExitCurrentIteration" /> property and
        ///     cease execution if the property returns true.
        /// </remarks>
        public bool ShouldExitCurrentIteration => InternalShouldExitCurrentIteration;

        /// <summary>
        ///     Internal/virtual support for LowestBreakIteration.
        /// </summary>
        internal virtual long? InternalLowestBreakIteration
        {
            get
            {
                DebugEx.Fail("This method is not supported.");
                throw new NotSupportedException("This method is not supported.");
            }
        }

        /// <summary>
        ///     Internal/virtual support for ShouldExitCurrentIteration.
        /// </summary>
        internal virtual bool InternalShouldExitCurrentIteration
        {
            get
            {
                DebugEx.Fail("This method is not supported.");
                throw new NotSupportedException("This method is not supported.");
            }
        }

        /// <summary>
        ///     Communicates that the <see cref="Parallel" /> loop should cease execution at the system's earliest
        ///     convenience of iterations beyond the current iteration.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The <see cref="Stop()" /> method was previously called. <see cref="Break()" /> and <see cref="Stop()" />
        ///     may not be used in combination by iterations of the same loop.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         <see cref="Break()" /> may be used to communicate to the loop that no other iterations after the
        ///         current iteration need be run. For example, if <see cref="Break()" /> is called from the 100th
        ///         iteration of a for loop iterating in parallel from 0 to 1000, all iterations less than 100 should
        ///         still be run, but the iterations from 101 through to 1000 are not necessary.
        ///     </para>
        ///     <para>
        ///         For long-running iterations that may already be executing, <see cref="Break()" /> causes
        ///         <see cref="LowestBreakIteration" />
        ///         to be set to the current iteration's index if the current index is less than the current value of
        ///         <see cref="LowestBreakIteration" />.
        ///     </para>
        ///     <para>
        ///         <see cref="Break()" /> is typically employed in search-based algorithms where an ordering is
        ///         present in the data source.
        ///     </para>
        /// </remarks>
        public void Break()
        {
            InternalBreak();
        }

        /// <summary>
        ///     Communicates that the <see cref="Parallel" /> loop should cease execution at the system's earliest
        ///     convenience.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The <see cref="Break()" /> method was previously called.  <see cref="Break()" /> and
        ///     <see cref="Stop()" /> may not be used in combination by iterations of the same loop.
        /// </exception>
        /// <remarks>
        ///     <para>
        ///         <see cref="Stop()" /> may be used to communicate to the loop that no other iterations need be run.
        ///         For long-running iterations that may already be executing, <see cref="Stop()" /> causes
        ///         <see cref="IsStopped" />
        ///         to return true for all other iterations of the loop, such that another iteration may check
        ///         <see cref="IsStopped" /> and exit early if it's observed to be true.
        ///     </para>
        ///     <para>
        ///         <see cref="Stop()" /> is typically employed in search-based algorithms, where once a result is found,
        ///         no other iterations need be executed.
        ///     </para>
        /// </remarks>
        public void Stop()
        {
            _flagsBase.Stop();
        }

        // Helper method to avoid repeating Break() logic between ParallelState32 and ParallelState32<TLocal>
        internal static void Break(int iteration, ParallelLoopStateFlags32 parallelFlags)
        {
            // Attempt to change state from "not stopped or broken or canceled or exceptional" to "broken".
            if
            (
                !parallelFlags.AtomicLoopStateUpdate
                (
                    ParallelLoopStateFlags.ParallelLoopStateBroken,
                    ParallelLoopStateFlags.ParallelLoopStateStopped | ParallelLoopStateFlags.ParallelLoopStateExceptional | ParallelLoopStateFlags.ParallelLoopStateCanceled,
                    out var oldValue
                )
            )
            {
                // If we were already stopped, we have a problem
                if ((oldValue & ParallelLoopStateFlags.ParallelLoopStateStopped) != 0)
                {
                    throw new InvalidOperationException
                    (
                        "SR.ParallelState_Break_InvalidOperationException_BreakAfterStop"
                    );
                }

                // Apparently we previously got cancelled or became exceptional. No action necessary
                return;
            }

            // replace shared LowestBreakIteration with CurrentIteration, but only if CurrentIteration
            // is less than LowestBreakIteration.
            var lowestBreakIteration = parallelFlags.LowestBreakIterationInternal;
            if (iteration >= lowestBreakIteration)
            {
                return;
            }

            var wait = new SpinWait();
            while (Interlocked.CompareExchange(ref parallelFlags.LowestBreakIterationInternal, iteration, lowestBreakIteration) != lowestBreakIteration)
            {
                wait.SpinOnce();
                lowestBreakIteration = parallelFlags.LowestBreakIterationInternal;
                if (iteration > lowestBreakIteration)
                {
                    break;
                }
            }
        }

        // Helper method to avoid repeating Break() logic between ParallelState64 and ParallelState64<TLocal>
        internal static void Break(long iteration, ParallelLoopStateFlags64 parallelFlags)
        {
            // Attempt to change state from "not stopped or broken or canceled or exceptional" to "broken".
            if
            (
                !parallelFlags.AtomicLoopStateUpdate
                (
                    ParallelLoopStateFlags.ParallelLoopStateBroken,
                    ParallelLoopStateFlags.ParallelLoopStateStopped | ParallelLoopStateFlags.ParallelLoopStateExceptional | ParallelLoopStateFlags.ParallelLoopStateCanceled,
                    out var oldValue
                )
            )
            {
                // If we were already stopped, we have a problem
                if ((oldValue & ParallelLoopStateFlags.ParallelLoopStateStopped) != 0)
                {
                    throw new InvalidOperationException
                    (
                        "SR.ParallelState_Break_InvalidOperationException_BreakAfterStop"
                    );
                }

                // Apparently we previously got cancelled or became exceptional. No action necessary
                return;
            }

            // replace shared LowestBreakIteration with CurrentIteration, but only if CurrentIteration
            // is less than LowestBreakIteration.
            var lowestBreakIteration = parallelFlags.LowestBreakIteration;
            if (iteration >= lowestBreakIteration)
            {
                return;
            }

            var wait = new SpinWait();
            while (Interlocked.CompareExchange(ref parallelFlags.LowestBreakIterationInternal, iteration, lowestBreakIteration) != lowestBreakIteration)
            {
                wait.SpinOnce();
                lowestBreakIteration = parallelFlags.LowestBreakIteration;
                if (iteration > lowestBreakIteration)
                {
                    break;
                }
            }
        }

        // Internal/virtual support for Break().
        internal virtual void InternalBreak()
        {
            DebugEx.Fail("This method is not supported.");
            throw new NotSupportedException("This method is not supported.");
        }
    }

    internal class ParallelLoopState32 : ParallelLoopState
    {
        private readonly ParallelLoopStateFlags32 _sharedParallelStateFlags;

        /// <summary>
        ///     Internal constructor to ensure an instance isn't created by users.
        /// </summary>
        /// <param name="sharedParallelStateFlags">
        ///     A flag shared among all threads participating
        ///     in the execution of a certain loop.
        /// </param>
        internal ParallelLoopState32(ParallelLoopStateFlags32 sharedParallelStateFlags)
            : base(sharedParallelStateFlags)
        {
            _sharedParallelStateFlags = sharedParallelStateFlags;
        }

        /// <summary>
        ///     Tracks the current loop iteration for the owning task.
        ///     This is used to compute whether or not the task should
        ///     terminate early due to a Break() call.
        /// </summary>
        internal int CurrentIteration { get; set; }

        /// <summary>
        ///     Returns the lowest iteration at which Break() has been called, or
        ///     null if Break() has not yet been called.
        /// </summary>
        internal override long? InternalLowestBreakIteration => _sharedParallelStateFlags.NullableLowestBreakIteration;

        /// <summary>
        ///     Returns true if we should be exiting from the current iteration
        ///     due to Stop(), Break() or exception.
        /// </summary>
        internal override bool InternalShouldExitCurrentIteration => _sharedParallelStateFlags.ShouldExitLoop(CurrentIteration);

        /// <summary>
        ///     Communicates that parallel tasks should stop when they reach a specified iteration element.
        ///     (which is CurrentIteration of the caller).
        /// </summary>
        /// <exception cref="InvalidOperationException">Break() called after Stop().</exception>
        /// <remarks>
        ///     This is shared with all other concurrent threads in the system which are participating in the
        ///     loop's execution. After calling Break(), no additional iterations will be executed on
        ///     the current thread, and other worker threads will execute once they get beyond the calling iteration.
        /// </remarks>
        internal override void InternalBreak()
        {
            Break(CurrentIteration, _sharedParallelStateFlags);
        }
    }

    /// <summary>
    ///     Allows independent iterations of a parallel loop to interact with other iterations.
    /// </summary>
    internal class ParallelLoopState64 : ParallelLoopState
    {
        private readonly ParallelLoopStateFlags64 _sharedParallelStateFlags;

        /// <summary>
        ///     Internal constructor to ensure an instance isn't created by users.
        /// </summary>
        /// <param name="sharedParallelStateFlags">
        ///     A flag shared among all threads participating
        ///     in the execution of a certain loop.
        /// </param>
        internal ParallelLoopState64(ParallelLoopStateFlags64 sharedParallelStateFlags)
            : base(sharedParallelStateFlags)
        {
            _sharedParallelStateFlags = sharedParallelStateFlags;
        }

        /// <summary>
        ///     Tracks the current loop iteration for the owning task.
        ///     This is used to compute whether or not the task should
        ///     terminate early due to a Break() call.
        /// </summary>
        internal long CurrentIteration { get; set; }

        /// <summary>
        ///     Returns the lowest iteration at which Break() has been called, or
        ///     null if Break() has not yet been called.
        /// </summary>
        internal override long? InternalLowestBreakIteration => _sharedParallelStateFlags.NullableLowestBreakIteration;

        /// <summary>
        ///     Returns true if we should be exiting from the current iteration
        ///     due to Stop(), Break() or exception.
        /// </summary>
        internal override bool InternalShouldExitCurrentIteration => _sharedParallelStateFlags.ShouldExitLoop(CurrentIteration);

        /// <summary>
        ///     Communicates that parallel tasks should stop when they reach a specified iteration element.
        ///     (which is CurrentIteration of the caller).
        /// </summary>
        /// <exception cref="InvalidOperationException">Break() called after Stop().</exception>
        /// <remarks>
        ///     Atomically sets shared StoppedBroken flag to BROKEN, then atomically sets shared
        ///     LowestBreakIteration to CurrentIteration, but only if CurrentIteration is less than
        ///     LowestBreakIteration.
        /// </remarks>
        internal override void InternalBreak()
        {
            Break(CurrentIteration, _sharedParallelStateFlags);
        }
    }

    /// <summary>
    ///     State information that is common between ParallelStateFlags class
    ///     and ParallelStateFlags64 class.
    /// </summary>
    internal class ParallelLoopStateFlags
    {
        internal const int ParallelLoopStateBroken = 2;
        internal const int ParallelLoopStateCanceled = 8;
        internal const int ParallelLoopStateExceptional = 1;
        internal const int ParallelLoopStateNone = 0;
        internal const int ParallelLoopStateStopped = 4;
        private volatile int _loopStateFlags;

        internal int LoopStateFlags => _loopStateFlags;

        internal bool AtomicLoopStateUpdate(int newState, int illegalStates)
        {
            return AtomicLoopStateUpdate(newState, illegalStates, out _);
        }

        internal bool AtomicLoopStateUpdate(int newState, int illegalStates, out int oldState)
        {
            var sw = new SpinWait();

            while (true)
            {
                oldState = _loopStateFlags;
                if ((oldState & illegalStates) != 0)
                {
                    return false;
                }

                if (Interlocked.CompareExchange(ref _loopStateFlags, oldState | newState, oldState) == oldState)
                {
                    return true;
                }

                sw.SpinOnce();
            }
        }

        internal void Cancel()
        {
            // we can set the canceled flag regardless of the state of other bits.
            AtomicLoopStateUpdate(ParallelLoopStateCanceled, ParallelLoopStateNone);
        }

        internal void SetExceptional()
        {
            // we can set the exceptional flag regardless of the state of other bits.
            AtomicLoopStateUpdate(ParallelLoopStateExceptional, ParallelLoopStateNone);
        }

        internal void Stop()
        {
            // disallow setting of ParallelLoopStateStopped bit only if ParallelLoopStateBroken was already set
            if (!AtomicLoopStateUpdate(ParallelLoopStateStopped, ParallelLoopStateBroken))
            {
                throw new InvalidOperationException("SR.ParallelState_Stop_InvalidOperationException_StopAfterBreak");
            }
        }
    }

    /// <summary>
    ///     An internal class used to share accounting information in 32-bit versions
    ///     of For()/ForEach() loops.
    /// </summary>
    internal class ParallelLoopStateFlags32 : ParallelLoopStateFlags
    {
        // Records the lowest iteration at which a Break() has been called,
        // or Int32.MaxValue if no break has been called.  Used directly
        // by Break().
        internal volatile int LowestBreakIterationInternal = int.MaxValue;

        // Not strictly necessary, but maintains consistency with ParallelStateFlags64
        internal int LowestBreakIteration => LowestBreakIterationInternal;

        // Does some processing to convert _lowestBreakIteration to a long?.
        internal long? NullableLowestBreakIteration
        {
            get
            {
                if (LowestBreakIterationInternal == int.MaxValue)
                {
                    return null;
                }

                // protect against torn read of 64-bit value
                long result = LowestBreakIterationInternal;
                return IntPtr.Size >= 8 ? result : Interlocked.Read(ref result);
            }
        }

        /// <summary>
        ///     Lets the caller know whether or not to prematurely exit the For/ForEach loop.
        ///     If this returns true, then exit the loop.  Otherwise, keep going.
        /// </summary>
        /// <param name="callerIteration">
        ///     The caller's current iteration point
        ///     in the loop.
        /// </param>
        /// <remarks>
        ///     The loop should exit on any one of the following conditions:
        ///     (1) Stop() has been called by one or more tasks.
        ///     (2) An exception has been raised by one or more tasks.
        ///     (3) Break() has been called by one or more tasks, and
        ///     CallerIteration exceeds the (lowest) iteration at which
        ///     Break() was called.
        ///     (4) The loop was canceled.
        /// </remarks>
        internal bool ShouldExitLoop(int callerIteration)
        {
            var flags = LoopStateFlags;
            return flags != ParallelLoopStateNone
                   &&
                   (
                       (flags & (ParallelLoopStateExceptional | ParallelLoopStateStopped | ParallelLoopStateCanceled)) != 0
                       || ((flags & ParallelLoopStateBroken) != 0 && callerIteration > LowestBreakIteration)
                   );
        }

        // This lighter version of ShouldExitLoop will be used when the body type doesn't contain a state.
        // Since simpler bodies cannot stop or break, we can safely skip checks for those flags here.
        internal bool ShouldExitLoop()
        {
            var flags = LoopStateFlags;
            return flags != ParallelLoopStateNone && (flags & (ParallelLoopStateExceptional | ParallelLoopStateCanceled)) != 0;
        }
    }

    /// <summary>
    ///     An internal class used to share accounting information in 64-bit versions
    ///     of For()/ForEach() loops.
    /// </summary>
    internal class ParallelLoopStateFlags64 : ParallelLoopStateFlags
    {
        // Records the lowest iteration at which a Break() has been called,
        // or Int64.MaxValue if no break has been called.  Used directly
        // by Break().
        internal long LowestBreakIterationInternal = long.MaxValue;

        // Performs a conditionally interlocked read of _lowestBreakIteration.
        internal long LowestBreakIteration => IntPtr.Size >= 8 ? LowestBreakIterationInternal : Interlocked.Read(ref LowestBreakIterationInternal);

        // Does some processing to convert _lowestBreakIteration to a long?.
        internal long? NullableLowestBreakIteration
        {
            get
            {
                if (LowestBreakIterationInternal == long.MaxValue)
                {
                    return null;
                }

                return IntPtr.Size >= 8 ? LowestBreakIterationInternal : Interlocked.Read(ref LowestBreakIterationInternal);
            }
        }

        /// <summary>
        ///     Lets the caller know whether or not to prematurely exit the For/ForEach loop.
        ///     If this returns true, then exit the loop.  Otherwise, keep going.
        /// </summary>
        /// <param name="callerIteration">
        ///     The caller's current iteration point
        ///     in the loop.
        /// </param>
        /// <remarks>
        ///     The loop should exit on any one of the following conditions:
        ///     (1) Stop() has been called by one or more tasks.
        ///     (2) An exception has been raised by one or more tasks.
        ///     (3) Break() has been called by one or more tasks, and
        ///     CallerIteration exceeds the (lowest) iteration at which
        ///     Break() was called.
        ///     (4) The loop has been canceled.
        /// </remarks>
        internal bool ShouldExitLoop(long callerIteration)
        {
            var flags = LoopStateFlags;
            return flags != ParallelLoopStateNone
                   &&
                   (
                       (flags & (ParallelLoopStateExceptional | ParallelLoopStateStopped | ParallelLoopStateCanceled)) != 0
                       || ((flags & ParallelLoopStateBroken) != 0 && callerIteration > LowestBreakIteration)
                   );
        }

        // This lighter version of ShouldExitLoop will be used when the body type doesn't contain a state.
        // Since simpler bodies cannot stop or break, we can safely skip checks for those flags here.
        internal bool ShouldExitLoop()
        {
            var flags = LoopStateFlags;
            return flags != ParallelLoopStateNone && (flags & (ParallelLoopStateExceptional | ParallelLoopStateCanceled)) != 0;
        }
    }
}

#endif