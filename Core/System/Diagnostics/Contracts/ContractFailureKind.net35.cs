#if NET20 || NET30 || NET35

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System.Diagnostics.CodeAnalysis;

namespace System.Diagnostics.Contracts
{

    public enum ContractFailureKind
    {
        Precondition,
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Postcondition")]
        Postcondition,
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Postcondition")]
        PostconditionOnException,
        Invariant,
        Assert,
        Assume,
    }
}

#endif