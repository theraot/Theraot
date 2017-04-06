#if NET20 || NET30 || NET35

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
            // EMpty
        }

        protected InternalSpecialCancelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Empty
        }
    }
}

#endif