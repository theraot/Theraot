#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions
{
    public interface IArgumentProvider
    {
        int ArgumentCount { get; }

        Expression GetArgument(int index);
    }
}

#endif