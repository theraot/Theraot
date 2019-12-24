#if LESSTHAN_NET35

#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CC0074 // Make field readonly
#pragma warning disable CC0091 // Use static method
#pragma warning disable RCS1225 // Make class sealed.
#pragma warning disable S1104 // Fields should not have public accessibility

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Threading;
using Theraot;
using Theraot.Collections.ThreadSafe;

namespace System.Runtime.CompilerServices
{
    //
    // A CallSite provides a fast mechanism for call-site caching of dynamic dispatch
    // behavior. Each site will hold onto a delegate that provides a fast-path dispatch
    // based on previous types that have been seen at the call-site. This delegate will
    // call UpdateAndExecute if it is called with types that it hasn't seen before.
    // Updating the binding will typically create (or lookup) a new delegate
    // that supports fast-paths for both the new type and for any types that
    // have been seen previously.
    //
    // DynamicSites will generate the fast-paths specialized for sets of runtime argument
    // types. However, they will generate exactly the right amount of code for the types
    // that are seen in the program so that int addition will remain as fast as it would
    // be with custom implementation of the addition, and the user-defined types can be
    // as fast as ints because they will all have the same optimal dynamically generated
    // fast-paths.
    //
    // DynamicSites don't encode any particular caching policy, but use their
    // CallSiteBinding to encode a caching policy.
    //

    /// <summary>
    ///     A Dynamic Call Site base class. This type is used as a parameter type to the
    ///     dynamic site targets. The first parameter of the delegate (T) below must be
    ///     of this type.
    /// </summary>
    public class CallSite
    {
        /// <summary>
        ///     String used for generated CallSite methods.
        /// </summary>
        internal const string CallSiteTargetMethodName = "CallSite.Target";

        /// <summary>
        ///     Used by Matchmaker sites to indicate rule match.
        /// </summary>
        internal bool Match;

        /// <summary>
        ///     Cache of CallSite constructors for a given delegate type.
        /// </summary>
        private static volatile CacheDict<Type, Func<CallSiteBinder, CallSite>>? _siteConstructors;

        // only CallSite<T> derives from this
        internal CallSite(CallSiteBinder? binder)
        {
            Binder = binder;
        }

        /// <summary>
        ///     Class responsible for binding dynamic operations on the dynamic site.
        /// </summary>
        public CallSiteBinder? Binder { get; }

        /// <summary>
        ///     Creates a CallSite with the given delegate type and binder.
        /// </summary>
        /// <param name="delegateType">The CallSite delegate type.</param>
        /// <param name="binder">The CallSite binder.</param>
        /// <returns>The new CallSite.</returns>
        public static CallSite Create(Type delegateType, CallSiteBinder binder)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
            {
                throw new ArgumentException("Type must be derived from System.Delegate");
            }
            var constructors = _siteConstructors;
            if (constructors == null)
            {
                // It's okay to just set this, worst case we're just throwing away some data
                _siteConstructors = constructors = new CacheDict<Type, Func<CallSiteBinder, CallSite>>(100);
            }
            if (constructors.TryGetValue(delegateType, out var ctor))
            {
                return ctor(binder);
            }
            var method = typeof(CallSite<>).MakeGenericType(delegateType).GetMethod(nameof(Create));
            ctor = (Func<CallSiteBinder, CallSite>)method.CreateDelegate(typeof(Func<CallSiteBinder, CallSite>));
            constructors.Add(delegateType, ctor);
            return ctor(binder);
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Dynamic site type.
    /// </summary>
    /// <typeparam name="T">The delegate type.</typeparam>
    public class CallSite<T> : CallSite where T : class
    {
        /// <summary>
        ///     The Level 0 cache - a delegate specialized based on the site history.
        /// </summary>
        public T? Target;

        /// <summary>
        ///     an instance of matchmaker site to opportunistically reuse when site is polymorphic
        /// </summary>
        internal CallSite? CachedMatchmaker;

        /// <summary>
        ///     The Level 1 cache - a history of the dynamic site.
        /// </summary>
        internal T[]? Rules;

        private const int _maxRules = 10;

        private CallSite()
            : base(null)
        {
            // Empty
        }

        /// <summary>
        ///     The update delegate. Called when the dynamic site experiences cache miss.
        /// </summary>
        /// <returns>The update delegate.</returns>
        public T Update => throw new NotSupportedException();

        /// <summary>
        ///     Creates an instance of the dynamic call site, initialized with the binder responsible for the
        ///     runtime binding of the dynamic operations at this call site.
        /// </summary>
        /// <param name="binder">The binder responsible for the runtime binding of the dynamic operations at this call site.</param>
        /// <returns>The new instance of dynamic call site.</returns>
        public static CallSite<T> Create(CallSiteBinder binder)
        {
            No.Op(binder);
            throw new NotSupportedException();
        }

        internal void AddRule(T newRule)
        {
            var rules = Rules;
            if (rules == null)
            {
                Rules = new[] { newRule };
                return;
            }

            T[] temp;
            if (rules.Length < _maxRules - 1)
            {
                temp = new T[rules.Length + 1];
                Array.Copy(rules, 0, temp, 1, rules.Length);
            }
            else
            {
                temp = new T[_maxRules];
                Array.Copy(rules, 0, temp, 1, _maxRules - 1);
            }

            temp[0] = newRule;
            Rules = temp;
        }

        internal CallSite<T> CreateMatchMaker()
        {
            return new CallSite<T>();
        }

        internal CallSite GetMatchmaker()
        {
            // check if we have a cached matchmaker and attempt to atomically grab it.
            var matchmaker = CachedMatchmaker;
            if (matchmaker == null)
            {
                return matchmaker ?? new CallSite<T> { Match = true };
            }

            matchmaker = Interlocked.Exchange(ref CachedMatchmaker, null);
            Debug.Assert(matchmaker?.Match != false, "cached site should be set up for matchmaking");

            return matchmaker ?? new CallSite<T> { Match = true };
        }

        // moves rule +2 up.
        internal void MoveRule(int i)
        {
            if (i <= 1)
            {
                return;
            }

            var rules = Rules;
            var rule = rules![i];

            rules[i] = rules[i - 1];
            rules[i - 1] = rules[i - 2];
            rules[i - 2] = rule;
        }

        internal void ReleaseMatchmaker(CallSite matchMaker)
        {
            // If "Rules" has not been created, this is the first (and likely the only) Update of the site.
            // 90% sites stay monomorphic and will never need a matchmaker again.
            // Otherwise store the matchmaker for the future use.
            if (Rules != null)
            {
                CachedMatchmaker = matchMaker;
            }
        }
    }
}

#endif