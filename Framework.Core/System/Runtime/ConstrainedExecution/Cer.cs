#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
namespace System.Runtime.ConstrainedExecution
{
    public enum Cer
    {
        None = 0,
        MayFail = 1,
        Success = 2
    }
}

#endif