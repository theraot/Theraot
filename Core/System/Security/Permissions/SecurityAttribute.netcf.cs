#if NETCF

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Security.Permissions
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple=true, Inherited=false)]
    [ComVisible(true)]
    [Serializable]
    public abstract class SecurityAttribute : Attribute
    {
        internal SecurityAction _action;
        internal bool _unrestricted;

        protected SecurityAttribute(SecurityAction action)
        {
            _action = action;
        }

        public abstract IPermission CreatePermission();

        public SecurityAction Action
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
            }
        }

        public bool Unrestricted
        {
            get
            {
                return _unrestricted;
            }
            set
            {
                _unrestricted = value;
            }
        }
    }
}

#endif