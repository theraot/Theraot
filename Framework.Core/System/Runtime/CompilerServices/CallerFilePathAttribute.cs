#if LESSTHAN_NET45

namespace System.Runtime.CompilerServices
{
    /// <inheritdoc />
    /// <summary>
    ///     Allows you to obtain the full path of the source file that contains the caller. This is the file path at the time
    ///     of compile.
    /// </summary>
    /// <remarks>
    ///     You apply the <b>CallerFilePath</b> attribute to an optional parameter that has a default value.
    ///     You must specify an explicit default value for the optional parameter.
    ///     You can't apply this attribute to parameters that aren't specified as optional.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CallerFilePathAttribute : Attribute
    {
        // Empty
    }
}

#endif