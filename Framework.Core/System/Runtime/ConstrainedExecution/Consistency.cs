#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

namespace System.Runtime.ConstrainedExecution
{
    public enum Consistency
    {
        MayCorruptProcess = 0,
        MayCorruptAppDomain = 1,
        MayCorruptInstance = 2,
        WillNotCorruptState = 3
    }
}

#endif