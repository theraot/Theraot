#if LESSTHAN_NET35

#pragma warning disable CA1822 // Marcar miembros como static
#pragma warning disable CC0091 // Use static method

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class InterpretedFrame
    {
        public readonly IStrongBox[]? Closure;

        public readonly object?[] Data;

        public int InstructionIndex;

        public int StackIndex;

        internal readonly Interpreter Interpreter;

        [ThreadStatic]
        private static InterpretedFrame? _currentFrame;

        private readonly int[] _continuations;
        private int _continuationIndex;
        private int _pendingContinuation;
        private object? _pendingValue;

        internal InterpretedFrame(Interpreter interpreter, IStrongBox[]? closure)
        {
            Interpreter = interpreter;
            StackIndex = interpreter.LocalCount;
            Data = new object[StackIndex + interpreter.Instructions.MaxStackDepth];

            _continuations = new int[interpreter.Instructions.MaxContinuationDepth];

            Closure = closure;

            _pendingContinuation = -1;
            _pendingValue = Interpreter.NoValue;
        }

        public string? Name => Interpreter.Name;

        public InterpretedFrame? Parent { get; internal set; }

        internal string?[] Trace
        {
            get
            {
                var trace = new List<string?>();
                var frame = this;
                do
                {
                    trace.Add(frame.Name);
                } while (frame.Parent != null && (frame = frame.Parent) != null);

                return trace.ToArray();
            }
        }

        public static bool IsInterpretedFrame(MethodBase method)
        {
            return method.DeclaringType == typeof(Interpreter) && string.Equals(method.Name, "Run", StringComparison.Ordinal);
        }

        public void Dup()
        {
            var i = StackIndex;
            Data[i] = Data[i - 1];
            StackIndex = i + 1;
        }

        public DebugInfo? GetDebugInfo(int instructionIndex)
        {
            return DebugInfo.GetMatchingDebugInfo(Interpreter.DebugInfos, instructionIndex);
        }

        public IEnumerable<InterpretedFrameInfo> GetStackTraceDebugInfo()
        {
            var frame = this;
            do
            {
                yield return new InterpretedFrameInfo(frame.Name, frame.GetDebugInfo(frame.InstructionIndex));
            } while (frame.Parent != null && (frame = frame.Parent) != null);
        }

        public int Goto(int labelIndex, object value, bool gotoExceptionHandler)
        {
            // TODO: we know this at compile time (except for compiled loop):
            var target = Interpreter.Labels[labelIndex];
            Debug.Assert
            (
                !gotoExceptionHandler || (_continuationIndex == target.ContinuationStackDepth),
                "When it's time to jump to the exception handler, all previous finally blocks should already be processed"
            );

            if (_continuationIndex == target.ContinuationStackDepth)
            {
                SetStackDepth(target.StackDepth);
                if (value != Interpreter.NoValue)
                {
                    Data[StackIndex - 1] = value;
                }

                return target.Index - InstructionIndex;
            }

            // if we are in the middle of executing jump we forget the previous target and replace it by a new one:
            _pendingContinuation = labelIndex;
            _pendingValue = value;
            return YieldToCurrentContinuation();
        }

        public object? Peek()
        {
            return Data[StackIndex - 1];
        }

        public object? Pop()
        {
            return Data[--StackIndex];
        }

        public void Push(object? value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(bool value)
        {
            Data[StackIndex++] = value ? Utils.BoxedTrue : Utils.BoxedFalse;
        }

        public void Push(int value)
        {
            Data[StackIndex++] = ScriptingRuntimeHelpers.Int32ToObject(value);
        }

        public void Push(byte value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(sbyte value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(short value)
        {
            Data[StackIndex++] = value;
        }

        public void Push(ushort value)
        {
            Data[StackIndex++] = value;
        }

        public void PushContinuation(int continuation)
        {
            _continuations[_continuationIndex++] = continuation;
        }

        public void RemoveContinuation()
        {
            _continuationIndex--;
        }

        public int YieldToCurrentContinuation()
        {
            var target = Interpreter.Labels[_continuations[_continuationIndex - 1]];
            SetStackDepth(target.StackDepth);
            return target.Index - InstructionIndex;
        }

        /// <summary>
        ///     Get called from the LeaveFinallyInstruction
        /// </summary>
        public int YieldToPendingContinuation()
        {
            Debug.Assert(_pendingContinuation >= 0);
            var pendingTarget = Interpreter.Labels[_pendingContinuation];

            // the current continuation might have higher priority (continuationIndex is the depth of the current continuation):
            if (pendingTarget.ContinuationStackDepth < _continuationIndex)
            {
                var currentTarget = Interpreter.Labels[_continuations[_continuationIndex - 1]];
                SetStackDepth(currentTarget.StackDepth);
                return currentTarget.Index - InstructionIndex;
            }

            SetStackDepth(pendingTarget.StackDepth);
            if (_pendingValue != Interpreter.NoValue)
            {
                Data[StackIndex - 1] = _pendingValue;
            }

            // Set the _pendingContinuation and _pendingValue to the default values if we finally gets to the Goto target
            _pendingContinuation = -1;
            _pendingValue = Interpreter.NoValue;
            return pendingTarget.Index - InstructionIndex;
        }

        internal InterpretedFrame? Enter()
        {
            var currentFrame = EnterCurrentFrame(this);
            return Parent = currentFrame;
        }

        internal bool IsJumpHappened()
        {
            return _pendingContinuation >= 0;
        }

        internal void Leave(InterpretedFrame? prevFrame)
        {
            LeaveCurrentFrame(prevFrame);
        }

        internal void PopPendingContinuation()
        {
            _pendingValue = Pop();
            _pendingContinuation = (int)Pop()!;
        }

        internal void PushPendingContinuation()
        {
            Push(_pendingContinuation);
            Push(_pendingValue);

            _pendingContinuation = -1;
            _pendingValue = Interpreter.NoValue;
        }

        internal void SaveTraceToException(Exception exception)
        {
            if (exception.Data[typeof(InterpretedFrameInfo)] == null)
            {
                exception.Data[typeof(InterpretedFrameInfo)] = GetStackTraceDebugInfo().ToArray();
            }
        }

        internal void SetStackDepth(int depth)
        {
            StackIndex = Interpreter.LocalCount + depth;
        }

        private static InterpretedFrame? EnterCurrentFrame(InterpretedFrame frame)
        {
            var currentFrame = _currentFrame;
            _currentFrame = frame;
            return currentFrame;
        }

        private static void LeaveCurrentFrame(InterpretedFrame? prevFrame)
        {
            _currentFrame = prevFrame;
        }
    }
}

#endif