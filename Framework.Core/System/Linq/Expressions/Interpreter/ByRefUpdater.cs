#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class ByRefUpdater
    {
        public readonly int ArgumentIndex;

        protected ByRefUpdater(int argumentIndex)
        {
            ArgumentIndex = argumentIndex;
        }

        public virtual void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
            // Empty
        }

        public abstract void Update(InterpretedFrame frame, object? value);
    }
}

#endif