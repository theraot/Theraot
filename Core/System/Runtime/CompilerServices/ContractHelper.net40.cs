#if NET20 || NET30 || NET35 || NET40

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
        [DebuggerNonUserCode]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static string RaiseContractFailedEvent(ContractFailureKind failureKind, string userMessage, string conditionText, Exception innerException)
        {
            var resultFailureMessage = "Contract failed"; // default in case implementation does not assign anything.
            RaiseContractFailedEventImplementation(failureKind, userMessage, conditionText, innerException, ref resultFailureMessage);
            return resultFailureMessage;
        }

        [DebuggerNonUserCode]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void TriggerFailure(ContractFailureKind kind, string displayMessage, string userMessage, string conditionText, Exception innerException)
        {
            TriggerFailureImplementation(kind, displayMessage, userMessage, conditionText, innerException);
        }

        internal const int Cor_E_Codecontractfailed = unchecked((int)0x80131542);
        private static readonly SafeCollection<EventHandler<ContractFailedEventArgs>> _contractFailedEvent = new SafeCollection<EventHandler<ContractFailedEventArgs>>();

        /// <summary>
        /// Allows a managed application environment such as an interactive interpreter (IronPython) or a
        /// web browser host (Jolt hosting Silverlight in IE) to be notified of contract failures and
        /// potentially "handle" them, either by throwing a particular exception type, etc.  If any of the
        /// event handlers sets the Cancel flag in the ContractFailedEventArgs, then the Contract class will
        /// not pop up an assert dialog box or trigger escalation policy.  Hooking this event requires
        /// full trust.
        /// </summary>
        internal static event EventHandler<ContractFailedEventArgs> InternalContractFailed
        {
            [SecurityCritical]
            add
            {
                // Eagerly prepare each event handler _marked with a reliability contract_, to
                // attempt to reduce out of memory exceptions while reporting contract violations.
                // This only works if the new handler obeys the constraints placed on
                // constrained execution regions.  Eagerly preparing non-reliable event handlers
                // would be a perf hit and wouldn't significantly improve reliability.
                // UE: Please mention reliable event handlers should also be marked with the
                // PrePrepareMethodAttribute to avoid CER eager preparation work when ngen'ed.
                // System.Runtime.CompilerServices.RuntimeHelpers.PrepareContractedDelegate(value); // TODO? I'm afraid I can't do that.
                _contractFailedEvent.Add(value);
            }
            [SecurityCritical]
            remove
            {
                _contractFailedEvent.Remove(value);
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private static string GetDisplayMessage(ContractFailureKind failureKind, string userMessage,
            string conditionText)
        {
            // Well-formatted English messages will take one of four forms.  A sentence ending in
            // either a period or a colon, the condition string, then the message tacked
            // on to the end with two spaces in front.
            // Note that both the conditionText and userMessage may be null.  Also,
            // on Silverlight we may not be able to look up a friendly string for the
            // error message.  Let's leverage Silverlight's default error message there.
            var failureMessage = ContractHelperEx.GetFailureMessage(failureKind, conditionText);

            // Now add in the user message, if present.
            if (!string.IsNullOrEmpty(userMessage))
            {
                return failureMessage + "  " + userMessage;
            }
            return failureMessage;
        }

        [DebuggerNonUserCode]
        [SecuritySafeCritical]
        private static void RaiseContractFailedEventImplementation(ContractFailureKind failureKind, string userMessage,
            string conditionText, Exception innerException, ref string resultFailureMessage)
        {
            if (failureKind < ContractFailureKind.Precondition || failureKind > ContractFailureKind.Assume)
            {
                throw new ArgumentException(string.Format("Invalid enum value: {0}", failureKind), "failureKind");
            }

            Contract.EndContractBlock();

            string returnValue;
            var displayMessage = "contract failed."; // Incomplete, but in case of OOM during resource lookup...
            ContractFailedEventArgs eventArgs = null; // In case of OOM.
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                displayMessage = GetDisplayMessage(failureKind, userMessage, conditionText);
                var contractFailedEventLocal = _contractFailedEvent;
                if (contractFailedEventLocal != null)
                {
                    eventArgs = new ContractFailedEventArgs(failureKind, displayMessage, conditionText, innerException);
                    foreach (var @delegate in contractFailedEventLocal)
                    {
                        var handler = @delegate;
                        try
                        {
                            handler(null, eventArgs);
                        }
                        catch (Exception e)
                        {
#if NET20 || NET30 || NET35
                            eventArgs.ThrownDuringHandler = e;
#else
                            GC.KeepAlive(e);
#endif
                            eventArgs.SetUnwind();
                        }
                    }
                    if (eventArgs.Unwind)
                    {
#if NET20 || NET30 || NET35
                        // unwind
                        if (innerException == null)
                        {
                            innerException = eventArgs.ThrownDuringHandler;
                        }
#endif
                        throw new ContractException(failureKind, displayMessage, userMessage, conditionText, innerException);
                    }
                }
            }
            finally
            {
                if (eventArgs != null && eventArgs.Handled)
                {
                    returnValue = null; // handled
                }
                else
                {
                    returnValue = displayMessage;
                }
            }
            resultFailureMessage = returnValue;
        }

        [DebuggerNonUserCode]
        [SecuritySafeCritical]
        private static void TriggerFailureImplementation(ContractFailureKind kind, string displayMessage,
            string userMessage, string conditionText, Exception innerException)
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