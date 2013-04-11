#if NET20 || NET30

using System.Collections.ObjectModel;

namespace System.Linq.Expressions
{
    public sealed class MemberInitExpression : Expression
    {
        private ReadOnlyCollection<MemberBinding> _bindings;
        private NewExpression _newExpression;

        internal MemberInitExpression(NewExpression newExpression, ReadOnlyCollection<MemberBinding> bindings)
            : base(ExpressionType.MemberInit, newExpression.Type)
        {
            _newExpression = newExpression;
            _bindings = bindings;
        }

        public ReadOnlyCollection<MemberBinding> Bindings
        {
            get
            {
                return _bindings;
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
            emitContext.EmitCollection(_bindings, local);
            emitContext.EmitLoad(local);
        }
    }
}

#endif