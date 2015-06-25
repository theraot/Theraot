#if NET20 || NET30 || NET35 || NET40
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==

namespace System.Runtime.CompilerServices
{
    public static class ContractHelper
    {
        public static void TriggerFailure(ContractFailureKind kind, string displayMessage, string userMessage, string conditionText, Exception innerException)
        {
            throw new NotImplementedException();
        }

        public static string RaiseContractFailedEvent(ContractFailureKind failureKind, string userMessage, string conditionText, Exception innerException)
        {
            throw new NotImplementedException();
        }
    }
}

#endif