#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;

namespace System.Diagnostics.Contracts
{
    public sealed class ContractFailedEventArgs : EventArgs
    {
        internal Exception? ThrownDuringHandler;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public ContractFailedEventArgs(ContractFailureKind failureKind, string message, string condition, Exception originalException)
        {
            Contract.Requires(originalException == null || failureKind == ContractFailureKind.PostconditionOnException);
            FailureKind = failureKind;
            Message = message;
            Condition = condition;
            OriginalException = originalException;
        }

        public string Condition { get; }

        public ContractFailureKind FailureKind { get; }

        // Whether the event handler "handles" this contract failure, or to fail via escalation policy.
        public bool Handled { get; private set; }

        public string Message { get; }

        public Exception? OriginalException { get; }

        public bool Unwind { get; private set; }

        [SecurityCritical]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void SetHandled()
        {
            Handled = true;
        }

        [SecurityCritical]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void SetUnwind()
        {
            Unwind = true;
        }
    }
}

#endif