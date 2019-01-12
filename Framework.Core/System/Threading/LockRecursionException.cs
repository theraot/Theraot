#if LESSTHAN_NET35

using System.Runtime.Serialization;

namespace System.Threading
{
    [Serializable]
    public class LockRecursionException : Exception
    {
        public LockRecursionException()
        {
            //Empty
        }

        public LockRecursionException(string message)
            : base(message)
        {
            //Empty
        }

        public LockRecursionException(string message, Exception inner)
            : base(message, inner)
        {
            //Empty
        }

        protected LockRecursionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //Empty
        }
    }
}

#endif