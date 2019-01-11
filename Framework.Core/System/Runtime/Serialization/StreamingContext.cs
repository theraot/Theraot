#if LESSTHAN_NETSTANDARD13

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    public /*readonly*/ struct StreamingContext
    {
        public StreamingContext(StreamingContextStates state)
            : this(state, null)
        {
            // Empty
        }

        public StreamingContext(StreamingContextStates state, object additional)
        {
            State = state;
            Context = additional;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StreamingContext))
            {
                return false;
            }
            var ctx = (StreamingContext)obj;
            return ctx.Context == Context && ctx.State == State;
        }

        public override int GetHashCode() => (int)State;

        public StreamingContextStates State { get; }

        public object Context { get; }
    }

    [Flags]
    public enum StreamingContextStates
    {
        CrossProcess = 0x01,
        CrossMachine = 0x02,
        File = 0x04,
        Persistence = 0x08,
        Remoting = 0x10,
        Other = 0x20,
        Clone = 0x40,
        CrossAppDomain = 0x80,
        All = 0xFF,
    }
}

#endif