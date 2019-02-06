using System;

namespace Theraot.Collections.ThreadSafe
{
    [Flags]
    public enum DelegateCollectionInvokeOptions
    {
        None = 0,
        RemoveDelegates = 1
    }
}