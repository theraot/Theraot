// Needed for NET40

using System;
using System.Threading;

namespace Theraot.Threading
{
    public static partial class ThreadingHelper
    {
        public static bool SpinWaitRelativeExchangeBounded(ref int check, int value, int minValue, int maxValue, out int lastValue)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                lastValue = Volatile.Read(ref check);
                if (lastValue < minValue || lastValue > maxValue || lastValue + value < minValue || lastValue > maxValue - value)
                {
                    return false;
                }

                var result = lastValue + value;
                var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
                if (tmp == lastValue)
                {
                    return true;
                }

                spinWait.SpinOnce();
            }
        }

        public static bool SpinWaitRelativeExchangeUnlessNegative(ref int check, int value, out int lastValue)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                lastValue = Volatile.Read(ref check);
                if (lastValue < 0 || lastValue < -value)
                {
                    return false;
                }

                var result = lastValue + value;
                var tmp = Interlocked.CompareExchange(ref check, result, lastValue);
                if (tmp == lastValue)
                {
                    return true;
                }

                spinWait.SpinOnce();
            }
        }

        public static bool SpinWaitRelativeSet(ref int check, int value)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var tmpA = Volatile.Read(ref check);
                var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
                if (tmpB == tmpA)
                {
                    return true;
                }

                spinWait.SpinOnce();
            }
        }

        public static bool SpinWaitRelativeSet(ref int check, int value, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            }

            if (milliseconds == -1)
            {
                return SpinWaitRelativeSet(ref check, value);
            }

            var spinWait = new SpinWait();
            var start = TicksNow();
            while (true)
            {
                var tmpA = Volatile.Read(ref check);
                var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
                if (tmpB == tmpA)
                {
                    return true;
                }

                if (Milliseconds(TicksNow() - start) >= milliseconds)
                {
                    return false;
                }

                spinWait.SpinOnce();
            }
        }

        public static void SpinWaitSet(ref int check, int value, int comparand)
        {
            var spinWait = new SpinWait();
            while (Interlocked.CompareExchange(ref check, value, comparand) != comparand)
            {
                spinWait.SpinOnce();
            }
        }

        public static bool SpinWaitSet(ref int check, int value, int comparand, int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            }

            if (milliseconds == -1)
            {
                SpinWaitSet(ref check, value, comparand);
                return true;
            }

            var spinWait = new SpinWait();
            var start = TicksNow();
            while (true)
            {
                if (Interlocked.CompareExchange(ref check, value, comparand) == comparand)
                {
                    return true;
                }

                if (Milliseconds(TicksNow() - start) >= milliseconds)
                {
                    return false;
                }

                spinWait.SpinOnce();
            }
        }

        public static bool SpinWaitSetUnless(ref int check, int value, int comparand, int unless)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var lastValue = Volatile.Read(ref check);
                if (lastValue == unless)
                {
                    return false;
                }

                var tmpB = Interlocked.CompareExchange(ref check, value, comparand);
                if (tmpB == comparand)
                {
                    return true;
                }

                spinWait.SpinOnce();
            }
        }

        public static void SpinWaitUntil(Func<bool> verification)
        {
            var spinWait = new SpinWait();
            while (!verification.Invoke())
            {
                spinWait.SpinOnce();
            }
        }

        public static bool SpinWaitUntil(Func<bool> verification, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            if (milliseconds < -1L || milliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            if (milliseconds == -1)
            {
                SpinWaitUntil(verification);
                return true;
            }

            var spinWait = new SpinWait();
            var start = TicksNow();
            while (true)
            {
                if (verification.Invoke())
                {
                    return true;
                }

                if (Milliseconds(TicksNow() - start) >= milliseconds)
                {
                    return false;
                }

                spinWait.SpinOnce();
            }
        }

        public static void SpinWaitUntil(ref int check, int comparand)
        {
            var spinWait = new SpinWait();
            while (Volatile.Read(ref check) != comparand)
            {
                spinWait.SpinOnce();
            }
        }

        public static void SpinWaitWhile(ref int check, int comparand)
        {
            var spinWait = new SpinWait();
            while (Volatile.Read(ref check) == comparand)
            {
                spinWait.SpinOnce();
            }
        }

        public static void SpinWaitWhileNull<T>(ref T? check)
            where T : class
        {
            var spinWait = new SpinWait();
            while (Volatile.Read(ref check) == null)
            {
                spinWait.SpinOnce();
            }
        }
    }
}