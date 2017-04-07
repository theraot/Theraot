#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public partial class ProgressiveList<T> : IExtendedList<T>
    {
        IReadOnlyList<T> IExtendedList<T>.AsReadOnly
        {
            get { return this; }
        }

        T IExtendedList<T>.this[int index]
        {
            get { return this[index]; }

            set { throw new NotSupportedException(); }
        }

        bool IExtendedCollection<T>.Remove(T item, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.Move(int oldIndex, int newIndex)
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.RemoveRange(int index, int count)
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.Reverse()
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.Reverse(int index, int count)
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.Sort(IComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.Sort(int index, int count, IComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        void IExtendedList<T>.Swap(int indexA, int indexB)
        {
            throw new NotSupportedException();
        }
    }
}

#endif