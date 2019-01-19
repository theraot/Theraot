#if LESSTHAN_NETSTANDARD13

#pragma warning disable CA2235 // Mark all non-serializable fields

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
** Purpose: Default IComparer implementation.
**
===========================================================*/

using System.Globalization;
using System.Runtime.Serialization;

namespace System.Collections
{
    [Serializable]
    public sealed class Comparer : IComparer, ISerializable
    {

        public static readonly Comparer Default = new Comparer(CultureInfo.CurrentCulture);
        public static readonly Comparer DefaultInvariant = new Comparer(CultureInfo.InvariantCulture);
        private readonly CompareInfo _compareInfo;

        public Comparer(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            _compareInfo = culture.CompareInfo;
        }

        private Comparer(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            _compareInfo = (CompareInfo)info.GetValue("CompareInfo", typeof(CompareInfo));
        }

        // Compares two Objects by calling CompareTo.
        // If a == b, 0 is returned.
        // If a implements IComparable, a.CompareTo(b) is returned.
        // If a doesn't implement IComparable and b does, -(b.CompareTo(a)) is returned.
        // Otherwise an exception is thrown.
        //
        public int Compare(object x, object y)
        {
            if (x == y)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (x is string sa && y is string sb)
            {
                return _compareInfo.Compare(sa, sb);
            }

            if (x is IComparable ia)
            {
                return ia.CompareTo(y);
            }

            if (y is IComparable ib)
            {
                return -ib.CompareTo(x);
            }

            throw new ArgumentException("At least one object must implement IComparable.");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("CompareInfo", _compareInfo);
        }
    }
}

#endif