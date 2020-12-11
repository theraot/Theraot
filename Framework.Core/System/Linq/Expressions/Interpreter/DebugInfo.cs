#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class DebugInfo
    {
        public string? FileName;
        public int Index;
        public bool IsClear;
        public int StartLine, EndLine;
        private static readonly DebugInfoComparer _debugComparer = new DebugInfoComparer();

        public static DebugInfo? GetMatchingDebugInfo(DebugInfo[] debugInfos, int index)
        {
            //Create a faked DebugInfo to do the search
            var d = new DebugInfo
            {
                Index = index
            };

            //to find the closest debug info before the current index

            var i = Array.BinarySearch(debugInfos, d, _debugComparer);
            if (i >= 0)
            {
                return debugInfos[i];
            }

            //~i is the index for the first bigger element
            //if there is no bigger element, ~i is the length of the array
            i = ~i;
            if (i == 0)
            {
                return null;
            }

            //return the last one that is smaller
            i--;

            return debugInfos[i];
        }

        public override string ToString()
        {
            return IsClear ? string.Format(CultureInfo.InvariantCulture, "{0}: clear", Index) : string.Format(CultureInfo.InvariantCulture, "{0}: [{1}-{2}] '{3}'", Index, StartLine, EndLine, FileName);
        }

        private sealed class DebugInfoComparer : IComparer<DebugInfo>
        {
            //We allow comparison between int and DebugInfo here
            int IComparer<DebugInfo>.Compare(DebugInfo x, DebugInfo y)
            {
                var d1Index = x.Index;
                var d2Index = y.Index;
                if (d1Index > d2Index)
                {
                    return 1;
                }

                if (d1Index == d2Index)
                {
                    return 0;
                }

                return -1;
            }
        }
    }
}

#endif