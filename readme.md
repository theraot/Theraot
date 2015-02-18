Theraot's Libraries
===

Theraot's Libraries are an ongoing effort to ease the work on .NET, including a backport of recent .NET features to .NET 2.0 among other things (I would like to highlight In Memory Transactions, Needles [See "Needle" below] and Wait Free data structures).

---
Introduction
---

Theraot.Core is as close as I am to a ".NET Compatibility Pack" capable to bring code from recent .NET versions back to .NET 2.0.

Although this project it is not just a "Compatibility Pack" because it adds additional classes and methods in a separate namespace, this additional code is used internally to implement the backport code.

Currently there is no documentation, although any behavior that differs from what the BCL does can be considered a bug.

My libraries are and will probably always be work in progress... please, do not hesitate to report bugs and request features.

---
Binary Downloads
---

The binary downloads are available from: http://www.4shared.com/folder/o2pF-8Oe/Theraot.html

Note: I publish new binary downloads "sparsely", if you want a build of a particular revision, you can request it to me.

---
Features
---

Theraot's Libraries...

  - can be built for .NET 2.0, 3.0, 3.5, 4.0 and 4.5 with the help of conditional compilation to keep only the required code for the particular version.
  - includes lock-free and wait-free structures developed in `HashBucket` (another project of mine).
  - includes (among others) the following types to be used in old versions of .NET back to .NET 2.0:
    - `System.Collections.Concurrent.ConcurrentDictionary<TKey, TValue>`: Done
    - `System.Collections.Concurrent...`: Pending
    - `System.Collections.Generic.HashSet<T>`: Done [See Note 1]
    - `System.Collections.Generic.SortedSet<T>`: Done
    - `System.Collections.Linq`: Up to .NET 3.5
    - `System.Collections.Linq.Expressions`: Up to .NET 3.5
    - `System.Collections.ObjectModel.ObservableCollection<T>`: Work in progress
    - `System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>`: Done
    - `System.Collections.StructuralComparisons`: Done
    - `System.Numerics.BigInteger`: Done [Taken from Mono][See Note 2]
    - `System.Numerics.Complex`: Done [Taken from Mono]
    - `System.Runtime.CompilerServices.DynamicAttribute`: Done
    - `System.Runtime.CompilerServices.ExtensionAttribute`: Done
    - `System.Threading.Tasks`: Work in progress
    - `System.Therading.CancellationToken`: Done
    - `System.Therading.CountdownEvent`: Done
    - `System.Therading.ManualResetEventSlim`: Done
    - `System.Therading.SpinWait`: Done
    - `System.Therading.ThreadLocal<T>`: Done
    - `System.Therading.Volatile`: Done
    - `System.Action<*>`: Done
    - `System.AggregateException`: Done
    - `System.Func<*>`: Done
    - `System.IObservable<T>`: Done
    - `System.IObserver<T>`: Done
    - `System.Progress<T>`: Done
    - `System.Lazy<T>` & `System.Lazy<T, TMetadata>`: Done
    - `System.Tuple<*>`: Done
    - `System.WeakReference<T>`: Done
  - Uses less than 1MB in disk.
  - Keeps a consistent code style in the whole code [See Note 3]

Note 1: `HashSet<T>` is available in .NET 3.5 and even though Theraot.Core adds `ISet<T>` it wont cast to it on .NET 3.5.

Note 2: I have provided the optimization for the cast from `System.Numerics.BigInteger` to float and double. I have contributed code to Mono.

Note 3: I intent to keep the code readable, yet documentation is low priority at this point. 

---
**Extended Features**

Not everything in the code is a backport, some parts are utility and helper methods that has been developed along side the backport effort.

