#if NET20 || NET30 || NET35

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System.Runtime.ConstrainedExecution;

namespace System.Diagnostics.Contracts
{
    public static class Contract
    {
        /// <summary>
        /// Marker to indicate the end of the contract section of a method.
        /// </summary>
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void EndContractBlock()
        {
            // Empty
        }
    }
}

#endif