#if LESSTHAN_NETSTANDARD13

#pragma warning disable CS0618 // Type or member is obsolete

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Collections
{
    internal sealed class CompatibleComparer : IEqualityComparer
    {
        internal CompatibleComparer(IHashCodeProvider? hashCodeProvider, IComparer? comparer)
        {
            HashCodeProvider = hashCodeProvider;
            Comparer = comparer;
        }

        internal IComparer? Comparer { get; }
        internal IHashCodeProvider? HashCodeProvider { get; }

        public int Compare(object a, object b)
        {
            if (a == b)
            {
                return 0;
            }

            if (a == null)
            {
                return -1;
            }

            if (b == null)
            {
                return 1;
            }

            if (Comparer != null)
            {
                return Comparer.Compare(a, b);
            }

            if (a is IComparable ia)
            {
                return ia.CompareTo(b);
            }

            throw new ArgumentException("At least one object must implement IComparable.");
        }

        public new bool Equals(object x, object y) => Compare(x, y) == 0;

        public int GetHashCode(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return HashCodeProvider != null
                ? HashCodeProvider.GetHashCode(obj)
                : obj.GetHashCode();
        }
    }
}

#endif