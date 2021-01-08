#pragma warning disable CC0037 // Remove commented code.
#pragma warning disable S125 // Sections of code should not be commented out
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
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using TestRunner.AuxiliaryTypes;
using Theraot;

namespace TestRunner
{
    public static class AvailabilityTests
    {
        public static void MethodInfoMethodAvailability()
        {
            const MethodInfo methodInfo = null;
            No.Op<Func<Type, Delegate>>(methodInfo.CreateDelegate);
            No.Op<Func<Type, object, Delegate>>(methodInfo.CreateDelegate);
        }

        public static void SortedSetMethodAvailability()
        {
            const SortedSet<T> sortedSet = null;
            No.Op<Func<T, IEnumerable<T>>>(sortedSet.Append);
            No.Op<TryGetValue<T>>(sortedSet.TryGetValue);
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

        public static void TaskCompletionSourceMethodAvailability()
        {
            const TaskCompletionSource<int> intSource = null;
            No.Op<Func<bool>>(intSource.TrySetCanceled);
            No.Op<Func<CancellationToken, bool>>(intSource.TrySetCanceled);
            No.Op<Action>(intSource.SetCanceled);
            No.Op<Action<CancellationToken>>(intSource.SetCanceled);

            const TaskCompletionSource source = null;
            No.Op<Func<bool>>(source.TrySetCanceled);
            No.Op<Func<CancellationToken, bool>>(source.TrySetCanceled);
            No.Op<Action>(source.SetCanceled);
            No.Op<Action<CancellationToken>>(source.SetCanceled);
        }

        public static void ToStringMethodAvailability()
        {
            No.Op<Func<IFormatProvider, string>>(provider => default(bool).ToString(provider));
            No.Op<Func<IFormatProvider, string>>(provider => default(char).ToString(provider));
            No.Op<Func<IFormatProvider, string>>(default(string).ToString);
        }

        public static void TypeAvailability()
        {
            // System.Collections.Concurrent

            No.Op(typeof(BlockingCollection<T>));
            No.Op(typeof(ConcurrentBag<T>));
            No.Op(typeof(ConcurrentDictionary<TKey, TValue>));
            No.Op(typeof(ConcurrentQueue<T>));
            No.Op(typeof(ConcurrentStack<T>));
            No.Op(typeof(EnumerablePartitionerOptions));
            No.Op(typeof(IProducerConsumerCollection<T>));
            No.Op(typeof(OrderablePartitioner<TSource>));
            No.Op(typeof(Partitioner));
            No.Op(typeof(Partitioner<TSource>));

            // System.Collections.Generic

            No.Op(typeof(CollectionExtensions));
            No.Op(typeof(Comparer<T>));
            No.Op(typeof(Dictionary<TKey, TValue>));
            No.Op(typeof(EqualityComparer<T>));
            No.Op(typeof(HashSet<T>));
            No.Op(typeof(ICollection<T>));
            No.Op(typeof(IComparer<T>));
            No.Op(typeof(IDictionary<TKey, TValue>));
            No.Op(typeof(IEnumerable<T>));
            No.Op(typeof(IEnumerator<T>));
            No.Op(typeof(IEqualityComparer<T>));
            No.Op(typeof(IList<T>));
            No.Op(typeof(IReadOnlyCollection<T>));
            No.Op(typeof(IReadOnlyDictionary<TKey, TValue>));
            No.Op(typeof(IReadOnlyList<T>));
            No.Op(typeof(ISet<T>));
            No.Op(typeof(IReadOnlySet<T>));
            No.Op(typeof(KeyNotFoundException));
            No.Op(typeof(KeyValuePair));
            No.Op(typeof(KeyValuePair<TKey, TValue>));
            No.Op(typeof(LinkedList<T>));
            No.Op(typeof(LinkedListNode<T>));
            No.Op(typeof(List<T>));
            No.Op(typeof(Queue<T>));
            No.Op(typeof(SortedDictionary<TKey, TValue>));
            // No.Op(typeof(global::System.Collections.Generic.SortedList<int, int>));
            No.Op(typeof(SortedSet<T>));
            No.Op(typeof(Stack<T>));

            // System.Collections.ObjectModel

            No.Op(typeof(Collection<T>));
            No.Op(typeof(KeyedCollection<TKey, TItem>));
            No.Op(typeof(ObservableCollection<T>));
            No.Op(typeof(ReadOnlyCollection<T>));
            No.Op(typeof(ReadOnlyDictionary<TKey, TValue>));

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
            No.Op(typeof(Action<T>));
            No.Op(typeof(Action<T1, T2>));
            No.Op(typeof(Action<T1, T2, T3>));
            No.Op(typeof(Action<T1, T2, T3, T4>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6, T7>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>));
            No.Op(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>));

            No.Op(typeof(Func<TResult>));
            No.Op(typeof(Func<T, TResult>));
            No.Op(typeof(Func<T1, T2, TResult>));
            No.Op(typeof(Func<T1, T2, T3, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, T6, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, T6, T7, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>));
            No.Op(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>));

