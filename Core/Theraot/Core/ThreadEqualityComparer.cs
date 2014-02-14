using System.Collections.Generic;
using System.Threading;
using Theraot.Threading;

namespace Theraot.Core
{
    public class ThreadEqualityComparer : IEqualityComparer<Thread>
    {
        private static readonly ThreadEqualityComparer _default = new ThreadEqualityComparer();

        private ThreadEqualityComparer()
        {
            //Empty
        }

        public static ThreadEqualityComparer Default
        {
            get
            {
                return _default;
            }
        }

        public bool Equals(Thread x, Thread y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(Thread obj)
        {
            return ThreadingHelper.ThreadUniqueId;
        }
    }
}