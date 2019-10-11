#if LESSTHAN_NET45

#pragma warning disable CA1030 // Use events where appropriate

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
**
** Implementation details of CLR Contracts.
**
===========================================================*/

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.ConstrainedExecution;
using System.Security;
using Theraot.Collections.ThreadSafe;

namespace System.Runtime.CompilerServices
{
    public static class ContractHelper
    {
        internal const int Cor_E_CodeContractFailed = unchecked((int)0x80131542);

        private static readonly IEvent<ContractFailedEventArgs> _contractFailedEvent = new StrongEvent<ContractFailedEventArgs>(true);

        /// <summary>
        ///     Allows a managed application environment such as an interactive interpreter (IronPython) or a
        ///     web browser host (Jolt hosting Silverlight in IE) to be notified of contract failures and
        ///     potentially "handle" them, either by throwing a particular exception type, etc.  If any of the
        ///     event handlers sets the Cancel flag in the ContractFailedEventArgs, then the Contract class will
        ///     not pop up an assert dialog box or trigger escalation policy.  Hooking this event requires
        ///     full trust.
        /// </summary>
        internal static event EventHandler<ContractFailedEventArgs> InternalContractFailed
        {
            [SecurityCritical]
            add => _contractFailedEvent.Add(value);
            [SecurityCritical]
            remove => _contractFailedEvent.Remove(value);
        }

        [DebuggerNonUserCode]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static string? RaiseContractFailedEvent(ContractFailureKind failureKind, string? userMessage, string? conditionText, Exception? innerException)
        {
            string? resultFailureMessage = "Contract failed"; // default in case implementation does not assign anything.
            RaiseContractFailedEventImplementation(failureKind, userMessage, conditionText, innerException, ref resultFailureMessage);
            return resultFailureMessage;
        }

        [DebuggerNonUserCode]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void TriggerFailure(ContractFailureKind kind, string displayMessage, string? userMessage, string? conditionText, Exception? innerException)
        {
            TriggerFailureImplementation(kind, displayMessage, userMessage, conditionText, innerException);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private static string GetDisplayMessage(ContractFailureKind failureKind, string? userMessage, string? conditionText)
        {
            // Well-formatted English messages will take one of four forms.  A sentence ending in
            // either a period or a colon, the condition string, then the message tacked
            // on to the end with two spaces in front.
            // Note that both the conditionText and userMessage may be null.  Also,
            // on Silverlight we may not be able to look up a friendly string for the
            // error message.  Let's leverage Silverlight's default error message there.
            var failureMessage = ContractHelperEx.GetFailureMessage(failureKind, conditionText);

            // Now add in the user message, if present.
            if (!(userMessage == null || string.IsNullOrEmpty(userMessage)))
            {
                return failureMessage + "  " + userMessage;
            }

            return failureMessage;
        }

        [DebuggerNonUserCode]
        [SecuritySafeCritical]
        private static void RaiseContractFailedEventImplementation(ContractFailureKind failureKind, string? userMessage, string? conditionText, Exception? innerException, ref string? resultFailureMessage)
        {
            if (failureKind < ContractFailureKind.Precondition || failureKind > ContractFailureKind.Assume)
            {
                throw new ArgumentException($"Invalid enum value: {failureKind}", nameof(failureKind));
            }

            Contract.EndContractBlock();

            string? returnValue;
            var displayMessage = "contract failed."; // Incomplete, but in case of OOM during resource lookup...
            ContractFailedEventArgs? eventArgs = null; // In case of OOM.
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                displayMessage = GetDisplayMessage(failureKind, userMessage, conditionText);
                eventArgs = new ContractFailedEventArgs(failureKind, displayMessage, conditionText, innerException);
                _contractFailedEvent.Invoke
                (
                    OnException,
                    null,
                    eventArgs
                );
                if (eventArgs.Unwind)
                {
#if LESSTHAN_NET40
                    // unwind
                    if (innerException == null)
                    {
                        innerException = eventArgs.ThrownDuringHandler;
                    }
#endif
                    throw new ContractException(failureKind, displayMessage, userMessage, conditionText, innerException);
                }
            }
            finally
            {
                returnValue = eventArgs?.Handled == true ? null : displayMessage;
            }

            resultFailureMessage = returnValue;

            void OnException(Exception exception)
            {
#if LESSTHAN_NET40
                eventArgs.ThrownDuringHandler = exception;
#else
                Theraot.No.Op(exception);
#endif
                eventArgs.SetUnwind();
            }
        }

        [DebuggerNonUserCode]
        [SecuritySafeCritical]
        private static void TriggerFailureImplementation(ContractFailureKind kind, string displayMessage, string? userMessage, string? conditionText, Exception? innerException)
        {
            // If we're here, our intent is to pop up a dialog box (if we can).  For developers
            // interacting live with a debugger, this is a good experience.  For Silverlight
            // hosted in Internet Explorer, the assert window is great.  If we cannot
            // pop up a dialog box, throw an exception (consider a library compiled with
            // "Assert On Failure" but used in a process that can't pop up asserts, like an
            // NT Service).  For the CLR hosted by server apps like SQL or Exchange, we should
            // trigger escalation policy.
            if (!Environment.UserInteractive)
            {
                throw new ContractException(kind, displayMessage, userMessage, conditionText, innerException);
            }

            ContractHelperEx.Fail(displayMessage);
        }
    }
}

#endif