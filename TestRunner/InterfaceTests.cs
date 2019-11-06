using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Theraot.Collections;

namespace TestRunner
{
    public static class InterfaceTests
    {
        public static readonly IReadOnlyDictionary<string, int> DictionaryExAsIReadOnlyDictionary = new DictionaryEx<string, int>();
        public static readonly IReadOnlyList<string> EmptyCollectionAsIReadOnlyCollection = EmptyCollection<string>.Instance;
        public static readonly IReadOnlyCollection<string> HashSetExAsIReadOnlyCollection = new HashSetEx<string>();
        public static readonly ISet<string> HashSetExAsISet = new HashSetEx<string>();
        public static readonly IReadOnlyCollection<int> ListExAsReadOnlyAsIReadOnlyCollection = (new ListEx<int>()).AsReadOnly();
        public static readonly IReadOnlyList<int> ListExAsReadOnlyAsIReadOnlyList = (new ListEx<int>()).AsReadOnly();
        public static readonly IReadOnlyList<string> ListExAsReadOnlyList = new ListEx<string>();
        public static readonly IReadOnlyList<string> ReadOnlyCollectionExAsIReadOnlyList = new ReadOnlyCollectionEx<string>(ArrayEx.Empty<string>());
    }
}