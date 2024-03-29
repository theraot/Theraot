﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal class ByRefNewInstruction : NewInstruction
    {
        private readonly ByRefUpdater[] _byrefArgs;

        internal ByRefNewInstruction(ConstructorInfo target, int argumentCount, ByRefUpdater[] byrefArgs)
            : base(target, argumentCount)
        {
            _byrefArgs = byrefArgs;
        }

        public override string InstructionName => "ByRefNew";

        public sealed override int Run(InterpretedFrame frame)
        {
            var first = frame.StackIndex - ArgumentCount;

            var args = GetArgs(frame, first);

            try
            {
                object ret;
                try
                {
                    ret = Constructor.Invoke(args);
                }
                catch (TargetInvocationException e)
                {
                    ExceptionHelpers.UnwrapAndRethrow(e);
                    throw ContractUtils.Unreachable;
                }

                frame.Data[first] = ret;
                frame.StackIndex = first + 1;
            }
            finally
            {
                foreach (var arg in _byrefArgs)
                {
                    arg.Update(frame, args[arg.ArgumentIndex]);
                }
            }

            return 1;
        }
    }

    internal class NewInstruction : Instruction
    {
        protected readonly int ArgumentCount;
        protected readonly ConstructorInfo Constructor;

        public NewInstruction(ConstructorInfo constructor, int argumentCount)
        {
            Constructor = constructor;
            ArgumentCount = argumentCount;
        }

        public override int ConsumedStack => ArgumentCount;
        public override string InstructionName => "New";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var first = frame.StackIndex - ArgumentCount;

            var args = GetArgs(frame, first);

            object ret;
            try
            {
                ret = Constructor.Invoke(args);
            }
            catch (TargetInvocationException e)
            {
                ExceptionHelpers.UnwrapAndRethrow(e);
                throw ContractUtils.Unreachable;
            }

            frame.Data[first] = ret;
            frame.StackIndex = first + 1;

            return 1;
        }

        public override string ToString()
        {
            return $"New {Constructor.DeclaringType.Name}({Constructor})";
        }

        protected object?[] GetArgs(InterpretedFrame frame, int first)
        {
            if (ArgumentCount <= 0)
            {
                return ArrayEx.Empty<object>();
            }

            var args = new object[ArgumentCount];

            for (var i = 0; i < args.Length; i++)
            {
                args[i] = frame.Data[first + i]!;
            }

            return args;
        }
    }
}

#endif