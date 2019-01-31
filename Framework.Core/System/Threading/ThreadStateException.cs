#if LESSTHAN_NETSTANDARD13
namespace System.Threading
{
    [Runtime.InteropServices.ComVisible(true)]
    public class ThreadStateException : SystemException
    {
        public ThreadStateException()
        {
            // Empty
        }

        public ThreadStateException(string message)
            : base(message)
        {
            // Empty
        }

        public ThreadStateException(string message, Exception inner)
            : base(message, inner)
        {
            // Empty
        }
    }
}

#endif