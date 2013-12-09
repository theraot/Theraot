#if NET20 || NET30 || NET35

namespace System.Threading.Tasks
{
    [Flags]
    [Serializable]
    public enum TaskCreationOptions
    {
        None = 0,
        PreferFairness = 1,
        LongRunning = 2,
        AttachedToParent = 4,
        DenyChildAttach = 8,
        HideScheduler = 16
    }
}

#endif