#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
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

    internal sealed class LoadLocalInstruction : LocalAccessInstruction, IBoxableInstruction
    {
        internal LoadLocalInstruction(int index)
            : base(index)
        {
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex++] = frame.Data[Index];
            return +1;
        }

        public Instruction BoxIfIndexMatches(int index)
        {
            return (index == Index) ? InstructionList.LoadLocalBoxed(index) : null;
        }
    }

    internal sealed class LoadLocalBoxedInstruction : LocalAccessInstruction
    {
        internal LoadLocalBoxedInstruction(int index)
            : base(index)
        {
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var box = (IStrongBox)frame.Data[Index];
            frame.Data[frame.StackIndex++] = box.Value;
            return +1;
        }
    }

    internal sealed class LoadLocalFromClosureInstruction : LocalAccessInstruction
    {
        internal LoadLocalFromClosureInstruction(int index)
            : base(index)
        {
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var box = frame.Closure[Index];
            frame.Data[frame.StackIndex++] = box.Value;
            return +1;
        }
    }

    internal sealed class LoadLocalFromClosureBoxedInstruction : LocalAccessInstruction
    {
        internal LoadLocalFromClosureBoxedInstruction(int index)
            : base(index)
        {
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var box = frame.Closure[Index];
            frame.Data[frame.StackIndex++] = box;
            return +1;
        }
    }

    #endregion Load

    #region Store, Assign

    internal sealed class AssignLocalInstruction : LocalAccessInstruction, IBoxableInstruction
    {
        internal AssignLocalInstruction(int index)
            : base(index)
        {
        }

        public override int ConsumedStack
        {
            get { return 1; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[Index] = frame.Peek();
            return +1;
        }

        public Instruction BoxIfIndexMatches(int index)
        {
            return (index == Index) ? InstructionList.AssignLocalBoxed(index) : null;
        }
    }

    internal sealed class StoreLocalInstruction : LocalAccessInstruction, IBoxableInstruction
    {
        internal StoreLocalInstruction(int index)
            : base(index)
        {
        }

        public override int ConsumedStack
        {
            get { return 1; }
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[Index] = frame.Pop();
            return +1;
        }

        public Instruction BoxIfIndexMatches(int index)
        {
            return (index == Index) ? InstructionList.StoreLocalBoxed(index) : null;
        }
    }

    internal sealed class AssignLocalBoxedInstruction : LocalAccessInstruction
    {
        internal AssignLocalBoxedInstruction(int index)
            : base(index)
        {
        }

        public override int ConsumedStack
        {
            get { return 1; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var box = (IStrongBox)frame.Data[Index];
            box.Value = frame.Peek();
            return +1;
        }
    }

    internal sealed class StoreLocalBoxedInstruction : LocalAccessInstruction
    {
        internal StoreLocalBoxedInstruction(int index)
            : base(index)
        {
        }

        public override int ConsumedStack
        {
            get { return 1; }
        }

        public override int ProducedStack
        {
            get { return 0; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var box = (IStrongBox)frame.Data[Index];
            box.Value = frame.Data[--frame.StackIndex];
            return +1;
        }
    }

    internal sealed class AssignLocalToClosureInstruction : LocalAccessInstruction
    {
        internal AssignLocalToClosureInstruction(int index)
            : base(index)
        {
        }

        public override int ConsumedStack
        {
            get { return 1; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var box = frame.Closure[Index];
            box.Value = frame.Peek();
            return +1;
        }
    }

    internal sealed class ValueTypeCopyInstruction : Instruction
    {
        public static readonly ValueTypeCopyInstruction Instruction = new ValueTypeCopyInstruction();

        public override int ConsumedStack
        {
            get { return 1; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var o = frame.Pop();
            frame.Push(o == null ? null : RuntimeHelpers.GetObjectValue(o));
            return +1;
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

        internal sealed class Reference : InitializeLocalInstruction, IBoxableInstruction
        {
            internal Reference(int index)
                : base(index)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = null;
                return 1;
            }

            public Instruction BoxIfIndexMatches(int index)
            {
                return (index == Index) ? InstructionList.InitImmutableRefBox(index) : null;
            }

            public override string InstructionName
            {
                get { return "InitRef"; }
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

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = _defaultValue;
                return 1;
            }

            public Instruction BoxIfIndexMatches(int index)
            {
                return (index == Index) ? new ImmutableBox(index) : null;
            }

            public override string InstructionName
            {
                get { return "InitImmutableValue"; }
            }
        }

        internal sealed class ImmutableBox : InitializeLocalInstruction
        {
            // immutable value:

            internal ImmutableBox(int index)
                : base(index)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = new StrongBox<object>();
                return 1;
            }

            public override string InstructionName
            {
                get { return "InitImmutableBox"; }
            }
        }

        internal sealed class ImmutableRefValue : InitializeLocalInstruction, IBoxableInstruction
        {
            private readonly Type _type; // TODO not used

            internal ImmutableRefValue(int index, Type type)
                : base(index)
            {
                _type = type;
            }

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = null;
                return 1;
            }

            public Instruction BoxIfIndexMatches(int index)
            {
                return (index == Index) ? new ImmutableRefBox(index) : null;
            }

            public override string InstructionName
            {
                get { return "InitImmutableValue"; }
            }
        }

        internal sealed class ImmutableRefBox : InitializeLocalInstruction
        {
            // immutable value:
            internal ImmutableRefBox(int index)
                : base(index)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = new StrongBox<object>();
                return 1;
            }

            public override string InstructionName
            {
                get { return "InitImmutableBox"; }
            }
        }

        internal sealed class ParameterBox : InitializeLocalInstruction
        {
            public ParameterBox(int index)
                : base(index)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = new StrongBox<object>(frame.Data[Index]);
                return 1;
            }
        }

        internal sealed class Parameter : InitializeLocalInstruction, IBoxableInstruction
        {
            internal Parameter(int index, Type parameterType)
                : base(index)
            {
                GC.KeepAlive(parameterType);
            }

            public override int Run(InterpretedFrame frame)
            {
                // nop
                return 1;
            }

            public Instruction BoxIfIndexMatches(int index)
            {
                if (index == Index)
                {
                    return InstructionList.ParameterBox(index);
                }
                return null;
            }

            public override string InstructionName
            {
                get { return "InitParameter"; }
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

            public override int Run(InterpretedFrame frame)
            {
                try
                {
                    frame.Data[Index] = Activator.CreateInstance(_type);
                }
                catch (TargetInvocationException e)
                {
                    ExceptionHelpers.UpdateForRethrow(e.InnerException);
                    throw e.InnerException;
                }

                return 1;
            }

            public Instruction BoxIfIndexMatches(int index)
            {
                return (index == Index) ? new MutableBox(index) : null;
            }

            public override string InstructionName
            {
                get { return "InitMutableValue"; }
            }
        }

        internal sealed class MutableBox : InitializeLocalInstruction
        {
            internal MutableBox(int index)
                : base(index)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                frame.Data[Index] = new StrongBox<object>();
                return 1;
            }

            public override string InstructionName
            {
                get { return "InitMutableBox"; }
            }
        }
    }

    #endregion Initialize

    #region RuntimeVariables

    internal sealed class RuntimeVariablesInstruction : Instruction
    {
        private readonly int _count;

        public RuntimeVariablesInstruction(int count)
        {
            _count = count;
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override int ConsumedStack
        {
            get { return _count; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var ret = new IStrongBox[_count];
            for (int i = ret.Length - 1; i >= 0; i--)
            {
                ret[i] = (IStrongBox)frame.Pop();
            }
            frame.Push(RuntimeVariables.Create(ret));
            return +1;
        }

        public override string ToString()
        {
            return "GetRuntimeVariables()";
        }
    }

    #endregion RuntimeVariables
}

#endif