#if NET20 || NET30

using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
    public sealed class MemberMemberBinding : MemberBinding
    {
        private readonly ReadOnlyCollection<MemberBinding> _bindings;

        internal MemberMemberBinding(MemberInfo member, ReadOnlyCollection<MemberBinding> bindings)
            : base(MemberBindingType.MemberBinding, member)
        {
            _bindings = bindings;
        }

        public ReadOnlyCollection<MemberBinding> Bindings
        {
            get
            {
                return _bindings;
            }
        }

        internal override void Emit(EmitContext emitContext, LocalBuilder local)
        {
            var member = EmitLoadMember(emitContext, local);
            foreach (var binding in _bindings)
            {
                binding.Emit(emitContext, member);
            }
        }
    }
}

#endif