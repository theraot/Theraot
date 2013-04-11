#if NET20 || NET30

using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class ParameterExpression : Expression
    {
        private string _name;

        internal ParameterExpression(Type type, string name)
            : base(ExpressionType.Parameter, type)
        {
            _name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            int position = -1;
            if (emitContext.IsLocalParameter(this, ref position))
            {
                EmitLocalParameter(emitContext, position);
                return;
            }
            int level = 0;
            if (emitContext.IsHoistedLocal(this, ref level, ref position))
            {
                EmitHoistedLocal(emitContext, level, position);
                return;
            }
            throw new InvalidOperationException("Parameter out of scope");
        }

        private void EmitHoistedLocal(EmitContext emitContext, int level, int position)
        {
            emitContext.EmitScope();
            for (int i = 0; i < level; i++)
            {
                emitContext.EmitParentScope();
            }
            emitContext.EmitLoadLocals();
            emitContext.ILGenerator.Emit(OpCodes.Ldc_I4, position);
            emitContext.ILGenerator.Emit(OpCodes.Ldelem, typeof(object));
            emitContext.EmitLoadStrongBoxValue(Type);
        }

        private void EmitLocalParameter(EmitContext emitContext, int position)
        {
            emitContext.ILGenerator.Emit(OpCodes.Ldarg, position);
        }
    }
}

#endif