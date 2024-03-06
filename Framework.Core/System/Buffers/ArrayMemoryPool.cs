#if LESSTHAN_NET40 
// BASEDON: https://github.com/dotnet/runtime/blob/v5.0.0/src/libraries/System.Memory/src/System/Buffers/ArrayMemoryPool.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.



using System.Runtime.InteropServices;

namespace System.Buffers
{
    internal sealed partial class ArrayMemoryPool<T> : MemoryPool<T>
    {
        private const int MaximumBufferSize = int.MaxValue;

        public sealed override int MaxBufferSize => MaximumBufferSize;

        public sealed override IMemoryOwner<T> Rent(int minimumBufferSize = -1)
        {
            if (minimumBufferSize == -1)
                minimumBufferSize = 1 + (4095 / Marshal.SizeOf(typeof(T)));
            else if (((uint)minimumBufferSize) > MaximumBufferSize)
                throw new ArgumentOutOfRangeException(nameof(minimumBufferSize));

            return new ArrayMemoryPoolBuffer(minimumBufferSize);
        }

        protected sealed override void Dispose(bool disposing) { }  // ArrayMemoryPool is a shared pool so Dispose() would be a nop even if there were native resources to dispose.
    }
}
#endif