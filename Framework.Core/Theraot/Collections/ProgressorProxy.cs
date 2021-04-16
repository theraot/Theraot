// Needed for NET40

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
// ReSharper disable ImplicitlyCapturedClosure

using System;

namespace Theraot.Collections
{
    internal sealed class ProgressorProxy
    {
        private readonly IClosable _node;

        public ProgressorProxy(IClosable node)
        {
            _node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public bool IsClosed => _node.IsClosed;
    }
}