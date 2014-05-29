#if NET20 || NET30

using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class MemberAssignment : MemberBinding
    {
        private readonly Expression expression;

        internal MemberAssignment(MemberInfo member, Expression expression)
            : base(MemberBindingType.Assignment, member)
        {
            this.expression = expression;
        }

        public Expression Expression
        {
            get
            {
                return expression;
            }
        }

        internal override void Emit(EmitContext emitContext, LocalBuilder local)
        {
            Member.OnFieldOrProperty(
                field => EmitFieldAssignment(emitContext, field, local),
                prop => EmitPropertyAssignment(emitContext, prop, local));
        }

        private void EmitFieldAssignment(EmitContext emitContext, FieldInfo field, LocalBuilder local)
        {
            emitContext.EmitLoadSubject(local);
            expression.Emit(emitContext);
            emitContext.ig.Emit(OpCodes.Stfld, field);
        }

        private void EmitPropertyAssignment(EmitContext emitContext, PropertyInfo property, LocalBuilder local)
        {
            var setter = property.GetSetMethod(true);
            if (setter == null)
            {
                throw new InvalidOperationException();
            }
            emitContext.EmitLoadSubject(local);
            expression.Emit(emitContext);
            emitContext.EmitCall(setter);
        }
    }
}

#endif