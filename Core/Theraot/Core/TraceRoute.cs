#if FAT

using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Theraot.Core
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class TraceRoute
    {
        private const int _bufferSize = 32;
        private const int _timeoutMilliseconds = 1000;

        public static void Trace(IPAddress destination, Func<IPAddress, TracertNode, bool> callback)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            else
            {
                if (callback == null)
                {
                    throw new ArgumentNullException("callback");
                }
                else
                {
                    var syncroot = new object();
                    var buffer = new byte[_bufferSize];
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
                                if (ping != null)
                                {
                                    ping.Dispose();
                                }
                            }
                            finally
                            {
                                ping = null;
                            }
                        }
                        else
                        {
                            lock (syncroot)
                            {
                                if (ping == null)
                                {
                                    address = destination;
                                    status = IPStatus.Unknown;
                                    goto back;
                                }
                                else
                                {
                                    options.Ttl++;
                                    ping.SendAsync(destination, _timeoutMilliseconds, buffer, options, null);
                                }
                            }
                        }
                    };
                    ping.SendAsync(destination, _timeoutMilliseconds, buffer, options, null);
                }
            }
        }
    }
}

#endif