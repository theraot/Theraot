#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13

namespace System.Runtime.CompilerServices
{
    public static class FormattableStringFactory
    {
        public static FormattableString Create(string format, params object[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}

#endif