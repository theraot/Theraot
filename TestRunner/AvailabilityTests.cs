#pragma warning disable CC0037 // Remove commented code.
// ReSharper disable StyleCop.SA1512
// ReSharper disable StyleCop.SA1515

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using Theraot;

namespace TestRunner
{
    public static class AvailabilityTests
    {
        public static void TypeAvailability()
        {
            // System.Collections.Concurrent

            No.Op(typeof(BlockingCollection<int>));
            No.Op(typeof(ConcurrentBag<int>));
            No.Op(typeof(ConcurrentDictionary<int, int>));
            No.Op(typeof(ConcurrentQueue<int>));
            No.Op(typeof(ConcurrentStack<int>));
            No.Op(typeof(EnumerablePartitionerOptions));
            No.Op(typeof(IProducerConsumerCollection<int>));
            No.Op(typeof(OrderablePartitioner<int>));
            No.Op(typeof(Partitioner));
            No.Op(typeof(Partitioner<int>));

            // System.Collections.Generic

            No.Op(typeof(CollectionExtensions));
            No.Op(typeof(Comparer<int>));
            No.Op(typeof(Dictionary<int, int>));
            No.Op(typeof(EqualityComparer<int>));
            No.Op(typeof(HashSet<int>));
            No.Op(typeof(ICollection<int>));
            No.Op(typeof(IComparer<int>));
            No.Op(typeof(IDictionary<int, int>));
            No.Op(typeof(IEnumerable<int>));
            No.Op(typeof(IEnumerator<int>));
            No.Op(typeof(IEqualityComparer<int>));
            No.Op(typeof(IList<int>));
            No.Op(typeof(IReadOnlyCollection<int>));
            No.Op(typeof(IReadOnlyDictionary<int, int>));
            No.Op(typeof(IReadOnlyList<int>));
            No.Op(typeof(ISet<int>));
            No.Op(typeof(KeyNotFoundException));
            No.Op(typeof(KeyValuePair<int, int>));
            No.Op(typeof(LinkedList<int>));
            No.Op(typeof(LinkedListNode<int>));
            No.Op(typeof(List<int>));
            No.Op(typeof(Queue<int>));
            No.Op(typeof(SortedDictionary<int, int>));
            // No.Op(typeof(global::System.Collections.Generic.SortedList<int, int>));
            No.Op(typeof(SortedSet<int>));
            No.Op(typeof(Stack<int>));

            // System.Collections.ObjectModel

            No.Op(typeof(Collection<int>));
            No.Op(typeof(KeyedCollection<int, int>));
            No.Op(typeof(ObservableCollection<int>));
            No.Op(typeof(ReadOnlyCollection<int>));
            No.Op(typeof(ReadOnlyDictionary<int, int>));

            // System.Collections.Specialized

            // No.Op(typeof(global::System.Collections.Specialized.BitVector32));
            // No.Op(typeof(global::System.Collections.Specialized.CollectionsUtil));
            // No.Op(typeof(global::System.Collections.Specialized.HybridDictionary));
            No.Op(typeof(INotifyCollectionChanged));
            // No.Op(typeof(global::System.Collections.Specialized.IOrderedDictionary));
            // No.Op(typeof(global::System.Collections.Specialized.ListDictionary));
            // No.Op(typeof(global::System.Collections.Specialized.NameObjectCollectionBase));
            // No.Op(typeof(global::System.Collections.Specialized.NameValueCollection));
            No.Op(typeof(NotifyCollectionChangedAction));
            No.Op(typeof(NotifyCollectionChangedEventArgs));
            No.Op(typeof(NotifyCollectionChangedEventHandler));
            // No.Op(typeof(global::System.Collections.Specialized.OrderedDictionary));
            // No.Op(typeof(global::System.Collections.Specialized.StringCollection));
            // No.Op(typeof(global::System.Collections.Specialized.StringDictionary));
            // No.Op(typeof(global::System.Collections.Specialized.StringEnumerator));

            // System.Collections

            No.Op(typeof(ArrayList));
            No.Op(typeof(BitArray));
            // No.Op(typeof(global::System.Collections.CaseInsensitiveComparer));
            // No.Op(typeof(global::System.Collections.CaseInsensitiveHashCodeProvider));
            // No.Op(typeof(global::System.Collections.CollectionBase));
            No.Op(typeof(Comparer));
            // No.Op(typeof(global::System.Collections.DictionaryBase));
            No.Op(typeof(DictionaryEntry));
            No.Op(typeof(Hashtable));
            No.Op(typeof(ICollection));
            No.Op(typeof(IComparer));
            No.Op(typeof(IDictionary));
            No.Op(typeof(IDictionaryEnumerator));
            No.Op(typeof(IEnumerable));
            No.Op(typeof(IEnumerator));
            No.Op(typeof(IEqualityComparer));
            // No.Op(typeof(global::System.Collections.IHashCodeProvider));
            No.Op(typeof(IList));
            No.Op(typeof(IStructuralComparable));
            No.Op(typeof(IStructuralEquatable));
            // No.Op(typeof(global::System.Collections.Queue));
            // No.Op(typeof(global::System.Collections.ReadOnlyCollectionBase));
            No.Op(typeof(SortedList));
            // No.Op(typeof(global::System.Collections.Stack));
            No.Op(typeof(StructuralComparisons));

            // System

            No.Op(typeof(Action));
            No.Op(typeof(Action<int>));
            No.Op(typeof(Action<int, int>));
            No.Op(typeof(Action<int, int, int>));
            No.Op(typeof(Action<int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Action<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>));

            No.Op(typeof(Func<int>));
            No.Op(typeof(Func<int, int>));
            No.Op(typeof(Func<int, int, int>));
            No.Op(typeof(Func<int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>));
            No.Op(typeof(Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>));

            No.Op(typeof(Tuple));
            No.Op(typeof(Tuple<int>));
            No.Op(typeof(Tuple<int, int>));
            No.Op(typeof(Tuple<int, int, int>));
            No.Op(typeof(Tuple<int, int, int, int>));
            No.Op(typeof(Tuple<int, int, int, int, int>));
            No.Op(typeof(Tuple<int, int, int, int, int, int>));
            No.Op(typeof(Tuple<int, int, int, int, int, int, int>));
            No.Op(typeof(Tuple<int, int, int, int, int, int, int, int>));

            No.Op(typeof(CallInfo));
            // No.Op(typeof(global::System.Runtime.CompilerServices.Closure));
            No.Op(typeof(CodeAccessPermission));
            No.Op(typeof(ContractAbbreviatorAttribute));
            No.Op(typeof(DisplayAttribute));
            No.Op(typeof(DynamicAttribute));
            No.Op(typeof(EnumerableExecutor));
            No.Op(typeof(HostProtectionAttribute));
            No.Op(typeof(HostProtectionResource));
            No.Op(typeof(IFormatterConverter));
            No.Op(typeof(IInvokeOnGetBinder));
            No.Op(typeof(IUnrestrictedPermission));
            No.Op(typeof(PureAttribute));
            No.Op(typeof(ReliabilityContractAttribute));
            // No.Op(typeof(global::System.Runtime.CompilerServices.RuleCache<string>));
            No.Op(typeof(SecurityAction));
            No.Op(typeof(SecurityElement));
            No.Op(typeof(SecurityPermissionAttribute));
            No.Op(typeof(SerializableAttribute));
            No.Op(typeof(SerializationInfo));
            No.Op(typeof(StreamingContext));
            No.Op(typeof(Task));
            No.Op(typeof(TupleElementNamesAttribute));
            No.Op(typeof(TupleExtensions));
            No.Op(typeof(ValueTask<int>));
            No.Op(typeof(ValueTuple));
            No.Op(typeof(Volatile));
            No.Op(typeof(ParallelOptions));
            No.Op(typeof(ContractFailedEventArgs));
            No.Op(typeof(ApplicationException));
            No.Op(typeof(ConditionalWeakTable<object, object>));
            No.Op(typeof(BigInteger));
            No.Op(typeof(Thread));
            No.Op(typeof(ThreadPool));
            No.Op(typeof(Timer));
            No.Op(typeof(SemaphoreSlim));
            No.Op(typeof(BindingFlags));
            No.Op(typeof(WaitCallback));
            No.Op(typeof(TypeCode));
        }

