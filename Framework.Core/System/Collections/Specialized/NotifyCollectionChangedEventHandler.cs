#if LESSTHAN_NET30

#pragma warning disable CA1003 // Use generic event handler instances

namespace System.Collections.Specialized
{
    public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);
}

#endif