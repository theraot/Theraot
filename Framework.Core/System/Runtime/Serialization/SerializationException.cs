#if LESSTHAN_NETSTANDARD13

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    [Serializable]
    public class SerializationException : SystemException
    {
        private static readonly string _nullMessage = "Serialization error";

        // Creates a new SerializationException with its message
        // string set to a default message.
        public SerializationException()
            : base(_nullMessage)
        {
            HResult = -2146233076;
        }

        public SerializationException(string message)
            : base(message)
        {
            HResult = -2146233076;
        }

        // ReSharper disable once UnusedMember.Global
        public SerializationException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = -2146233076;
        }

        protected SerializationException(SerializationInfo info, StreamingContext context)
        {
            GC.KeepAlive(info);
            GC.KeepAlive(context);
        }
    }
}

#endif