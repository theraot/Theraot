#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Class that contains helper methods for DLR CallSites.
    /// </summary>
    public static class CallSiteHelpers
    {
        // ReSharper disable once PossibleNullReferenceException
        private static readonly Type _knownNonDynamicMethodType = typeof(object).GetMethod(nameof(ToString)).GetType();

        /// <summary>
        /// Checks if a <see cref="MethodBase"/> is internally used by DLR and should not
        /// be displayed on the language code's stack.
        /// </summary>
        /// <param name="mb">The input <see cref="MethodBase"/></param>
        /// <returns>
        /// True if the input <see cref="MethodBase"/> is internally used by DLR and should not
        /// be displayed on the language code's stack. Otherwise, false.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool IsInternalFrame(MethodBase mb)
        {
            // All the dynamic methods created for DLR rules have a special name.
            // We also check if the method has a different type than the known
            // non-static method. If it does, it is a dynamic method.
            // This could be improved if the CLR provides a way to attach some information
            // to the dynamic method we create, like CustomAttributes.
            if (mb.Name == CallSite.CallSiteTargetMethodName && mb.GetType() != _knownNonDynamicMethodType)
            {
                return true;
            }
            return false;
        }
    }
}

#endif