#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13

namespace System.Runtime.CompilerServices
{
    public static class FormattableStringFactory
    {
        public static FormattableString Create(string format, params object?[] arguments)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }
            return new ConcreteFormattableString(format, arguments);
        }

        private sealed class ConcreteFormattableString : FormattableString
        {
            private readonly object?[] _arguments;

            internal ConcreteFormattableString(string format, object?[] arguments)
            {
                Format = format;
                _arguments = arguments;
            }

            public override int ArgumentCount => _arguments.Length;

            public override string Format { get; }

            public override object? GetArgument(int index)
            {
                return _arguments[index];
            }

            public override object?[] GetArguments()
            {
                return _arguments;
            }

            public override string ToString(IFormatProvider? formatProvider)
            {
                return string.Format(formatProvider, Format, _arguments);
            }
        }
    }
}

#endif