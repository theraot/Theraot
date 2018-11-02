#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Interpreter
{
    /// <summary>
    /// Manages creation of interpreted delegates. These delegates will get
    /// compiled if they are executed often enough.
    /// </summary>
    internal sealed class LightDelegateCreator
    {
        // null if we are forced to compile
        private readonly Interpreter _interpreter;

        private readonly Expression _lambda;

        internal LightDelegateCreator(Interpreter interpreter, LambdaExpression lambda)
        {
            Assert.NotNull(lambda);
            _interpreter = interpreter;
            _lambda = lambda;
        }

        internal Interpreter Interpreter
        {
            get { return _interpreter; }
        }

        public Delegate CreateDelegate()
        {
            return CreateDelegate(null);
        }

        internal Delegate CreateDelegate(IStrongBox[] closure)
        {
            // we'll create an interpreted LightLambda
            return new LightLambda(this, closure).MakeDelegate(DelegateType);
        }

        private Type DelegateType
        {
            get
            {
                var le = _lambda as LambdaExpression;
                if (le != null)
                {
                    return le.Type;
                }

                return null;
            }
        }
    }
}

#endif