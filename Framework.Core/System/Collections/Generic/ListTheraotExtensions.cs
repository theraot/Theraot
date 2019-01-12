using System.Collections.ObjectModel;

namespace System.Collections.Generic
{
    public static class ListTheraotExtensions
    {
#if LESSTHAN_NETSTANDARD13
        public static ReadOnlyCollection<T> AsReadOnly<T>(this List<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }
#endif
    }
}