            No.Op(typeof(Tuple));
            No.Op(typeof(Tuple<T1>));
            No.Op(typeof(Tuple<T1, T2>));
            No.Op(typeof(Tuple<T1, T2, T3>));
            No.Op(typeof(Tuple<T1, T2, T3, T4>));
            No.Op(typeof(Tuple<T1, T2, T3, T4, T5>));
            No.Op(typeof(Tuple<T1, T2, T3, T4, T5, T6>));
            No.Op(typeof(Tuple<T1, T2, T3, T4, T5, T6, T7>));
            No.Op(typeof(Tuple<T1, T2, T3, T4, T5, T6, T7, T8>));

            No.Op(typeof(ValueTuple));
            No.Op(typeof(ValueTuple<T1>));
            No.Op(typeof(ValueTuple<T1, T2>));
            No.Op(typeof(ValueTuple<T1, T2, T3>));
            No.Op(typeof(ValueTuple<T1, T2, T3, T4>));
            No.Op(typeof(ValueTuple<T1, T2, T3, T4, T5>));
            No.Op(typeof(ValueTuple<T1, T2, T3, T4, T5, T6>));
            No.Op(typeof(ValueTuple<T1, T2, T3, T4, T5, T6, T7>));
            No.Op(typeof(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>));
            
#if LESSTHAN_NETSTANDARD11 && LESSTHAN_NET45

            No.Op(typeof(Span<T>));
            No.Op(typeof(ReadOnlySpan<T>));

#endif

            // System.Threading.Tasks.Extensions

            No.Op(typeof(AsyncMethodBuilderAttribute));
            No.Op(typeof(AsyncValueTaskMethodBuilder));
            No.Op(typeof(AsyncValueTaskMethodBuilder<TResult>));
            No.Op(typeof(ConfiguredValueTaskAwaitable));
            No.Op(typeof(ConfiguredValueTaskAwaitable<TResult>));
            No.Op(typeof(ValueTaskAwaiter));
            No.Op(typeof(ValueTaskAwaiter<TResult>));
            No.Op(typeof(ValueTask));
            No.Op(typeof(ValueTask<TResult>));
            No.Op(typeof(IValueTaskSource));
            No.Op(typeof(IValueTaskSource<TResult>));
            No.Op(typeof(ValueTaskSourceOnCompletedFlags));
            No.Op(typeof(ValueTaskSourceStatus));

            // Microsoft.Bcl.AsyncInterfaces
            No.Op(typeof(IAsyncDisposable));
            No.Op(typeof(IAsyncEnumerable<T>));
            No.Op(typeof(IAsyncEnumerator<T>));
            No.Op(typeof(AsyncIteratorMethodBuilder));
            No.Op(typeof(AsyncIteratorStateMachineAttribute));
            No.Op(typeof(ConfiguredAsyncDisposable));
            No.Op(typeof(ConfiguredCancelableAsyncEnumerable<T>));
            No.Op(typeof(EnumeratorCancellationAttribute));
            No.Op(typeof(TaskAsyncEnumerableExtensions));
            No.Op(typeof(ManualResetValueTaskSourceCore<TResult>));

