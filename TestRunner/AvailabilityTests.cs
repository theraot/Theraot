using Theraot;

#pragma warning disable CC0037 // Remove commented code.

namespace TestRunner
{
    public static class AvailabilityTests
    {
        public static void TypeAvailability()
        {
            // System.Collections.Concurrent

            No.Op(typeof(global::System.Collections.Concurrent.BlockingCollection<int>));
            No.Op(typeof(global::System.Collections.Concurrent.ConcurrentBag<int>));
            No.Op(typeof(global::System.Collections.Concurrent.ConcurrentDictionary<int, int>));
            No.Op(typeof(global::System.Collections.Concurrent.ConcurrentQueue<int>));
            No.Op(typeof(global::System.Collections.Concurrent.ConcurrentStack<int>));
            No.Op(typeof(global::System.Collections.Concurrent.EnumerablePartitionerOptions));
            No.Op(typeof(global::System.Collections.Concurrent.IProducerConsumerCollection<int>));
            No.Op(typeof(global::System.Collections.Concurrent.OrderablePartitioner<int>));
            No.Op(typeof(global::System.Collections.Concurrent.Partitioner));
            No.Op(typeof(global::System.Collections.Concurrent.Partitioner<int>));
            
            // System.Collections.Generic

            No.Op(typeof(global::System.Collections.Generic.CollectionExtensions));
            No.Op(typeof(global::System.Collections.Generic.Comparer<int>));
            No.Op(typeof(global::System.Collections.Generic.Dictionary<int, int>));
            No.Op(typeof(global::System.Collections.Generic.EqualityComparer<int>));
            No.Op(typeof(global::System.Collections.Generic.HashSet<int>));
            No.Op(typeof(global::System.Collections.Generic.ICollection<int>));
            No.Op(typeof(global::System.Collections.Generic.IComparer<int>));
            No.Op(typeof(global::System.Collections.Generic.IDictionary<int, int>));
            No.Op(typeof(global::System.Collections.Generic.IEnumerable<int>));
            No.Op(typeof(global::System.Collections.Generic.IEnumerator<int>));
            No.Op(typeof(global::System.Collections.Generic.IEqualityComparer<int>));
            No.Op(typeof(global::System.Collections.Generic.IList<int>));
            No.Op(typeof(global::System.Collections.Generic.IReadOnlyCollection<int>));
            No.Op(typeof(global::System.Collections.Generic.IReadOnlyDictionary<int, int>));
            No.Op(typeof(global::System.Collections.Generic.IReadOnlyList<int>));
            No.Op(typeof(global::System.Collections.Generic.ISet<int>));
            No.Op(typeof(global::System.Collections.Generic.KeyNotFoundException));
            No.Op(typeof(global::System.Collections.Generic.KeyValuePair<int, int>));
            No.Op(typeof(global::System.Collections.Generic.LinkedList<int>));
            No.Op(typeof(global::System.Collections.Generic.LinkedListNode<int>));
            No.Op(typeof(global::System.Collections.Generic.List<int>));
            No.Op(typeof(global::System.Collections.Generic.Queue<int>));
            No.Op(typeof(global::System.Collections.Generic.SortedDictionary<int, int>));
            // No.Op(typeof(global::System.Collections.Generic.SortedList<int, int>));
            No.Op(typeof(global::System.Collections.Generic.SortedSet<int>));
            No.Op(typeof(global::System.Collections.Generic.Stack<int>));
            
            // System.Collections.ObjectModel

            No.Op(typeof(global::System.Collections.ObjectModel.Collection<int>));
            No.Op(typeof(global::System.Collections.ObjectModel.KeyedCollection<int, int>));
            No.Op(typeof(global::System.Collections.ObjectModel.ObservableCollection<int>));
            No.Op(typeof(global::System.Collections.ObjectModel.ReadOnlyCollection<int>));
            No.Op(typeof(global::System.Collections.ObjectModel.ReadOnlyDictionary<int, int>));
            
            // System.Collections.Specialized

            // No.Op(typeof(global::System.Collections.Specialized.BitVector32));
            // No.Op(typeof(global::System.Collections.Specialized.CollectionsUtil));
            // No.Op(typeof(global::System.Collections.Specialized.HybridDictionary));
            No.Op(typeof(global::System.Collections.Specialized.INotifyCollectionChanged));
            // No.Op(typeof(global::System.Collections.Specialized.IOrderedDictionary));
            // No.Op(typeof(global::System.Collections.Specialized.ListDictionary));
            // No.Op(typeof(global::System.Collections.Specialized.NameObjectCollectionBase));
            // No.Op(typeof(global::System.Collections.Specialized.NameValueCollection));
            No.Op(typeof(global::System.Collections.Specialized.NotifyCollectionChangedAction));
            No.Op(typeof(global::System.Collections.Specialized.NotifyCollectionChangedEventArgs));
            No.Op(typeof(global::System.Collections.Specialized.NotifyCollectionChangedEventHandler));
            // No.Op(typeof(global::System.Collections.Specialized.OrderedDictionary));
            // No.Op(typeof(global::System.Collections.Specialized.StringCollection));
            // No.Op(typeof(global::System.Collections.Specialized.StringDictionary));
            // No.Op(typeof(global::System.Collections.Specialized.StringEnumerator));
            
            // System.Collections

            No.Op(typeof(global::System.Collections.ArrayList));
            No.Op(typeof(global::System.Collections.BitArray));
            // No.Op(typeof(global::System.Collections.CaseInsensitiveComparer));
            // No.Op(typeof(global::System.Collections.CaseInsensitiveHashCodeProvider));
            // No.Op(typeof(global::System.Collections.CollectionBase));
            No.Op(typeof(global::System.Collections.Comparer));
            // No.Op(typeof(global::System.Collections.DictionaryBase));
            No.Op(typeof(global::System.Collections.DictionaryEntry));
            No.Op(typeof(global::System.Collections.Hashtable));
            No.Op(typeof(global::System.Collections.ICollection));
            No.Op(typeof(global::System.Collections.IComparer));
            No.Op(typeof(global::System.Collections.IDictionary));
            No.Op(typeof(global::System.Collections.IDictionaryEnumerator));
            No.Op(typeof(global::System.Collections.IEnumerable));
            No.Op(typeof(global::System.Collections.IEnumerator));
            No.Op(typeof(global::System.Collections.IEqualityComparer));
            // No.Op(typeof(global::System.Collections.IHashCodeProvider));
            No.Op(typeof(global::System.Collections.IList));
            No.Op(typeof(global::System.Collections.IStructuralComparable));
            No.Op(typeof(global::System.Collections.IStructuralEquatable));
            // No.Op(typeof(global::System.Collections.Queue));
            // No.Op(typeof(global::System.Collections.ReadOnlyCollectionBase));
            No.Op(typeof(global::System.Collections.SortedList));
            // No.Op(typeof(global::System.Collections.Stack));
            No.Op(typeof(global::System.Collections.StructuralComparisons));
            
            // System

            No.Op(typeof(global::System.Action));
            No.Op(typeof(global::System.Action<int>));
            No.Op(typeof(global::System.Action<int, int>));
            No.Op(typeof(global::System.Action<int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>));

            No.Op(typeof(global::System.Func<int>));
            No.Op(typeof(global::System.Func<int, int>));
            No.Op(typeof(global::System.Func<int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>));

            No.Op(typeof(global::System.Tuple));
            No.Op(typeof(global::System.Tuple<int>));
            No.Op(typeof(global::System.Tuple<int, int>));
            No.Op(typeof(global::System.Tuple<int, int, int>));
            No.Op(typeof(global::System.Tuple<int, int, int, int>));
            No.Op(typeof(global::System.Tuple<int, int, int, int, int>));
            No.Op(typeof(global::System.Tuple<int, int, int, int, int, int>));
            No.Op(typeof(global::System.Tuple<int, int, int, int, int, int, int>));
            No.Op(typeof(global::System.Tuple<int, int, int, int, int, int, int, int>));
            
            No.Op(typeof(global::System.Dynamic.CallInfo));
            // No.Op(typeof(global::System.Runtime.CompilerServices.Closure));
            No.Op(typeof(global::System.Security.CodeAccessPermission));
            No.Op(typeof(global::System.Diagnostics.Contracts.ContractAbbreviatorAttribute));
            No.Op(typeof(global::System.ComponentModel.DataAnnotations.DisplayAttribute));
            No.Op(typeof(global::System.Runtime.CompilerServices.DynamicAttribute));
            No.Op(typeof(global::System.Linq.EnumerableExecutor));
            No.Op(typeof(global::System.Security.Permissions.HostProtectionAttribute));
            No.Op(typeof(global::System.Security.Permissions.HostProtectionResource));
            No.Op(typeof(global::System.Runtime.Serialization.IFormatterConverter));
            No.Op(typeof(global::System.Dynamic.IInvokeOnGetBinder));
            No.Op(typeof(global::System.Security.Permissions.IUnrestrictedPermission));
            No.Op(typeof(global::System.Diagnostics.Contracts.PureAttribute));
            No.Op(typeof(global::System.Runtime.ConstrainedExecution.ReliabilityContractAttribute));
            // No.Op(typeof(global::System.Runtime.CompilerServices.RuleCache<string>));
            No.Op(typeof(global::System.Security.Permissions.SecurityAction));
            No.Op(typeof(global::System.Security.SecurityElement));
            No.Op(typeof(global::System.Security.Permissions.SecurityPermissionAttribute));
            No.Op(typeof(global::System.SerializableAttribute));
            No.Op(typeof(global::System.Runtime.Serialization.SerializationInfo));
            No.Op(typeof(global::System.Runtime.Serialization.StreamingContext));
            No.Op(typeof(global::System.Threading.Tasks.Task));
            No.Op(typeof(global::System.Runtime.CompilerServices.TupleElementNamesAttribute));
            No.Op(typeof(global::System.TupleExtensions));
            No.Op(typeof(global::System.Threading.Tasks.ValueTask<int>));
            No.Op(typeof(global::System.ValueTuple));
            No.Op(typeof(global::System.Threading.Volatile));
        }
    }
}