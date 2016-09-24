using System;
using System.Globalization;
using System.Threading;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class RuntimeUniqueIdProdiver
    {
        private static int _lastId = int.MinValue;

        public static UniqueId GetNextId()
        {
            return new UniqueId(Interlocked.Increment(ref _lastId));
        }

        public struct UniqueId : IEquatable<UniqueId>
        {
            private readonly int _id;

            internal UniqueId(int id)
            {
                _id = id;
            }

            public static bool operator !=(UniqueId x, UniqueId y)
            {
                return !x.Equals(y);
            }

            public static bool operator ==(UniqueId x, UniqueId y)
            {
                return x.Equals(y);
            }

            public bool Equals(UniqueId other)
            {
                return other._id.Equals(_id);
            }

            public override bool Equals(object obj)
            {
                if (obj is UniqueId)
                {
                    return Equals((UniqueId)obj);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return _id;
            }

            public override string ToString()
            {
                return _id.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}