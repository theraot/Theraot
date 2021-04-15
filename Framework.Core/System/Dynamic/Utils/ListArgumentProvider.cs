#if LESSTHAN_NET40

#pragma warning disable CA1812 // Avoid uninstantiated internal classes

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
    /// <summary>
    ///     Provides a wrapper around an IArgumentProvider which exposes the argument providers
    ///     members out as an IList of Expression.  This is used to avoid allocating an array
    ///     which needs to be stored inside of a ReadOnlyCollection.  Instead this type has
    ///     the same amount of overhead as an array without duplicating the storage of the
    ///     elements.  This ensures that internally we can avoid creating and copying arrays
    ///     while users of the Expression trees also don't pay a size penalty for this internal
    ///     optimization.  See IArgumentProvider for more general information on the Expression
    ///     tree optimizations being used here.
    /// </summary>
    internal sealed class ListArgumentProvider : ListProvider<Expression>
    {
        private readonly IArgumentProvider _provider;

        internal ListArgumentProvider(IArgumentProvider provider, Expression arg0)
        {
            _provider = provider;
            First = arg0;
        }

        protected override int ElementCount => _provider.ArgumentCount;

        protected override Expression First { get; }

        protected override Expression GetElement(int index)
        {
            return _provider.GetArgument(index);
        }
    }
}

#endif