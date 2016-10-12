namespace System.Security
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple=false, Inherited=false)]
    public sealed class SecuritySafeCriticalAttribute : Attribute
    {
        public SecuritySafeCriticalAttribute()
        {
            // Empty
        }
    }
}