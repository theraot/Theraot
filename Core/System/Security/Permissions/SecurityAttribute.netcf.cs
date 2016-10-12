#if NETCF

using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    [ComVisible(true)]
    [Serializable]
    public abstract class SecurityAttribute : Attribute
    {
        private SecurityAction _action;
        private bool _unrestricted;

        protected SecurityAttribute(SecurityAction action)
        {
            _action = action;
        }

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

        public abstract IPermission CreatePermission();
    }
}

#endif