#if LESSTHAN_NET35

using System.Collections.Generic;

namespace System.Linq.Expressions.Interpreter
{
    internal struct LabelScopeChangeInfo : IEquatable<LabelScopeChangeInfo>
    {
        public readonly LabelScopeKind Kind;
        public readonly IList<Expression>? Nodes;
        public readonly LabelScopeInfo Parent;

        public LabelScopeChangeInfo(LabelScopeInfo parent, LabelScopeKind kind, IList<Expression>? nodes)
        {
            Parent = parent;
            Kind = kind;
            Nodes = nodes;
        }

        public static implicit operator (LabelScopeInfo parent, LabelScopeKind kind, IList<Expression>? nodes)(LabelScopeChangeInfo value)
        {
            return (value.Parent, value.Kind, value.Nodes);
        }

        public static implicit operator LabelScopeChangeInfo((LabelScopeInfo parent, LabelScopeKind kind, IList<Expression>? nodes) value)
        {
            return new LabelScopeChangeInfo(value.parent, value.kind, value.nodes);
        }

        public void Deconstruct(out LabelScopeInfo parent, out LabelScopeKind kind, out IList<Expression>? nodes)
        {
            parent = Parent;
            kind = Kind;
            nodes = Nodes;
        }

        public override bool Equals(object? obj)
        {
            return obj is LabelScopeChangeInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = 1209964386;
            hashCode = (hashCode * -1521134295) + EqualityComparer<LabelScopeInfo>.Default.GetHashCode(Parent);
            hashCode = (hashCode * -1521134295) + Kind.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<IList<Expression>?>.Default.GetHashCode(Nodes);
            return hashCode;
        }

        public bool Equals(LabelScopeChangeInfo other)
        {
            var (parent, kind, nodes) = other;
            return EqualityComparer<LabelScopeInfo>.Default.Equals(Parent, parent)
                   && Kind == kind
                   && EqualityComparer<IList<Expression>?>.Default.Equals(Nodes, nodes);
        }
    }
}

#endif