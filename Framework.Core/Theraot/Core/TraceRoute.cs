#if FAT

using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Theraot.Core
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class TraceRoute
    {
        public static void Trace(IPAddress destination, Func<IPAddress, TracertNode, bool> callback)
        {
            const int BufferSize = 32;
            const int TimeoutMilliseconds = 1000;

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
            var syncRoot = new object();
            var buffer = new byte[BufferSize];
            var options = new PingOptions(1, true);
            var ping = new Ping();
            ping.PingCompleted += (sender, e) =>
            {
                var address = e.Reply.Address;
                var status = e.Reply.Status;
            back:
                var done = !callback.Invoke(destination, new TracertNode(address, status, e.Reply.Options.Ttl)) || address.Equals(destination);
                if (done)
                {
                    try
                    {
                        ping?.Dispose();
                    }
                    finally
                    {
                        ping = null;
                    }
                }
                else
                {
                    lock (syncRoot)
                    {
                        if (ping == null)
                        {
                            address = destination;
                            status = IPStatus.Unknown;
                            goto back;
                        }
                        options.Ttl++;
                        ping.SendAsync(destination, TimeoutMilliseconds, buffer, options, null);
                    }
                }
            };
            ping.SendAsync(destination, TimeoutMilliseconds, buffer, options, null);
        }
    }
}

#endif