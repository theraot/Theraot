#if NETCF

using System;
using System.Runtime.InteropServices;

namespace System.Security
{
    [ComVisible(true)]
    public interface IPermission : ISecurityEncodable
    {
        IPermission Copy();

        void Demand();

        IPermission Intersect(IPermission target);

        bool IsSubsetOf(IPermission target);

        IPermission Union(IPermission target);
    }
}

#endif