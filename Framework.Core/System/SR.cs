#if LESSTHAN_NET45

namespace System
{
    internal static class SR
    {
        internal static string ArgumentCannotBeOfTypeVoid => "Argument type cannot be System.Void.";

        internal static string UnknownBindingType => "Unknown binding type";

        internal static string UnsupportedExpressionType => "The expression type '{0}' is not supported";
    }
}

#endif