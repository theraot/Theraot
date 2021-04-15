#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Dynamic
{
    /// <summary>
    ///     Represents a dynamically assigned class.  Expando objects which share the same
    ///     members will share the same class.  Classes are dynamically assigned as the
    ///     expando object gains members.
    /// </summary>
    internal class ExpandoClass
    {
        internal static readonly ExpandoClass Empty = new();
        internal readonly string[] Keys; // list of names associated with each element in the data array, sorted
        private const int _emptyHashCode = 6551;
        private readonly int _hashCode; // pre-calculated hash code of all the keys the class contains
        private Dictionary<int, List<WeakReference>>? _transitions; // cached transitions

        // hash code of the empty ExpandoClass.

        // The empty Expando class - all Expando objects start off w/ this class.

        /// <summary>
        ///     Constructs the empty ExpandoClass.  This is the class used when an
        ///     empty Expando object is initially constructed.
        /// </summary>
        internal ExpandoClass()
        {
            _hashCode = _emptyHashCode;
            Keys = ArrayEx.Empty<string>();
        }

        /// <summary>
        ///     Constructs a new ExpandoClass that can hold onto the specified keys.  The
        ///     keys must be sorted ordinally.  The hash code must be pre-calculated for
        ///     the keys.
        /// </summary>
        internal ExpandoClass(string[] keys, int hashCode)
        {
            _hashCode = hashCode;
            Keys = keys;
        }

        internal ExpandoClass FindNewClass(string newKey, object lockObject)
        {
            // just XOR the newKey hash code
            var hashCode = _hashCode ^ StringComparer.Ordinal.GetHashCode(newKey);
            lock (lockObject)
            {
                var infos = GetTransitionList(hashCode);

                for (var i = 0; i < infos.Count; i++)
                {
                    if (infos[i].Target is not ExpandoClass @class)
                    {
                        infos.RemoveAt(i);
                        i--;
                        continue;
                    }

                    var classKeys = @class.Keys;
                    if (string.Equals(classKeys[classKeys.Length - 1], newKey, StringComparison.Ordinal))
                    {
                        // the new key is the key we added in this transition
                        return @class;
                    }
                }

                // no applicable transition, create a new one
                var keys = Keys;
                var newKeys = new string[keys.Length + 1];
                Array.Copy(keys, 0, newKeys, 0, keys.Length);
                newKeys[keys.Length] = newKey;
                var ec = new ExpandoClass(newKeys, hashCode);

                infos.Add(new WeakReference(ec));
                return ec;
            }
        }

        internal int GetValueIndex(string name, bool caseInsensitive, ExpandoObject obj)
        {
            return caseInsensitive ? GetValueIndexCaseInsensitive(name, obj) : GetValueIndexCaseSensitive(name, obj.LockObject);
        }

        internal int GetValueIndexCaseSensitive(string name, object lockObject)
        {
            lock (lockObject)
            {
                var keys = Keys;
                for (var i = 0; i < keys.Length; i++)
                {
                    if (string.Equals
                    (
                        keys[i],
                        name,
                        StringComparison.Ordinal
                    ))
                    {
                        return i;
                    }
                }

                return ExpandoObject.NoMatch;
            }
        }

        private List<WeakReference> GetTransitionList(int hashCode)
        {
            if (_transitions == null)
            {
                _transitions = new Dictionary<int, List<WeakReference>>();
            }

            if (!_transitions.TryGetValue(hashCode, out var infos))
            {
                _transitions[hashCode] = infos = new List<WeakReference>();
            }

            return infos;
        }

        /// <summary>
        ///     Gets the index at which the value should be stored for the specified name,
        ///     the method is only used in the case-insensitive case.
        /// </summary>
        /// <param name="name">the name of the member</param>
        /// <param name="obj">
        ///     The ExpandoObject associated with the class
        ///     that is used to check if a member has been deleted.
        /// </param>
        /// <returns>
        ///     the exact match if there is one
        ///     if there is exactly one member with case insensitive match, return it
        ///     otherwise we throw AmbiguousMatchException.
        /// </returns>
        private int GetValueIndexCaseInsensitive(string name, ExpandoObject obj)
        {
            var caseInsensitiveMatch = ExpandoObject.NoMatch; //the location of the case-insensitive matching member
            lock (obj.LockObject)
            {
                var keys = Keys;
                for (var i = keys.Length - 1; i >= 0; i--)
                {
                    //if the matching member is deleted, continue searching
                    if
                    (
                        !string.Equals
                        (
                            keys[i],
                            name,
                            StringComparison.OrdinalIgnoreCase
                        )
                        || obj.IsDeletedMember(i)
                    )
                    {
                        continue;
                    }

                    if (caseInsensitiveMatch == ExpandoObject.NoMatch)
                    {
                        caseInsensitiveMatch = i;
                    }
                    else
                    {
                        //Ambiguous match, stop searching
                        return ExpandoObject.AmbiguousMatchFound;
                    }
                }
            }

            //There is exactly one member with case insensitive match.
            return caseInsensitiveMatch;
        }
    }
}

#endif