#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13

namespace System.Runtime.CompilerServices
{
    public abstract class FormattableString : IFormattable
    {
        public abstract int ArgumentCount { get; }
        public abstract string Format { get; }

        public static string CurrentCulture(FormattableString formattable)
        {
            throw new NotImplementedException();
        }

        public static string Invariant(FormattableString formattable)
        {
            throw new NotImplementedException();
        }

        public abstract object GetArgument(int index);

        public abstract object[] GetArguments();

        public abstract string ToString(IFormatProvider formatProvider);

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }
    }
}

#endif