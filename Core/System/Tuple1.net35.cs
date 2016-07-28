#if NET35

using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace System
{
    [Serializable]
    public class Tuple<T1> : IStructuralEquatable, IStructuralComparable, IComparable
    {
        private readonly T1 _item1;

        public Tuple(T1 item1)
        {
            _item1 = item1;
        }

        public T1 Item1
        {
            get
            {
                return _item1;
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
            var tuple = other as Tuple<T1>;
            if (tuple == null)
            {
                return false;
            }
            else
            {
                return comparer.Equals(_item1, tuple._item1);
            }
        }

        int System.Collections.IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return comparer.GetHashCode(_item1);
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
        int System.IComparable.CompareTo(object obj)
        {
            return CompareTo(obj, Comparer<object>.Default);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0})", _item1);
        }

        private int CompareTo(object other, IComparer comparer)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                var tuple = other as Tuple<T1>;
                if (tuple == null)
                {
                    throw new ArgumentException("other");
                }
                else
                {
                    return comparer.Compare(_item1, tuple._item1);
                }
            }
        }
    }
}

#endif