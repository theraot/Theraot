#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class BranchFalseInstruction : OffsetInstruction
    {
        private static Instruction[]? _cache;

        public override Instruction[] Cache => _cache ??= new Instruction[CacheSize];

        public override int ConsumedStack => 1;
        public override string InstructionName => "BranchFalse";

        public override int Run(InterpretedFrame frame)
        {
            Debug.Assert(Offset != Unknown);

            if (!(bool)frame.Pop()!)
            {
                return Offset;
            }

            return 1;
        }
    }

    internal class BranchInstruction : OffsetInstruction
    {
        internal readonly bool HasResult;
        internal readonly bool HasValue;
        private static Instruction[][][]? _caches;

        public BranchInstruction(bool hasResult, bool hasValue)
        {
            HasResult = hasResult;
            HasValue = hasValue;
        }

        internal BranchInstruction()
            : this(false, false)
        {
            // Empty
        }

        public override Instruction[] Cache
        {
            get
            {
                var caches = GetCaches();
                return caches[ConsumedStack][ProducedStack] ??= new Instruction[CacheSize];
            }
        }

        public override int ConsumedStack => HasValue ? 1 : 0;

        public override string InstructionName => "Branch";

        public override int ProducedStack => HasResult ? 1 : 0;

        public override int Run(InterpretedFrame frame)
        {
            Debug.Assert(Offset != Unknown);

            return Offset;
        }

        private static Instruction[][][] GetCaches()
        {
            var caches = _caches;
            if (caches != null)
            {
                return caches;
            }

            caches = new[] { new Instruction[2][], new Instruction[2][] };
            _caches = caches;
            return caches;
        }
    }

    internal sealed class BranchTrueInstruction : OffsetInstruction
    {
        private static Instruction[]? _cache;

        public override Instruction[] Cache => _cache ??= new Instruction[CacheSize];

        public override int ConsumedStack => 1;
        public override string InstructionName => "BranchTrue";

        public override int Run(InterpretedFrame frame)
        {
            Debug.Assert(Offset != Unknown);

            if ((bool)frame.Pop()!)
            {
                return Offset;
            }

            return 1;
        }
    }

    internal sealed class CoalescingBranchInstruction : OffsetInstruction
    {
        private static Instruction[]? _cache;

        public override Instruction[] Cache => _cache ??= new Instruction[CacheSize];

        public override int ConsumedStack => 1;
        public override string InstructionName => "CoalescingBranch";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            Debug.Assert(Offset != Unknown);

            return frame.Peek() != null ? Offset : 1;
        }
    }

    // no-op: we need this just to balance the stack depth and aid debugging of the instruction list.
    internal sealed class EnterExceptionFilterInstruction : Instruction
    {
        internal static readonly EnterExceptionFilterInstruction Instance = new();

        private EnterExceptionFilterInstruction()
        {
            // Empty
        }

        public override string InstructionName => "EnterExceptionFilter";

        // The exception is pushed onto the stack in the filter runner.
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            return 1;
        }
    }

    // no-op: we need this just to balance the stack depth.
    internal sealed class EnterExceptionHandlerInstruction : Instruction
    {
        internal static readonly EnterExceptionHandlerInstruction NonVoid = new(true);
        internal static readonly EnterExceptionHandlerInstruction Void = new(false);

        // True if try-expression is non-void.
        private readonly bool _hasValue;

        private EnterExceptionHandlerInstruction(bool hasValue)
        {
            _hasValue = hasValue;
        }

        // If an exception is throws in try-body the expression result of try-body is not evaluated and loaded to the stack.
        // So the stack doesn't contain the try-body's value when we start executing the handler.
        // However, while emitting instructions try block falls thru the catch block with a value on stack.
        // We need to declare it consumed so that the stack state upon entry to the handler corresponds to the real
        // stack depth after throw jumped to this catch block.
        public override int ConsumedStack => _hasValue ? 1 : 0;

        public override string InstructionName => "EnterExceptionHandler";

        // A variable storing the current exception is pushed to the stack by exception handling.
        // Catch handlers: The value is immediately popped and stored into a local.
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            // nop (the exception value is pushed by the interpreter in HandleCatch)
            return 1;
        }
    }

    internal sealed class EnterFaultInstruction : IndexedBranchInstruction
    {
        private static readonly EnterFaultInstruction[] _cache = new EnterFaultInstruction[CacheSize];

        private EnterFaultInstruction(int labelIndex)
            : base(labelIndex)
        {
            // Empty
        }

        public override string InstructionName => "EnterFault";
        public override int ProducedStack => 2;

        public override int Run(InterpretedFrame frame)
        {
            Debug.Assert(!frame.IsJumpHappened());

            frame.SetStackDepth(GetLabel(frame).StackDepth);
            frame.PushPendingContinuation();
            frame.RemoveContinuation();
            return 1;
        }

        internal static EnterFaultInstruction Create(int labelIndex)
        {
            if (labelIndex < CacheSize)
            {
                return _cache[labelIndex] ??= new EnterFaultInstruction(labelIndex);
            }

            return new EnterFaultInstruction(labelIndex);
        }
    }

    /// <summary>
    ///     The first instruction of finally block.
    /// </summary>
    internal sealed class EnterFinallyInstruction : IndexedBranchInstruction
    {
        private static readonly EnterFinallyInstruction[] _cache = new EnterFinallyInstruction[CacheSize];

        private EnterFinallyInstruction(int labelIndex)
            : base(labelIndex)
        {
            // Empty
        }

        public override int ConsumedContinuations => 1;
        public override string InstructionName => "EnterFinally";
        public override int ProducedStack => 2;

        public override int Run(InterpretedFrame frame)
        {
            // If _pendingContinuation == -1 then we were getting into the finally block because an exception was thrown
            //      in this case we need to set the stack depth
            // Else we were getting into this finally block from a 'Goto' jump, and the stack depth is already set properly
            if (!frame.IsJumpHappened())
            {
                frame.SetStackDepth(GetLabel(frame).StackDepth);
            }

            frame.PushPendingContinuation();
            frame.RemoveContinuation();
            return 1;
        }

        internal static EnterFinallyInstruction Create(int labelIndex)
        {
            if (labelIndex < CacheSize)
            {
                return _cache[labelIndex] ??= new EnterFinallyInstruction(labelIndex);
            }

            return new EnterFinallyInstruction(labelIndex);
        }
    }

    internal sealed class EnterTryCatchFinallyInstruction : IndexedBranchInstruction
    {
        private readonly bool _hasFinally;

        private EnterTryCatchFinallyInstruction(int targetIndex, bool hasFinally)
            : base(targetIndex)
        {
            _hasFinally = hasFinally;
        }

        public override string InstructionName => _hasFinally ? "EnterTryFinally" : "EnterTryCatch";

        public override int ProducedContinuations => _hasFinally ? 1 : 0;

        internal TryCatchFinallyHandler? Handler { get; private set; }

        public override int Run(InterpretedFrame frame)
        {
            if (_hasFinally)
            {
                // Push finally.
                frame.PushContinuation(LabelIndex);
            }

            var prevInstrIndex = frame.InstructionIndex;
            frame.InstructionIndex++;

            // Start to run the try/catch/finally blocks
            var instructions = frame.Interpreter.Instructions.Instructions;
            try
            {
                // run the try block
                var index = frame.InstructionIndex;
                while (index >= Handler!.TryStartIndex && index < Handler.TryEndIndex)
                {
                    index += instructions[index].Run(frame);
                    frame.InstructionIndex = index;
                }

                // we finish the try block and is about to jump out of the try/catch blocks
                if (index == Handler.GotoEndTargetIndex)
                {
                    // run the 'Goto' that jumps out of the try/catch/finally blocks
                    Debug.Assert(instructions[index] is GotoInstruction, "should be the 'Goto' instruction that jumps out the try/catch/finally");
                    frame.InstructionIndex += instructions[index].Run(frame);
                }
            }
            catch (Exception exception) when (Handler!.HasHandler(frame, exception, out var exHandler, out var unwrappedException))
            {
                Debug.Assert(!(unwrappedException is RethrowException));
                frame.InstructionIndex += frame.Goto(exHandler.LabelIndex, unwrappedException, true);
                var rethrow = false;
                try
                {
                    // run the catch block
                    var index = frame.InstructionIndex;
                    while (index >= exHandler.HandlerStartIndex && index < exHandler.HandlerEndIndex)
                    {
                        index += instructions[index].Run(frame);
                        frame.InstructionIndex = index;
                    }

                    // we finish the catch block and is about to jump out of the try/catch blocks
                    if (index == Handler.GotoEndTargetIndex)
                    {
                        // run the 'Goto' that jumps out of the try/catch/finally blocks
                        Debug.Assert(instructions[index] is GotoInstruction, "should be the 'Goto' instruction that jumps out the try/catch/finally");
                        frame.InstructionIndex += instructions[index].Run(frame);
                    }
                }
                catch (RethrowException)
                {
                    // a rethrow instruction in a catch block gets to run
                    rethrow = true;
                }

                if (rethrow)
                {
                    throw;
                }
            }
            finally
            {
                if (Handler!.IsFinallyBlockExist)
                {
                    // We get to the finally block in two paths:
                    //  1. Jump from the try/catch blocks. This includes two sub-routes:
                    //        a. 'Goto' instruction in the middle of try/catch block
                    //        b. try/catch block runs to its end. Then the 'Goto(end)' will be trigger to jump out of the try/catch block
                    //  2. Exception thrown from the try/catch blocks
                    // In the first path, the continuation mechanism works and frame.InstructionIndex will be updated to point to the first instruction of the finally block
                    // In the second path, the continuation mechanism is not involved and frame.InstructionIndex is not updated
#if DEBUG
                    var isFromJump = frame.IsJumpHappened();
                    Debug.Assert(!isFromJump || (isFromJump && Handler.FinallyStartIndex == frame.InstructionIndex), "we should already jump to the first instruction of the finally");
#endif
                    // run the finally block
                    // we cannot jump out of the finally block, and we cannot have an immediate rethrow in it
                    var index = frame.InstructionIndex = Handler.FinallyStartIndex;
                    while (index >= Handler.FinallyStartIndex && index < Handler.FinallyEndIndex)
                    {
                        index += instructions[index].Run(frame);
                        frame.InstructionIndex = index;
                    }
                }
            }

            return frame.InstructionIndex - prevInstrIndex;
        }

        public override string ToString()
        {
            return _hasFinally ? "EnterTryFinally[" + LabelIndex + "]" : "EnterTryCatch";
        }

        internal static EnterTryCatchFinallyInstruction CreateTryCatch()
        {
            return new EnterTryCatchFinallyInstruction(UnknownInstrIndex, false);
        }

        internal static EnterTryCatchFinallyInstruction CreateTryFinally(int labelIndex)
        {
            return new EnterTryCatchFinallyInstruction(labelIndex, true);
        }

        internal void SetTryHandler(TryCatchFinallyHandler tryHandler)
        {
            Handler = tryHandler;
        }
    }

    internal sealed class EnterTryFaultInstruction : IndexedBranchInstruction
    {
        internal EnterTryFaultInstruction(int targetIndex)
            : base(targetIndex)
        {
            // Empty
        }

        public override string InstructionName => "EnterTryFault";
        public override int ProducedContinuations => 1;

        internal TryFaultHandler? Handler { get; private set; }

        public override int Run(InterpretedFrame frame)
        {
            // Push fault.
            frame.PushContinuation(LabelIndex);

            var prevInstrIndex = frame.InstructionIndex;
            frame.InstructionIndex++;

            // Start to run the try/fault blocks
            var instructions = frame.Interpreter.Instructions.Instructions;

            // C# 6 has no direct support for fault blocks, but they can be faked or coerced out of the compiler
            // in several ways. Catch-and-rethrow can work in specific cases, but not generally as the double-pass
            // will not work correctly with filters higher up the call stack. Iterators can be used to produce real
            // fault blocks, but it depends on an implementation detail rather than a guarantee, and is rather
            // indirect. This leaves using a finally block and not doing anything in it if the body ran to
            // completion, which is the approach used here.
            var ranWithoutFault = false;
            try
            {
                // run the try block
                var index = frame.InstructionIndex;
                while (index >= Handler!.TryStartIndex && index < Handler.TryEndIndex)
                {
                    index += instructions[index].Run(frame);
                    frame.InstructionIndex = index;
                }

                // run the 'Goto' that jumps out of the try/fault blocks
                Debug.Assert(instructions[index] is GotoInstruction, "should be the 'Goto' instruction that jumps out the try/fault");

                // if we've arrived here there was no exception thrown. As the fault block won't run, we need to
                // pop the continuation for it here, before Gotoing the end of the try/fault.
                ranWithoutFault = true;
                frame.RemoveContinuation();
                frame.InstructionIndex += instructions[index].Run(frame);
            }
            finally
            {
                if (!ranWithoutFault)
                {
                    // run the fault block
                    // we cannot jump out of the finally block, and we cannot have an immediate rethrow in it
                    var index = frame.InstructionIndex = Handler!.FinallyStartIndex;
                    while (index >= Handler.FinallyStartIndex && index < Handler.FinallyEndIndex)
                    {
                        index += instructions[index].Run(frame);
                        frame.InstructionIndex = index;
                    }
                }
            }

            return frame.InstructionIndex - prevInstrIndex;
        }

        internal void SetTryHandler(TryFaultHandler tryHandler)
        {
            Debug.Assert(Handler == null, "the tryHandler can be set only once");
            Handler = tryHandler;
        }
    }

    /// <summary>
    ///     <para>
    ///         This instruction implements a goto expression that can jump out of any expression.
    ///         It pops values (arguments) from the evaluation stack that the expression tree nodes in between
    ///         the goto expression and the target label node pushed and not consumed yet.
    ///         A goto expression can jump into a node that evaluates arguments only if it carries
    ///         a value and jumps right after the first argument (the carried value will be used as the first argument).
    ///         Goto can jump into an arbitrary child of a BlockExpression since the block doesn't accumulate values
    ///         on evaluation stack as its child expressions are being evaluated.
    ///     </para>
    ///     <para>
    ///         Goto needs to execute any finally blocks on the way to the target label.
    ///         <example>
    ///             {
    ///             f(1, 2, try { g(3, 4, try { goto L } finally { ... }, 6) } finally { ... }, 7, 8)
    ///             L: ...
    ///             }
    ///         </example>
    ///         The goto expression here jumps to label L while having 4 items on evaluation stack (1, 2, 3 and 4).
    ///         The jump needs to execute both finally blocks, the first one on stack level 4 the
    ///         second one on stack level 2. So, it needs to jump the first finally block, pop 2 items from the stack,
    ///         run second finally block and pop another 2 items from the stack and set instruction pointer to label L.
    ///     </para>
    ///     <para>
    ///         Goto also needs to rethrow ThreadAbortException iff it jumps out of a catch handler and
    ///         the current thread is in "abort requested" state.
    ///     </para>
    /// </summary>
    internal sealed class GotoInstruction : IndexedBranchInstruction
    {
        private const int _variants = 8;
        private static readonly GotoInstruction[] _cache = new GotoInstruction[_variants * CacheSize];

        private readonly bool _hasResult;
        private readonly bool _hasValue;
        private readonly bool _labelTargetGetsValue;

        private GotoInstruction(int targetIndex, bool hasResult, bool hasValue, bool labelTargetGetsValue)
            : base(targetIndex)
        {
            _hasResult = hasResult;
            _hasValue = hasValue;
            _labelTargetGetsValue = labelTargetGetsValue;
        }

        public override int ConsumedStack => _hasValue ? 1 : 0;
        public override string InstructionName => "Goto";

        // Should technically return 1 for ConsumedContinuations and ProducedContinuations for gotos that target a label whose continuation depth
        // is different from the current continuation depth. This is because we will consume one continuation from the _continuations
        // and at meantime produce a new _pendingContinuation. However, in case of forward gotos, we don't not know that is the
        // case until the label is emitted. By then the consumed and produced stack information is useless.
        // The important thing here is that the stack balance is 0.
        public override int ProducedStack => _hasResult ? 1 : 0;

        public override int Run(InterpretedFrame frame)
        {
            // Are we jumping out of catch/finally while aborting the current thread?
            // goto the target label or the current finally continuation:
            var value = _hasValue ? frame.Pop()! : Interpreter.NoValue;
            return frame.Goto(LabelIndex, _labelTargetGetsValue ? value : Interpreter.NoValue, false);
        }

        internal static GotoInstruction Create(int labelIndex, bool hasResult, bool hasValue, bool labelTargetGetsValue)
        {
            if (labelIndex >= CacheSize)
            {
                return new GotoInstruction(labelIndex, hasResult, hasValue, labelTargetGetsValue);
            }

            var index = (_variants * labelIndex) | (labelTargetGetsValue ? 4 : 0) | (hasResult ? 2 : 0) | (hasValue ? 1 : 0);
            return _cache[index] ??= new GotoInstruction(labelIndex, hasResult, hasValue, labelTargetGetsValue);
        }
    }

    internal sealed class IntSwitchInstruction<T> : Instruction
    {
        private readonly Dictionary<T, int> _cases;

        internal IntSwitchInstruction(Dictionary<T, int> cases)
        {
            Assert.NotNull(cases);
            _cases = cases;
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "IntSwitch";

        public override int Run(InterpretedFrame frame)
        {
            return _cases.TryGetValue((T)frame.Pop()!, out var target) ? target : 1;
        }
    }

    // no-op: we need this just to balance the stack depth and aid debugging of the instruction list.
    internal sealed class LeaveExceptionFilterInstruction : Instruction
    {
        internal static readonly LeaveExceptionFilterInstruction Instance = new();

        private LeaveExceptionFilterInstruction()
        {
            // Empty
        }

        // The exception and the boolean result are popped from the stack in the filter runner.
        public override int ConsumedStack => 2;

        public override string InstructionName => "LeaveExceptionFilter";

        public override int Run(InterpretedFrame frame)
        {
            return 1;
        }
    }

    /// <summary>
    ///     The last instruction of a catch exception handler.
    /// </summary>
    internal sealed class LeaveExceptionHandlerInstruction : IndexedBranchInstruction
    {
        private static readonly LeaveExceptionHandlerInstruction[] _cache = new LeaveExceptionHandlerInstruction[2 * CacheSize];

        private readonly bool _hasValue;

        private LeaveExceptionHandlerInstruction(int labelIndex, bool hasValue)
            : base(labelIndex)
        {
            _hasValue = hasValue;
        }

        // The catch block yields a value if the body is non-void. This value is left on the stack.
        public override int ConsumedStack => _hasValue ? 1 : 0;

        public override string InstructionName => "LeaveExceptionHandler";
        public override int ProducedStack => _hasValue ? 1 : 0;

        public override int Run(InterpretedFrame frame)
        {
            // CLR rethrows ThreadAbortException when leaving catch handler if abort is requested on the current thread.
            return GetLabel(frame).Index - frame.InstructionIndex;
        }

        internal static LeaveExceptionHandlerInstruction Create(int labelIndex, bool hasValue)
        {
            if (labelIndex >= CacheSize)
            {
                return new LeaveExceptionHandlerInstruction(labelIndex, hasValue);
            }

            var index = (2 * labelIndex) | (hasValue ? 1 : 0);
            return _cache[index] ??= new LeaveExceptionHandlerInstruction(labelIndex, hasValue);
        }
    }

    internal sealed class LeaveFaultInstruction : Instruction
    {
        internal static readonly Instruction Instance = new LeaveFaultInstruction();

        private LeaveFaultInstruction()
        {
            // Empty
        }

        public override int ConsumedContinuations => 1;
        public override int ConsumedStack => 2;
        public override string InstructionName => "LeaveFault";

        public override int Run(InterpretedFrame frame)
        {
            frame.PopPendingContinuation();

            Debug.Assert(!frame.IsJumpHappened());
            // Just return 1, and the real instruction index will be calculated by GotoHandler later
            return 1;
        }
    }

    /// <summary>
    ///     The last instruction of finally block.
    /// </summary>
    internal sealed class LeaveFinallyInstruction : Instruction
    {
        internal static readonly Instruction Instance = new LeaveFinallyInstruction();

        private LeaveFinallyInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "LeaveFinally";

        public override int Run(InterpretedFrame frame)
        {
            frame.PopPendingContinuation();

            // If _pendingContinuation == -1 then we were getting into the finally block because an exception was thrown
            // In this case we just return 1, and the real instruction index will be calculated by GotoHandler later
            return !frame.IsJumpHappened() ? 1 : frame.YieldToPendingContinuation();
            // jump to goto target or to the next finally:
        }
    }

    internal abstract class OffsetInstruction : Instruction
    {
        internal const int CacheSize = 32;
        internal const int Unknown = int.MinValue;

        // the offset to jump to (relative to this instruction):
        protected int Offset = Unknown;

        public abstract Instruction[] Cache { get; }

        public Instruction Fixup(int offset)
        {
            Debug.Assert(Offset == Unknown && offset != Unknown);
            Offset = offset;

            var cache = Cache;
            if (cache != null && offset >= 0 && offset < cache.Length)
            {
                return cache[offset] ??= this;
            }

            return this;
        }

        public override string ToDebugString(int instructionIndex, object? cookie, Func<int, int> labelIndexer, IList<object>? objects)
        {
            return ToString() + (Offset != Unknown ? " -> " + (instructionIndex + Offset) : "");
        }

        public override string ToString()
        {
            return InstructionName + (Offset == Unknown ? "(?)" : "(" + Offset + ")");
        }
    }

    internal sealed class StringSwitchInstruction : Instruction
    {
        private readonly Dictionary<string, int> _cases;
        private readonly StrongBox<int> _nullCase;

        internal StringSwitchInstruction(Dictionary<string, int> cases, StrongBox<int> nullCase)
        {
            Assert.NotNull(cases);
            Assert.NotNull(nullCase);
            _cases = cases;
            _nullCase = nullCase;
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "StringSwitch";

        public override int Run(InterpretedFrame frame)
        {
            var value = frame.Pop();

            if (value == null)
            {
                return _nullCase.Value;
            }

            return _cases.TryGetValue((string)value, out var target) ? target : 1;
        }
    }
}

#endif