This are some parts worth of mention:

  - `Theraot.Collections`
    - `Specialized`
      - `AVLTree<TKey, TValue>`: A binary sorted and balanced tree.
      - `NullAwareDictionary<TKey, TValue>`: A dictionary that can store a null key.
    - `ThreadSafe`
      - `Bucket<T>`: A fixed-size wait-free collection.
      - `FixedSizeQueue<T>`: A fixed-size wait-free queue.
      - `SafeDictionary<TKey, TValue>`: A wait-free hash based dictionary.
      - `SafeSet<T>`: A wait-free hash based set.
      - `SafeQueuet<T>`: A wait-free queue.
      - `NeedleBucket<T, TNeedle>`: A fixed-size wait-free collection of a given Needle type.
      - `WeakDelegateSet`: A lock-free set of weak references to delegates.
      - `WeakEvent<TEventArgs>`: A weak event implementation.
      - `WeakSetBucket<T, TNeedle>`: A lock-free set of weak references.
    - `Extensions`: A huge plus beyond Linq.
    - `Progressive*<*>`: A set of classes that walk an `IEnumerable<T>` on demand and cache the result.
  - `Theraot.Core`
    - `ActionHelper`: A helper class with Lazy static Noop and Throw Actions.
    - `ComparerExtensions`: A helper class with Extension Methods for `IComparer<T>`
    - `EnumHelper`: A helper class for enums.
    - `EqualityComparerHelper<T>`: A helper class to create `IEqualityComparer<T>` for multiple types. [See Note 1]
    - `FuncHelper`: A helper class with Lazy static Default, Return and Throw Funcs.
    - `NumericHelper`: A helper class with methods from integer square root to primality test including extracting mantissa and exp from double value and more.
    - `StringHelper`: A helper class with methods from Append to Implode and more.
    - `StreamExtensions`: A helper class with Extension Methods for Stream.
    - `OrderedEnumerable`: A custom ordered Enumerable.
    - `TypeHelper`: A helper class with A bunch of type related helper methods.
  - `Theraot.Threading`
    - `Needles`: [See "Needle" below]
    - `Disposable`: A general purpose disposable object with Action callback.
    - `DisposableAkin`: Same as above, but disposable only by the thread that created it. [See Note 2]
    - `GCMonitor`: Allows to get notifications on Garbage Collection. [See Note 3]
    - `IExtendedDisposable`: A `IDisposible` that you can query to know if it was disposed. [See Note 4]
    - `NoTrackingThreadLocal<T>` & `TrackingThreadLocal<T>`: The backends for the backport of `System.Threading.ThreadLocal`.
    - `ReentryGuard`: A helper class that allows to protect a code from reentry.
    - `SingleTimeExecution`: A thread-safe way to wrap code to be called only once.
    - `ThreadinHelper`: Provides unique ids for managed threads, generic `VolatileRead` and *Write, and conditional `SpinWait`. [See Note 5]
    - `Work`: a task scheduler implementation, intended to be part of the backport of `System.Threading.Tasks`.

Note 1: `EqualityComparerHelper<T>` creates equality comparers for delegates, tuples, and Needles [See "Needle" below] among others. This facilitates their use as keys in dictionaries.

Note 2: Restricting `DisposableAkin` to only the same thread actually makes the implementation simpler and more efficient.

Note 3: The notifications of `GCMonitor` will run in a dedicated thread, make sure to not waste it's time. It is strongly suggested to use it to start asynchronous operations.

Note 4: Do not rely on the `IsDisposed` property as it may change any moment if another thread call `Dispose`, use `DisposedConditional` instead.

Note 5: The `SpinWait` methods in `ThreadingHelper` provides alternatives to `System.Threading.SpinWait`.

---
**Needles**

A needle is concept similar to "handle", "pointer", and "reference". A needle is an object that can be used to access a value.

The first needle is just `Needle<T>` and it is an strong reference. If you prefer a `struct` you should use StructNeedle<T>.

As counterpart of Needle<T> there is WeakNeedle<T> which is a weak reference.

LazyNeedle<T> and CacheNeedle<T> are the Lazy counterparts to Needle<T> and WeakNeedle<T> respectively.

This lazy initializable needles are `IPromise<T>` which represents needle that will be available in the future. Among those:
  - `PromiseNeedle<T>` is a needle that is initialized on demand by the process that created the object.
  - `FutureNeedle<T>` is a needle that is initialized on a asynchronous operation.

