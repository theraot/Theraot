#if NETCF

using System.Runtime.InteropServices;

namespace System.Diagnostics
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    [ComVisible(true)]
    public sealed class DebuggerBrowsableAttribute : Attribute
    {
        private DebuggerBrowsableState _state;

        public DebuggerBrowsableAttribute(DebuggerBrowsableState state)
        {
            if (state != DebuggerBrowsableState.Collapsed && state != DebuggerBrowsableState.Never && state != DebuggerBrowsableState.RootHidden)
            {
                throw new ArgumentOutOfRangeException("state");
            }
            _state = state;
        }

        public DebuggerBrowsableState State
        {
            get
            {
                return _state;
            }
        }
    }
}

#endif