using System;
using System.Threading;

namespace Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static class ThreadingHelper
    {
        private static readonly RuntimeUniqueIdProdiver _threadIdProvider = new RuntimeUniqueIdProdiver();
        private static readonly int IntSleepCountHint = 5;
        private static readonly int IntSpinWaitHint = 20;
        private static NoTrackingThreadLocal<RuntimeUniqueIdProdiver.UniqueId> _threadRuntimeUniqueId = new NoTrackingThreadLocal<RuntimeUniqueIdProdiver.UniqueId>(_threadIdProvider.GetNextId);

        public static RuntimeUniqueIdProdiver.UniqueId ThreadUniqueId
        {
            get
            {
                return _threadRuntimeUniqueId.Value;
            }
        }

        public static void SpinWait(ref int check, int comparand)
        {
            int backCount = GetBackCount();
            if (Thread.VolatileRead(ref check) == comparand)
            {
                return;
            }
            else
            {
            retry:
                if (Thread.VolatileRead(ref check) == comparand)
                {
                    return;
                }
                else
                {
                    if (backCount == 0)
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {
                        Thread.SpinWait(IntSpinWaitHint);
                        backCount--;
                    }
                    goto retry;
                }
            }
        }

        public static void SpinWait(Func<bool> verification)
        {
            int backCount = GetBackCount();
            if (verification == null)
            {
                return;
            }
            else
            {
            retry:
                if (verification())
                {
                    return;
                }
                else
                {
                    if (backCount == 0)
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {
                        Thread.SpinWait(IntSpinWaitHint);
                        backCount--;
                    }
                    goto retry;
                }
            }
        }

        public static bool SpinWait(Func<bool> verification, IComparable<TimeSpan> timeout)
        {
            int backCount = GetBackCount();
            if (verification == null)
            {
                return true;
            }
            else
            {
            retry:
                if (verification())
                {
                    return true;
                }
                else
                {
                    var start = DateTime.Now;
                    if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
                    {
                        if (backCount == 0)
                        {
                            Thread.Sleep(0);
                        }
                        else
                        {
                            Thread.SpinWait(IntSpinWaitHint);
                            backCount--;
                        }
                        goto retry;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public static void SpinWaitExchange(ref int check, int value, int comparand, int ignoreComparand)
        {
            int backCount = GetBackCount();
            var tmp = Interlocked.CompareExchange(ref check, value, comparand);
            if (tmp == comparand || tmp == ignoreComparand)
            {
                return;
            }
            else
            {
            retry:
                tmp = Interlocked.CompareExchange(ref check, value, comparand);
                if (tmp == comparand || tmp == ignoreComparand)
                {
                    return;
                }
                else
                {
                    if (backCount == 0)
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {
                        Thread.SpinWait(IntSpinWaitHint);
                        backCount--;
                    }
                    goto retry;
                }
            }
        }

        public static void SpinWaitExchangeRelative(ref int check, int value, int ignoreComparand)
        {
            int backCount = GetBackCount();
            var tmpA = Thread.VolatileRead(ref check);
            var tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
            if (tmpB == tmpA || tmpB == ignoreComparand)
            {
                return;
            }
            else
            {
            retry:
                tmpA = Thread.VolatileRead(ref check);
                tmpB = Interlocked.CompareExchange(ref check, tmpA + value, tmpA);
                if (tmpB == tmpA || tmpB == ignoreComparand)
                {
                    return;
                }
                else
                {
                    if (backCount == 0)
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {
                        Thread.SpinWait(IntSpinWaitHint);
                        backCount--;
                    }
                    goto retry;
                }
            }
        }

        //[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "By Design")]
        public static T VolatileRead<T>(ref T address)
            where T : class
        {
            T copy = address;
            Thread.MemoryBarrier();
            return copy;
        }

        //[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "By Design")]
        public static void VolatileWrite<T>(ref T address, T value)
            where T : class
        {
            Thread.MemoryBarrier();
            address = value;
        }

        private static int GetBackCount()
        {
            if (Environment.ProcessorCount > 1)
            {
                return IntSleepCountHint;
            }
            else
            {
                return 0;
            }
        }
    }
}