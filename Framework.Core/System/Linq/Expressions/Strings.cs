#if LESSTHAN_NET45

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions
{
    /// <summary>
    ///    Strongly-typed and parameterized string resources.
    /// </summary>
    internal static class Strings
    {

        /// <summary>
        /// A string like "More than one method '{0}' on type '{1}' is compatible with the supplied arguments."
        /// </summary>
        internal static string MethodWithMoreThanOneMatch(object p0, object p1) => SR.Format(SR.MethodWithMoreThanOneMatch, p0, p1);

        /// <summary>
        /// A string like "{0} must be greater than or equal to {1}"
        /// </summary>
        internal static string OutOfRange(object p0, object p1) => SR.Format(SR.OutOfRange, p0, p1);
    }
}

#endif