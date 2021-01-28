#if LESSTHAN_NET35

#pragma warning disable CA1062 // Validate arguments of public methods
#pragma warning disable CC0021 // Use nameof

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Linq.Expressions.Interpreter
{
    public class LightLambda
    {
        private readonly IStrongBox[]? _closure;

        private readonly Interpreter _interpreter;

        internal LightLambda(LightDelegateCreator delegateCreator, IStrongBox[]? closure)
        {
            _closure = closure;
            _interpreter = delegateCreator.Interpreter;
        }

        internal string DebugView => new DebugViewPrinter(_interpreter).ToString();

        public object Run(params object?[] arguments)
        {
            var frame = MakeFrame();
            for (var i = 0; i < arguments.Length; i++)
            {
                frame.Data[i] = arguments[i];
            }

            var currentFrame = frame.Enter();
            try
            {
                _interpreter.Run(frame);
            }
            finally
            {
                for (var i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = frame.Data[i];
                }

                frame.Leave(currentFrame);
            }

            return frame.Pop()!;
        }

        public object? RunVoid(params object?[] arguments)
        {
            var frame = MakeFrame();
            for (var i = 0; i < arguments.Length; i++)
            {
                frame.Data[i] = arguments[i];
            }

            var currentFrame = frame.Enter();
            try
            {
                _interpreter.Run(frame);
            }
            finally
            {
                for (var i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = frame.Data[i];
                }

                frame.Leave(currentFrame);
            }

            return null;
        }

        internal Delegate MakeDelegate(Type delegateType)
        {
            var method = delegateType.GetInvokeMethod();
            return method.ReturnType == typeof(void) ? DelegateHelpers.CreateObjectArrayDelegate(delegateType, RunVoid) : DelegateHelpers.CreateObjectArrayDelegate(delegateType, Run);
        }

        private InterpretedFrame MakeFrame()
        {
            return new InterpretedFrame(_interpreter, _closure);
        }

        private sealed class DebugViewPrinter
        {
            private readonly Dictionary<int, string> _handlerEnter = new();
            private readonly Dictionary<int, int> _handlerExit = new();
            private readonly Interpreter _interpreter;
            private readonly Dictionary<int, int> _tryStart = new();
            private string _indent = "  ";

            public DebugViewPrinter(Interpreter interpreter)
            {
                _interpreter = interpreter;

                Analyze();
            }

            public override string ToString()
            {
                var sb = new StringBuilder();

                var name = _interpreter.Name ?? "lambda_method";
                sb.Append("object ").Append(name).AppendLine("(object[])");
                sb.AppendLine("{");

                sb.Append("  .locals ").Append(_interpreter.LocalCount).AppendLine();
                sb.Append("  .maxstack ").Append(_interpreter.Instructions.MaxStackDepth).AppendLine();
                sb.Append("  .maxcontinuation ").Append(_interpreter.Instructions.MaxContinuationDepth).AppendLine();
                sb.AppendLine();

                var instructions = _interpreter.Instructions.Instructions;
                var debugView = new InstructionArray.DebugView(_interpreter.Instructions);
                var instructionViews = debugView.GetInstructionViews();

                for (var i = 0; i < instructions.Length; i++)
                {
                    EmitExits(sb, i);

                    if (_tryStart.TryGetValue(i, out var startCount))
                    {
                        for (var j = 0; j < startCount; j++)
                        {
                            sb.Append(_indent).AppendLine(".try");
                            sb.Append(_indent).AppendLine("{");
                            Indent();
                        }
                    }

                    if (_handlerEnter.TryGetValue(i, out var handler))
                    {
                        sb.Append(_indent).AppendLine(handler);
                        sb.Append(_indent).AppendLine("{");
                        Indent();
                    }

                    var instructionView = instructionViews[i];

                    sb.Append(_indent).Append("IP_").AppendFormat("{0:0000}", i).Append(": ").AppendLine(instructionView.GetValue());
                }

                EmitExits(sb, instructions.Length);

                sb.AppendLine("}");

                return sb.ToString();
            }

            private void AddHandlerExit(int index)
            {
                _handlerExit[index] = _handlerExit.TryGetValue(index, out var count) ? count + 1 : 1;
            }

            private void AddTryStart(int index)
            {
                if (!_tryStart.TryGetValue(index, out var count))
                {
                    _tryStart.Add(index, 1);
                    return;
                }

                _tryStart[index] = count + 1;
            }

            private void Analyze()
            {
                foreach (var instruction in _interpreter.Instructions.Instructions)
                {
                    switch (instruction)
                    {
                        case EnterTryCatchFinallyInstruction enterTryCatchFinally:
                            AnalyzeEnterTryCatchFinallyInstruction(enterTryCatchFinally);
                            break;

                        case EnterTryFaultInstruction enterTryFault:
                            AnalyzeEnterTryFaultInstruction(enterTryFault);
                            break;

                        default:
                            break;
                    }
                }
            }

            private void AnalyzeEnterTryCatchFinallyInstruction(EnterTryCatchFinallyInstruction enterTryCatchFinally)
            {
                var handler = enterTryCatchFinally.Handler;
                AddTryStart(handler!.TryStartIndex);
                AddHandlerExit(handler.TryEndIndex + 1 /* include Goto instruction that acts as a "leave" */);
                if (handler.IsFinallyBlockExist)
                {
                    _handlerEnter.Add(handler.FinallyStartIndex, "finally");
                    AddHandlerExit(handler.FinallyEndIndex);
                }

                if (!handler.IsCatchBlockExist)
                {
                    return;
                }

                foreach (var catchHandler in handler.Handlers)
                {
                    _handlerEnter.Add(
                        catchHandler.HandlerStartIndex - 1 /* include EnterExceptionHandler instruction */,
                        catchHandler.ToString());
                    AddHandlerExit(catchHandler.HandlerEndIndex);
                    var filter = catchHandler.Filter;
                    if (filter == null)
                    {
                        continue;
                    }

                    _handlerEnter.Add(filter.StartIndex - 1 /* include EnterExceptionFilter instruction */,
                        "filter");
                    AddHandlerExit(filter.EndIndex);
                }
            }

            private void AnalyzeEnterTryFaultInstruction(EnterTryFaultInstruction enterTryFault)
            {
                var handler = enterTryFault.Handler;
                AddTryStart(handler!.TryStartIndex);
                AddHandlerExit(handler.TryEndIndex + 1 /* include Goto instruction that acts as a "leave" */);
                _handlerEnter.Add(handler.FinallyStartIndex, "fault");
                AddHandlerExit(handler.FinallyEndIndex);
            }

            private void Dedent()
            {
                _indent = new string(' ', _indent.Length - 2);
            }

            private void EmitExits(StringBuilder sb, int index)
            {
                if (!_handlerExit.TryGetValue(index, out var exitCount))
                {
                    return;
                }

                for (var j = 0; j < exitCount; j++)
                {
                    Dedent();
                    sb.Append(_indent).AppendLine("}");
                }
            }

            private void Indent()
            {
                _indent = new string(' ', _indent.Length + 2);
            }
        }
    }
}

#endif