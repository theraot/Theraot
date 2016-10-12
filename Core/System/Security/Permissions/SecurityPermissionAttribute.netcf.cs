using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    [ComVisible(true)]
    [Serializable]
    public sealed class SecurityPermissionAttribute : CodeAccessSecurityAttribute
    {
        private SecurityPermissionFlag _flags;

        public bool Assertion
        {
            get
            {
                return (_flags & SecurityPermissionFlag.Assertion) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.Assertion : _flags & (SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool BindingRedirects
        {
            get
            {
                return (_flags & SecurityPermissionFlag.BindingRedirects) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.BindingRedirects : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure));
            }
        }

        public bool ControlAppDomain
        {
            get
            {
                return (_flags & SecurityPermissionFlag.ControlAppDomain) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.ControlAppDomain : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool ControlDomainPolicy
        {
            get
            {
                return (_flags & SecurityPermissionFlag.ControlDomainPolicy) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.ControlDomainPolicy : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool ControlEvidence
        {
            get
            {
                return (_flags & SecurityPermissionFlag.ControlEvidence) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.ControlEvidence : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool ControlPolicy
        {
            get
            {
                return (_flags & SecurityPermissionFlag.ControlPolicy) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.ControlPolicy : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool ControlPrincipal
        {
            get
            {
                return (_flags & SecurityPermissionFlag.ControlPrincipal) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.ControlPrincipal : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool ControlThread
        {
            get
            {
                return (_flags & SecurityPermissionFlag.ControlThread) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.ControlThread : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool Execution
        {
            get
            {
                return (_flags & SecurityPermissionFlag.Execution) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.Execution : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public SecurityPermissionFlag Flags
        {
            get
            {
                return _flags;
            }
            set
            {
                _flags = value;
            }
        }

        [ComVisible(true)]
        public bool Infrastructure
        {
            get
            {
                return (_flags & SecurityPermissionFlag.Infrastructure) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.Infrastructure : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool RemotingConfiguration
        {
            get
            {
                return (_flags & SecurityPermissionFlag.RemotingConfiguration) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.RemotingConfiguration : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool SerializationFormatter
        {
            get
            {
                return (_flags & SecurityPermissionFlag.SerializationFormatter) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.SerializationFormatter : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool SkipVerification
        {
            get
            {
                return (_flags & SecurityPermissionFlag.SkipVerification) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.SkipVerification : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public bool UnmanagedCode
        {
            get
            {
                return (_flags & SecurityPermissionFlag.UnmanagedCode) != SecurityPermissionFlag.NoFlags;
            }
            set
            {
                _flags = (value ? _flags | SecurityPermissionFlag.UnmanagedCode : _flags & (SecurityPermissionFlag.Assertion | SecurityPermissionFlag.SkipVerification | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            }
        }

        public SecurityPermissionAttribute(SecurityAction action)
            : base(action)
        {
        }

        public override IPermission CreatePermission()
        {
            if (_unrestricted)
            {
                return new SecurityPermission(PermissionState.Unrestricted);
            }
            return new SecurityPermission(_flags);
        }
    }
}