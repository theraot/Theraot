#if NET20 || NET30

using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class ElementInit
    {
        private readonly MethodInfo _addMethod;
        private readonly ReadOnlyCollection<Expression> _arguments;

        internal ElementInit(MethodInfo addMethod, ReadOnlyCollection<Expression> arguments)
        {
            _addMethod = addMethod;
            _arguments = arguments;
        }

        public MethodInfo AddMethod
        {
            get
            {
                return _addMethod;
            }
        }

        public ReadOnlyCollection<Expression> Arguments
        {
            get
            {
                return _arguments;
            }
        }

        public override string ToString()
        {
            return ExpressionPrinter.ToString(this);
        }

        internal void Emit(EmitContext emitContext, LocalBuilder local)
        {
            emitContext.EmitCall(local, _arguments, _addMethod);
            EmitPopIfNeeded(emitContext);
        }

        private void EmitPopIfNeeded(EmitContext emitContext)
        {
            if (_addMethod.ReturnType == typeof(void))
            {
                return;
            }
            emitContext.ig.Emit(OpCodes.Pop);
        }
    }
}

#endif