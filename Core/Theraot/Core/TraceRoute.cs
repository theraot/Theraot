#if FAT
﻿using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Theraot.Core
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static class TraceRoute
    {
        private const int INT_BufferSize = 32;
        private const int INT_TimeoutMilliseconds = 1000;

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
                    var buffer = new byte[INT_BufferSize];
                    var options = new PingOptions(1, true);
                    var ping = new Ping();
                    ping.PingCompleted += (sender, e) =>
                    {
                        bool done;
                        var address = e.Reply.Address;
                        var status = e.Reply.Status;
                    back:
                        if (callback.Invoke(destination, new TracertNode(address, status, e.Reply.Options.Ttl)))
                        {
                            done = address.Equals(destination);
                        }
                        else
                        {
                            done = true;
                        }
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
                                    ping.SendAsync(destination, INT_TimeoutMilliseconds, buffer, options, null);
                                }
                            }
                        }
                    };
                    ping.SendAsync(destination, INT_TimeoutMilliseconds, buffer, options, null);
                }
            }
        }
    }
}
#endif