#if LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Permissions
{
    [System.Runtime.InteropServices.ComVisible(true)]
    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Constructor | System.AttributeTargets.Method | System.AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
    [System.Serializable]
    public sealed class HostProtectionAttribute : CodeAccessSecurityAttribute
    {
        public HostProtectionAttribute() : base(default(SecurityAction))
        {
        }

        public HostProtectionAttribute(SecurityAction action) : base(default(SecurityAction))
        {
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
            return default(IPermission);
        }
    }
}

#endif