#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Collections.Concurrent
{
    [SerializableAttribute]
    [ComVisible(false)]
    [DebuggerDisplay("Count = {Count}")]
    [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class ConcurrentQueue<T> : IProducerConsumerCollection<T>, IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
    {
    }
}

#endif