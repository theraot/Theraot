#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.ConstrainedExecution;

namespace System.Diagnostics.Contracts.Internal
{
    [Obsolete("Use the ContractHelper class in the System.Runtime.CompilerServices namespace instead.")]
    public static class ContractHelper
    {
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