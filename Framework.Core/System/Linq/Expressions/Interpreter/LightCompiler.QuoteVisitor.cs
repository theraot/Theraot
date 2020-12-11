#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed partial class LightCompiler
    {
        private sealed class QuoteVisitor : ExpressionVisitor
        {
            public readonly HashSet<ParameterExpression> HoistedParameters = new HashSet<ParameterExpression>();
            private readonly Dictionary<ParameterExpression, int> _definedParameters = new Dictionary<ParameterExpression, int>();

            protected internal override Expression VisitBlock(BlockExpression node)
            {
                PushParameters(node.Variables);

                base.VisitBlock(node);

                PopParameters(node.Variables);

                return node;
            }

            protected internal override Expression VisitLambda<T>(Expression<T> node)
            {
                IEnumerable<ParameterExpression> parameters = ArrayEx.Empty<ParameterExpression>();

                var count = node.ParameterCount;

                if (count > 0)
                {
                    var parameterList = new List<ParameterExpression>(count);

                    for (var i = 0; i < count; i++)
                    {
                        parameterList.Add(node.GetParameter(i));
                    }

                    parameters = parameterList;
                }

                PushParameters(parameters);

                base.VisitLambda(node);

                PopParameters(parameters);

                return node;
            }

            protected internal override Expression VisitParameter(ParameterExpression node)
            {
                if (!_definedParameters.ContainsKey(node))
                {
                    HoistedParameters.Add(node);
                }

                return node;
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                if (node.Variable != null)
                {
                    PushParameters(new[] { node.Variable });
                }

                Visit(node.Body);
                Visit(node.Filter);
                if (node.Variable != null)
                {
                    PopParameters(new[] { node.Variable });
                }

                return node;
            }

            private void PopParameters(IEnumerable<ParameterExpression> parameters)
            {
                foreach (var param in parameters)
                {
                    var count = _definedParameters[param];
                    if (count == 0)
                    {
                        _definedParameters.Remove(param);
                    }
                    else
                    {
                        _definedParameters[param] = count - 1;
                    }
                }
            }

            private void PushParameters(IEnumerable<ParameterExpression> parameters)
            {
                foreach (var param in parameters)
                {
                    if (_definedParameters.TryGetValue(param, out var count))
                    {
                        _definedParameters[param] = count + 1;
                    }
                    else
                    {
                        _definedParameters[param] = 1;
                    }
                }
            }
        }
    }
}

#endif