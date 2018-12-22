#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_5 || NETSTANDARD1_6

using System.Diagnostics;

namespace System.Security.Permissions
{
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    internal sealed class SecurityPermissionAttribute : Attribute
    {
        public SecurityPermissionAttribute(SecurityAction action)
        {
            Action = action;
        }

        public bool UnmanagedCode { get; set; }
        public SecurityAction Action { get; }
    }
}

#endif