Finally, `Transact` and `Transact.Needle<T>` are used to create memory transaction [See below].

Other needles include: `NotNull<T>` and `WeakDelegateNeedle<T>`.

---
**Lockable Needles**

`LockableNeedle<T>` is only included in "FAT" builds. It provides functionality halfway between a `ReadWriteLock` and a transaction.

To modify a `LockableNeedle<T>` it is required to enter the `LockableContext` that was used to create the `LockableNeedle<T>`. Any other thread attempting to modify the `LockableNeedle<T>` will wait until the current thread leaves the `LockableNeedle<T>`.

There is no restriction to read `LockableNeedle<T>`, it will be an atomic operation if `T` is an atomic type (all classes and any primitive types with size not greater than that of IntPrt).

---
**Transactions**

Transactions are only included in "FAT" builds. Using them is easy, here is a simple example:

    var needle = Transact.CreateNeedle(1);

    using (var transaction = new Transact())
    {
        needle.Value = 2;
        transaction.Commit();
    }

Nesting transactions are also supported, refer to the tests for complex use cases.

They only work on `Transact.Needle`, disposing the Transact releases all changes done using it, which is equivalent to a rollback to the las time `Commit` was called.

If the value of any of the used `Transact.Needle` has changed by third party after it was readed under a transaction and before calling `Commit` then the call will fail (returns false).

The system requires a way to clone any type used in the `Transact.Needle`. The default clone behavior is as follows:

- value types are supported by default.
- if `IClonalbe` or `IClonable<T>` (added in FAT builds) are available they are used.
- if none of these are available the system will use serialization instead.

Otherwise provide an `ICloner<T>` (added in FAT builds), if one is provided it takes preference over default clone behavior. To get an `ICloner<T>` with the default behavior call `CloneHelper<T>.GetCloner()`.

It is also possible to explicitly call `Rollback` which will release all changes up to the last call to Commit. Multiple transactions over different needles can happen simultaneously without locking or waiting.

The system supports up to 512 threads attempting to Commit simultaneously per AppDomain, any extra thread calling `Commit` will wait. When multiple threads are trying to Commit on the same needles, it is guaranteed that at least one will succeed.

---
**Works**

Works are only included in "FAT" builds, and are a prototype for Task.

Currently Works require a `WorkContext` that serves as thread pool. A default `WorkContext` is provided with as many dedotated threads as `Environment.ProcessorCount`.

It is possible to wait a `Work` to complete, while doing so, the waiting thread donates it's time to execute pending works.

---
Notes
---

