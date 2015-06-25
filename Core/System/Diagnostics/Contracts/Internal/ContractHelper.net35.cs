#if NET20 || NET30 || NET35

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

// Note: In .NET FX 4.5, we duplicated the ContractHelper class in the System.Runtime.CompilerServices
// namespace to remove an ugly wart of a namespace from the Windows 8 profile.  But we still need the
// old locations left around, so we can support rewritten .NET FX 4.0 libraries.  Consider removing
// these from our reference assembly in a future version.

using System.Diagnostics.CodeAnalysis;

namespace System.Diagnostics.Contracts.Internal
{
    [Obsolete("Use the ContractHelper class in the System.Runtime.CompilerServices namespace instead.")]
    public static class ContractHelper
    {
        /// <summary>
        /// Rewriter will call this method on a contract failure to allow listeners to be notified.
        /// The method should not perform any failure (assert/throw) itself.
        /// </summary>
        /// <returns>null if the event was handled and should not trigger a failure.
        ///          Otherwise, returns the localized failure message</returns>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [System.Diagnostics.DebuggerNonUserCode]
        #if FEATURE_RELIABILITY_CONTRACTS
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        #endif
        public static string RaiseContractFailedEvent(ContractFailureKind failureKind, String userMessage, String conditionText, Exception innerException)
        {
            return System.Runtime.CompilerServices.ContractHelper.RaiseContractFailedEvent(failureKind, userMessage, conditionText, innerException);
        }

        /// <summary>
        /// Rewriter calls this method to get the default failure behavior.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        #if FEATURE_RELIABILITY_CONTRACTS
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        #endif
        public static void TriggerFailure(ContractFailureKind kind, String displayMessage, String userMessage, String conditionText, Exception innerException)
        {
            System.Runtime.CompilerServices.ContractHelper.TriggerFailure(kind, displayMessage, userMessage, conditionText, innerException);
        }
    }
}

#endif