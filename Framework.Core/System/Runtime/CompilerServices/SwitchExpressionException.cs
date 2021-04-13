#if TARGETS_NET || LESSTHAN_NETCOREAPP30 || LESSTHAN_NETSTANDARD21

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Indicates that a switch expression that was non-exhaustive failed to match its input
    /// at runtime, e.g. in the C# 8 expression
    /// <code>
    /// 3 switch { 4 => 5 }
    /// </code>.
    /// The exception optionally contains an object representing the unmatched value.
    /// </summary>
    [Serializable]
    public sealed class SwitchExpressionException : InvalidOperationException
    {
        public SwitchExpressionException()
            : base("SwitchExpressionException")
        {
            // Empty
        }

        public SwitchExpressionException(Exception? innerException) :
            base("SwitchExpressionException", innerException)
        {
        }

        public SwitchExpressionException(object? unmatchedValue) : this()
        {
            UnmatchedValue = unmatchedValue;
        }

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16

        public SwitchExpressionException(string? message)
            : base(message)
        {
            // Empty
        }

        public SwitchExpressionException(string? message, Exception? innerException)
            : base(message, innerException)
        {
            // Empty
        }

        private SwitchExpressionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            UnmatchedValue = info.GetValue(nameof(UnmatchedValue), typeof(object));
        }

#endif

        public override string Message
        {
            get
            {
                if (UnmatchedValue is null)
                {
                    return base.Message;
                }

                return base.Message + Environment.NewLine + UnmatchedValue;
            }
        }

        public object? UnmatchedValue { get; }

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(UnmatchedValue), UnmatchedValue, typeof(object));
        }

#endif
    }
}

#endif