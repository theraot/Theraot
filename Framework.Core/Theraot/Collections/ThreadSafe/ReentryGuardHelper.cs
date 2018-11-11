// Needed for NET40

using System;
using System.Collections.Generic;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
    internal static class ReentryGuardHelper
    {
        [ThreadStatic]
        private static HashSet<UniqueId> _guard;

        public static bool Enter(UniqueId id)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (GCMonitor.FinalizingForUnload)
            {
                return false;
            }
            var guard = _guard;
            if (guard == null)
            {
                _guard = new HashSet<UniqueId> { id };
                return true;
            }
            if (!guard.Contains(id))
            {
                guard.Add(id);
                return true;
            }
            return false;
        }

        public static bool IsTaken(UniqueId id)
        {
            return _guard.Contains(id);
        }

        public static void Leave(UniqueId id)
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