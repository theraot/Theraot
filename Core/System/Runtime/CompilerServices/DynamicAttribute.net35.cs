#if NET20 || NET30 || NET35

using System.Collections.Generic;
using Theraot.Core;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class DynamicAttribute : Attribute
    {
        private static readonly IList<bool> _empty = Array.AsReadOnly(new[] { true });

        private IList<bool> _transformFlags;

        public DynamicAttribute()
        {
            _transformFlags = _empty;
        }

        public DynamicAttribute(bool[] transformFlags)
        {
            _transformFlags = Check.NotNullArgument(transformFlags, "tranformFlags");
        }

        public IList<bool> TransformFlags
        {
            get
            {
                return _transformFlags;
            }
        }
    }
}

#endif