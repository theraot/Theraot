#if LESSTHAN_NET40
//BASEDON: https://github.com/dotnet/runtime/blob/v5.0.0/src/libraries/System.Private.CoreLib/src/System/Buffers/IMemoryOwner.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Buffers
{
    /// <summary>
    /// Owner of Memory<typeparamref name="T"/> that is responsible for disposing the underlying memory appropriately.
    /// </summary>
    public interface IMemoryOwner<T> : IDisposable
    {
        /// <summary>
        /// Returns a Memory<typeparamref name="T"/>.
        /// </summary>
        Memory<T> Memory { get; }
    }
}
#endif