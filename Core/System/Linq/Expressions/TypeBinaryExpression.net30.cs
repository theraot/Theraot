#if NET20 || NET30

using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class TypeBinaryExpression : Expression
    {
        private readonly Expression _expression;
        private readonly Type _typeOperand;

        internal TypeBinaryExpression(ExpressionType nodeType, Expression expression, Type typeOperand, Type type)
            : base(nodeType, type)
        {
            _expression = expression;
            _typeOperand = typeOperand;
        }

        public Expression Expression
        {
            get
            {
                return _expression;
            }
        }

        public Type TypeOperand
        {
            get
            {
                return _typeOperand;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            var ig = emitContext.ig;
            if (_expression.Type == typeof(void))
            {
                ig.Emit(OpCodes.Ldc_I4_0);
                return;
            }
            emitContext.EmitIsInst(_expression, _typeOperand);
            ig.Emit(OpCodes.Ldnull);
            ig.Emit(OpCodes.Cgt_Un);
        }
    }
}

#endif