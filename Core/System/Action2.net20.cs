#if NET20 || NET30

namespace System
{
    public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
}

#endif