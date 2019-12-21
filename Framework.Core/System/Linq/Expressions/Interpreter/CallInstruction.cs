#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal class ByRefMethodInfoCallInstruction : MethodInfoCallInstruction
    {
        private readonly ByRefUpdater[] _byrefArgs;

        internal ByRefMethodInfoCallInstruction(MethodInfo target, int argumentCount, ByRefUpdater[] byrefArgs)
            : base(target, argumentCount)
        {
            _byrefArgs = byrefArgs;
        }

        public override int ProducedStack => Target.ReturnType == typeof(void) ? 0 : 1;

        public sealed override int Run(InterpretedFrame frame)
        {
            var first = frame.StackIndex - ArgumentCountProtected;
            object?[]? args = null;
            object? instance = null;
            try
            {
                object? ret;
                if (Target.IsStatic)
                {
                    args = GetArgs(frame, first, 0);
                    try
                    {
                        ret = Target.Invoke(null, args);
                    }
                    catch (TargetInvocationException e)
                    {
                        ExceptionHelpers.UnwrapAndRethrow(e);
                        throw ContractUtils.Unreachable;
                    }
                }
                else
                {
                    instance = frame.Data[first];
                    NullCheck(instance);
                    args = GetArgs(frame, first, 1);
                    if (TryGetLightLambdaTarget(instance, out var targetLambda))
                    {
                        // no need to Invoke, just interpret the lambda body
                        ret = InterpretLambdaInvoke(targetLambda, args);
                    }
                    else
                    {
                        try
                        {
                            ret = Target.Invoke(instance, args);
                        }
                        catch (TargetInvocationException e)
                        {
                            ExceptionHelpers.UnwrapAndRethrow(e);
                            throw ContractUtils.Unreachable;
                        }
                    }
                }
                if (Target.ReturnType != typeof(void))
                {
                    frame.Data[first] = ret;
                    frame.StackIndex = first + 1;
                }
                else
                {
                    frame.StackIndex = first;
                }
            }
            finally
            {
                if (args != null)
                {
                    foreach (var arg in _byrefArgs)
                    {
                        // -1: instance param, just copy back the exact instance invoked with, which
                        // gets passed by reference from reflection for value types.
                        arg.Update(frame, arg.ArgumentIndex == -1 ? instance : args[arg.ArgumentIndex]);
                    }
                }
            }
            return 1;
        }
    }

    internal abstract class CallInstruction : Instruction
    {
        /// <summary>
        ///     The number of arguments including "this" for instance methods.
        /// </summary>
        public abstract int ArgumentCount { get; }

        public override int ConsumedStack => ArgumentCount;

        public override string InstructionName => "Call";

        public static void ArrayItemSetter1(Array array, int index1, object value)
        {
            array.SetValue(value, index1);
        }

        public static void ArrayItemSetter2(Array array, int index1, int index2, object value)
        {
            array.SetValue(value, index1, index2);
        }

        public static void ArrayItemSetter3(Array array, int index1, int index2, int index3, object value)
        {
            array.SetValue(value, index1, index2, index3);
        }

        public static CallInstruction Create(MethodInfo info)
        {
            return Create(info, info.GetParameters());
        }

        public static CallInstruction Create(MethodInfo info, ParameterInfo[] parameters)
        {
            var argumentCount = parameters.Length;
            if (!info.IsStatic)
            {
                argumentCount++;
            }
            // A workaround for CLR behavior (Unable to create delegates for Array.Get/Set):
            // T[]::Address - not supported by ETs due to T& return value
            if (info.DeclaringType?.IsArray == true && (string.Equals(info.Name, "Get", StringComparison.Ordinal) || string.Equals(info.Name, "Set", StringComparison.Ordinal)))
            {
                return GetArrayAccessor(info, argumentCount);
            }
            return new MethodInfoCallInstruction(info, argumentCount);
        }

        protected static bool TryGetLightLambdaTarget(object instance, [NotNullWhen(true)] out LightLambda? lightLambda)
        {
            if (instance is Delegate del && del.Target is Func<object[], object> thunk && thunk.Target is LightLambda found)
            {
                lightLambda = found;
                return true;
            }
            lightLambda = null;
            return false;
        }

        protected object? InterpretLambdaInvoke(LightLambda targetLambda, object?[] args)
        {
            return ProducedStack > 0 ? targetLambda.Run(args) : targetLambda.RunVoid(args);
        }

        private static CallInstruction GetArrayAccessor(MethodInfo info, int argumentCount)
        {
            var arrayType = info.DeclaringType;
            var isGetter = string.Equals(info.Name, "Get", StringComparison.Ordinal);
            MethodInfo? alternativeMethod = null;
            switch (arrayType?.GetArrayRank())
            {
                case 1:
                    alternativeMethod = isGetter ? arrayType.GetMethod("GetValue", new[] { typeof(int) }) : typeof(CallInstruction).GetMethod(nameof(ArrayItemSetter1));
                    break;

                case 2:
                    alternativeMethod = isGetter ? arrayType.GetMethod("GetValue", new[] { typeof(int), typeof(int) }) : typeof(CallInstruction).GetMethod(nameof(ArrayItemSetter2));
                    break;

                case 3:
                    alternativeMethod = isGetter ? arrayType.GetMethod("GetValue", new[] { typeof(int), typeof(int), typeof(int) }) : typeof(CallInstruction).GetMethod(nameof(ArrayItemSetter3));
                    break;

                default:
                    break;
            }
            return alternativeMethod == null ? new MethodInfoCallInstruction(info, argumentCount) : Create(alternativeMethod);
        }
    }

    internal class MethodInfoCallInstruction : CallInstruction
    {
        protected readonly int ArgumentCountProtected;
        protected readonly MethodInfo Target;

        internal MethodInfoCallInstruction(MethodInfo target, int argumentCount)
        {
            Target = target;
            ArgumentCountProtected = argumentCount;
        }

        public override int ArgumentCount => ArgumentCountProtected;
        public override int ProducedStack => Target.ReturnType == typeof(void) ? 0 : 1;

        public override int Run(InterpretedFrame frame)
        {
            var first = frame.StackIndex - ArgumentCountProtected;
            object? ret;
            if (Target.IsStatic)
            {
                var args = GetArgs(frame, first, 0);
                try
                {
                    ret = Target.Invoke(null, args);
                }
                catch (TargetInvocationException e)
                {
                    ExceptionHelpers.UnwrapAndRethrow(e);
                    throw ContractUtils.Unreachable;
                }
            }
            else
            {
                var instance = frame.Data[first];
                NullCheck(instance);
                var args = GetArgs(frame, first, 1);
                if (TryGetLightLambdaTarget(instance, out var targetLambda))
                {
                    // no need to Invoke, just interpret the lambda body
                    ret = InterpretLambdaInvoke(targetLambda, args);
                }
                else
                {
                    try
                    {
                        ret = Target.Invoke(instance, args);
                    }
                    catch (TargetInvocationException e)
                    {
                        ExceptionHelpers.UnwrapAndRethrow(e);
                        throw ContractUtils.Unreachable;
                    }
                }
            }
            if (Target.ReturnType != typeof(void))
            {
                frame.Data[first] = ret;
                frame.StackIndex = first + 1;
            }
            else
            {
                frame.StackIndex = first;
            }
            return 1;
        }

        public override string ToString()
        {
            return "Call(" + Target + ")";
        }

        protected object?[] GetArgs(InterpretedFrame frame, int first, int skip)
        {
            var count = ArgumentCountProtected - skip;
            if (count <= 0)
            {
                return ArrayEx.Empty<object>();
            }
            var args = new object?[count];
            for (var i = 0; i < args.Length; i++)
            {
                args[i] = frame.Data[first + i + skip];
            }
            return args;
        }
    }
}

#endif