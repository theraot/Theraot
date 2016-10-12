#if NETCF

using System.Runtime.InteropServices;

namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly)]
    [ComVisible(true)]
    public sealed class AssemblyFileVersionAttribute : Attribute
    {
        private readonly string _version;

        public AssemblyFileVersionAttribute(string version)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }
            _version = version;
        }

        public string Version
        {
            get
            {
                return _version;
            }
        }
    }
}

#endif