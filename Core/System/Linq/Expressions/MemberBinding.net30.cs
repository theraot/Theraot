#if NET20 || NET30

using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public abstract class MemberBinding
    {
        private readonly MemberBindingType _bindingType;
        private readonly MemberInfo _member;

        protected MemberBinding(MemberBindingType bindingType, MemberInfo member)
        {
            _bindingType = bindingType;
            _member = member;
        }

        public MemberBindingType BindingType
        {
            get
            {
                return _bindingType;
            }
        }

        public MemberInfo Member
        {
            get
            {
                return _member;
            }
        }

        public override string ToString()
        {
            return ExpressionPrinter.ToString(this);
        }

        internal abstract void Emit(EmitContext emitContext, LocalBuilder local);

        internal LocalBuilder EmitLoadMember(EmitContext emitContext, LocalBuilder local)
        {
            emitContext.EmitLoadSubject(local);
            return _member.OnFieldOrProperty
            (
                field => EmitLoadField(emitContext, field),
                prop => EmitLoadProperty(emitContext, prop)
            );
        }

        private LocalBuilder EmitLoadField(EmitContext emitContext, FieldInfo field)
        {
            var store = emitContext.ILGenerator.DeclareLocal(field.FieldType);
            emitContext.ILGenerator.Emit(OpCodes.Ldfld, field);
            emitContext.ILGenerator.Emit(OpCodes.Stloc, store);
            return store;
        }

        private LocalBuilder EmitLoadProperty(EmitContext emitContext, PropertyInfo property)
        {
            var getter = property.GetGetMethod(true);
            if (getter == null)
            {
                throw new NotSupportedException();
            }
            else
            {
                var store = emitContext.ILGenerator.DeclareLocal(property.PropertyType);
                emitContext.EmitCall(getter);
                emitContext.ILGenerator.Emit(OpCodes.Stloc, store);
                return store;
            }
        }
    }
}

#endif