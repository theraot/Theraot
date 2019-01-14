#if LESSTHAN_NET45

#pragma warning disable CA1032 // Implement standard exception constructors
#pragma warning disable CA1064 // Exceptions should be public
#pragma warning disable RCS1194 // Implement exception constructors.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.Diagnostics.Contracts
{
    [Serializable]
    internal sealed class ContractException : Exception
    {
        public ContractException(ContractFailureKind kind, string failure, string userMessage, string condition, Exception innerException)
            : base(failure, innerException)
        {
            HResult = ContractHelper.Cor_E_CodeContractFailed;
            Kind = kind;
            UserMessage = userMessage;
            Condition = condition;
        }

        // Called by COM Interop, if we see Cor_E_CodeContractFailed as an HRESULT.
        // ReSharper disable once UnusedMember.Local
        private ContractException()
        {
            HResult = ContractHelper.Cor_E_CodeContractFailed;
        }

        private ContractException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Kind = (ContractFailureKind)info.GetInt32(nameof(Kind));
            UserMessage = info.GetString(nameof(UserMessage));
            Condition = info.GetString(nameof(Condition));
        }

        public string Condition { get; }
        public ContractFailureKind Kind { get; }

        public string UserMessage { get; }

        [SecurityCritical]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Kind), Kind);
            info.AddValue(nameof(UserMessage), UserMessage);
            info.AddValue(nameof(Condition), Condition);
        }
    }
}

#endif