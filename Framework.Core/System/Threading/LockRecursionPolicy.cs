#if LESSTHAN_NET35

namespace System.Threading
{
    [Serializable]
    public enum LockRecursionPolicy
    {
        NoRecursion,
        SupportsRecursion
    }
}

#endif