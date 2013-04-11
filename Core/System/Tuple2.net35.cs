#if NET20 || NET30 || NET35

using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace System
{
    [Serializable]
    public class Tuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable
    {
        private readonly T1 _item1;
        private readonly T2 _item2;

        public Tuple(T1 item1, T2 item2)
        {
            _item1 = item1;
            _item2 = item2;
        }

        public T1 Item1
        {
            get
            {
                return _item1;
            }
        }

        public T2 Item2
        {
            get
            {
                return _item2;
            }
        }

        public override bool Equals(object obj)
        {
            return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<object>.Default);
        }

        public override int GetHashCode()
        {
            return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default);
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
        int System.Collections.IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            return CompareTo(other, comparer);
        }

        bool System.Collections.IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
        {
            var tuple = other as Tuple<T1, T2>;
            if (tuple == null)
            {
                return false;
            }
            else
            {
                return
                    comparer.Equals(_item1, tuple._item1) &&
                    comparer.Equals(_item2, tuple._item2);
            }
        }

        int System.Collections.IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            int hash = comparer.GetHashCode(_item1);
            hash = (hash << 5) - hash + comparer.GetHashCode(_item2);
            return hash;
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
        int System.IComparable.CompareTo(object obj)
        {
            return CompareTo(obj, Comparer<object>.Default);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1})", _item1, _item2);
        }

        private int CompareTo(object other, IComparer comparer)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                var tuple = other as Tuple<T1, T2>;
                if (tuple == null)
                {
                    throw new ArgumentException("other");
                }
                else
                {
                    int result = comparer.Compare(_item1, tuple._item1);
                    if (result == 0)
                    {
                        result = comparer.Compare(_item2, tuple._item2);
                    }
                    return result;
                }
            }
        }
    }
}

#endif