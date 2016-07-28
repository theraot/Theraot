#if NET35

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
        internal Exception ThrownDuringHandler;
        private readonly string _condition;
        private readonly ContractFailureKind _failureKind;
        private readonly string _message;
        private readonly Exception _originalException;
        private bool _handled;
        private bool _unwind;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public ContractFailedEventArgs(ContractFailureKind failureKind, string message, string condition, Exception originalException)
        {
            Contract.Requires(originalException == null || failureKind == ContractFailureKind.PostconditionOnException);
            _failureKind = failureKind;
            _message = message;
            _condition = condition;
            _originalException = originalException;
        }

        public string Condition
        {
            get
            {
                return _condition;
            }
        }

        public ContractFailureKind FailureKind
        {
            get
            {
                return _failureKind;
            }
        }

        // Whether the event handler "handles" this contract failure, or to fail via escalation policy.
        public bool Handled
        {
            get
            {
                return _handled;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        public Exception OriginalException
        {
            get
            {
                return _originalException;
            }
        }

        public bool Unwind
        {
            get
            {
                return _unwind;
            }
        }

        [SecurityCritical]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void SetHandled()
        {
            _handled = true;
        }

        [SecurityCritical]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void SetUnwind()
        {
            _unwind = true;
        }
    }
}

#endif