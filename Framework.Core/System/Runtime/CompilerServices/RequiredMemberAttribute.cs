// https://github.com/dotnet/runtime/blob/v7.0.13/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/RequiredMemberAttribute.cs
#if !NET7_0_OR_GREATER
#define SYSTEM_PRIVATE_CORELIB

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.CompilerServices
{
    /// <summary>Specifies that a type has required members or that a member is required.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#if SYSTEM_PRIVATE_CORELIB
    public
#else
    internal
#endif
        sealed class RequiredMemberAttribute : Attribute
    { }
}
#endif
