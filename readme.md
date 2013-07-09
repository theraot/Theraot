Theraot's Libraries
===

Theraot's Libraries are an ongoing effort to ease the work on .NET, including a backport of recent .NET features to .NET 2.0 among other things.

---
Introduction
---

My libraries are and will probably always be work in progress... please, do not hesitate to report bugs and request features.

I started this work as a accumulative repository of classes and functions used in my projects. I've been keeping the code compatibility with Mono instead of Visual Studio so that my code can run in other platforms... this also required to stay a bit behind the latest .NET version.

In addition to that, I always preferred to compile for .NET 2.0 for some reason. So I started using LinqBridge and similar solutions. When .NET 4.0 came out these solutions started to slack behind... so I looked for a better solution.

I soon learned there were no good solution for this problem. So I decided to take the task starting at April 2011 to make a "Compatibility Pack" for .NET, as a result I started to incorporate code from Mono to my code, and did a great refactoring of the libraries to isolate only the required to backport .NET code.

Theraot.Core is as close as I am to that "Compatibility Pack", although it is not just a "Compatibility Pack" because it adds additional classes and functions in a separate namespace, this additional code is used internally to implement the backport code.

Currently there is no documentation, although any behaviour that differs from what the BCL does can be considered a bug.

---
Features
---

Theraot's Libraries...

  - can be built for .NET 2.0, 3.0, 3.5, 4.0 and 4.5 with the help of conditional compilation to keep only the required code for the particular version.
  - includes code from HashBucket (another project of mine).
  - includes (among others) the following types to be used in old versions of .NET back to .NET 2.0:
    - System.Collections.Concurrent: Work in progress
    - System.Collections.Generic.HashSet: Done
    - System.Collections.Generic.SortedSet: Done
    - System.Collections.StructuralComparison: Done
    - System.Collections.Linq: Nearly Done [Mostly taken from Mono]
    - System.Collections.Linq.Expressions: Nearly Done [Mostly taken from Mono]
    - System.Numerics: Done [Taken from Mono]^1
    - System.Runtime.CompilerServices.DynamicAttribute: Done
    - System.Runtime.CompilerServices.ExtensionAttribute: Done
    - System.Therading.ThreadLocal: Done
    - System.Threading.Tasks: Planned
    - System.Action: Done
    - System.Func: Done
    - System.IObservable: Done
    - System.IObserver: Done
    - System.Lazy: Done
    - System.Tuple: Done
  - uses less than 1MB in disk
  - keeps a consistent code style in the whole code^2
    
^1: I mantain my copy of System.Numerics that was taken from Mono. I have provided the optimization for the cast from BigInteger to float and double.

^2: I intent to keep the code readable, yet documentation is low priority at this point. 

---
Extended Features
---

Not everything in the code is a backport, some parts are utility and helper methods that has been developed along side the backport effort.

This are some parts worth of mention:

    - Theraot.Collections
        - Specialized
            - AVLTree
            - NullAwareDictionary
        - Extensions : A huge plus beyond Linq
        - Progressive* : A set of classes that walk an IEnumerable<T> on demand and cache the result
    - Theraot.Core
        - ActionHelper : Lazy static Noop and Throw Actions
        - FuncHelper : Lazy static Default, Return and Throw Funcs
        - NumericHelper : A lot of functions from integer square root to primality test including extracting mantissa and exp from double value and more.
        - StringHelper : A lot of functions from Append to Implode and more.
        - TypeHelper : A bunch of type related helper functions
     - Theraot.Threading
        - Some code from the project HashBucket
        - Disposable : A general purpose disposable object with Action callback.
        - DisposableAkin : Same as above, but can only be disposed by the same thread that created it ^1.
        - IExtendedDisposable : A IDisposible that you can query to know if it was disposed ^2.
        - NoTrackingThreadLocal & TrackingThreadLocal : The backends for the backport of System.Threading.ThreadLocal
        - SingleTimeExecution : A thread-safe way to wrap code to be called only once.
        - ThreadinHelper : Provides unique ids for managed threads, generic VolatileRead and *Write, and conditional SpinWait.
        
^1: This actually makes the implementation simpler and more efficient.

^2: Do not rely on the IsDisposed property as it may change any moment if another thread call Dispose, use DisposedConditional instead.

---
FAT Features
---

Some code in this libraries has been developed for past or future features but is not currently used as part of the backports, so they are only provided in "FAT" builds.

This are some parts worth of mention:

    - Theraot.Core
        - ICloneable<T> & ICloner<T> & CloneHelper : Generalization of ICloneable as generic
        - TraceRoute & TraceNode : A Network Traceroute implementation
    - Theraot.Threading
        - Some code from the project HashBucket
        - CritialDisposible : A variant of Disposible that inherits from CritialFinalizerObject ^1.
        - Work : a task scheduler implementation, intended to be part of the backport of System.Threading.Tasks

^1: In theory you shouldn't need it, if you need it, chances are something else is wrong.

---

There are a few things that are beyond the scope of my work:

  - I cannot provide Generic Variance
  - I cannot extend reflection (I recommend to use Mono.Cecil)
  - I cannot add some methods such as String.Join(string, IEnumerator<string>), I'll provide helper functions instead.
  - I will not include backports of Reactive Extensions or any other code not in the BCL, but I may provide similar functionality.
  - I have no intention to backport GUI libraries.

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
Binary
---

The binary downloads are available from: http://www.4shared.com/folder/o2pF-8Oe/Theraot.html
