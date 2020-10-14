namespace System.Threading
{
    public static class TimeoutEx
    {
        [Runtime.InteropServices.ComVisible(false)]
        public static readonly TimeSpan InfiniteTimeSpan =
#if LESSTHAN_NET45
            new TimeSpan(0, 0, 0, 0, Timeout.Infinite);

#else
            Timeout.InfiniteTimeSpan;

#endif
    }
}