#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
    internal static class ExpressionVisitorUtils
    {
        public static Expression[]? VisitArguments(ExpressionVisitor visitor, IArgumentProvider nodes)
        {
            Expression[]? newNodes = null;
            for (int i = 0, n = nodes.ArgumentCount; i < n; i++)
            {
                var curNode = nodes.GetArgument(i);
                var node = visitor.Visit(curNode);

                if (newNodes == null && node == curNode)
                {
                    continue;
                }
                if (newNodes == null)
                {
                    newNodes = new Expression[n];
                    for (var j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes.GetArgument(j);
                    }
                }
                newNodes[i] = node;
            }

            return newNodes;
        }

        public static Expression[]? VisitBlockExpressions(ExpressionVisitor visitor, BlockExpression block)
        {
            Expression[]? newNodes = null;
            for (int i = 0, n = block.ExpressionCount; i < n; i++)
            {
                var curNode = block.GetExpression(i);
                var node = visitor.Visit(curNode);

                if (newNodes != null)
                {
                    newNodes[i] = node;
                }
                else if (node != curNode)
                {
                    newNodes = new Expression[n];
                    for (var j = 0; j < i; j++)
                    {
                        newNodes[j] = block.GetExpression(j);
                    }

                    newNodes[i] = node;
                }
            }

            return newNodes;
        }

        public static ParameterExpression[]? VisitParameters(ExpressionVisitor visitor, IParameterProvider nodes, string callerName)
        {
            ParameterExpression[]? newNodes = null;
            for (int i = 0, n = nodes.ParameterCount; i < n; i++)
            {
                var curNode = nodes.GetParameter(i);
                var node = visitor.VisitAndConvert(curNode, callerName);

                if (newNodes != null)
                {
                    newNodes[i] = node;
                }
                else if (node != curNode)
                {
                    newNodes = new ParameterExpression[n];
                    for (var j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes.GetParameter(j);
                    }

                    newNodes[i] = node;
                }
            }

            return newNodes;
        }
    }
}

#endif