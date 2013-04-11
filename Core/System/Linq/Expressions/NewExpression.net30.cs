#if NET20 || NET30

using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class NewExpression : Expression
    {
        private ReadOnlyCollection<Expression> _arguments;
        private ConstructorInfo _constructor;
        private ReadOnlyCollection<MemberInfo> _members;

        internal NewExpression(Type type, ReadOnlyCollection<Expression> arguments)
            : base(ExpressionType.New, type)
        {
            _arguments = arguments;
        }

        internal NewExpression(ConstructorInfo constructor, ReadOnlyCollection<Expression> arguments, ReadOnlyCollection<MemberInfo> members)
            : base(ExpressionType.New, constructor.DeclaringType)
        {
            _constructor = constructor;
            _arguments = arguments;
            _members = members;
        }

        public ReadOnlyCollection<Expression> Arguments
        {
            get
            {
                return _arguments;
            }
        }

        public ConstructorInfo Constructor
        {
            get
            {
                return _constructor;
            }
        }

        public ReadOnlyCollection<MemberInfo> Members
        {
            get
            {
                return _members;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            var generator = emitContext.ILGenerator;
            var type = Type;
            LocalBuilder local = null;
            if (type.IsValueType)
            {
                local = generator.DeclareLocal(type);
                generator.Emit(OpCodes.Ldloca, local);
                if (_constructor == null)
                {
                    generator.Emit(OpCodes.Initobj, type);
                    generator.Emit(OpCodes.Ldloc, local);
                    return;
                }
            }
            emitContext.EmitCollection(_arguments);
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Call, _constructor);
                generator.Emit(OpCodes.Ldloc, local);
            }
            else
            {
                generator.Emit(OpCodes.Newobj, _constructor ?? GetDefaultConstructor(type));
            }
        }

        private static ConstructorInfo GetDefaultConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }
    }
}

#endif