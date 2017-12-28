#if NET20 || NET30 || NET35

using System.Collections.Generic;
using Theraot.Core;

namespace System.Collections
{
    public static class StructuralComparisons
    {
        private static readonly InternalComparer _comparer = new InternalComparer();

        public static IComparer StructuralComparer
        {
            get { return _comparer; }
        }

        public static IEqualityComparer StructuralEqualityComparer
        {
            get { return _comparer; }
        }

        private sealed class InternalComparer : IComparer, IEqualityComparer
        {
            int IComparer.Compare(object x, object y)
            {
                var comparable = x as IStructuralComparable;
                if (comparable != null)
                {
                    return comparable.CompareTo(y, this);
                }
                return Comparer.Default.Compare(x, y);
            }

            bool IEqualityComparer.Equals(object x, object y)
            {
                bool result;
                if (NullComparison(x, y, out result))
                {
                    return result;
                }
                var comparable = x as IStructuralEquatable;
                if (comparable != null)
                {
                    return comparable.Equals(y, this);
                }
                var typeX = x.GetType();
                var typeY = y.GetType();
                if (typeX.IsArray && typeY.IsArray)
                {
                    if (typeX.GetElementType() == typeY.GetElementType())
                    {
                        CheckRank(x, y, typeX, typeY);
                        var xLengthInfo = typeX.GetProperty("Length");
                        var yLengthInfo = typeY.GetProperty("Length");
                        if (xLengthInfo == null || yLengthInfo == null)
                        {
                            // should never happen
                            throw new ArgumentException("Valid arrays required");
                        }
                        if ((int)xLengthInfo.GetValue(x, TypeHelper.EmptyObjects) != (int)yLengthInfo.GetValue(y, TypeHelper.EmptyObjects))
                        {
                            return false;
                        }
                        var xEnumeratorInfo = typeX.GetMethod("GetEnumerator");
                        var yEnumeratorInfo = typeX.GetMethod("GetEnumerator");
                        IEnumerator firstEnumerator = null;
                        IEnumerator secondEnumerator = null;
                        var comparer = this as IEqualityComparer;
                        try
                        {
                            firstEnumerator = (IEnumerator)xEnumeratorInfo.Invoke(x, TypeHelper.EmptyObjects);
                            secondEnumerator = (IEnumerator)yEnumeratorInfo.Invoke(y, TypeHelper.EmptyObjects);
                            while (firstEnumerator.MoveNext())
                            {
                                if (!secondEnumerator.MoveNext())
                                {
                                    return false;
                                }
                                if (!comparer.Equals(firstEnumerator.Current, secondEnumerator.Current))
                                {
                                    return false;
                                }
                            }
                            return !secondEnumerator.MoveNext();
                        }
                        finally
                        {
                            var disposableX = firstEnumerator as IDisposable;
                            if (disposableX != null)
                            {
                                disposableX.Dispose();
                            }
                            var disposableY = secondEnumerator as IDisposable;
                            if (disposableY != null)
                            {
                                disposableY.Dispose();
                            }
                        }
                    }
                    return false;
                }
                return EqualityComparer<object>.Default.Equals(x, y);
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                var comparer = obj as IStructuralEquatable;
                if (comparer != null)
                {
                    return comparer.GetHashCode(this);
                }
                return EqualityComparer<object>.Default.GetHashCode(obj);
            }

            private static void CheckRank(object x, object y, Type typeX, Type typeY)
            {
                var xRankInfo = typeX.GetProperty("Rank");
                var yRankInfo = typeY.GetProperty("Rank");
                if (xRankInfo == null || yRankInfo == null)
                {
                    // should never happen
                    throw new ArgumentException("Valid arrays required");
                }
                if ((int)xRankInfo.GetValue(x, TypeHelper.EmptyObjects) != 1)
                {
                    throw new ArgumentException("Only one-dimensional arrays are supported", "x");
                }
                if ((int)yRankInfo.GetValue(y, TypeHelper.EmptyObjects) != 1)
                {
                    throw new ArgumentException("Only one-dimensional arrays are supported", "y");
                }
            }

            private static bool NullComparison(object x, object y, out bool result)
            {
                var xNull = ReferenceEquals(x, null);
                var yNull = ReferenceEquals(y, null);
                result = xNull == yNull;
                return xNull || yNull;
            }
        }
    }
}

#endif