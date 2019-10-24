#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Interpreter
{
    /// <summary>
    ///     Manages creation of interpreted delegates.
    /// </summary>
    internal sealed class LightDelegateCreator
    {
        private readonly LambdaExpression _lambda;

        internal LightDelegateCreator(Interpreter interpreter, LambdaExpression lambda)
        {
            Interpreter = interpreter;
            _lambda = lambda;
        }

        internal Interpreter Interpreter { get; }

        internal Delegate CreateDelegate(IStrongBox[]? closure)
        {
            // we'll create an interpreted LightLambda
            return new LightLambda(this, closure).MakeDelegate(_lambda.Type);
        }
    }
}

#endif