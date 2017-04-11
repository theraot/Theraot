// SortSequenceContext.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2008 Novell, Inc. (http://www.novell.com)
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
    internal class SortSequenceContext<TElement, TKey> : SortContext<TElement>
    {
        private readonly IComparer<TKey> _comparer;
        private readonly Func<TElement, TKey> _selector;
        private TKey[] _keys;

        public SortSequenceContext(Func<TElement, TKey> selector, IComparer<TKey> comparer, SortDirection direction, SortContext<TElement> childContext)
            : base(direction, childContext)
        {
            _selector = selector;
            _comparer = comparer;
        }

        public override int Compare(int firstIndex, int secondIndex)
        {
            var comparison = _comparer.Compare(_keys[firstIndex], _keys[secondIndex]);
            if (comparison == 0)
            {
                if (ChildContext != null)
                {
                    return ChildContext.Compare(firstIndex, secondIndex);
                }
                comparison = Direction == SortDirection.Descending
                             ? secondIndex - firstIndex
                             : firstIndex - secondIndex;
            }
            return Direction == SortDirection.Descending ? -comparison : comparison;
        }

        public override void Initialize(TElement[] elements)
        {
            if (ChildContext != null)
            {
                ChildContext.Initialize(elements);
            }
            _keys = new TKey[elements.Length];
            for (int i = 0; i < _keys.Length; i++)
            {
                _keys[i] = _selector(elements[i]);
            }
        }
    }
}