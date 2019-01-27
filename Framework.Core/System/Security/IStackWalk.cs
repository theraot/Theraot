#if LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD
namespace System.Security
{
    public interface IStackWalk
    {
        void Assert();

        void Demand();

        void Deny();

        void PermitOnly();
    }
}

#endif