            No.Op(typeof(CallInfo));
            // No.Op(typeof(global::System.Runtime.CompilerServices.Closure));
            No.Op(typeof(CodeAccessPermission));
            No.Op(typeof(ContractAbbreviatorAttribute));
            No.Op(typeof(DisplayAttribute));
            No.Op(typeof(DynamicAttribute));
            No.Op(typeof(EnumerableExecutor));
            No.Op(typeof(HostProtectionAttribute));
            No.Op(typeof(HostProtectionResource));
            No.Op(typeof(IInvokeOnGetBinder));
            No.Op(typeof(IUnrestrictedPermission));
            No.Op(typeof(PureAttribute));
            No.Op(typeof(ReliabilityContractAttribute));
            // No.Op(typeof(global::System.Runtime.CompilerServices.RuleCache<string>));
            No.Op(typeof(SecurityAction));
            No.Op(typeof(SecurityElement));
            No.Op(typeof(SecurityPermissionAttribute));
            No.Op(typeof(Task));
            No.Op(typeof(TupleElementNamesAttribute));
            No.Op(typeof(TupleExtensions));
            No.Op(typeof(ValueTuple));
            No.Op(typeof(Volatile));
            No.Op(typeof(ParallelOptions));
            No.Op(typeof(ContractFailedEventArgs));
            No.Op(typeof(ApplicationException));
            No.Op(typeof(ConditionalWeakTable<TKey, TValue>));
            No.Op(typeof(BigInteger));
            No.Op(typeof(Thread));
            No.Op(typeof(ThreadPool));
            No.Op(typeof(Timer));
            No.Op(typeof(SemaphoreSlim));
            No.Op(typeof(BindingFlags));
            No.Op(typeof(WaitCallback));
            No.Op(typeof(TypeCode));
            No.Op(typeof(Converter<TInput, TOutput>));
            No.Op(typeof(Queryable));
            No.Op(typeof(BitOperations));
            No.Op(typeof(HashCode));
            No.Op(typeof(FormattableString));
            No.Op(typeof(FormattableStringFactory));
        }

        public static void TypeMethodAvailability()
        {
            No.Op<Func<Type, TypeCode>>(TypeEx.GetTypeCode);
            No.Op(TypeEx.EmptyTypes);
            const Type type = null;
            No.Op<Func<Type[], ConstructorInfo>>(type.GetConstructor);
            No.Op<Func<ConstructorInfo[]>>(type.GetConstructors);
            No.Op<Func<BindingFlags, ConstructorInfo[]>>(type.GetConstructors);
            No.Op<Func<MemberInfo[]>>(type.GetDefaultMembers);
            No.Op<Func<string, EventInfo>>(type.GetEvent);
            No.Op<Func<string, BindingFlags, EventInfo>>(type.GetEvent);
            No.Op<Func<EventInfo[]>>(type.GetEvents);
            No.Op<Func<BindingFlags, EventInfo[]>>(type.GetEvents);
            No.Op<Func<string, FieldInfo>>(type.GetField);
            No.Op<Func<string, BindingFlags, FieldInfo>>(type.GetField);
            No.Op<Func<FieldInfo[]>>(type.GetFields);
            No.Op<Func<BindingFlags, FieldInfo[]>>(type.GetFields);
            No.Op<Func<Type[]>>(type.GetGenericArguments);
            No.Op<Func<Type[]>>(type.GetInterfaces);
            No.Op<Func<string, MemberInfo[]>>(type.GetMember);
            No.Op<Func<string, BindingFlags, MemberInfo[]>>(type.GetMember);
            No.Op<Func<MemberInfo[]>>(type.GetMembers);
            No.Op<Func<BindingFlags, MemberInfo[]>>(type.GetMembers);
            No.Op<Func<string, Type[], MethodInfo>>(type.GetMethod);
            No.Op<Func<string, MethodInfo>>(type.GetMethod);
            No.Op<Func<string, BindingFlags, MethodInfo>>(type.GetMethod);
            No.Op<Func<MethodInfo[]>>(type.GetMethods);
            No.Op<Func<BindingFlags, MethodInfo[]>>(type.GetMethods);
            No.Op<Func<string, BindingFlags, Type>>(type.GetNestedType);
            No.Op<Func<BindingFlags, Type[]>>(type.GetNestedTypes);
            No.Op<Func<PropertyInfo[]>>(type.GetProperties);
            No.Op<Func<BindingFlags, PropertyInfo[]>>(type.GetProperties);
            No.Op<Func<string, Type, PropertyInfo>>(type.GetProperty);
            No.Op<Func<string, Type, Type[], PropertyInfo>>(type.GetProperty);
            No.Op<Func<string, PropertyInfo>>(type.GetProperty);
            No.Op<Func<string, BindingFlags, PropertyInfo>>(type.GetProperty);
            No.Op<Func<Type, bool>>(type.IsAssignableFrom);
            No.Op<Func<object, bool>>(type.IsInstanceOfType);
        }
    }
}