#if NET20 || NET30

using System.Collections.ObjectModel;

namespace System.Linq.Expressions
{
    public sealed class ListInitExpression : Expression
    {
        private ReadOnlyCollection<ElementInit> _initializers;
        private NewExpression _newExpression;

        internal ListInitExpression(NewExpression newExpression, ReadOnlyCollection<ElementInit> initializers)
            : base(ExpressionType.ListInit, newExpression.Type)
        {
            _newExpression = newExpression;
            _initializers = initializers;
        }

        public ReadOnlyCollection<ElementInit> Initializers
        {
            get
            {
                return _initializers;
            }
        }

        public NewExpression NewExpression
        {
            get
            {
                return _newExpression;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            var local = emitContext.EmitStored(_newExpression);
            emitContext.EmitCollection(_initializers, local);
            emitContext.EmitLoad(local);
        }
    }
}

#endif