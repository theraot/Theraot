#if NET20 || NET30

using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class NewArrayExpression : Expression
    {
        private readonly ReadOnlyCollection<Expression> _expressions;

        internal NewArrayExpression(ExpressionType expressionType, Type type, ReadOnlyCollection<Expression> expressions)
            : base(expressionType, type)
        {
            _expressions = expressions;
        }

        public ReadOnlyCollection<Expression> Expressions
        {
            get
            {
                return _expressions;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            var type = Type.GetElementType();
            switch (NodeType)
            {
                case ExpressionType.NewArrayInit:
                    EmitNewArrayInit(emitContext, type);
                    return;

                case ExpressionType.NewArrayBounds:
                    EmitNewArrayBounds(emitContext, type);
                    return;

                default:
                    throw new NotSupportedException();
            }
        }

        private static Type CreateArray(Type type, int rank)
        {
            return type.MakeArrayType(rank);
        }

        private static Type[] CreateTypeParameters(int rank)
        {
            return Enumerable.ToArray(Enumerable.Repeat(typeof(int), rank));
        }

        private static ConstructorInfo GetArrayConstructor(Type type, int rank)
        {
            return CreateArray(type, rank).GetConstructor(CreateTypeParameters(rank));
        }

        private void EmitNewArrayBounds(EmitContext emitContext, Type type)
        {
            int rank = _expressions.Count;
            emitContext.EmitCollection(_expressions);
            if (rank == 1)
            {
                emitContext.ILGenerator.Emit(OpCodes.Newarr, type);
                return;
            }
            emitContext.ILGenerator.Emit(OpCodes.Newobj, GetArrayConstructor(type, rank));
        }

        private void EmitNewArrayInit(EmitContext emitContext, Type type)
        {
            var size = _expressions.Count;
            emitContext.ILGenerator.Emit(OpCodes.Ldc_I4, size);
            emitContext.ILGenerator.Emit(OpCodes.Newarr, type);
            for (int i = 0; i < size; i++)
            {
                emitContext.ILGenerator.Emit(OpCodes.Dup);
                emitContext.ILGenerator.Emit(OpCodes.Ldc_I4, i);
                _expressions[i].Emit(emitContext);
                emitContext.ILGenerator.Emit(OpCodes.Stelem, type);
            }
        }
    }
}

#endif