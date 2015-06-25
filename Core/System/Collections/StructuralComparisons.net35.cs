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
            get
            {
                return _comparer;
            }
        }

        public static IEqualityComparer StructuralEqualityComparer
        {
            get
            {
                return _comparer;
            }
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
                else
                {
                    var comparable = x as IStructuralEquatable;
                    if (comparable != null)
                    {
                        return comparable.Equals(y, this);
                    }
                    var type_x = x.GetType();
                    var type_y = x.GetType();
                    if (type_x.IsArray && type_y.IsArray)
                    {
                        if (type_x.GetElementType() == type_y.GetElementType())
                        {
                            CheckRank(x, y, type_x, type_y);
                            var x_length_info = type_x.GetProperty("Length");
                            var y_length_info = type_y.GetProperty("Length");
                            if ((int)x_length_info.GetValue(x, TypeHelper.EmptyObjects) != (int)y_length_info.GetValue(y, TypeHelper.EmptyObjects))
                            {
                                return false;
                            }
                            else
                            {
                                var x_enumerator_info = type_x.GetMethod("GetEnumerator");
                                var y_enumerator_info = type_x.GetMethod("GetEnumerator");
                                IEnumerator first_enumerator = null;
                                IEnumerator second_enumerator = null;
                                var comparer = this as IEqualityComparer;
                                try
                                {
                                    first_enumerator = (IEnumerator)x_enumerator_info.Invoke(x, TypeHelper.EmptyObjects);
                                    second_enumerator = (IEnumerator)y_enumerator_info.Invoke(y, TypeHelper.EmptyObjects);
                                    while (first_enumerator.MoveNext())
                                    {
                                        if (!second_enumerator.MoveNext())
                                        {
                                            return false;
                                        }
                                        if (!comparer.Equals(first_enumerator.Current, second_enumerator.Current))
                                        {
                                            return false;
                                        }
                                    }
                                    return !second_enumerator.MoveNext();
                                }
                                finally
                                {
                                    var disposable_x = first_enumerator as IDisposable;
                                    if (disposable_x != null)
                                    {
                                        disposable_x.Dispose();
                                    }
                                    var disposable_y = second_enumerator as IDisposable;
                                    if (disposable_y != null)
                                    {
                                        disposable_y.Dispose();
                                    }
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return EqualityComparer<object>.Default.Equals(x, y);
                }
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

            private static void CheckRank(object x, object y, Type type_x, Type type_y)
            {
                var x_rank_info = type_x.GetProperty("Rank");
                var y_rank_info = type_y.GetProperty("Rank");
                if ((int)x_rank_info.GetValue(x, TypeHelper.EmptyObjects) != 1)
                {
                    throw new ArgumentException("Only one-dimensional arrays are supported", "x");
                }
                if ((int)y_rank_info.GetValue(y, TypeHelper.EmptyObjects) != 1)
                {
                    throw new ArgumentException("Only one-dimensional arrays are supported", "y");
                }
            }

            private static bool NullComparison(object x, object y, out bool result)
            {
                var x_null = ReferenceEquals(x, null);
                var y_null = ReferenceEquals(y, null);
                result = x_null == y_null;
                return x_null || y_null;
            }
        }
    }
}

#endif