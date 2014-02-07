using System.Threading;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
    internal static class BucketHelper
    {
        private static readonly object _null;

        static BucketHelper()
        {
            _null = new object();
        }

        public static object Null
        {
            get
            {
                return _null;
            }
        }

        public static void Recycle<T>(ref FixedSizeQueueBucket<T> entries) //Needed
        {
            var _entries = Interlocked.Exchange(ref entries, null);
            if (!ReferenceEquals(_entries, null))
            {
                _entries.Recycle();
            }
        }

        public static void Recycle<TKey, TValue>(ref FixedSizeHashBucket<TKey, TValue> entries) //Needed
        {
            var _entries = Interlocked.Exchange(ref entries, null);
            if (!ReferenceEquals(_entries, null))
            {
                _entries.Recycle();
            }
        }

        public static void Recycle<T>(ref Bucket<T> entries) //Needed
        {
            var _entries = Interlocked.Exchange(ref entries, null);
            if (!ReferenceEquals(_entries, null))
            {
                _entries.Recycle();
            }
        }

        public static void Recycle<T>(ref FixedSizeSetBucket<T> entries) //Needed
        {
            var _entries = Interlocked.Exchange(ref entries, null);
            if (!ReferenceEquals(_entries, null))
            {
                _entries.Recycle();
            }
        }
    }
}