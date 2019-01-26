#if LESSTHAN_NET35

#pragma warning disable CA1064 // Exceptions should be public

namespace System.Linq.Expressions.Interpreter
{
    /// <inheritdoc />
    /// <summary>
    ///     The re-throw instruction will throw this exception
    /// </summary>
    internal sealed class RethrowException : Exception
    {
        public RethrowException()
        {
            // Empty
        }

        public RethrowException(string message)
            : base(message)
        {
            // Empty
        }

        public RethrowException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Empty
        }
    }
}

#endif