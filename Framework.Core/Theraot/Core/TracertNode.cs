#if FAT
using System.Net;
using System.Net.NetworkInformation;

namespace Theraot.Core
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class TracertNode
    {
        internal TracertNode(IPAddress address, IPStatus status, int ttl)
        {
            Address = address;
            Status = status;
            Ttl = ttl;
        }

        public int Ttl { get; }

        internal IPAddress Address { get; }

        internal IPStatus Status { get; }
    }
}

#endif