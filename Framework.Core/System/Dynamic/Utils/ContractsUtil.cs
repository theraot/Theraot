// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Dynamic.Utils
{
    internal static class ContractUtils
    {
        public static Exception Unreachable
        {
            get
            {
                Debug.Assert(false, nameof(Unreachable));
                return new InvalidOperationException("Code supposed to be unreachable");
            }
        }

        public static void Requires(bool precondition, string paramName)
        {
            Debug.Assert(!string.IsNullOrEmpty(paramName));

            if (!precondition)
            {
                throw new ArgumentException("Invalid argument value", paramName);
            }
        }

        public static void RequiresNotNull(object value, string paramName)
        {
            Debug.Assert(!string.IsNullOrEmpty(paramName));

            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void RequiresNotEmpty<T>(ICollection<T> collection, string paramName)
        {
            RequiresNotNull(collection, paramName);
            if (collection.Count == 0)
            {
                throw new ArgumentException("Non empty collection required", paramName);
            }
        }

        public static void RequiresNotNullItems<T>(IList<T> array, string arrayName)
        {
            Debug.Assert(arrayName != null);
            RequiresNotNull(array, arrayName);

            for (var i = 0; i < array.Count; i++)
            {
                if (ReferenceEquals(array[i], null))
                {
                    throw new ArgumentNullException(string.Format(Globalization.CultureInfo.CurrentCulture, "{0}[{1}]", arrayName, i));
                }
            }
        }

        public static void RequiresArrayRange<T>(IList<T> array, int offset, int count, string offsetName, string countName)
        {
            Debug.Assert(!string.IsNullOrEmpty(offsetName));
            Debug.Assert(!string.IsNullOrEmpty(countName));
            Debug.Assert(array != null);

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(countName);
            }

            if (offset < 0 || array.Count - offset < count)
            {
                throw new ArgumentOutOfRangeException(offsetName);
            }
        }
    }
}