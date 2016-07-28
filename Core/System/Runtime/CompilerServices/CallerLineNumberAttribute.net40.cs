#if NET35 || NET40

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Allows you to obtain the line number in the source file at which the method is called.
    /// </summary>
    /// <remarks>
    /// You apply the <b>CallerFilePath</b> attribute to an optional parameter that has a default value. 
    /// You must specify an explicit default value for the optional parameter.
    /// You can't apply this attribute to parameters that aren't specified as optional.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerLineNumberAttribute : Attribute
    {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public CallerLineNumberAttribute()
        {
        }
    }
}

#endif