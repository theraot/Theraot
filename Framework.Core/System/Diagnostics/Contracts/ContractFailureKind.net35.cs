#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics.Contracts
{
    public enum ContractFailureKind
    {
        Precondition = 0,
        Postcondition = 1,
        PostconditionOnException = 2,
        Invariant = 3,
        Assert = 4,
        Assume = 5
    }
}

#endif