// Needed for NET40

using System;
using System.Collections.Generic;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
    internal static class ReentryGuardHelper
    {
        [ThreadStatic]
        private static HashSet<RuntimeUniqueIdProdiver.UniqueId> _guard;

        public static bool Enter(RuntimeUniqueIdProdiver.UniqueId id)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var guard = _guard;
            if (guard == null)
            {
                _guard = new HashSet<RuntimeUniqueIdProdiver.UniqueId> { id };
                return true;
            }
            if (!guard.Contains(id))
            {
                guard.Add(id);
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
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var guard = _guard;
            if (guard != null)
            {
                guard.Remove(id);
            }
        }
    }
}