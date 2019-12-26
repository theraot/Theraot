#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

namespace System.Security
{
    public interface IPermission : ISecurityEncodable
    {
        IPermission Copy();

        void Demand();

        IPermission? Intersect(IPermission target);

        bool IsSubsetOf(IPermission target);

        IPermission? Union(IPermission target);
    }
}

#endif