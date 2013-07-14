using System;

namespace Theraot.Threading.Needles
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.ComponentModel.ImmutableObject(true)]
    public sealed class EmptyReadOnlyNeedle<T> : IReadOnlyNeedle<T>
    {
        private static readonly EmptyReadOnlyNeedle<T> _instance = new EmptyReadOnlyNeedle<T>();

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "By Design")]
        public static EmptyReadOnlyNeedle<T> Instance
        {
            get
            {
                return _instance;
            }
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return false;
            }
        }

        T IReadOnlyNeedle<T>.Value
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "By Design")]
        public static bool operator !=(EmptyReadOnlyNeedle<T> left, EmptyReadOnlyNeedle<T> right)
        {
            return false;
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "By Design")]
        public static bool operator ==(EmptyReadOnlyNeedle<T> left, EmptyReadOnlyNeedle<T> right)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is EmptyReadOnlyNeedle<T>;
        }

        public override int GetHashCode()
        {
            return false.GetHashCode();
        }

        public override string ToString()
        {
            return "<Dead Needle>";
        }
    }
}