#if LESSTHAN_NET45

namespace System.Threading.Tasks.Sources
{
    public enum ValueTaskSourceStatus
    {
        Pending,
        Succeeded,
        Faulted,
        Canceled
    }
}

#endif