#if LESSTHAN_NET40

namespace System.Runtime.Versioning
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class TargetFrameworkAttribute : Attribute
    {
        public TargetFrameworkAttribute(string frameworkName)
        {
            FrameworkName = frameworkName ?? throw new ArgumentNullException(nameof(frameworkName));
        }

        public string? FrameworkDisplayName { get; set; }

        public string FrameworkName { get; }
    }
}

#endif