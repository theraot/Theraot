Theraot's Libraries
===

Theraot's Libraries are an ongoing effort to ease the work on .NET, including a backport of recent .NET features to .NET 2.0 among other things.

---
Introduction
---

Theraot.Core is as close as I am to a ".NET Compatibility Pack" capable to bring code from recent .NET versions back to .NET 2.0.

Although this project it is not just a "Compatibility Pack" because it adds additional classes and functions in a separate namespace, this additional code is used internally to implement the backport code.

Currently there is no documentation, although any behaviour that differs from what the BCL does can be considered a bug.

My libraries are and will probably always be work in progress... please, do not hesitate to report bugs and request features.

---
Binary Downloads
---

The binary downloads are available from: http://www.4shared.com/folder/o2pF-8Oe/Theraot.html

Note: I publish new binary downloads "sparsely", if you want a build of a particular revision, you can requeste it to me.

---
Features
---

Theraot's Libraries...

  - can be built for .NET 2.0, 3.0, 3.5, 4.0 and 4.5 with the help of conditional compilation to keep only the required code for the particular version.
  - includes lock-free and wait-free structures developen in HashBucket (another project of mine).
  - includes (among others) the following types to be used in old versions of .NET back to .NET 2.0:
    - System.Action: Done
    - System.Collections.Concurrent: Work in progress
    - System.Collections.Generic.HashSet: Done
    - System.Collections.Generic.SortedSet: Done
    - System.Collections.Linq: Up to .NET 3.5
    - System.Collections.Linq.Expressions: Up to .NET 3.4
    - System.Collections.ObjectModel.ObservableCollection: Work in progress
    - System.Collections.ObjectModel.ReadOnlyDictionary : Done
    - System.Collections.StructuralComparison: Done
    - System.Func: Done
    - System.IObservable: Done
    - System.IObserver: Done
    - System.Lazy: Done
    - System.Numerics.BigInteger: Done [Taken from Mono][See Note 1]
    - System.Numerics.Complex: Done [Taken from Mono]
    - System.Runtime.CompilerServices.DynamicAttribute: Done
    - System.Runtime.CompilerServices.ExtensionAttribute: Done
    - System.Therading.AggregateException: Done
    - System.Therading.ThreadLocal: Done
    - System.Threading.Tasks: Planned
    - System.Therading.Volatile: Done
    - System.Tuple: Done
    - System.WeakReferene: Done
  - Uses less than 1MB in disk
  - keeps a consistent code style in the whole code [See Note 2]
    
Note 1: I mantain my copy of System.Numerics.BigInteger that was taken from Mono. I have provided the optimization for the cast from BigInteger to float and double.

Note 2: I intent to keep the code readable, yet documentation is low priority at this point. 

---
Extended Features
---

Not everything in the code is a backport, some parts are utility and helper methods that has been developed along side the backport effort.

This are some parts worth of mention:

  - Theraot.Collections
    - Specialized
      - AVLTree : A binary soprted and balanced tree.
      - NullAwareDictionary : A dictionary that can store a null key.
    - ThreadSafe
      - Bucket : A fixed-size wait-free collection.
      - FixedSizeHashBucket : A fixed-size wait-free hash based dictionary.
      - FixedSizeQueueBucket : A fixed-size wait-free queue.
      - HashBucket : A lock-free hash based dictionary.
      - LazyBucket : A fixed-size wait-free lazy initialized collection.
      - QueueBucket : A lock-free queue.
    - Extensions : A huge plus beyond Linq.
    - Progressive* : A set of classes that walk an IEnumerable<T> on demand and cache the result.
  - Theraot.Core
    - ActionHelper : A helper class with Lazy static Noop and Throw Actions.
    - ComparerExtensions : A helper class with Extension Methods for IComparer<T>
    - EnumHelper : A helper class for enums.
    - FuncHelper : A helper class with Lazy static Default, Return and Throw Funcs.
    - NumericHelper : A helper class with A lot of functions from integer square root to primality test including extracting mantissa and exp from double value and more.
    - StringHelper : A helper class with A lot of functions from Append to Implode and more.
    - StreamExtensions : A helper class with Extension Methods for Stream.
    - OrderedEnumerable : A custom ordered Enumerable.
    - TypeHelper : A helper class with A bunch of type related helper functions.
  - Theraot.Threading
    - Needles : [See "Needle" below]
    - ArrayPool : An object pool to recycle arrays.
    - Disposable : A general purpose disposable object with Action callback.
    - DisposableAkin : Same as above, but can only be disposed by the same thread that created it. [See Note 1]
    - IExtendedDisposable : A IDisposible that you can query to know if it was disposed. [See Note 2]
    - NoTrackingThreadLocal & TrackingThreadLocal : The backends for the backport of System.Threading.ThreadLocal.
    - ReentryGuard : A helper class that allows to protect a code from reentry.
    - SingleTimeExecution : A thread-safe way to wrap code to be called only once.
    - ThreadinHelper : Provides unique ids for managed threads, generic VolatileRead and *Write, and conditional SpinWait.
        
Note 1: This actually makes the implementation simpler and more efficient.

Note 2: Do not rely on the IsDisposed property as it may change any moment if another thread call Dispose, use DisposedConditional instead.

---
"FAT" Features
---

Some code in this libraries has been developed for past or future features but is not currently used as part of the backports, so they are only provided in "FAT" builds.

