#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6

using System.Diagnostics;

namespace System.Security.Permissions
{
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    internal sealed class SecurityPermissionAttribute : Attribute
    {
        private readonly SecurityAction _securityAction;

        public SecurityPermissionAttribute(SecurityAction securityAction)
        {
            _securityAction = securityAction;
        }

        public bool UnmanagedCode { get; set; }
        public SecurityPermissionFlag Flags { get; set; }
    }
}

#endif