#if NET20 || NET30

namespace System
{
    /// <summary>Encapsulates a method that has no parameters and returns a value of the type specified by the <typeparam name="TResult" /> parameter.</summary>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
#if NETCF
    public delegate TResult Func<TResult>();
#else
    public delegate TResult Func<out TResult>();
#endif
}

#endif