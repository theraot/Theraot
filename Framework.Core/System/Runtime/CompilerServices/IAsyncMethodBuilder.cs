#if LESSTHAN_NET45

namespace System.Runtime.CompilerServices
{
    /// <summary>
    ///     Represents an asynchronous method builder.
    /// </summary>
    internal interface IAsyncMethodBuilder
    {
        void PreBoxInitialization();
    }
}

#endif