        public static void StringMethodAvailability()
        {
            No.Op<Func<string, string[], int, int, string>>(string.Join);
            No.Op<Func<string, string[], string>>(string.Join);
            No.Op<Func<string, object[], string>>(StringEx.Join);
            No.Op<Func<string, IEnumerable<string>, string>>(StringEx.Join);
            No.Op<Func<string, IEnumerable<int>, string>>(StringEx.Join);
            No.Op<Func<IEnumerable<int>, string>>(StringEx.Concat);
            No.Op<Func<object, string>>(string.Concat);
            No.Op<Func<object, object, string>>(string.Concat);
            No.Op<Func<object, object, object, string>>(string.Concat);
            No.Op<Func<string, bool>>(StringEx.IsNullOrWhiteSpace);
        }

        public static void StreamMethodAvailability()
        {
            // ReSharper disable once RedundantAssignment
            var stream = Stream.Null;
            No.Op<Action<Stream>>(stream.CopyTo);
            No.Op<Action<Stream, int>>(stream.CopyTo);
            No.Op<Func<Stream, Task>>(stream.CopyToAsync);
            No.Op<Func<Stream, int, Task>>(stream.CopyToAsync);
            No.Op<Func<Stream, int, CancellationToken, Task>>(stream.CopyToAsync);
            No.Op<Func<Task>>(stream.FlushAsync);
            No.Op<Func<CancellationToken, Task>>(stream.FlushAsync);
            No.Op<Func<byte[], int, int, Task<int>>>(stream.ReadAsync);
            No.Op<Func<byte[], int, int, CancellationToken, Task<int>>>(stream.ReadAsync);
            No.Op<Func<byte[], int, int, Task>>(stream.WriteAsync);
            No.Op<Func<byte[], int, int, CancellationToken, Task>>(stream.WriteAsync);
            No.Op(stream);
        }

        public static void MethodInfoMethodAvailability()
        {
            const MethodInfo methodInfo = null;
            No.Op<Func<Type, Delegate>>(methodInfo.CreateDelegate);
            No.Op<Func<Type, object, Delegate>>(methodInfo.CreateDelegate);
        }

        public static void TypeMethodAvailability()
        {
            No.Op<Func<Type, TypeCode>>(TypeEx.GetTypeCode);
            No.Op(TypeEx.EmptyTypes);
        }

        public static void ToStringMethodAvailability()
        {
            No.Op<Func<IFormatProvider, string>>(provider => default(bool).ToString(provider));
            No.Op<Func<IFormatProvider, string>>(provider => default(char).ToString(provider));
            No.Op<Func<IFormatProvider, string>>(default(string).ToString);
        }
    }
}