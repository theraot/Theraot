#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Interpreter
{
    internal interface IBoxableInstruction
    {
        Instruction BoxIfIndexMatches(int index);
    }

    internal abstract class LocalAccessInstruction : Instruction
    {
        internal readonly int Index;

        protected LocalAccessInstruction(int index)
        {
            Index = index;
        }

        public override string ToDebugString(int instructionIndex, object cookie, Func<int, int> labelIndexer, IList<object> objects)
        {
            return cookie == null ?
                InstructionName + "(" + Index + ")" :
                InstructionName + "(" + cookie + ": " + Index + ")";
        }
    }

    #region Load

    internal sealed class LoadLocalBoxedInstruction : LocalAccessInstruction
    {
        internal LoadLocalBoxedInstruction(int index)
            : base(index)
        {
        }

        public override string InstructionName => "LoadLocalBox";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var box = (IStrongBox)frame.Data[Index];
            frame.Data[frame.StackIndex++] = box.Value;
            return 1;
        }
    }

    internal sealed class LoadLocalFromClosureBoxedInstruction : LocalAccessInstruction
    {
        internal LoadLocalFromClosureBoxedInstruction(int index)
            : base(index)
        {
        }

        public override string InstructionName => "LoadLocal";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var box = frame.Closure[Index];
            frame.Data[frame.StackIndex++] = box;
            return 1;
        }
    }

    internal sealed class LoadLocalFromClosureInstruction : LocalAccessInstruction
    {
        internal LoadLocalFromClosureInstruction(int index)
            : base(index)
        {
        }

        public override string InstructionName => "LoadLocalClosure";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var box = frame.Closure[Index];
            frame.Data[frame.StackIndex++] = box.Value;
            return 1;
        }
    }

    internal sealed class LoadLocalInstruction : LocalAccessInstruction, IBoxableInstruction
    {
        internal LoadLocalInstruction(int index)
            : base(index)
        {
        }

        public override string InstructionName => "LoadLocal";
        public override int ProducedStack => 1;

        public Instruction BoxIfIndexMatches(int index)
        {
            return index == Index ? InstructionList.LoadLocalBoxed(index) : null;
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex++] = frame.Data[Index];
            return 1;
        }
    }

    #endregion Load

    #region Store, Assign

    internal sealed class AssignLocalBoxedInstruction : LocalAccessInstruction
    {
        internal AssignLocalBoxedInstruction(int index)
            : base(index)
        {
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "AssignLocalBox";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var box = (IStrongBox)frame.Data[Index];
            box.Value = frame.Peek();
            return 1;
        }
    }

    internal sealed class AssignLocalInstruction : LocalAccessInstruction, IBoxableInstruction
    {
        internal AssignLocalInstruction(int index)
            : base(index)
        {
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "AssignLocal";
        public override int ProducedStack => 1;

        public Instruction BoxIfIndexMatches(int index)
        {
            return index == Index ? InstructionList.AssignLocalBoxed(index) : null;
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[Index] = frame.Peek();
            return 1;
        }
    }

    internal sealed class AssignLocalToClosureInstruction : LocalAccessInstruction
    {
        internal AssignLocalToClosureInstruction(int index)
            : base(index)
        {
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "AssignLocalClosure";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var box = frame.Closure[Index];
            box.Value = frame.Peek();
            return 1;
        }
    }

    internal sealed class StoreLocalBoxedInstruction : LocalAccessInstruction
    {
        internal StoreLocalBoxedInstruction(int index)
            : base(index)
        {
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "StoreLocalBox";

        public override int Run(InterpretedFrame frame)
        {
            var box = (IStrongBox)frame.Data[Index];
            box.Value = frame.Data[--frame.StackIndex];
            return 1;
        }
    }

    internal sealed class StoreLocalInstruction : LocalAccessInstruction, IBoxableInstruction
    {
        internal StoreLocalInstruction(int index)
            : base(index)
        {
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "StoreLocal";

        public Instruction BoxIfIndexMatches(int index)
        {
            return index == Index ? InstructionList.StoreLocalBoxed(index) : null;
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[Index] = frame.Pop();
            return 1;
        }
    }

    internal sealed class ValueTypeCopyInstruction : Instruction
    {
        public static readonly ValueTypeCopyInstruction Instruction = new ValueTypeCopyInstruction();

        public override int ConsumedStack => 1;
        public override string InstructionName => "ValueTypeCopy";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var o = frame.Pop();
            frame.Push(o == null ? null : RuntimeHelpers.GetObjectValue(o));
            return 1;
        }
    }

    #endregion Store, Assign

    #region Initialize

    internal abstract class InitializeLocalInstruction : LocalAccessInstruction
    {
        internal InitializeLocalInstruction(int index)
            : base(index)
        {
        }

        internal sealed class ImmutableBox : InitializeLocalInstruction
        {
            // immutable value:

            private readonly object _defaultValue;

            internal ImmutableBox(int index, object defaultValue)
                : base(index)
            {
                _defaultValue = defaultValue;
            }

            public override string InstructionName => "InitImmutableBox";

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = new StrongBox<object>(_defaultValue);
                return 1;
            }
        }

        internal sealed class ImmutableRefBox : InitializeLocalInstruction
        {
            // immutable value:
            internal ImmutableRefBox(int index)
                : base(index)
            {
            }

            public override string InstructionName => "InitImmutableBox";

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = new StrongBox<object>();
                return 1;
            }
        }

        internal sealed class ImmutableValue : InitializeLocalInstruction, IBoxableInstruction
        {
            private readonly object _defaultValue;

            internal ImmutableValue(int index, object defaultValue)
                : base(index)
            {
                Debug.Assert(defaultValue != null);
                _defaultValue = defaultValue;
            }

            public override string InstructionName => "InitImmutableValue";

            public Instruction BoxIfIndexMatches(int index)
            {
                return index == Index ? new ImmutableBox(index, _defaultValue) : null;
            }

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = _defaultValue;
                return 1;
            }
        }

        internal sealed class MutableBox : InitializeLocalInstruction
        {
            private readonly Type _type;

            internal MutableBox(int index, Type type)
                : base(index)
            {
                _type = type;
            }

            public override string InstructionName => "InitMutableBox";

            public override int Run(InterpretedFrame frame)
            {
                object value;

                try
                {
                    value = Activator.CreateInstance(_type);
                }
                catch (TargetInvocationException e)
                {
                    ExceptionHelpers.UnwrapAndRethrow(e);
                    throw ContractUtils.Unreachable;
                }

                frame.Data[Index] = new StrongBox<object>(value);

                return 1;
            }
        }

        internal sealed class MutableValue : InitializeLocalInstruction, IBoxableInstruction
        {
            private readonly Type _type;

            internal MutableValue(int index, Type type)
                : base(index)
            {
                _type = type;
            }

            public override string InstructionName => "InitMutableValue";

            public Instruction BoxIfIndexMatches(int index)
            {
                return index == Index ? new MutableBox(index, _type) : null;
            }

            public override int Run(InterpretedFrame frame)
            {
                try
                {
                    frame.Data[Index] = Activator.CreateInstance(_type);
                }
                catch (TargetInvocationException e)
                {
                    ExceptionHelpers.UnwrapAndRethrow(e);
                    throw ContractUtils.Unreachable;
                }

                return 1;
            }
        }

        internal sealed class Parameter : InitializeLocalInstruction, IBoxableInstruction
        {
            internal Parameter(int index)
                : base(index)
            {
            }

            public override string InstructionName => "InitParameter";

            public Instruction BoxIfIndexMatches(int index)
            {
                if (index == Index)
                {
                    return InstructionList.ParameterBox(index);
                }
                return null;
            }

            public override int Run(InterpretedFrame frame)
            {
                // nop
                return 1;
            }
        }

        internal sealed class ParameterBox : InitializeLocalInstruction
        {
            public ParameterBox(int index)
                : base(index)
            {
            }

            public override string InstructionName => "InitParameterBox";

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = new StrongBox<object>(frame.Data[Index]);
                return 1;
            }
        }

        internal sealed class Reference : InitializeLocalInstruction, IBoxableInstruction
        {
            internal Reference(int index)
                : base(index)
            {
            }

            public override string InstructionName => "InitRef";

            public Instruction BoxIfIndexMatches(int index)
            {
                return index == Index ? InstructionList.InitImmutableRefBox(index) : null;
            }

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = null;
                return 1;
            }
        }
    }

    #endregion Initialize

    #region RuntimeVariables

    internal sealed class RuntimeVariablesInstruction : Instruction
    {
        public RuntimeVariablesInstruction(int count)
        {
            ConsumedStack = count;
        }

        public override int ConsumedStack { get; }
        public override string InstructionName => "GetRuntimeVariables";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var ret = new IStrongBox[ConsumedStack];
            for (var i = ret.Length - 1; i >= 0; i--)
            {
                ret[i] = (IStrongBox)frame.Pop();
            }
            frame.Push(RuntimeVariables.Create(ret));
            return 1;
        }
    }

    #endregion RuntimeVariables
}

#endif