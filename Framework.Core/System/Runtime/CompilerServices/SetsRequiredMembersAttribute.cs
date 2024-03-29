﻿// https://github.com/dotnet/runtime/blob/v7.0.13/src/libraries/System.Private.CoreLib/src/System/Diagnostics/CodeAnalysis/SetsRequiredMembersAttribute.cs
#if !NET7_0_OR_GREATER
#define SYSTEM_PRIVATE_CORELIB

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Specifies that this constructor sets all required members for the current type, and callers
    /// do not need to set any required members themselves.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
#if SYSTEM_PRIVATE_CORELIB
    public
#else
    internal
#endif
        sealed class SetsRequiredMembersAttribute : Attribute
    { }
}
#endif
