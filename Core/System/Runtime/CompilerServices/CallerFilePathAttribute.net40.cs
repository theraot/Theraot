#if NET20 || NET30 || NET35 || NET40

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Allows you to obtain the full path of the source file that contains the caller. This is the file path at the time of compile.
    /// </summary>
    /// <remarks>
    /// You apply the <b>CallerFilePath</b> attribute to an optional parameter that has a default value. 
    /// You must specify an explicit default value for the optional parameter.
    /// You can't apply this attribute to parameters that aren't specified as optional.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerFilePathAttribute : Attribute
    {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public CallerFilePathAttribute()
        {
        }
    }
}

#endif