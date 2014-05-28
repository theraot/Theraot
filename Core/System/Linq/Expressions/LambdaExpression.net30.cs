#if NET20 || NET30

using System.Collections.ObjectModel;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public class LambdaExpression : Expression
    {
        private readonly Expression body;
        private readonly ReadOnlyCollection<ParameterExpression> parameters;

        internal LambdaExpression(Type delegateType, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
            : base(ExpressionType.Lambda, delegateType)
        {
            this.body = body;
            this.parameters = parameters;
        }

        public Expression Body
        {
            get
            {
                return body;
            }
        }

        public ReadOnlyCollection<ParameterExpression> Parameters
        {
            get
            {
                return parameters;
            }
        }

        public Delegate Compile()
        {
            var context = new CompilationContext();
            context.AddCompilationUnit(this);
            return context.CreateDelegate();
        }

        internal override void Emit(EmitContext emitContext)
        {
            emitContext.EmitCreateDelegate(this);
        }

        internal void EmitBody(EmitContext emitContext)
        {
            body.Emit(emitContext);
            EmitPopIfNeeded(emitContext);
            emitContext.ILGenerator.Emit(OpCodes.Ret);
        }

        internal Type GetReturnType()
        {
            return Type.GetInvokeMethod().ReturnType;
        }

        private void EmitPopIfNeeded(EmitContext emitContext)
        {
            if (GetReturnType() == typeof(void) && body.Type != typeof(void))
            {
                emitContext.ILGenerator.Emit(OpCodes.Pop);
            }
        }
    }
}

#endif