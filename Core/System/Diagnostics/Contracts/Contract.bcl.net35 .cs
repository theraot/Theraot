#if NET20 || NET30 || NET35

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
#define DEBUG // The behavior of this contract library should be consistent regardless of build type.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;

namespace System.Diagnostics.Contracts
{
    public static partial class Contract
    {
        [ThreadStatic]
        private static bool _assertingMustUseRewriter;

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
                ContractHelper.InternalContractFailed += value;
            }
            [SecurityCritical]
            [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
            remove
            {
                ContractHelper.InternalContractFailed -= value;
            }
        }

        [SecuritySafeCritical]
        private static void AssertMustUseRewriter(ContractFailureKind kind, string contractKind)
        {
            if (_assertingMustUseRewriter)
            {
                ContractHelperEx.Fail("Asserting that we must use the rewriter went reentrant."); // Didn't rewrite this mscorlib?
            }
            _assertingMustUseRewriter = true;

            // For better diagnostics, report which assembly is at fault.  Walk up stack and
            // find the first non-mscorlib assembly.
            var thisAssembly = typeof(Contract).Assembly;  // In case we refactor mscorlib, use Contract class instead of Object.
            var stack = new StackTrace();
            Assembly probablyNotRewritten = null;
            for (var i = 0; i < stack.FrameCount; i++)
            {
                var declaringType = stack.GetFrame(i).GetMethod().DeclaringType;
                if (declaringType == null)
                {
                    // Not standard method info - ignoring
                    continue;
                }
                var caller = declaringType.Assembly;
                if (thisAssembly.Equals(caller))
                {
                    continue;
                }
                probablyNotRewritten = caller;
                break;
            }

            if (probablyNotRewritten == null)
            {
                probablyNotRewritten = thisAssembly;
            }
            var simpleName = probablyNotRewritten.GetName().Name;
            ContractHelper.TriggerFailure(kind, string.Format("The code has not been rewriten. ContractKind: {0} - Source: {1}", contractKind, simpleName), null, null, null);

            _assertingMustUseRewriter = false;
        }

        [SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Security.SecuritySafeCriticalAttribute")]
        [DebuggerNonUserCode]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private static void ReportFailure(ContractFailureKind failureKind, string userMessage, string conditionText, Exception innerException)
        {
            if (failureKind < ContractFailureKind.Precondition || failureKind > ContractFailureKind.Assume)
            {
                throw new ArgumentException(string.Format("Invalid enum value: {0}", failureKind), "failureKind");
            }

            EndContractBlock();

            // displayMessage == null means: yes we handled it. Otherwise it is the localized failure message
            var displayMessage = ContractHelper.RaiseContractFailedEvent(failureKind, userMessage, conditionText, innerException);

            if (displayMessage == null)
            {
                return;
            }

            ContractHelper.TriggerFailure(failureKind, displayMessage, userMessage, conditionText, innerException);
        }
    }
}

#endif