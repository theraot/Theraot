using System.Collections.Generic;
using System.Collections.ObjectModel;
using Theraot.Collections.ThreadSafe;

namespace TestRunner
{
    public class InterfaceTests
    {
        public static readonly IReadOnlyDictionary<string, int> DictionaryExAsIReadOnlyDictionary = new DictionaryEx<string, int>();
        public static readonly IReadOnlyCollection<string> HashSetExAsIReadOnlyCollection = new HashSetEx<string>();
        public static readonly ISet<string> HashSetExAsISet = new HashSetEx<string>();
        public static readonly IReadOnlyList<string> ListExAsReadOnlyList = new ListEx<string>();
        public static readonly IReadOnlyList<string> ReadOnlyCollectionExAsIReadOnlyList = new ReadOnlyCollectionEx<string>(ArrayReservoir<string>.EmptyArray);
    }
}