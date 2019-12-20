#if LESSTHAN_NET40

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1068 // CancellationToken parameters must come last

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security;

namespace System.Threading
{
    /// <inheritdoc />
    /// <summary>
    ///     Enables multiple tasks to cooperatively work on an algorithm in parallel through multiple phases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A group of tasks cooperate by moving through a series of phases, where each in the group signals it
    ///         has arrived at the <see cref="System.Threading.Barrier" /> in a given phase and implicitly waits for all
    ///         others to
    ///         arrive. The same <see cref="System.Threading.Barrier" /> can be used for multiple phases.
    ///     </para>
    ///     <para>
    ///         All public and protected members of <see cref="System.Threading.Barrier" /> are thread-safe and may be used
    ///         concurrently from multiple threads, with the exception of Dispose, which
    ///         must only be used when all other operations on the <see cref="System.Threading.Barrier" /> have
    ///         completed.
    ///     </para>
    /// </remarks>
    [DebuggerDisplay("Participant Count={ParticipantCount},Participants Remaining={ParticipantsRemaining}")]
    public class Barrier : IDisposable
    {
        // Bitmask to extract the current count
        private const int _currentMask = 0x7FFF0000;

        // The maximum participants the barrier can operate = 32767 ( 2 power 15 - 1 )
        private const int _maxParticipants = _totalMask;

        // Bitmask to extract the sense flag
        private const int _senseMask = unchecked((int)0x80000000);

        // Bitmask to extract the total count
        private const int _totalMask = 0x00007FFF;

        // The EC callback that invokes the post phase action
        [SecurityCritical]
        private static ContextCallback? _invokePostPhaseAction;

        // Even phases event
        private readonly ManualResetEventSlim _evenEvent;

        // Odd phases event
        private readonly ManualResetEventSlim _oddEvent;

        // The execution context of the creator thread
        private readonly ExecutionContext? _ownerThreadContext;

        // Post phase action after each phase
        private readonly Action<Barrier>? _postPhaseAction;

        // This is the ManagedThreadID of the postPhaseAction caller thread, this is used to determine if the SignalAndWait, Dispose or Add/RemoveParticipant caller thread is
        // the same thread as the postPhaseAction thread which means this method was called from the postPhaseAction which is illegal.
        // This value is captured before calling the action and reset back to zero after it.
        private int _actionCallerId;

        // The current barrier phase
        // We don't need to worry about overflow, the max value is 2^63-1; If it starts from 0 at a
        // rate of 4 billion increments per second, it will takes about 64 years to overflow.
        private long _currentPhase;

        //This variable holds the basic barrier variables:
        // 1- The current participants count
        // 2- The total participants count
        // 3- The sense flag (true if the current phase is even, false otherwise)
        // The first 15 bits are for the total count which means the maximum participants for the barrier is about 32K
        // The 16th bit is dummy
        // The next 15th bit for the current
        // And the last highest bit is for the sense
        private int _currentTotalCount;

        // dispose flag
        private bool _disposed;

