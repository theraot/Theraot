#if TARGETS_NETSTANDARD
namespace System
{
    public class SystemException : Exception
    {
        public SystemException()
        {
            // Empty
        }

        public SystemException(string message)
            : base(message)
        {
            // Empty
        }

        public SystemException(string message, Exception inner)
            : base(message, inner)
        {
            // Empty
        }
    }
}

#endif