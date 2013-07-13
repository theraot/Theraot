using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class SpecialNeedle<T> : WeakNeedle<T>
        where T : class
    {
        private GCHandleType _handleType;

        public SpecialNeedle(GCHandleType handleType)
        {
            _handleType = handleType;
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public SpecialNeedle(GCHandleType handleType, T target)
            : base(target)
        {
            _handleType = handleType;
        }

        protected override GCHandle GetNewHandle(T value, bool trackResurrection)
        {
            return GCHandle.Alloc(value, _handleType);
        }
    }
}