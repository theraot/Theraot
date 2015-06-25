#if NET20 || NET30

using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class NewExpression : Expression
    {
        private ReadOnlyCollection<Expression> arguments;
        private ConstructorInfo constructor;
        private ReadOnlyCollection<MemberInfo> members;

        internal NewExpression(Type type, ReadOnlyCollection<Expression> arguments)
            : base(ExpressionType.New, type)
        {
            this.arguments = arguments;
        }

        internal NewExpression(ConstructorInfo constructor, ReadOnlyCollection<Expression> arguments, ReadOnlyCollection<MemberInfo> members)
            : base(ExpressionType.New, constructor.DeclaringType)
        {
            this.constructor = constructor;
            this.arguments = arguments;
            this.members = members;
        }

        public ReadOnlyCollection<Expression> Arguments
        {
            get { return arguments; }
        }

        public ConstructorInfo Constructor
        {
            get { return constructor; }
        }

        public ReadOnlyCollection<MemberInfo> Members
        {
            get { return members; }
        }

        internal override void Emit(EmitContext ec)
        {
            var ig = ec.ig;
            var type = this.Type;

            LocalBuilder local = null;
            if (type.IsValueType)
            {
                local = ig.DeclareLocal(type);
                ig.Emit(OpCodes.Ldloca, local);

                if (constructor == null)
                {
                    ig.Emit(OpCodes.Initobj, type);
                    ig.Emit(OpCodes.Ldloc, local);
                    return;
                }
            }

            ec.EmitCollection(arguments);

            if (type.IsValueType)
            {
                ig.Emit(OpCodes.Call, constructor);
                ig.Emit(OpCodes.Ldloc, local);
            }
            else
                ig.Emit(OpCodes.Newobj, constructor ?? GetDefaultConstructor(type));
        }

        private static ConstructorInfo GetDefaultConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }
    }
}

#endif