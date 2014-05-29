#if NET20 || NET30

using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class MemberListBinding : MemberBinding
    {
        private readonly ReadOnlyCollection<ElementInit> _initializers;

        internal MemberListBinding(MemberInfo member, ReadOnlyCollection<ElementInit> initializers)
            : base(MemberBindingType.ListBinding, member)
        {
            _initializers = initializers;
        }

        public ReadOnlyCollection<ElementInit> Initializers
        {
            get
            {
                return _initializers;
            }
        }

        internal override void Emit(EmitContext emitContext, LocalBuilder local)
        {
            var member = EmitLoadMember(emitContext, local);
            foreach (var initializer in _initializers)
            {
                initializer.Emit(emitContext, member);
            }
        }
    }
}

#endif