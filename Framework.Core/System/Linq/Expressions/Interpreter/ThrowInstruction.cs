#if LESSTHAN_NET35

#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable CC0021 // Use nameof

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class ThrowInstruction : Instruction
    {
        internal static readonly ThrowInstruction Rethrow = new(true, true);
        internal static readonly ThrowInstruction Throw = new(true, false);
        internal static readonly ThrowInstruction VoidRethrow = new(false, true);
        internal static readonly ThrowInstruction VoidThrow = new(false, false);
        private readonly bool _hasResult, _rethrow;

        private ThrowInstruction(bool hasResult, bool isRethrow)
        {
            _hasResult = hasResult;
            _rethrow = isRethrow;
        }

        public override int ConsumedStack => 1;

        public override string InstructionName => "Throw";
        public override int ProducedStack => _hasResult ? 1 : 0;

        public override int Run(InterpretedFrame frame)
        {
            var ex = WrapThrownObject(frame.Pop()!);
            if (_rethrow)
            {
                throw new RethrowException();
            }

            throw ex!;
        }

        private static Exception? WrapThrownObject(object thrown)
        {
            return thrown == null
                ? null
                : thrown as Exception ?? new /*RuntimeWrappedException(thrown)*/ Exception();
        }
    }
}

#endif