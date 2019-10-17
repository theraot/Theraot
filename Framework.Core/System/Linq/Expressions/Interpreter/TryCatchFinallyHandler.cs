#if LESSTHAN_NET35

#pragma warning disable CC0021 // Use nameof

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theraot;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class TryCatchFinallyHandler
    {
        internal readonly int FinallyEndIndex;
        internal readonly int FinallyStartIndex;
        internal readonly int GotoEndTargetIndex;
        internal readonly int TryEndIndex;
        internal readonly int TryStartIndex;

        /// <inheritdoc />
        /// <summary>
        ///     No finally block
        /// </summary>
        internal TryCatchFinallyHandler(int tryStart, int tryEnd, int gotoEndTargetIndex, ExceptionHandler[] handlers)
            : this(tryStart, tryEnd, gotoEndTargetIndex, Instruction.UnknownInstrIndex, Instruction.UnknownInstrIndex, handlers)
        {
            Debug.Assert(handlers != null, "catch blocks should exist");
        }

        /// <summary>
        ///     Generic constructor
        /// </summary>
        internal TryCatchFinallyHandler(int tryStart, int tryEnd, int gotoEndLabelIndex, int finallyStart, int finallyEnd, ExceptionHandler[] handlers)
        {
            TryStartIndex = tryStart;
            TryEndIndex = tryEnd;
            FinallyStartIndex = finallyStart;
            FinallyEndIndex = finallyEnd;
            GotoEndTargetIndex = gotoEndLabelIndex;
            Handlers = handlers;
        }

        internal ExceptionHandler[] Handlers { get; }

        internal bool IsCatchBlockExist => Handlers != null;

        internal bool IsFinallyBlockExist
        {
            get
            {
                Debug.Assert(FinallyStartIndex != Instruction.UnknownInstrIndex == (FinallyEndIndex != Instruction.UnknownInstrIndex));
                return FinallyStartIndex != Instruction.UnknownInstrIndex;
            }
        }

        internal bool HasHandler(InterpretedFrame frame, Exception exception, [NotNullWhen(true)] out ExceptionHandler? handler, [NotNullWhen(true)] out object? unwrappedException)
        {
            frame.SaveTraceToException(exception);

            if (IsCatchBlockExist)
            {
                unwrappedException = exception is RuntimeWrappedException rwe ? rwe.WrappedException : exception;
                var exceptionType = unwrappedException.GetType();
                foreach (var candidate in Handlers)
                {
                    if (!candidate.Matches(exceptionType) || (candidate.Filter != null && !FilterPasses(frame, ref unwrappedException, candidate.Filter)))
                    {
                        continue;
                    }

                    handler = candidate;
                    return true;
                }
            }
            else
            {
                unwrappedException = null;
            }

            handler = null;
            return false;
        }

        private static bool FilterPasses(InterpretedFrame frame, ref object exception, ExceptionFilter filter)
        {
            var interpreter = frame.Interpreter;
            var instructions = interpreter.Instructions.Instructions;
            var stackIndex = frame.StackIndex;
            var frameIndex = frame.InstructionIndex;
            try
            {
                var index = interpreter.Labels[filter.LabelIndex].Index;
                frame.InstructionIndex = index;
                frame.Push(exception);
                while (index >= filter.StartIndex && index < filter.EndIndex)
                {
                    index += instructions[index].Run(frame);
                    frame.InstructionIndex = index;
                }

                // Exception is stored in a local at start of the filter, and loaded from it at the end, so it is now
                // on the top of the stack. It may have been assigned to in the course of the filter running.
                // If this is the handler that will be executed, then if the filter has assigned to the exception variable
                // that change should be visible to the handler. Otherwise, it should not, so we write it back only on true.
                var exceptionLocal = frame.Pop()!;
                if ((bool)frame.Pop()!)
                {
                    exception = exceptionLocal;
                    // Stack and instruction indices will be overwritten in the catch block anyway, so no need to restore.
                    return true;
                }
            }
            catch (Exception captured)
            {
                // Silently eating exceptions and returning false matches the CLR behavior.
                No.Op(captured);
            }

            frame.StackIndex = stackIndex;
            frame.InstructionIndex = frameIndex;
            return false;
        }
    }
}

#endif