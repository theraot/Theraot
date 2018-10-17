using System.Diagnostics;

namespace System.Security.Permissions
{
    [Conditional("DEBUG")]
    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Constructor | System.AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
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