        // In case the post phase action throws an exception, wraps it in BarrierPostPhaseException
        private Exception? _exception;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="System.Threading.Barrier" /> class.
        /// </summary>
        /// <param name="participantCount">The number of participating threads.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="participantCount" /> is less than 0
        ///     or greater than <see cref="System.Int16.MaxValue" />.
        /// </exception>
        public Barrier(int participantCount)
            : this(participantCount, null)
        {
            // Empty
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Barrier" /> class.
        /// </summary>
        /// <param name="participantCount">The number of participating threads.</param>
        /// <param name="postPhaseAction">
        ///     The <see cref="System.Action{T}" /> to be executed after each
        ///     phase.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="participantCount" /> is less than 0
        ///     or greater than <see cref="System.Int32.MaxValue" />.
        /// </exception>
        /// <remarks>
        ///     The <paramref name="postPhaseAction" /> delegate will be executed after
        ///     all participants have arrived at the barrier in one phase.  The participants
        ///     will not be released to the next phase until the postPhaseAction delegate
        ///     has completed execution.
        /// </remarks>
        public Barrier(int participantCount, Action<Barrier>? postPhaseAction)
        {
            // the count must be non negative value
            if (participantCount < 0 || participantCount > _maxParticipants)
            {
                throw new ArgumentOutOfRangeException(nameof(participantCount), participantCount, "The participantCount argument must be non-negative and less than or equal to 32767");
            }

            _currentTotalCount = participantCount;
            _postPhaseAction = postPhaseAction;

            //Lazily initialize the events
            _oddEvent = new ManualResetEventSlim(true);
            _evenEvent = new ManualResetEventSlim(false);

            // Capture the context if the post phase action is not null
            if (postPhaseAction != null)
            {
                _ownerThreadContext = ExecutionContext.Capture();
            }
        }

        /// <summary>
        ///     Gets the number of the barrier's current phase.
        /// </summary>
        public long CurrentPhaseNumber
        {
            // use the new Volatile.Read/Write method because it is cheaper than Interlocked.Read on AMD64 architecture
            get => Volatile.Read(ref _currentPhase);

            internal set => Volatile.Write(ref _currentPhase, value);
        }

        /// <summary>
        ///     Gets the total number of participants in the barrier.
        /// </summary>
        public int ParticipantCount => Volatile.Read(ref _currentTotalCount) & _totalMask;

        /// <summary>
        ///     Gets the number of participants in the barrier that haven't yet signaled
        ///     in the current phase.
        /// </summary>
        /// <remarks>
        ///     This could be 0 during a post-phase action delegate execution or if the
        ///     ParticipantCount is 0.
        /// </remarks>
        public int ParticipantsRemaining
        {
            get
            {
                var currentTotal = Volatile.Read(ref _currentTotalCount);
                var total = currentTotal & _totalMask;
                var current = (currentTotal & _currentMask) >> 16;
                return total - current;
            }
        }

        /// <summary>
        ///     Notifies the <see cref="Barrier" /> that there will be an additional participant.
        /// </summary>
        /// <returns>
        ///     The phase number of the barrier in which the new participants will first
        ///     participate.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     Adding a participant would cause the barrier's participant count to
        ///     exceed <see cref="System.Int16.MaxValue" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The current instance has already been
        ///     disposed.
        /// </exception>
        public long AddParticipant()
        {
            try
            {
                return AddParticipants(1);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException("Adding participantCount participants would result in the number of participants exceeding the maximum number allowed.");
            }
        }

        /// <summary>
        ///     Notifies the <see cref="Barrier" /> that there will be additional participants.
        /// </summary>
        /// <param name="participantCount">
        ///     The number of additional participants to add to the
        ///     barrier.
        /// </param>
        /// <returns>
        ///     The phase number of the barrier in which the new participants will first
        ///     participate.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="participantCount" /> is less than
        ///     0.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     Adding <paramref name="participantCount" /> participants would cause the
        ///     barrier's participant count to exceed <see cref="System.Int16.MaxValue" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The current instance has already been
        ///     disposed.
        /// </exception>
        public long AddParticipants(int participantCount)
        {
            // check dispose
            ThrowIfDisposed();

            if (participantCount < 1)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(participantCount), participantCount,
                    "The participantCount argument must be a positive value."
                );
            }

