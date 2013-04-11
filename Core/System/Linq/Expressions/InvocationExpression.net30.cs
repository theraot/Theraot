#if NET20 || NET30

using System.Collections.ObjectModel;

namespace System.Linq.Expressions
{
    public sealed class InvocationExpression : Expression
    {
        private ReadOnlyCollection<Expression> _arguments;
        private Expression _expression;

        internal InvocationExpression(Expression expression, Type type, ReadOnlyCollection<Expression> arguments)
            : base(ExpressionType.Invoke, type)
        {
            _expression = expression;
            _arguments = arguments;
        }

        public ReadOnlyCollection<Expression> Arguments
        {
            get
            {
                return _arguments;
            }
        }

        public Expression Expression
        {
            get
            {
                return _expression;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            emitContext.EmitCall(_expression, _arguments, _expression.Type.GetInvokeMethod());
        }
    }
}

#endif