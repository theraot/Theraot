#if FAT

using System.Net;
using System.Net.NetworkInformation;

namespace Theraot.Core
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class TracertNode
    {
        private readonly IPAddress _address;
        private readonly IPStatus _status;
        private readonly int _ttl;

        internal TracertNode(IPAddress address, IPStatus status, int Ttl)
        {
            _address = address;
            _status = status;
            _ttl = Ttl;
        }

        public int Ttl
        {
            get { return _ttl; }
        }

        internal IPAddress Address
        {
            get { return _address; }
        }

        internal IPStatus Status
        {
            get { return _status; }
        }
    }
}

#endif