            if (participantCount > _maxParticipants) //overflow
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(participantCount),
                    "Adding participantCount participants would result in the number of participants exceeding the maximum number allowed."
                );
            }

            // in case of this is called from the PHA
            if (_actionCallerId != 0 && Thread.CurrentThread.ManagedThreadId == _actionCallerId)
            {
                throw new InvalidOperationException("This method may not be called from within the postPhaseAction.");
            }

            var spinner = new SpinWait();
            long newPhase;
            while (true)
            {
                var currentTotal = Volatile.Read(ref _currentTotalCount);
                GetCurrentTotal(currentTotal, out var current, out var total, out var sense);
                if (participantCount + total > _maxParticipants) //overflow
                {
                    throw new ArgumentOutOfRangeException
                    (
                        nameof(participantCount),
                        "Adding participantCount participants would result in the number of participants exceeding the maximum number allowed."
                    );
                }

                if (SetCurrentTotal(currentTotal, current, total + participantCount, sense))
                {
                    // Calculating the first phase for that participant, if the current phase already finished return the next phase else return the current phase
                    // To know that the current phase is  the sense doesn't match the
                    // phase odd even, so that means it didn't yet change the phase count, so currentPhase +1 is returned, otherwise currentPhase is returned
                    var currentPhaseNumber = CurrentPhaseNumber;
                    newPhase = sense != (currentPhaseNumber % 2 == 0) ? currentPhaseNumber + 1 : currentPhaseNumber;

                    // If this participant is going to join the next phase, which means the postPhaseAction is being running, this participants must wait until this done
                    // and its event is reset.
                    // Without that, if the postPhaseAction takes long time, this means the event that the current participant is going to wait on is still set
                    // (FinishPhase didn't reset it yet) so it should wait until it reset
                    if (newPhase != currentPhaseNumber)
                    {
                        // Wait on the opposite event
                        if (sense)
                        {
                            _oddEvent.Wait();
                        }
                        else
                        {
                            _evenEvent.Wait();
                        }
                    }

                    //This else to fix the racing where the current phase has been finished, m_currentPhase has been updated but the events have not been set/reset yet
                    // otherwise when this participant calls SignalAndWait it will wait on a set event however all other participants have not arrived yet.
                    else
                    {
                        if (sense && _evenEvent.IsSet)
                        {
                            _evenEvent.Reset();
                        }
                        else if (!sense && _oddEvent.IsSet)
                        {
                            _oddEvent.Reset();
                        }
                    }

                    break;
                }

                spinner.SpinOnce();
            }

            return newPhase;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Releases all resources used by the current instance of <see cref="System.Threading.Barrier" />.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action.
        /// </exception>
        /// <remarks>
        ///     Unlike most of the members of <see cref="System.Threading.Barrier" />, Dispose is not thread-safe and may not be
        ///     used concurrently with other members of this instance.
        /// </remarks>
        public void Dispose()
        {
            // in case of this is called from the PHA
            if (_actionCallerId != 0 && Thread.CurrentThread.ManagedThreadId == _actionCallerId)
            {
                throw new InvalidOperationException("This method may not be called from within the postPhaseAction.");
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Notifies the <see cref="Barrier" /> that there will be one less participant.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     The barrier already has 0
        ///     participants.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The current instance has already been
        ///     disposed.
        /// </exception>
        public void RemoveParticipant()
        {
            RemoveParticipants(1);
        }

        /// <summary>
        ///     Notifies the <see cref="Barrier" /> that there will be fewer participants.
        /// </summary>
        /// <param name="participantCount">The number of additional participants to remove from the barrier.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="participantCount" /> is less than
        ///     0.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">The barrier already has 0 participants.</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The current instance has already been
        ///     disposed.
        /// </exception>
        public void RemoveParticipants(int participantCount)
        {
            // check dispose
            ThrowIfDisposed();

            // Validate input
            if (participantCount < 1)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(participantCount), participantCount,
                    "The participantCount argument must be a positive value."
                );
            }

            // in case of this is called from the PHA
            if (_actionCallerId != 0 && Thread.CurrentThread.ManagedThreadId == _actionCallerId)
            {
                throw new InvalidOperationException("This method may not be called from within the postPhaseAction.");
            }

            var spinner = new SpinWait();
            while (true)
            {
                var currentTotal = Volatile.Read(ref _currentTotalCount);
                GetCurrentTotal(currentTotal, out var current, out var total, out var sense);

                if (total < participantCount)
                {
                    throw new ArgumentOutOfRangeException
                    (
                        nameof(participantCount),
                        "The participantCount argument must be less than or equal the number of participants."
                    );
                }

                if (total - participantCount < current)
                {
                    throw new InvalidOperationException("The participantCount argument is greater than the number of participants that haven't yet arrived at the barrier in this phase.");
                }

                // If the remaining participants = current participants, then finish the current phase
                var remainingParticipants = total - participantCount;
                if (remainingParticipants > 0 && current == remainingParticipants)
                {
                    if (SetCurrentTotal(currentTotal, 0, total - participantCount, !sense))
                    {
                        FinishPhase(sense);
                        break;
                    }
                }
                else
                {
                    if (SetCurrentTotal(currentTotal, current, total - participantCount, sense))
                    {
                        break;
                    }
                }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        ///     Signals that a participant has reached the <see cref="Barrier" /> and waits for all other
        ///     participants to reach the barrier as well.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action, the barrier currently has 0 participants,
        ///     or the barrier is being used by more threads than are registered as participants.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The current instance has already been
        ///     disposed.
        /// </exception>
        public void SignalAndWait()
        {
            SignalAndWait(new CancellationToken());
        }

        /// <summary>
        ///     Signals that a participant has reached the <see cref="Barrier" /> and waits for all other
        ///     participants to reach the barrier, while observing a <see cref="System.Threading.CancellationToken" />.
        /// </summary>
        /// <param name="cancellationToken">
        ///     The <see cref="System.Threading.CancellationToken" /> to
        ///     observe.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action, the barrier currently has 0 participants,
        ///     or the barrier is being used by more threads than are registered as participants.
        /// </exception>
        /// <exception cref="System.OperationCanceledException">
        ///     <paramref name="cancellationToken" /> has been
        ///     canceled.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The current instance has already been
        ///     disposed.
        /// </exception>
        public void SignalAndWait(CancellationToken cancellationToken)
        {
#if DEBUG
            var result =
#endif
            SignalAndWait(Timeout.Infinite, cancellationToken);
#if DEBUG
            Debug.Assert(result);
#endif
        }

        /// <summary>
        ///     Signals that a participant has reached the <see cref="Barrier" /> and waits for all other
        ///     participants to reach the barrier as well, using a
        ///     <see cref="System.TimeSpan" /> to measure the time interval.
        /// </summary>
        /// <param name="timeout">
        ///     A <see cref="System.TimeSpan" /> that represents the number of
        ///     milliseconds to wait, or a <see cref="System.TimeSpan" /> that represents -1 milliseconds to
        ///     wait indefinitely.
        /// </param>
        /// <returns>true if all other participants reached the barrier; otherwise, false.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="timeout" />is a negative number
        ///     other than -1 milliseconds, which represents an infinite time-out, or it is greater than
        ///     <see cref="System.Int32.MaxValue" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action, the barrier currently has 0 participants,
        ///     or the barrier is being used by more threads than are registered as participants.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The current instance has already been
        ///     disposed.
        /// </exception>
        public bool SignalAndWait(TimeSpan timeout)
        {
            return SignalAndWait(timeout, new CancellationToken());
        }

        /// <summary>
        ///     Signals that a participant has reached the <see cref="Barrier" /> and waits for all other
        ///     participants to reach the barrier as well, using a
        ///     <see cref="System.TimeSpan" /> to measure the time interval, while observing a
        ///     <see cref="System.Threading.CancellationToken" />.
        /// </summary>
        /// <param name="timeout">
        ///     A <see cref="System.TimeSpan" /> that represents the number of
        ///     milliseconds to wait, or a <see cref="System.TimeSpan" /> that represents -1 milliseconds to
        ///     wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="System.Threading.CancellationToken" /> to
        ///     observe.
        /// </param>
        /// <returns>true if all other participants reached the barrier; otherwise, false.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="timeout" />is a negative number
        ///     other than -1 milliseconds, which represents an infinite time-out.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action, the barrier currently has 0 participants,
        ///     or the barrier is being used by more threads than are registered as participants.
        /// </exception>
        /// <exception cref="System.OperationCanceledException">
        ///     <paramref name="cancellationToken" /> has been
        ///     canceled.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The current instance has already been
        ///     disposed.
        /// </exception>
        public bool SignalAndWait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            if (milliseconds < -1L || milliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(timeout), timeout,
                    "The specified timeout must represent a value between -1 and Int32.MaxValue, inclusive."
                );
            }

            return SignalAndWait((int)milliseconds, cancellationToken);
        }

        /// <summary>
        ///     Signals that a participant has reached the <see cref="Barrier" /> and waits for all other
        ///     participants to reach the barrier as well, using a
        ///     32-bit signed integer to measure the time interval.
        /// </summary>
        /// <param name="millisecondsTimeout">
        ///     The number of milliseconds to wait, or <see cref="Timeout.Infinite" />(-1) to wait
        ///     indefinitely.
        /// </param>
        /// <returns>true if all other participants reached the barrier; otherwise, false.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="millisecondsTimeout" /> is a
        ///     negative number other than -1, which represents an infinite time-out.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action, the barrier currently has 0 participants,
        ///     or the barrier is being used by more threads than are registered as participants.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The current instance has already been
        ///     disposed.
        /// </exception>
        public bool SignalAndWait(int millisecondsTimeout)
        {
            return SignalAndWait(millisecondsTimeout, new CancellationToken());
        }

        /// <summary>
        ///     Signals that a participant has reached the barrier and waits for all other participants to reach
        ///     the barrier as well, using a
        ///     32-bit signed integer to measure the time interval, while observing a
        ///     <see cref="System.Threading.CancellationToken" />.
        /// </summary>
        /// <param name="millisecondsTimeout">
        ///     The number of milliseconds to wait, or <see cref="Timeout.Infinite" />(-1) to wait
        ///     indefinitely.
        /// </param>
        /// <param name="cancellationToken">
        ///     The <see cref="System.Threading.CancellationToken" /> to
        ///     observe.
        /// </param>
        /// <returns>true if all other participants reached the barrier; otherwise, false.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="millisecondsTimeout" /> is a
        ///     negative number other than -1, which represents an infinite time-out.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The method was invoked from within a post-phase action, the barrier currently has 0 participants,
        ///     or the barrier is being used by more threads than are registered as participants.
        /// </exception>
        /// <exception cref="System.OperationCanceledException">
        ///     <paramref name="cancellationToken" /> has been
        ///     canceled.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The current instance has already been
        ///     disposed.
        /// </exception>
        public bool SignalAndWait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(millisecondsTimeout), millisecondsTimeout,
                    "The specified timeout must represent a value between -1 and Int32.MaxValue, inclusive."
                );
            }

            // in case of this is called from the PHA
            if (_actionCallerId != 0 && Thread.CurrentThread.ManagedThreadId == _actionCallerId)
            {
                throw new InvalidOperationException("This method may not be called from within the postPhaseAction.");
            }

            // local variables to extract the basic barrier variable and update them
            // The are declared here instead of inside the loop body because the will be used outside the loop
            bool sense; // The sense of the barrier *before* the phase associated with this SignalAndWait call completes
            int total;
            int current;
            int currentTotal;
            long phase;
            var spinner = new SpinWait();
            while (true)
            {
                currentTotal = Volatile.Read(ref _currentTotalCount);
                GetCurrentTotal(currentTotal, out current, out total, out sense);
                phase = CurrentPhaseNumber;
                // throw if zero participants
                if (total == 0)
                {
                    throw new InvalidOperationException("The barrier has no registered participants.");
                }

                // Try to detect if the number of threads for this phase exceeded the total number of participants or not
                // This can be detected if the current is zero which means all participants for that phase has arrived and the phase number is not changed yet
                if (current == 0 && sense != (CurrentPhaseNumber % 2 == 0))
                {
                    throw new InvalidOperationException("The number of threads using the barrier exceeded the total number of registered participants.");
                }

                //This is the last thread, finish the phase
                if (current + 1 == total)
                {
                    if (SetCurrentTotal(currentTotal, 0, total, !sense))
                    {
                        FinishPhase(sense);
                        return true;
                    }
                }
                else if (SetCurrentTotal(currentTotal, current + 1, total, sense))
                {
                    break;
                }

                spinner.SpinOnce();
            }

            // ** Perform the real wait **
            // select the correct event to wait on, based on the current sense.
            var eventToWaitOn = sense ? _evenEvent : _oddEvent;

            var waitWasCanceled = false;
            var waitResult = false;
            try
            {
                waitResult = DiscontinuousWait(eventToWaitOn, millisecondsTimeout, cancellationToken, phase);
            }
            catch (OperationCanceledException)
            {
                waitWasCanceled = true;
            }
            catch (ObjectDisposedException) when (phase < CurrentPhaseNumber) // in case a race happen where one of the thread returned from SignalAndWait and the current thread calls Wait on a disposed event
            {
                waitResult = true;
            }

            if (!waitResult)
            {
                //reset the spinLock to prepare it for the next loop
                spinner.Reset();

                //If the wait timeout expired and all other thread didn't reach the barrier yet, update the current count back
                while (true)
                {
                    currentTotal = Volatile.Read(ref _currentTotalCount);
                    GetCurrentTotal(currentTotal, out current, out total, out var newSense);
                    // If the timeout expired and the phase has just finished, return true and this is considered as succeeded SignalAndWait
                    //otherwise the timeout expired and the current phase has not been finished yet, return false
                    //The phase is finished if the phase member variable is changed (incremented) or the sense has been changed
                    // we have to use the statements in the comparison below for two cases:
                    // 1- The sense is changed but the last thread didn't update the phase yet
                    // 2- The phase is already incremented but the sense flipped twice due to the termination of the next phase
                    if (phase < CurrentPhaseNumber || sense != newSense)
                    {
                        // The current phase has been finished, but we shouldn't return before the events are set/reset otherwise this thread could start
                        // next phase and the appropriate event has not reset yet which could make it return immediately from the next phase SignalAndWait
                        // before waiting other threads
                        WaitCurrentPhase(eventToWaitOn, phase);
                        Debug.Assert(phase < CurrentPhaseNumber);
                        break;
                    }

                    //The phase has not been finished yet, try to update the current count.
                    if (SetCurrentTotal(currentTotal, current - 1, total, sense))
                    {
                        //if here, then the attempt to back out was successful.
                        //throw (a fresh) OCE if cancellation woke the wait
                        //or return false if it was the timeout that woke the wait.
                        //
                        if (waitWasCanceled)
                        {
                            throw new OperationCanceledExceptionEx("The operation was canceled.", cancellationToken);
                        }

                        return false;
                    }

                    spinner.SpinOnce();
                }
            }

            if (_exception != null)
            {
                throw new BarrierPostPhaseException(_exception);
            }

            return true;
        }

        /// <summary>
        ///     When overridden in a derived class, releases the unmanaged resources used by the
        ///     <see cref="Barrier" />, and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     true to release both managed and unmanaged resources; false to release
        ///     only unmanaged resources.
        /// </param>
        /// <remarks>
        ///     Unlike most of the members of <see cref="Barrier" />, Dispose is not thread-safe and may not be
        ///     used concurrently with other members of this instance.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _oddEvent.Dispose();
                _evenEvent.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        ///     Extract the three variables current, total and sense from a given big variable
        /// </summary>
        /// <param name="currentTotal">The integer variable that contains the other three variables</param>
        /// <param name="current">The current participant count</param>
        /// <param name="total">The total participants count</param>
        /// <param name="sense">The sense flag</param>
        private static void GetCurrentTotal(int currentTotal, out int current, out int total, out bool sense)
        {
            total = currentTotal & _totalMask;
            current = (currentTotal & _currentMask) >> 16;
            sense = (currentTotal & _senseMask) == 0;
        }

        /// <summary>
        ///     The reason of discontinuous waiting instead of direct waiting on the event is to avoid the race where the sense is
        ///     changed twice because the next phase is finished (due to either RemoveParticipant is called or another thread
        ///     joined
        ///     the next phase instead of the current thread) so the current thread will be stuck on the event because it is reset
        ///     back
        ///     The maxWait and the shift numbers are arbitrarily chosen, there were no references picking them
        /// </summary>
        /// <param name="currentPhaseEvent">The current phase event</param>
        /// <param name="totalTimeout">wait timeout in milliseconds</param>
        /// <param name="token">cancellation token passed to SignalAndWait</param>
        /// <param name="observedPhase">The current phase number for this thread</param>
        /// <returns>True if the event is set or the phase number changed, false if the timeout expired</returns>
        private bool DiscontinuousWait(ManualResetEventSlim currentPhaseEvent, int totalTimeout, CancellationToken token, long observedPhase)
        {
            var maxWait = 100; // 100 ms
            const int waitTimeCeiling = 10000; // 10 seconds
            while (observedPhase == CurrentPhaseNumber)
            {
                // the next wait time, the min of the maxWait and the totalTimeout
                var waitTime = totalTimeout == Timeout.Infinite ? maxWait : Math.Min(maxWait, totalTimeout);

                if (currentPhaseEvent.Wait(waitTime, token))
                {
                    return true;
                }

                //update the total wait time
                if (totalTimeout != Timeout.Infinite)
                {
                    totalTimeout -= waitTime;
                    if (totalTimeout <= 0)
                    {
                        return false;
                    }
                }

                //if the maxWait exceeded 10 seconds then we will stop increasing the maxWait time and keep it 10 seconds, otherwise keep doubling it
                maxWait = maxWait >= waitTimeCeiling ? waitTimeCeiling : Math.Min(maxWait << 1, waitTimeCeiling);
            }

            //if we exited the loop because the observed phase doesn't match the current phase, then we have to spin to make sure
            //the event is set or the next phase is finished
            WaitCurrentPhase(currentPhaseEvent, observedPhase);

            return true;
        }

        /// <summary>
        ///     Finish the phase by invoking the post phase action, and setting the event, this must be called by the
        ///     last arrival thread
        /// </summary>
        /// <param name="observedSense">The current phase sense</param>
        [SecuritySafeCritical]
        private void FinishPhase(bool observedSense)
        {
            // Execute the PHA in try/finally block to reset the variables back in case of it threw an exception
            if (_postPhaseAction != null)
            {
                try
                {
                    // Capture the caller thread ID to check if the Add/RemoveParticipant(s) is called from the PHA
                    _actionCallerId = Thread.CurrentThread.ManagedThreadId;
                    if (_ownerThreadContext != null)
                    {
                        var handler = _invokePostPhaseAction;
                        if (handler == null)
                        {
                            _invokePostPhaseAction = handler = InvokePostPhaseAction;
                        }

                        ExecutionContext.Run(_ownerThreadContext, handler, this);
                    }
                    else
                    {
                        _postPhaseAction(this);
                    }

                    _exception = null; // reset the exception if it was set previously
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
                finally
                {
                    _actionCallerId = 0;
                    SetResetEvents(observedSense);
                }

                if (_exception != null)
                {
                    throw new BarrierPostPhaseException(_exception);
                }
            }
            else
            {
                SetResetEvents(observedSense);
            }

            static void InvokePostPhaseAction(object obj)
            {
                var thisBarrier = (Barrier)obj;
                thisBarrier._postPhaseAction!(thisBarrier);
            }
        }

        /// <summary>
        ///     Write the three variables current. total and the sense to the m_currentTotal
        /// </summary>
        /// <param name="currentTotal">The old current total to compare</param>
        /// <param name="current">The current participant count</param>
        /// <param name="total">The total participants count</param>
        /// <param name="sense">The sense flag</param>
        /// <returns>True if the CAS succeeded, false otherwise</returns>
        private bool SetCurrentTotal(int currentTotal, int current, int total, bool sense)
        {
            var newCurrentTotal = (current << 16) | total;

            if (!sense)
            {
                newCurrentTotal |= _senseMask;
            }

            return Interlocked.CompareExchange(ref _currentTotalCount, newCurrentTotal, currentTotal) == currentTotal;
        }

        /// <summary>
        ///     Sets the current phase event and reset the next phase event
        /// </summary>
        /// <param name="observedSense">The current phase sense</param>
        private void SetResetEvents(bool observedSense)
        {
            CurrentPhaseNumber++;
            if (observedSense)
            {
                _oddEvent.Reset();
                _evenEvent.Set();
            }
            else
            {
                _evenEvent.Reset();
                _oddEvent.Set();
            }
        }

        /// <summary>
        ///     Throw ObjectDisposedException if the barrier is disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Barrier), "The barrier has been disposed.");
            }
        }

        /// <summary>
        ///     Wait until the current phase finishes completely by spinning until either the event is set,
        ///     or the phase count is incremented more than one time
        /// </summary>
        /// <param name="currentPhaseEvent">The current phase event</param>
        /// <param name="observedPhase">The current phase for that thread</param>
        private void WaitCurrentPhase(ManualResetEventSlim currentPhaseEvent, long observedPhase)
        {
            //spin until either of these two conditions succeeds
            //1- The event is set
            //2- the phase count is incremented more than one time, this means the next phase is finished as well,
            //but the event will be reset again, so we check the phase count instead
            var spinner = new SpinWait();
            while (!currentPhaseEvent.IsSet && CurrentPhaseNumber - observedPhase <= 1)
            {
                spinner.SpinOnce();
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     The exception that is thrown when the post-phase action of a <see cref="System.Threading.Barrier" /> fails.
    /// </summary>
    [Serializable]
    public class BarrierPostPhaseException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="System.Threading.BarrierPostPhaseException" /> class.
        /// </summary>
        public BarrierPostPhaseException()
            : this((string?)null)
        {
            // Empty
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="System.Threading.BarrierPostPhaseException" /> class with the
        ///     specified inner exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public BarrierPostPhaseException(Exception innerException)
            : this(null, innerException)
        {
            // Empty
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="System.Threading.BarrierPostPhaseException" /> class with a
        ///     specified error message.
        /// </summary>
        /// <param name="message">A string that describes the exception.</param>
        public BarrierPostPhaseException(string? message)
            : this(message, null)
        {
            // Empty
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="System.Threading.BarrierPostPhaseException" /> class with a
        ///     specified error message and inner exception.
        /// </summary>
        /// <param name="message">A string that describes the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public BarrierPostPhaseException(string? message, Exception? innerException)
            : base(message ?? "The postPhaseAction failed with an exception.", innerException)
        {
            // Empty
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the BarrierPostPhaseException class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected BarrierPostPhaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Empty
        }
    }
}

#endif