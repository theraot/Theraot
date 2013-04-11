#if NET20 || NET30 || NET35

using System.Collections.Generic;

namespace System.Collections
{
    public static class StructuralComparisons
    {
        private static readonly InternalComparer comparer = new InternalComparer();

        public static IComparer StructuralComparer
        {
            get
            {
                return comparer;
            }
        }

        public static IEqualityComparer StructuralEqualityComparer
        {
            get
            {
                return comparer;
            }
        }

        private sealed class InternalComparer : IComparer, IEqualityComparer
        {
            int IComparer.Compare(object x, object y)
            {
                var comparer = x as IStructuralComparable;
                if (comparer != null)
                {
                    return comparer.CompareTo(y, this);
                }
                return Comparer.Default.Compare(x, y);
            }

            bool IEqualityComparer.Equals(object x, object y)
            {
                var comparer = x as IEqualityComparer;
                if (comparer != null)
                {
                    return comparer.Equals(y, this);
                }
                return EqualityComparer<object>.Default.Equals(x, y);
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                var comparer = obj as IEqualityComparer;
                if (comparer != null)
                {
                    return comparer.GetHashCode(this);
                }
                return EqualityComparer<object>.Default.GetHashCode(obj);
            }
        }
    }
}

#endif