This are some parts worth of mention:

  - Theraot.Collections
    - ThreadSafe
      - CircularBucket : A fixed-size wait-free circular collection.
      - FixedSizeSetBucket : A fixed-size wait-free set.
      - SetBucket : A lock-free set.
      - WeakDelegateSet : A lock-free set of weak references to delegates.
      - WeakEvent : A weak event implementation.
      - WeakHashBucket : A lock-free hash based dictionary of weak references.
      - WeakSetBucket : A lock-free set of weak references.
  - Theraot.Core
    - ICloneable<T> & ICloner<T> & CloneHelper : Generalization of ICloneable as generic
    - EqualityComparerHelper<T> : A helper function to create equality comparers for types such as delegates and tuples.
    - TraceRoute & TraceNode : A Network Traceroute implementation
  - Theraot.Threading
    - GCMonitor : Allows to get notifications on Garbage Collection. [See Note 1]
    - CritialDisposible : A variant of Disposible that inherits from CritialFinalizerObject. [See Note 2]
    - Work : a task scheduler implementation, intended to be part of the backport of System.Threading.Tasks.

Note 1: The notifications of GCMonitor will run in the finalizer thread, make sure to not waste it's time. It is strongly suggested to use it to start async operations.

Note 2: In theory you shouldn't need the CriticalDisposable, if you need it, chances are something else is wrong.

---
Needles
---

A needle is concept similat to "handle", "pointer", and "reference". A needle is an object that can be used to access a value.

The first needle is just Needle<T> and it is an strong reference. If you prefer a struct you should use StructNeedle<T>.

As counterpart of Needle<T> there is WeakNeedle<T> which is a weak reference.

LazyNeedle<T> and CacheNeedle<T> are the Lazy counterparts to Needle<T> and WeakNeedle<T> respectively.

This lazy initializable needles are "IPromised<T>" which represents needle that will be available in the future. Among those:
  - PromiseNeedle<T> is a needle that is initialized on demand by the process that created the object.
  - FutureNeedle<T> is a needle that is initialized on a async operation.

Finally, Transact and Transact.Needle are used to create memory transaction.

Other needles include: NotNull<T> and WeakDelegateNeedle<T>.

---
Notes
---

There are a few things that are beyond the scope of my work:

  - I cannot provide Generic Variance
  - I cannot extend reflection (I recommend to use Mono.Cecil)
  - I cannot add some methods such as String.Join(string, IEnumerator<string>), I'll provide helper functions instead. For a list see below.
  - I cannot improve the Garbage Collector. Try using Mono as a back-end.
  - I will not include backports of Reactive Extensions or any other code not in the BCL, but I may provide similar functionality. See below.
  - I have no intention to backport GUI libraries.

This features are not planned to be added or improved:

  - Environment.SpecialFolder
  - Environment.Is64BitProcess
  - Environment.Is64BitOperatingSystem
  - IntPtr and UIntPtr Addition and Subtraction
  - ServiceInstaller.DelayedAutoStart

The following are the notable methods that has been added to existing types:

  - Comparer<T>.Create was added in .NET 4.5, use ComparerExtensions.ToComparer instead.
  - Enum.HasFlag was added in .NET 4.0, use EnumHelper.HasFlag instead.
  - Stream.CopyTo was added in .NET 4.0, use StreamExtensions.CopyTo instead.
  - String.IsNullOrWhiteSpace was added in .NET 4.0, use StringHelper.IsNullOrWhiteSpace instead.
  - String.Concat and String.Join has new overloads in .NET 4.0, use StringHelper.Concat and StringHelper.Join instead.

Others:

  - The class ReaderWriterLockSlim was added in .NET 3.5, if ReaderWriterLock is not good enough, I suggest the use of memory transaction via Theraot.Threading.Needles.Transact.
  - The class TimeZoneInfo was added in .NET 3.5... pending.
  - Path.Combine new overloads in .NET 4.0... pending.
  - Stopwatch.Restart was added in .NET 4.0, use StopwatchExtensions.Restart instead.
  - StringBuilder.Clear was added in .NET 4.0, use StringBuilderExtensions.Clear instead.
  - Remember to use culture specific overloads.
  - DateTimeOffset is new in .NET 3.5... pending.
  - Enum.TryParse was added in .NET 4.0... pending.
  - MemoryCache was added in .NET 4.0... under consideration.

---
Compiling
---

The compiling configuration has been set for ease of batch building from Visual Studio, building from Xamarin Studio is also supported.

---
Help
---

If anybody is willing to help with the development of this code, the most useful thing at this moment would be to try it out. If you have some work that may need to be backported, or you want to develop for an old version of .NET using this code, please report any problems.

---
License
---

The code is under MIT license

    Copyright (c) 2011 - 2013 Alfonso J. Ramos and the individual authors of the included files

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

The reason for this license is that this library includes code from Mono under MIT License.

---
Warranty
---

Aside from the license, I can only warranty the following: It did work on my machine.

---
Motivation
---

I started this work as a accumulative repository of classes and functions used in my projects. I've been keeping the code compatibility with Mono instead of Visual Studio so that my code can run in other platforms (Linux, basically). This also required to stay a bit behind the latest .NET version.

In addition to that, I always preferred to compile for .NET 2.0 for some reason. So I started using LinqBridge and similar solutions. When .NET 4.0 came out these solutions to this problem started to slack behind... so I looked for a better solution.

Eventually I decided to take the task starting at April 2011 to make a "Compatibility Pack" for .NET, as a result I started to incorporate code from Mono to my code, and did a great refactoring of the libraries to isolate only the required to backport .NET code.