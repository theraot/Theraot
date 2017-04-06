namespace System.Collections.Generic
{
#if NET40

    public interface IReadOnlyList<out T> : IReadOnlyCollection<T>
    {
        T this[int index]
        {
            get;
        }
    }
#endif
#if NET20 || NET30 || NET35

    public interface IReadOnlyList<T> : IReadOnlyCollection<T>
    {
        T this[int index]
        {
            get;
        }
    }

#endif
}