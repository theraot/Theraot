using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Theraot.Collections.Specialized
{
    public static class ArrayReadOnlyCollection
    {
        public static ArrayReadOnlyCollection<T> Create<T>(params T[] list)
        {
            return new ArrayReadOnlyCollection<T>(list);
        }
    }

    public class ArrayReadOnlyCollection<T> : ReadOnlyCollectionEx<T>
    {
        protected internal ArrayReadOnlyCollection(params T[] list)
            : base(list)
        {
            Wrapped = list;
        }

        internal T[] Wrapped { get; }

        public override int GetHashCode()
        {
            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var cmp = EqualityComparer<T>.Default;
            var h = 6551;
            foreach (var t in this)
            {
                h ^= (h << 5) ^ cmp.GetHashCode(t);
            }
            return h;
        }
    }
}