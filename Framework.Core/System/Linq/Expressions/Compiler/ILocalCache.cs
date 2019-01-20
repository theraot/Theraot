#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection.Emit;

namespace System.Linq.Expressions.Compiler
{
    internal interface ILocalCache
    {
        void FreeLocal(LocalBuilder local);

        LocalBuilder GetLocal(Type type);
    }
}

#endif