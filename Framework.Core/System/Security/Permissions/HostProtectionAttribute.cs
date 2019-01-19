#if LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Permissions
{
    [Runtime.InteropServices.ComVisible(true)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
    [Serializable]
    public sealed class HostProtectionAttribute : CodeAccessSecurityAttribute
    {
        public HostProtectionAttribute()
            : base(default)
        {
            // Empty
        }

        public HostProtectionAttribute(SecurityAction action)
            : base(default)
        {
            Theraot.No.Op(action);
        }

        public bool ExternalProcessMgmt { get; set; }
        public bool ExternalThreading { get; set; }
        public bool MayLeakOnAbort { get; set; }
        public HostProtectionResource Resources { get; set; }
        public bool SecurityInfrastructure { get; set; }
        public bool SelfAffectingProcessMgmt { get; set; }
        public bool SelfAffectingThreading { get; set; }
        public bool SharedState { get; set; }
        public bool Synchronization { get; set; }
        public bool UI { get; set; }

        public override IPermission CreatePermission()
        {
            return default;
        }
    }
}

#endif