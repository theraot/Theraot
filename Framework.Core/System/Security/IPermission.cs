#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
namespace System.Security
{
    public interface IPermission : ISecurityEncodable
    {
        IPermission Copy();
        IPermission Intersect(IPermission target);
        IPermission Union(IPermission target);
        bool IsSubsetOf(IPermission target);
        void Demand();
    }
}

#endif