There are a few things that are beyond the scope of my work:

  - I cannot provide Generic Variance.
  - I cannot extend reflection (I recommend to use [Mono.Cecil](https://github.com/jbevain/cecil))
  - I cannot add some methods such as `String.Join(string, IEnumerator<string>)`, I'll provide helper methods instead. Whenever possible this helper methods are going to extension methods to minimize the changes needed in code. For a list see below.
  - I cannot improve the Garbage Collector. Try using Mono as a back-end.
  - I will not include backports of Reactive Extensions or any other code not in the BCL, but I may provide similar functionality.
  - I have no intention to backport GUI libraries.
  - I cannot modify `EventHandler<T>` to remove the generic constraint that's present in .NET prior .NET 4.5. For workaround see below.
  - I cannot modify `OperationCanceledException` to add support to `CancellationToken`. For workaround see below.
  - I cannot modify `HashSet<T>` on .NET 3.5 to allow casting to `ISet<T>`
  - In the type `IGrouping<TKey, TElement>` `TKey` is covariant from .NET 4.0 onward, I cannot make it covariant in .NET 3.5.

These features are not planned to be added or improved:

  - `Environment.SpecialFolder`
  - `Environment.Is64BitProcess`
  - `Environment.Is64BitOperatingSystem`
  - `IntPtr` and `UIntPtr` Addition and Subtraction
  - `ServiceInstaller.DelayedAutoStart`

The following are the notable methods that has been added to existing types:

  - `Comparer<T>.Create` was added in .NET 4.5, use `ComparerExtensions.ToComparer` instead.
  - `Enum.HasFlag` was added in .NET 4.0, use `EnumHelper.HasFlag` instead.
  - `Stream.CopyTo` was added in .NET 4.0, use `StreamExtensions.CopyTo` instead.
  - `String.IsNullOrWhiteSpace` was added in .NET 4.0, use `StringHelper.IsNullOrWhiteSpace` instead.
  - `String.Concat` and `String.Join` has new overloads in .NET 4.0, use `StringHelper.Concat` and `StringHelper.Join` instead.
  - `Stopwatch.Restart` was added in .NET 4.0, use `StopwatchExtensions.Restart` instead.
  - `StringBuilder.Clear` was added in .NET 4.0, use `StringBuilderExtensions.Clear` instead.

Note: the class `ReadOnlyCollection` exists since .NET 2.0, it takes a `IList<T>` instead of `ICollection<T>` and does not implement `IReadOnlyCollection<T>` which was added in .NET 4.5 - the class `ExtendedReadOnlyColletion<T>` aliviates these problems.

Others:

  - The class `ReaderWriterLockSlim` was added in .NET 3.5, if `ReaderWriterLock` is not good enough:
    - Try using `Theraot.Threading.ReadWriteLock`, refer to tests for examples.
    - Otherwise use of memory transaction via `Theraot.Threading.Needles.Transact` [See Above].
  - The classes added in .NET 4.5 that require `EventHandler<T>` without the generic constraint may get backported using `Theraot.Core.NewEventHandler<T>`. Avoid using `EventHandler<T>` explicitly in those cases.
  - The classes added in .NET 4.0 or .NET 4.5 that require `OperationCanceledException.CancellationToken` et al. may get backported using `Theraot.Core.NewOperationCanceledException`. Avoid creating this exceptions yourself.
  - The class `TimeZoneInfo` was added in .NET 3.5... pending.
  - `Path.Combine` new overloads in .NET 4.0... pending.
  - `DateTimeOffset` is new in .NET 3.5... pending.
  - `Enum.TryParse` was added in .NET 4.0... pending.
  - `MemoryCache` was added in .NET 4.0... under consideration.
  - Remember to use culture specific overloads.

---
Compiling
---

The compiling configuration has been set for ease of batch building from Visual Studio, building from Xamarin Studio is also supported.

---
Tests
---

This repository includes a test proyect for NUnit 2.6.3 you can use any NUnit test runner to execute the tests. The library is included nunit.framework.dll for ease of use.

To run the unit test you can use one of the following:
  - NUnit test runner included in the NUnit installation available from nunit.org
  - From Xamarin Studio using the NUnit integration
  - From Visual Studio using the Test Explorer with NUnit Test Adapter available from visualstudiogallery.msdn.microsoft.com
  - From Visual Studio with TestDriven.net
  - From Visual Studio with Reshaper
  - Any other test runner compatible with NUnit

---
Help
---

If anybody is willing to help with the development of this code, the most useful thing at this moment would be to try it out. If you have some work that may need to be backported, or you want to develop for an old version of .NET using this code, please report any problems.

---
License
---

The code is under MIT license

    Copyright (c) 2011 - 2014 Alfonso J. Ramos and the individual authors of the included files

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

The reason for this license is that this library includes code from Mono under MIT License.

This repository also includes a copy of NUnit 2.6.3 which includes its own license file.

---
Warranty
---

Aside from the license, I can only warranty the following: It did work on my machine.

---
Motivation
---

I started this work as a accumulative repository of classes and functions used in my projects. I've been keeping the code compatibility with Mono instead of Visual Studio so that my code can run in other platforms (Linux, basically). This also required to stay a bit behind the latest .NET version.

In addition to that, I always preferred to compile for .NET 2.0 for some reason. So I started using LinqBridge and similar solutions. When .NET 4.0 came out these solutions to this problem started to slack behind... so I looked for a better solution.

Eventually I decided to take the task starting at early 2011 to make a "Compatibility Pack" for .NET, as a result I started to incorporate code from Mono to my code, and did a great refactoring of the libraries to isolate only the required to backport .NET code.