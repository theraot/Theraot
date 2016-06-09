#if NET20 || NET30 || NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;


// Note: In .NET FX 4.5, we duplicated the ContractHelper class in the System.Runtime.CompilerServices
// namespace to remove an ugly wart of a namespace from the Windows 8 profile.  But we still need the
// old locations left around, so we can support rewritten .NET FX 4.0 libraries.  Consider removing
// these from our reference assembly in a future version.
namespace System.Diagnostics.Contracts.Internal
{
    [Obsolete("Use the ContractHelper class in the System.Runtime.CompilerServices namespace instead.")]
    public static class ContractHelper
    {

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [DebuggerNonUserCode]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static string RaiseContractFailedEvent(ContractFailureKind failureKind, string userMessage, string conditionText, Exception innerException)
        {
            return Runtime.CompilerServices.ContractHelper.RaiseContractFailedEvent(failureKind, userMessage, conditionText, innerException);
        }

        [DebuggerNonUserCode]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void TriggerFailure(ContractFailureKind kind, string displayMessage, string userMessage, string conditionText, Exception innerException)
        {
            Runtime.CompilerServices.ContractHelper.TriggerFailure(kind, displayMessage, userMessage, conditionText, innerException);
        }

    }
}

#endif