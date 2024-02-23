#if NETFRAMEWORK && !NET45_OR_GREATER
namespace System.Reflection;

public static class ParameterInfoTheraotExtensions
{
    // TODO: DefaultValue may throw FormatException or return null for structs or Type.Missing
    // https://github.com/dotnet/runtime/blob/v7.0.13/src/libraries/Common/src/Extensions/ParameterDefaultValue/ParameterDefaultValue.netstandard.cs
    // https://github.com/dotnet/runtime/issues/43757#issuecomment-715564940
    // https://github.com/dotnet/runtime/blob/v7.0.13/src/coreclr/System.Private.CoreLib/src/System/Reflection/RuntimeParameterInfo.cs#L287-L298
    public static bool HasDefaultValue(this ParameterInfo @this)
        => @this.DefaultValue is var v && v != DBNull.Value && v != Type.Missing;
}
#endif
