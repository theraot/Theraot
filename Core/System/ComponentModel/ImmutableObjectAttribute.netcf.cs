#if NETCF

namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ImmutableObjectAttribute : Attribute
    {
        private readonly bool _immutable;

        public ImmutableObjectAttribute(bool immutable)
        {
            _immutable = immutable;
        }

        public bool Immutable
        {
            get
            {
                return _immutable;
            }
        }
    }
}

#endif