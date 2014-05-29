#if NET20 || NET30

using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class ConditionalExpression : Expression
    {
        private readonly Expression _ifFalse;
        private readonly Expression _ifTrue;
        private readonly Expression _test;

        internal ConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse)
            : base(ExpressionType.Conditional, ifTrue.Type)
        {
            _test = test;
            _ifTrue = ifTrue;
            _ifFalse = ifFalse;
        }

        public Expression IfFalse
        {
            get
            {
                return _ifFalse;
            }
        }

        public Expression IfTrue
        {
            get
            {
                return _ifTrue;
            }
        }

        public Expression Test
        {
            get
            {
                return _test;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            var ig = emitContext.ig;
            var false_target = ig.DefineLabel();
            var end_target = ig.DefineLabel();
            _test.Emit(emitContext);
            ig.Emit(OpCodes.Brfalse, false_target);
            _ifTrue.Emit(emitContext);
            ig.Emit(OpCodes.Br, end_target);
            ig.MarkLabel(false_target);
            _ifFalse.Emit(emitContext);
            ig.MarkLabel(end_target);
        }
    }
}

#endif