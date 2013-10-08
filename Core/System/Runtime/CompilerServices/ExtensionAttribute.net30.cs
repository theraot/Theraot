#if NET20 || NET30

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute
    {
        public ExtensionAttribute()
        {
            //Empty
        }
    }
}

#endif
