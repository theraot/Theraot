#if NETCF

using System.Runtime.InteropServices;

namespace System.Security
{
    [ComVisible(true)]
    public interface ISecurityEncodable
    {
        void FromXml(SecurityElement e);

        SecurityElement ToXml();
    }
}

#endif