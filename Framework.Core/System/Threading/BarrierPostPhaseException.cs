#if LESSTHAN_NET40

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1068 // CancellationToken parameters must come last

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.Threading
{
    /// <summary>
    ///     The exception that is thrown when the post-phase action of a <see cref="Barrier" /> fails.
    /// </summary>
    [Serializable]
    public class BarrierPostPhaseException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BarrierPostPhaseException" /> class.
        /// </summary>
        public BarrierPostPhaseException()
            : this((string?)null)
        {
            // Empty
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BarrierPostPhaseException" /> class with the
        ///     specified inner exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public BarrierPostPhaseException(Exception innerException)
            : this(message: null, innerException)
        {
            // Empty
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BarrierPostPhaseException" /> class with a
        ///     specified error message.
        /// </summary>
        /// <param name="message">A string that describes the exception.</param>
        public BarrierPostPhaseException(string? message)
            : this(message, innerException: null)
        {
            // Empty
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BarrierPostPhaseException" /> class with a
        ///     specified error message and inner exception.
        /// </summary>
        /// <param name="message">A string that describes the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public BarrierPostPhaseException(string? message, Exception? innerException)
            : base(message ?? "The postPhaseAction failed with an exception.", innerException)
        {
            // Empty
        }

        /// <summary>
        ///     Initializes a new instance of the BarrierPostPhaseException class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected BarrierPostPhaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Empty
        }
    }
}

#endif