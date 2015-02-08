using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    internal static class PoolHelper
    {
        [ThreadStatic]
        private static List<int> _guard;
        private static int _id = int.MinValue;

        public static int GetId()
        {
            return Interlocked.Increment(ref _id);
        }

        public static bool Enter(int id)
        {
            if (_guard == null)
            {
                _guard = new List<int> { id };
                return true;
            }
            if (!_guard.Contains(id))
            {
                _guard.Add(id);
                return true;
            }
            return false;
        }

        public static void Leave(int id)
        {
            _guard.Remove(id);
        }
    }
}