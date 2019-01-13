#if LESSTHAN_NET40

#pragma warning disable CA1003 // Use generic event handler instances
#pragma warning disable RCS1159 // Use EventHandler<T>.

namespace System.Collections.Specialized
{
    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}

#endif