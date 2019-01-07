#if LESSTHAN_NET40

namespace System.Collections.Specialized
{
    public enum NotifyCollectionChangedAction
    {
        Add = 0,
        Remove = 1,
        Replace = 2,
        Move = 3,
        Reset = 4
    }
}

#endif