#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.
    /// Represents a cache of runtime binding rules.
    /// </summary>
    /// <typeparam name="T">The delegate type.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never), DebuggerStepThrough]
    // ReSharper disable once UnusedTypeParameter
    public class RuleCache<T>
        where T : class
    {
        // Adds to end or inserts items at InsertPosition
        private const int _insertPosition = _maxRules / 2;

        private const int _maxRules = 128;
        private readonly object _cacheLock = new object();
        private T[] _rules = Theraot.Collections.ThreadSafe.ArrayReservoir<T>.EmptyArray;

        internal RuleCache()
        {
            // Empty
        }

        internal void AddRule(T newRule)
        {
            // need a lock to make sure we are not losing rules.
            lock (_cacheLock)
            {
                var rules = Threading.Volatile.Read(ref _rules);
                Threading.Volatile.Write(ref _rules, AddOrInsert(rules, newRule));
            }
        }

        internal T[] GetRules()
        {
            return Threading.Volatile.Read(ref _rules);
        }

        // move the rule +2 up.
        // this is called on every successful rule.
        internal void MoveRule(T rule, int i)
        {
            // limit search to MaxSearch elements.
            // Rule should not get too far unless it has been already moved up.
            // need a lock to make sure we are moving the right rule and not losing any.
            lock (_cacheLock)
            {
                const int MaxSearch = 8;
                var rules = Threading.Volatile.Read(ref _rules);
                var count = rules.Length - i;
                if (count > MaxSearch)
                {
                    count = MaxSearch;
                }

                var oldIndex = -1;
                var max = Math.Min(rules.Length, i + count);
                for (var index = i; index < max; index++)
                {
                    if (rules[index] == rule)
                    {
                        oldIndex = index;
                        break;
                    }
                }
                if (oldIndex < 2)
                {
                    return;
                }
                var oldRule = rules[oldIndex];
                rules[oldIndex] = rules[oldIndex - 1];
                rules[oldIndex - 1] = rules[oldIndex - 2];
                rules[oldIndex - 2] = oldRule;
            }
        }

        internal void ReplaceRule(T oldRule, T newRule)
        {
            // need a lock to make sure we are replacing the right rule
            lock (_cacheLock)
            {
                var rules = Threading.Volatile.Read(ref _rules);
                var i = Array.IndexOf(rules, oldRule);
                if (i >= 0)
                {
                    rules[i] = newRule;
                    return; // DONE
                }

                // could not find it.
                Threading.Volatile.Write(ref _rules, AddOrInsert(rules, newRule));
            }
        }

        private static T[] AddOrInsert(T[] rules, T item)
        {
            if (rules.Length < _insertPosition)
            {
                return Theraot.Collections.Extensions.AddLast(rules, item);
            }

            T[] newRules;

            var newLength = rules.Length + 1;
            if (newLength > _maxRules)
            {
                newLength = _maxRules;
                newRules = rules;
            }
            else
            {
                newRules = new T[newLength];
                Array.Copy(rules, 0, newRules, 0, _insertPosition);
            }

            newRules[_insertPosition] = item;
            Array.Copy(rules, _insertPosition, newRules, _insertPosition + 1, newLength - _insertPosition - 1);
            return newRules;
        }
    }
}

#endif