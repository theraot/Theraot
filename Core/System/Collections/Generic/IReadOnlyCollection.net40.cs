namespace System.Collections.Generic
{
#if NET40

    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        int Count
        {
            get;
        }
    }

#endif
#if NET20 || NET30 || NET35

    public interface IReadOnlyCollection<T> : IEnumerable<T>
    {
        int Count
        {
            get;
        }
    }

#endif
}