// Needed for NET40

using System;
using System.Collections.Generic;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
    internal static class ThreadLocalFlagHelper
    {
        [ThreadStatic]
        private static HashSet<RuntimeUniqueIdProdiver.UniqueId> _guard;

        public static bool Enter(RuntimeUniqueIdProdiver.UniqueId id)
        {
            if (_guard == null)
            {
                _guard = new HashSet<RuntimeUniqueIdProdiver.UniqueId> { id };
                return true;
            }
            if (!_guard.Contains(id))
            {
                _guard.Add(id);
                return true;
            }
            return false;
        }

        public static bool IsTaken(RuntimeUniqueIdProdiver.UniqueId id)
        {
            return _guard.Contains(id);
        }

        public static void Leave(RuntimeUniqueIdProdiver.UniqueId id)
        {
            _guard.Remove(id);
        }
    }
}