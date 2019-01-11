#if LESSTHAN_NET45

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security;

namespace System.Runtime.CompilerServices
{
    internal static class ContractHelperEx
    {
        [SecuritySafeCritical]
        internal static void Fail(string message)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            else
            {
                Environment.FailFast(message);
            }
        }

        internal static string GetFailureMessage(ContractFailureKind failureKind)
        {
            return GetFailureMessage(failureKind, "");
        }

        internal static string GetFailureMessage(ContractFailureKind failureKind, string conditionText)
        {
            string result;
            var withCondition = !string.IsNullOrEmpty(conditionText);
            switch (failureKind)
            {
                case ContractFailureKind.Assert:
                    result = withCondition ? $"Assertion failed: {conditionText}" : "Assertion failed.";
                    break;

                case ContractFailureKind.Assume:
                    result = withCondition ? $"Assumption failed: {conditionText}" : "Assumption failed.";
                    break;

                case ContractFailureKind.Precondition:
                    result = withCondition ? $"Precondition failed: {conditionText}" : "Precondition failed.";
                    break;

                case ContractFailureKind.Postcondition:
                    result = withCondition ? $"Postcondition failed: {conditionText}" : "Postcondition failed.";
                    break;

                case ContractFailureKind.Invariant:
                    result = withCondition ? $"Invariant failed: {conditionText}" : "Invariant failed.";
                    break;

                case ContractFailureKind.PostconditionOnException:
                    result = withCondition ? $"Postcondition failed after throwing an exception: {conditionText}" : "Postcondition failed after throwing an exception.";
                    break;

                default:
                    result = "Assumption failed.";
                    Contract.Assume(false, "Unreachable code");
                    break;
            }
            return result;
        }
    }
}

#endif