#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal partial class CallInstruction
    {
        public virtual object InvokeInstance(object instance, params object[] args)
        {
            switch (args.Length)
            {
                case 0:
                    return Invoke(instance);

                case 1:
                    return Invoke(instance, args[0]);

                case 2:
                    return Invoke(instance, args[0], args[1]);

                case 3:
                    return Invoke(instance, args[0], args[1], args[2]);

                case 4:
                    return Invoke(instance, args[0], args[1], args[2], args[3]);

                case 5:
                    return Invoke(instance, args[0], args[1], args[2], args[3], args[4]);

                case 6:
                    return Invoke(instance, args[0], args[1], args[2], args[3], args[4], args[5]);

                case 7:
                    return Invoke(instance, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);

                case 8:
                    return Invoke(instance, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);

                default:
                    throw new InvalidOperationException();
            }
        }

        public virtual object Invoke(params object[] args)
        {
            switch (args.Length)
            {
                case 0:
                    return Invoke();

                case 1:
                    return Invoke(args[0]);

                case 2:
                    return Invoke(args[0], args[1]);

                case 3:
                    return Invoke(args[0], args[1], args[2]);

                case 4:
                    return Invoke(args[0], args[1], args[2], args[3]);

                case 5:
                    return Invoke(args[0], args[1], args[2], args[3], args[4]);

                case 6:
                    return Invoke(args[0], args[1], args[2], args[3], args[4], args[5]);

                case 7:
                    return Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);

                case 8:
                    return Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);

                case 9:
                    return Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);

                default:
                    throw new InvalidOperationException();
            }
        }

        public virtual object Invoke()
        {
            throw new InvalidOperationException();
        }

        public virtual object Invoke(object arg0)
        {
            throw new InvalidOperationException();
        }

        public virtual object Invoke(object arg0, object arg1)
        {
            throw new InvalidOperationException();
        }

        public virtual object Invoke(object arg0, object arg1, object arg2)
        {
            throw new InvalidOperationException();
        }

        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3)
        {
            throw new InvalidOperationException();
        }

        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            throw new InvalidOperationException();
        }

        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            throw new InvalidOperationException();
        }

        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            throw new InvalidOperationException();
        }

        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            throw new InvalidOperationException();
        }

        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            throw new InvalidOperationException();
        }
    }

    internal sealed class ActionCallInstruction : CallInstruction
    {
        private readonly Action _target;

        public override int ArgumentCount
        {
            get { return 0; }
        }

        public ActionCallInstruction(Action target)
        {
            _target = target;
        }

        public override int ProducedStack
        {
            get { return 0; }
        }

        public ActionCallInstruction(MethodInfo target)
        {
            _target = (Action)target.CreateDelegate(typeof(Action), target);
        }

        public override object Invoke()
        {
            _target();
            return null;
        }

        public override int Run(InterpretedFrame frame)
        {
            _target();
            frame.StackIndex -= 0;
            return 1;
        }
    }

    internal sealed class ActionCallInstruction<T0> : CallInstruction
    {
        private readonly Action<T0> _target;

        public override int ProducedStack
        {
            get { return 0; }
        }

        public override int ArgumentCount
        {
            get { return 1; }
        }

        public ActionCallInstruction(Action<T0> target)
        {
            _target = target;
        }

        public ActionCallInstruction(MethodInfo target)
        {
            _target = (Action<T0>)target.CreateDelegate(typeof(Action<T0>), target);
        }

        public override object Invoke(object arg0)
        {
            _target(arg0 != null ? (T0)arg0 : default);
            return null;
        }

        public override int Run(InterpretedFrame frame)
        {
            _target((T0)frame.Data[frame.StackIndex - 1]);
            frame.StackIndex -= 1;
            return 1;
        }
    }

    internal sealed class ActionCallInstruction<T0, T1> : CallInstruction
    {
        private readonly Action<T0, T1> _target;

        public override int ProducedStack
        {
            get { return 0; }
        }

        public override int ArgumentCount
        {
            get { return 2; }
        }

        public ActionCallInstruction(Action<T0, T1> target)
        {
            _target = target;
        }

        public ActionCallInstruction(MethodInfo target)
        {
            _target = (Action<T0, T1>)target.CreateDelegate(typeof(Action<T0, T1>), target);
        }

        public override object Invoke(object arg0, object arg1)
        {
            _target(arg0 != null ? (T0)arg0 : default, arg1 != null ? (T1)arg1 : default);
            return null;
        }

        public override int Run(InterpretedFrame frame)
        {
            _target((T0)frame.Data[frame.StackIndex - 2], (T1)frame.Data[frame.StackIndex - 1]);
            frame.StackIndex -= 2;
            return 1;
        }
    }

    internal sealed class FuncCallInstruction<TRet> : CallInstruction
    {
        private readonly Func<TRet> _target;

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int ArgumentCount
        {
            get { return 0; }
        }

        public FuncCallInstruction(Func<TRet> target)
        {
            _target = target;
        }

        public FuncCallInstruction(MethodInfo target)
        {
            _target = (Func<TRet>)target.CreateDelegate(typeof(Func<TRet>), target);
        }

        public override object Invoke()
        {
            return _target();
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex - 0] = _target();
            frame.StackIndex -= -1;
            return 1;
        }
    }

    internal sealed class FuncCallInstruction<T0, TRet> : CallInstruction
    {
        private readonly Func<T0, TRet> _target;

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int ArgumentCount
        {
            get { return 1; }
        }

        public FuncCallInstruction(Func<T0, TRet> target)
        {
            _target = target;
        }

        public FuncCallInstruction(MethodInfo target)
        {
            _target = (Func<T0, TRet>)target.CreateDelegate(typeof(Func<T0, TRet>), target);
        }

        public override object Invoke(object arg0)
        {
            return _target(arg0 != null ? (T0)arg0 : default);
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex - 1] = _target((T0)frame.Data[frame.StackIndex - 1]);
            frame.StackIndex -= 0;
            return 1;
        }
    }

    internal sealed class FuncCallInstruction<T0, T1, TRet> : CallInstruction
    {
        private readonly Func<T0, T1, TRet> _target;

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int ArgumentCount
        {
            get { return 2; }
        }

        public FuncCallInstruction(Func<T0, T1, TRet> target)
        {
            _target = target;
        }

        public FuncCallInstruction(MethodInfo target)
        {
            _target = (Func<T0, T1, TRet>)target.CreateDelegate(typeof(Func<T0, T1, TRet>), target);
        }

        public override object Invoke(object arg0, object arg1)
        {
            return _target(arg0 != null ? (T0)arg0 : default, arg1 != null ? (T1)arg1 : default);
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex - 2] = _target((T0)frame.Data[frame.StackIndex - 2], (T1)frame.Data[frame.StackIndex - 1]);
            frame.StackIndex -= 1;
            return 1;
        }
    }
}

#endif