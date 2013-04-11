#if NET20 ||NET30

using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class MemberExpression : Expression
    {
        private Expression _expression;
        private MemberInfo _member;

        internal MemberExpression(Expression expression, MemberInfo member, Type type)
            : base(ExpressionType.MemberAccess, type)
        {
            _expression = expression;
            _member = member;
        }

        public Expression Expression
        {
            get
            {
                return _expression;
            }
        }

        public MemberInfo Member
        {
            get
            {
                return _member;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            _member.OnFieldOrProperty
            (
                field => EmitFieldAccess(emitContext, field),
                prop => EmitPropertyAccess(emitContext, prop)
            );
        }

        private void EmitFieldAccess(EmitContext emitContext, FieldInfo field)
        {
            if (field.IsStatic)
            {
                emitContext.ILGenerator.Emit(OpCodes.Ldsfld, field);
            }
            else
            {
                emitContext.EmitLoadSubject(_expression);
                emitContext.ILGenerator.Emit(OpCodes.Ldfld, field);
            }
        }

        private void EmitPropertyAccess(EmitContext emitContext, PropertyInfo property)
        {
            var getter = property.GetGetMethod(true);
            if (!getter.IsStatic)
            {
                emitContext.EmitLoadSubject(_expression);
            }
            else
            {
                emitContext.EmitCall(getter);
            }
        }
    }
}

#endif