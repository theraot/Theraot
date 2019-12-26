// Needed for NET35 (TASK)

using System;

namespace Theraot
{
    public readonly struct VoidStruct : IEquatable<VoidStruct>
    {
        public static bool operator !=(VoidStruct left, VoidStruct right)
        {
            _ = left;
            _ = right;
            return false;
        }

        public static bool operator ==(VoidStruct left, VoidStruct right)
        {
            _ = left;
            _ = right;
            return true;
        }

        public bool Equals(VoidStruct other)
        {
            return true;
        }

        public override bool Equals(object? obj)
        {
            return obj is VoidStruct;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}