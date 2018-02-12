#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract partial class CallInstruction : Instruction
    {
        /// <summary>
        /// The number of arguments including "this" for instance methods.
        /// </summary>
        public abstract int ArgumentCount { get; }

        #region Construction

        public override string InstructionName
        {
            get { return "Call"; }
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
            if (info.DeclaringType != null && info.DeclaringType.IsArray && (info.Name == "Get" || info.Name == "Set"))
            {
                return GetArrayAccessor(info, argumentCount);
            }

            return new MethodInfoCallInstruction(info, argumentCount);
        }

        private static CallInstruction GetArrayAccessor(MethodInfo info, int argumentCount)
        {
            var arrayType = info.DeclaringType;
            var isGetter = info.Name == "Get";
            MethodInfo alternativeMethod = null;

            switch (arrayType.GetArrayRank())
            {
                case 1:
                    alternativeMethod = isGetter ?
                        arrayType.GetMethod("GetValue", new[] { typeof(int) }) :
                        typeof(CallInstruction).GetMethod("ArrayItemSetter1");
                    break;

                case 2:
                    alternativeMethod = isGetter ?
                        arrayType.GetMethod("GetValue", new[] { typeof(int), typeof(int) }) :
                        typeof(CallInstruction).GetMethod("ArrayItemSetter2");
                    break;

                case 3:
                    alternativeMethod = isGetter ?
                        arrayType.GetMethod("GetValue", new[] { typeof(int), typeof(int), typeof(int) }) :
                        typeof(CallInstruction).GetMethod("ArrayItemSetter3");
                    break;
            }

            if (alternativeMethod == null)
            {
                return new MethodInfoCallInstruction(info, argumentCount);
            }

            return Create(alternativeMethod);
        }

        public static void ArrayItemSetter1(Array array, int index0, object value)
        {
            array.SetValue(value, index0);
        }

        public static void ArrayItemSetter2(Array array, int index0, int index1, object value)
        {
            array.SetValue(value, index0, index1);
        }

        public static void ArrayItemSetter3(Array array, int index0, int index1, int index2, object value)
        {
            array.SetValue(value, index0, index1, index2);
        }

        #endregion Construction

        #region Instruction

        public override int ConsumedStack
        {
            get { return ArgumentCount; }
        }

        public override string ToString()
        {
            return "Call()";
        }

        #endregion Instruction

        /// <summary>
        /// If the target of invokation happens to be a delegate
        /// over enclosed instance lightLambda, return that instance.
        /// We can interpret LightLambdas directly.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="lightLambda"></param>
        /// <returns></returns>
        protected static bool TryGetLightLambdaTarget(object instance, out LightLambda lightLambda)
        {
            var del = instance as Delegate;
            if ((object)del != null)
            {
                var thunk = del.Target as Func<object[], object>;
                if ((object)thunk != null)
                {
                    lightLambda = thunk.Target as LightLambda;
                    if (lightLambda != null)
                    {
                        return true;
                    }
                }
            }

            lightLambda = null;
            return false;
        }

        protected object InterpretLambdaInvoke(LightLambda targetLambda, object[] args)
        {
            if (ProducedStack > 0)
            {
                return targetLambda.Run(args);
            }
            return targetLambda.RunVoid(args);
        }
    }

    internal class MethodInfoCallInstruction : CallInstruction
    {
        private readonly MethodInfo _target;
        private readonly int _argumentCount;

        public override int ArgumentCount
        {
            get { return _argumentCount; }
        }

        internal MethodInfoCallInstruction(MethodInfo target, int argumentCount)
        {
            _target = target;
            _argumentCount = argumentCount;
        }

        public override int ProducedStack
        {
            get { return _target.ReturnType == typeof(void) ? 0 : 1; }
        }

        public override object Invoke(params object[] args)
        {
            return InvokeWorker(args);
        }

        public override object Invoke()
        {
            return InvokeWorker();
        }

        public override object Invoke(object arg0)
        {
            return InvokeWorker(arg0);
        }

        public override object Invoke(object arg0, object arg1)
        {
            return InvokeWorker(arg0, arg1);
        }

        public override object InvokeInstance(object instance, params object[] args)
        {
            if (_target.IsStatic)
            {
                try
                {
                    return _target.Invoke(null, args);
                }
                catch (TargetInvocationException e)
                {
                    throw ExceptionHelpers.UpdateForRethrow(e.InnerException);
                }
            }

            LightLambda targetLambda;
            if (TryGetLightLambdaTarget(instance, out targetLambda))
            {
                // no need to Invoke, just interpret the lambda body
                return InterpretLambdaInvoke(targetLambda, SkipFirstArg(args));
            }

            try
            {
                return _target.Invoke(instance, args);
            }
            catch (TargetInvocationException e)
            {
                throw ExceptionHelpers.UpdateForRethrow(e.InnerException);
            }
        }

        private object InvokeWorker(params object[] args)
        {
            if (_target.IsStatic)
            {
                try
                {
                    return _target.Invoke(null, args);
                }
                catch (TargetInvocationException e)
                {
                    throw ExceptionHelpers.UpdateForRethrow(e.InnerException);
                }
            }

            LightLambda targetLambda;
            if (TryGetLightLambdaTarget(args[0], out targetLambda))
            {
                // no need to Invoke, just interpret the lambda body
                return InterpretLambdaInvoke(targetLambda, SkipFirstArg(args));
            }

            try
            {
                return _target.Invoke(args[0], SkipFirstArg(args));
            }
            catch (TargetInvocationException e)
            {
                throw ExceptionHelpers.UpdateForRethrow(e.InnerException);
            }
        }

        private static object[] SkipFirstArg(object[] args)
        {
            var newArgs = new object[args.Length - 1];
            for (var i = 0; i < newArgs.Length; i++)
            {
                newArgs[i] = args[i + 1];
            }
            return newArgs;
        }

        public override int Run(InterpretedFrame frame)
        {
            var first = frame.StackIndex - _argumentCount;
            var args = new object[_argumentCount];
            for (var i = 0; i < args.Length; i++)
            {
                args[i] = frame.Data[first + i];
            }

            var ret = Invoke(args);
            if (_target.ReturnType != typeof(void))
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
    }

    internal class ByRefMethodInfoCallInstruction : CallInstruction
    {
        private readonly ByRefUpdater[] _byrefArgs;
        private readonly MethodInfo _target;
        private readonly int _argumentCount;

        public override int ArgumentCount
        {
            get { return _argumentCount; }
        }

        internal ByRefMethodInfoCallInstruction(MethodInfo target, int argumentCount, ByRefUpdater[] byrefArgs)
        {
            _target = target;
            _argumentCount = argumentCount;
            _byrefArgs = byrefArgs;
        }

        public override int ProducedStack
        {
            get { return (_target.ReturnType == typeof(void) ? 0 : 1); }
        }

        public sealed override int Run(InterpretedFrame frame)
        {
            var first = frame.StackIndex - _argumentCount;
            object[] args = null;
            object instance = null;
            try
            {
                object ret;
                if (_target.IsStatic)
                {
                    args = new object[_argumentCount];
                    for (var i = 0; i < args.Length; i++)
                    {
                        args[i] = frame.Data[first + i];
                    }
                    try
                    {
                        ret = _target.Invoke(null, args);
                    }
                    catch (TargetInvocationException e)
                    {
                        throw ExceptionHelpers.UpdateForRethrow(e.InnerException);
                    }
                }
                else
                {
                    args = new object[_argumentCount - 1];
                    for (var i = 0; i < args.Length; i++)
                    {
                        args[i] = frame.Data[first + i + 1];
                    }

                    instance = frame.Data[first];

                    LightLambda targetLambda;
                    if (TryGetLightLambdaTarget(instance, out targetLambda))
                    {
                        // no need to Invoke, just interpret the lambda body
                        ret = InterpretLambdaInvoke(targetLambda, args);
                    }
                    else
                    {
                        try
                        {
                            ret = _target.Invoke(instance, args);
                        }
                        catch (TargetInvocationException e)
                        {
                            throw ExceptionHelpers.UpdateForRethrow(e.InnerException);
                        }
                    }
                }

                if (_target.ReturnType != typeof(void))
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
                        if (arg.ArgumentIndex == -1)
                        {
                            // instance param, just copy back the exact instance invoked with, which
                            // gets passed by reference from reflection for value types.
                            arg.Update(frame, instance);
                        }
                        else
                        {
                            arg.Update(frame, args[arg.ArgumentIndex]);
                        }
                    }
                }
            }

            return 1;
        }
    }
}

#endif