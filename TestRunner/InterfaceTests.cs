using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TestRunner.AuxiliaryTypes;
using Theraot.Collections;

namespace TestRunner
{
    internal static class InterfaceTests
    {
        public static readonly IReadOnlyDictionary<TKey, TValue> DictionaryExAsIReadOnlyDictionary = new DictionaryEx<TKey, TValue>();
        public static readonly IReadOnlyList<T> EmptyCollectionAsIReadOnlyCollection = EmptyCollection<T>.Instance;
        public static readonly IReadOnlyCollection<T> HashSetExAsIReadOnlyCollection = new HashSetEx<T>();
        public static readonly ISet<T> HashSetExAsISet = new HashSetEx<T>();
        public static readonly IReadOnlyCollection<T> ListExAsReadOnlyAsIReadOnlyCollection = (new ListEx<T>()).AsReadOnly();
        public static readonly IReadOnlyList<T> ListExAsReadOnlyAsIReadOnlyList = (new ListEx<T>()).AsReadOnly();
        public static readonly IReadOnlyList<T> ListExAsReadOnlyList = new ListEx<T>();
        public static readonly IReadOnlyList<T> ReadOnlyCollectionExAsIReadOnlyList = new ReadOnlyCollectionEx<T>(ArrayEx.Empty<T>());
    }
}