#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13

namespace System
{
    public abstract class FormattableString : IFormattable
    {
        public abstract int ArgumentCount { get; }
        public abstract string Format { get; }

        public static string CurrentCulture(FormattableString formattable)
        {
            if (formattable == null)
            {
                throw new ArgumentNullException(nameof(formattable));
            }

            return formattable.ToString(Globalization.CultureInfo.CurrentCulture);
        }

        public static string Invariant(FormattableString formattable)
        {
            if (formattable == null)
            {
                throw new ArgumentNullException(nameof(formattable));
            }

            return formattable.ToString(Globalization.CultureInfo.InvariantCulture);
        }

        public abstract object? GetArgument(int index);

        public abstract object?[] GetArguments();

        public abstract string ToString(IFormatProvider? formatProvider);

        public override string ToString()
        {
            return ToString(Globalization.CultureInfo.CurrentCulture);
        }

        string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
        {
            _ = format;
            return ToString(formatProvider);
        }
    }
}

#endif