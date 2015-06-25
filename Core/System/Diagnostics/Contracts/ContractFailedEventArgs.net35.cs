#if NET20 || NET30 || NET35

// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==

namespace System.Diagnostics.Contracts
{
    public sealed class ContractFailedEventArgs : EventArgs
    {
        private ContractFailureKind _failureKind;
        private String _message;
        private String _condition;
        private Exception _originalException;
        private bool _handled;
        private bool _unwind;

        internal Exception thrownDuringHandler;

        public ContractFailedEventArgs(ContractFailureKind failureKind, String message, String condition, Exception originalException)
        {
            // TODO: Contract.Requires
            // Contract.Requires(originalException == null || failureKind == ContractFailureKind.PostconditionOnException);
            _failureKind = failureKind;
            _message = message;
            _condition = condition;
            _originalException = originalException;
        }

        public String Message
        {
            get
            {
                return _message;
            }
        }

        public String Condition
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

        public Exception OriginalException
        {
            get
            {
                return _originalException;
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

        public void SetHandled()
        {
            _handled = true;
        }

        public bool Unwind
        {
            get
            {
                return _unwind;
            }
        }

        public void SetUnwind()
        {
            _unwind = true;
        }
    }
}

#endif