#if LESSTHAN_NET30

#pragma warning disable RCS1159 // Use EventHandler<T>.

namespace System.Collections.Specialized
{
    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}

#endif