#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Diagnostics;

namespace System.Security.Permissions
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    [Runtime.InteropServices.ComVisible(true)]
    public abstract class SecurityAttribute : Attribute
    {
        protected SecurityAttribute(SecurityAction action)
        {
            Action = action;
        }

        public SecurityAction Action { get; set; }

        public bool Unrestricted { get; set; }

        public abstract IPermission CreatePermission();
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    [Runtime.InteropServices.ComVisible(true)]
    public abstract class CodeAccessSecurityAttribute : SecurityAttribute
    {
        protected CodeAccessSecurityAttribute(SecurityAction action)
            : base(action)
        {
            // Empty
        }
    }

    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class SecurityPermissionAttribute : CodeAccessSecurityAttribute
    {
        public SecurityPermissionAttribute(SecurityAction action)
            : base(action)
        {
        }

        public SecurityPermissionFlag Flags { get; set; } = SecurityPermissionFlag.NoFlags;

        public bool Assertion
        {
            get => (Flags & SecurityPermissionFlag.Assertion) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.Assertion : Flags & ~SecurityPermissionFlag.Assertion;
        }

        public bool UnmanagedCode
        {
            get => (Flags & SecurityPermissionFlag.UnmanagedCode) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.UnmanagedCode : Flags & ~SecurityPermissionFlag.UnmanagedCode;
        }

        public bool SkipVerification
        {
            get => (Flags & SecurityPermissionFlag.SkipVerification) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.SkipVerification : Flags & ~SecurityPermissionFlag.SkipVerification;
        }

        public bool Execution
        {
            get => (Flags & SecurityPermissionFlag.Execution) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.Execution : Flags & ~SecurityPermissionFlag.Execution;
        }

        public bool ControlThread
        {
            get => (Flags & SecurityPermissionFlag.ControlThread) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.ControlThread : Flags & ~SecurityPermissionFlag.ControlThread;
        }

        public bool ControlEvidence
        {
            get => (Flags & SecurityPermissionFlag.ControlEvidence) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.ControlEvidence : Flags & ~SecurityPermissionFlag.ControlEvidence;
        }

        public bool ControlPolicy
        {
            get => (Flags & SecurityPermissionFlag.ControlPolicy) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.ControlPolicy : Flags & ~SecurityPermissionFlag.ControlPolicy;
        }

        public bool SerializationFormatter
        {
            get => (Flags & SecurityPermissionFlag.SerializationFormatter) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.SerializationFormatter : Flags & ~SecurityPermissionFlag.SerializationFormatter;
        }

        public bool ControlDomainPolicy
        {
            get => (Flags & SecurityPermissionFlag.ControlDomainPolicy) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.ControlDomainPolicy : Flags & ~SecurityPermissionFlag.ControlDomainPolicy;
        }

        public bool ControlPrincipal
        {
            get => (Flags & SecurityPermissionFlag.ControlPrincipal) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.ControlPrincipal : Flags & ~SecurityPermissionFlag.ControlPrincipal;
        }

        public bool ControlAppDomain
        {
            get => (Flags & SecurityPermissionFlag.ControlAppDomain) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.ControlAppDomain : Flags & ~SecurityPermissionFlag.ControlAppDomain;
        }

        public bool RemotingConfiguration
        {
            get => (Flags & SecurityPermissionFlag.RemotingConfiguration) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.RemotingConfiguration : Flags & ~SecurityPermissionFlag.RemotingConfiguration;
        }

        [Runtime.InteropServices.ComVisible(true)]
        public bool Infrastructure
        {
            get => (Flags & SecurityPermissionFlag.Infrastructure) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.Infrastructure : Flags & ~SecurityPermissionFlag.Infrastructure;
        }

        public bool BindingRedirects
        {
            get => (Flags & SecurityPermissionFlag.BindingRedirects) != 0;
            set => Flags = value ? Flags | SecurityPermissionFlag.BindingRedirects : Flags & ~SecurityPermissionFlag.BindingRedirects;
        }

        public override IPermission CreatePermission()
        {
            if (Unrestricted)
            {
                return new SecurityPermission(PermissionState.Unrestricted);
            }
            else
            {
                return new SecurityPermission(Flags);
            }
        }
    }

    public sealed class SecurityPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        public SecurityPermission(PermissionState state)
        {
            Theraot.No.Op(state);
        }

        public SecurityPermission(SecurityPermissionFlag flag)
        {
            Theraot.No.Op(flag);
        }

        public SecurityPermissionFlag Flags { get; set; }
        public override IPermission Copy() { return this; }
        public override void FromXml(SecurityElement esd) { }
        public override IPermission Intersect(IPermission target) { return default; }
        public override bool IsSubsetOf(IPermission target) { return false; }
        public bool IsUnrestricted() { return false; }
        public override SecurityElement ToXml() { return default; }
        public override IPermission Union(IPermission target) { return default; }
    }
}

#endif