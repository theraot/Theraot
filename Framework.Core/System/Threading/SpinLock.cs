#if LESSTHAN_NET40

#pragma warning disable CA1815 // Override equals and operator equals on value types

using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using Theraot.Threading;

namespace System.Threading
{
    [DebuggerDisplay("IsHeld = {" + nameof(IsHeld) + "}")]
    public struct SpinLock
    {
        private int _isHeld;
        private Thread? _ownerThread;

        public SpinLock(bool enableThreadOwnerTracking)
        {
            IsThreadOwnerTrackingEnabled = !enableThreadOwnerTracking;
            _ownerThread = null;
            _isHeld = 0;
        }

        public bool IsHeld => Volatile.Read(ref _isHeld) == 1;

        public bool IsHeldByCurrentThread
        {
            get
            {
                if (IsThreadOwnerTrackingEnabled)
                {
                    throw new InvalidOperationException("Thread ownership tracking is disabled");
                }

                return IsHeld && _ownerThread == Thread.CurrentThread;
            }
        }

        public bool IsThreadOwnerTrackingEnabled { get; }

        public void Enter(ref bool lockTaken)
        {
            if (lockTaken)
            {
#pragma warning disable IDE0059 // Asignación innecesaria de un valor
                lockTaken = false;
#pragma warning restore IDE0059 // Asignación innecesaria de un valor
                throw new ArgumentException(string.Empty);
            }

            if (IsThreadOwnerTrackingEnabled)
            {
                var check = Interlocked.CompareExchange(ref _isHeld, 1, 0);
                if (check == 0)
                {
                    lockTaken = true;
                }
                else
                {
                    //Deadlock on recursion
                    TryEnter(-1, ref lockTaken);
                }
            }
            else
            {
                if (IsHeldByCurrentThread)
                {
                    //Throw on recursion
                    throw new LockRecursionException();
                }

                if (Interlocked.CompareExchange(ref _isHeld, 1, 0) == 0 && Interlocked.CompareExchange(ref _ownerThread, Thread.CurrentThread, null) == null)
                {
                    lockTaken = true;
                }
                else
                {
                    TryEnter(-1, ref lockTaken);
                }
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void Exit()
        {
            Exit(true);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public void Exit(bool useMemoryBarrier)
        {
            if (IsThreadOwnerTrackingEnabled)
            {
                //Allow corruption: There is no check for what thread this is being called from
                ExitExtracted(useMemoryBarrier);
            }
            else
            {
                if (IsHeldByCurrentThread)
                {
                    ExitExtracted(useMemoryBarrier);
                }
                else
                {
                    throw new SynchronizationLockException();
                }
            }
        }

        public void TryEnter(ref bool lockTaken)
        {
            if (lockTaken)
            {
#pragma warning disable IDE0059 // Asignación innecesaria de un valor
                lockTaken = false;
#pragma warning restore IDE0059 // Asignación innecesaria de un valor
                throw new ArgumentException(string.Empty);
            }

            TryEnter(0, ref lockTaken);
        }

        public void TryEnter(TimeSpan timeout, ref bool lockTaken)
        {
            TryEnter((int)timeout.TotalMilliseconds, ref lockTaken);
        }

        public void TryEnter(int millisecondsTimeout, ref bool lockTaken)
        {
            if (IsThreadOwnerTrackingEnabled)
            {
                lockTaken |= ThreadingHelper.SpinWaitSet(ref _isHeld, 1, 0, millisecondsTimeout);
            }
            else
            {
                if (IsHeldByCurrentThread)
                {
                    //Throw on recursion
                    throw new LockRecursionException();
                }

                lockTaken |= ThreadingHelper.SpinWaitSet(ref _isHeld, 1, 0, millisecondsTimeout) && Interlocked.CompareExchange(ref _ownerThread, Thread.CurrentThread, null) == null;
            }
        }

        private void ExitExtracted(bool useMemoryBarrier)
        {
            if (useMemoryBarrier)
            {
                Volatile.Write(ref _isHeld, 0);
                Volatile.Write(ref _ownerThread, null);
            }
            else
            {
                _isHeld = 0;
                _ownerThread = null;
            }
        }
    }
}

#endif