// QuickSort.cs
//
// Authors:
//   Alejandro Serrano "Serras" (trupill@yahoo.es)
//   Marek Safar  <marek.safar@gmail.com>
//   Jb Evain (jbevain@novell.com)
//
// (C) 2007 - 2008 Novell, Inc. (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Needed for NET30

using System.Collections.Generic;

namespace System.Linq
{
    internal class QuickSort<TElement>
    {
        private readonly SortContext<TElement> _context;
        private readonly TElement[] _elements;
        private readonly int[] _indexes;

        private QuickSort(IEnumerable<TElement> source, SortContext<TElement> context)
        {
            _elements = Enumerable.ToArray(source);
            _indexes = CreateIndexes(_elements.Length);
            _context = context;
        }

        public static IEnumerable<TElement> Sort(IEnumerable<TElement> source, SortContext<TElement> context)
        {
            var sorter = new QuickSort<TElement>(source, context);
            sorter.PerformSort();
            foreach (int item in sorter._indexes)
            {
                yield return sorter._elements[item];
            }
        }

        private static int[] CreateIndexes(int length)
        {
            var indexes = new int[length];
            for (int i = 0; i < length; i++)
            {
                indexes[i] = i;
            }

            return indexes;
        }

        private int CompareItems(int firstIndex, int secondIndex)
        {
            return _context.Compare(firstIndex, secondIndex);
        }

        private void InsertionSort(int left, int right)
        {
            for (int i = left + 1; i <= right; i++)
            {
                int j, tmp = _indexes[i];

                for (j = i; j > left && CompareItems(tmp, _indexes[j - 1]) < 0; j--)
                {
                    _indexes[j] = _indexes[j - 1];
                }

                _indexes[j] = tmp;
            }
        }

        // We look at the first, middle, and last items in the subarray.
        // Then we put the largest on the right side, the smallest on
        // the left side, and the median becomes our pivot.
        private int MedianOfThree(int left, int right)
        {
            int center = (left + right) / 2;
            if (CompareItems(_indexes[center], _indexes[left]) < 0)
            {
                Swap(left, center);
            }
            if (CompareItems(_indexes[right], _indexes[left]) < 0)
            {
                Swap(left, right);
            }
            if (CompareItems(_indexes[right], _indexes[center]) < 0)
            {
                Swap(center, right);
            }
            Swap(center, right - 1);
            return _indexes[right - 1];
        }

        private void PerformSort()
        {
            // If the source contains just zero or one element, there's no need to sort
            if (_elements.Length <= 1)
            {
                return;
            }
            _context.Initialize(_elements);
            // Then sorts the elements according to the collected
            // key values and the selected ordering
            Sort(0, _indexes.Length - 1);
        }

        private void Sort(int left, int right)
        {
            if (left + 3 <= right)
            {
                int l = left, r = right - 1, pivot = MedianOfThree(left, right);
                while (true)
                {
                    while (CompareItems(_indexes[++l], pivot) < 0)
                    {
                    }
                    while (CompareItems(_indexes[--r], pivot) > 0)
                    {
                    }
                    if (l < r)
                    {
                        Swap(l, r);
                    }
                    else
                    {
                        break;
                    }
                }
                // Restore pivot
                Swap(l, right - 1);
                // Partition and sort
                Sort(left, l - 1);
                Sort(l + 1, right);
            }
            else
            {
                // If there are three items in the subarray, insertion sort is better
                InsertionSort(left, right);
            }
        }

        private void Swap(int left, int right)
        {
            int temp = _indexes[right];
            _indexes[right] = _indexes[left];
            _indexes[left] = temp;
        }
    }
}