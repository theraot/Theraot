#if NET20 || NET30 || NET35

namespace System.Collections.Specialized
{
    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}

#endif