using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Theraot.Collections.Specialized
{
    public static class HashableReadOnlyCollection
    {
        public static HashableReadOnlyCollection<T> Create<T>(params T[] list)
        {
            return new HashableReadOnlyCollection<T>(list);
        }
    }

    public class HashableReadOnlyCollection<T> : ReadOnlyCollectionEx<T>
    {
        protected internal HashableReadOnlyCollection(params T[] list)
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