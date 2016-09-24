#if NET35 || NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Security;
using System.Diagnostics.Contracts;

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
                    result = withCondition ? string.Format("Assertion failed: {0}", conditionText) : "Assertion failed.";
                    break;

                case ContractFailureKind.Assume:
                    result = withCondition ? string.Format("Assumption failed: {0}", conditionText) : "Assumption failed.";
                    break;

                case ContractFailureKind.Precondition:
                    result = withCondition ? string.Format("Precondition failed: {0}", conditionText) : "Precondition failed.";
                    break;

                case ContractFailureKind.Postcondition:
                    result = withCondition ? string.Format("Postcondition failed: {0}", conditionText) : "Postcondition failed.";
                    break;

                case ContractFailureKind.Invariant:
                    result = withCondition ? string.Format("Invariant failed: {0}", conditionText) : "Invariant failed.";
                    break;

                case ContractFailureKind.PostconditionOnException:
                    result = withCondition ? string.Format("Postcondition failed after throwing an exception: {0}", conditionText) : "Postcondition failed after throwing an exception.";
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