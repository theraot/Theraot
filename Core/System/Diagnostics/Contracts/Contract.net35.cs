#if NET20 || NET30 || NET35

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;

namespace System.Diagnostics.Contracts
{
    public static class Contract
    {
        /// <summary>
        /// Allows a managed application environment such as an interactive interpreter (IronPython)
        /// to be notified of contract failures and 
        /// potentially "handle" them, either by throwing a particular exception type, etc.  If any of the
        /// event handlers sets the Cancel flag in the ContractFailedEventArgs, then the Contract class will
        /// not pop up an assert dialog box or trigger escalation policy.  Hooking this event requires 
        /// full trust, because it will inform you of bugs in the appdomain and because the event handler
        /// could allow you to continue execution.
        /// </summary>
        public static event EventHandler<ContractFailedEventArgs> ContractFailed
        {
            [SecurityCritical]
            [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
            add
            {
                System.Runtime.CompilerServices.ContractHelper.InternalContractFailed += value;
            }
            [SecurityCritical]
            [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
            remove
            {
                System.Runtime.CompilerServices.ContractHelper.InternalContractFailed -= value;
            }
        }

        /// <summary>
        /// In debug builds, perform a runtime check that <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">Expression to check to always be true.</param>
        [Pure]
        [Conditional("DEBUG")]
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Assert(bool condition)
        {
            if (!condition)
                ReportFailure(ContractFailureKind.Assert, null, null, null);
        }

        /// <summary>
        /// In debug builds, perform a runtime check that <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">Expression to check to always be true.</param>
        /// <param name="userMessage">If it is not a constant string literal, then the contract may not be understood by tools.</param>
        [Pure]
        [Conditional("DEBUG")]
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Assert(bool condition, String userMessage)
        {
            if (!condition)
                ReportFailure(ContractFailureKind.Assert, userMessage, null, null);
        }

        /// <summary>
        /// Without contract rewriting, failing Assert/Assumes end up calling this method.
        /// Code going through the contract rewriter never calls this method. Instead, the rewriter produced failures call
        /// System.Runtime.CompilerServices.ContractHelper.RaiseContractFailedEvent, followed by 
        /// System.Runtime.CompilerServices.ContractHelper.TriggerFailure.
        /// </summary>
        [SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Security.SecuritySafeCriticalAttribute")]
        [System.Diagnostics.DebuggerNonUserCode]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        static void ReportFailure(ContractFailureKind failureKind, String userMessage, String conditionText, Exception innerException)
        {
            if (failureKind < ContractFailureKind.Precondition || failureKind > ContractFailureKind.Assume)
            {
                throw new ArgumentException(string.Format("Invalid enum value: {0}", failureKind), "failureKind");
            }
            Contract.EndContractBlock();

            // displayMessage == null means: yes we handled it. Otherwise it is the localized failure message
            var displayMessage = System.Runtime.CompilerServices.ContractHelper.RaiseContractFailedEvent(failureKind, userMessage, conditionText, innerException);

            if (displayMessage == null) return;

            System.Runtime.CompilerServices.ContractHelper.TriggerFailure(failureKind, displayMessage, userMessage, conditionText, innerException);
        }

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