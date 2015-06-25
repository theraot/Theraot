#if NET20 || NET30 || NET35

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

namespace System.Diagnostics.Contracts
{
    public static class Contract
    {
        /// <summary>
        /// Marker to indicate the end of the contract section of a method.
        /// </summary>
        [Conditional("CONTRACTS_FULL")]
        #if FEATURE_RELIABILITY_CONTRACTS
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        #endif
        public static void EndContractBlock()
        {
            // Empty
        }
    }
}

#endif