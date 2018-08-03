#if NETCOREAPP1_0 || NETCOREAPP1_1

using System;
using System.Diagnostics;

namespace System.Security.Permissions
{
    [Conditional("DEBUG")]
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