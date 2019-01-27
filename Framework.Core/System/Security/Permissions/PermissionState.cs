#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
namespace System.Security.Permissions
{
    public enum PermissionState
    {
        None = 0,
        Unrestricted = 1
    }
}

#endif