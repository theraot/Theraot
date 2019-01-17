#if LESSTHAN_NET45

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions
{
    internal interface IParameterProvider
    {
        /// <summary>
        /// Gets the number of parameter expressions of the node.
        /// </summary>
        int ParameterCount
        {
            get;
        }

        /// <summary>
        /// Gets the parameter expression with the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the argument expression to get.</param>
        /// <returns>The expression representing the parameter at the specified <paramref name="index"/>.</returns>
        ParameterExpression GetParameter(int index);
    }
}

#endif