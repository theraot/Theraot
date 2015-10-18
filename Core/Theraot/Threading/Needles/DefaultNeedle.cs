#if FAT

using System.Collections.Generic;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    [System.ComponentModel.ImmutableObject(true)]
    public sealed class DefaultNeedle<T> : IReadOnlyNeedle<T>
    {
        private static readonly DefaultNeedle<T> _instance = new DefaultNeedle<T>();

        private DefaultNeedle()
        {
            //Empty
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "By Design")]
        public static DefaultNeedle<T> Instance
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
                return true;
            }
        }

        public T Value
        {
            get
            {
                return default(T);
            }
        }

        public static explicit operator T(DefaultNeedle<T> needle)
        {
            return Check.NotNullArgument(needle, "needle").Value;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "By Design")]
        public static bool operator !=(DefaultNeedle<T> left, DefaultNeedle<T> right)
        {
            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "By Design")]
        public static bool operator ==(DefaultNeedle<T> left, DefaultNeedle<T> right)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is DefaultNeedle<T>;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(default(T));
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

#endif