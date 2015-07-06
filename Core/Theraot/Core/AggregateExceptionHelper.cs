using System;
using System.Threading;

namespace Theraot.Core
{
    public static class AggregateExceptionHelper
    {
        public static void AddException(ref AggregateException target, Exception source)
        {
            target = ReferenceEquals(target, null) ? new AggregateException(target) : (new AggregateException(source, target)).Flatten();
        }

        public static void AddException(ref Exception target, Exception source)
        {
            target = ReferenceEquals(target, null) ? new AggregateException(target) : (new AggregateException(source, target)).Flatten();
        }
    }
}
