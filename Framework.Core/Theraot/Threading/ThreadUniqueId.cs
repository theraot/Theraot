using System;

namespace Theraot.Threading
{
    public static class ThreadUniqueId
    {
        [ThreadStatic]
        private static UniqueId? _currentThreadId;

        public static UniqueId CurrentThreadId
        {
            get
            {
                if (_currentThreadId != null)
                {
                    return _currentThreadId.Value;
                }

                var result = RuntimeUniqueIdProvider.GetNextId();
                _currentThreadId = result;
                return result;
            }
        }
    }
}
