#if NET20 || NET30 || NET35

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
    internal static class ExpressionVisitorUtils
    {
        public static Expression[] VisitArguments(ExpressionVisitor visitor, IArgumentProvider nodes)
        {
            Expression[] newNodes = null;
            for (int i = 0, n = nodes.ArgumentCount; i < n; i++)
            {
                Expression curNode = nodes.GetArgument(i);
                Expression node = visitor.Visit(curNode);

                if (newNodes != null)
                {
                    newNodes[i] = node;
                }
                else if (!ReferenceEquals(node, curNode))
                {
                    newNodes = new Expression[n];
                    for (int j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes.GetArgument(j);
                    }
                    newNodes[i] = node;
                }
            }
            return newNodes;
        }
    }
}

#endif