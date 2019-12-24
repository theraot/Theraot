#if LESSTHAN_NET40

#pragma warning disable CA1064 // Exceptions should be public
#pragma warning disable S3871 // Exception types should be "public"

using System;
using System.Runtime.Serialization;

namespace Theraot.Core
{
    [Serializable]
    internal class InternalSpecialCancelException : Exception
    {
        public InternalSpecialCancelException()
        {
            // Empty
        }

        public InternalSpecialCancelException(string message)
            : base(message)
        {
            // Empty
        }

        public InternalSpecialCancelException(string message, Exception inner)
            : base(message, inner)
        {
            // Empty
        }

        protected InternalSpecialCancelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Empty
        }
    }
}

#endif