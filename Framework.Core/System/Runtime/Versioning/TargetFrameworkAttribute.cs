#if NET20 || NET30 || NET35

namespace System.Runtime.Versioning
{
    [AttributeUsageAttribute(AttributeTargets.Assembly)]
    public sealed class TargetFrameworkAttribute : Attribute
    {
        public TargetFrameworkAttribute(string frameworkName)
        {
            FrameworkName = frameworkName ?? throw new ArgumentNullException(nameof(frameworkName));
        }

        public string FrameworkName { get; }

        public string FrameworkDisplayName { get; set; }
    }
}

#endif