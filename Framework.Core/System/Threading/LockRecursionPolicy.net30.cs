#if NET20 || NET30

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