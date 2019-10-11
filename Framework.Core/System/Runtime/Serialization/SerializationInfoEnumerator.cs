#if LESSTHAN_NETSTANDARD13
#pragma warning disable CA1815 // Override equals and operator equals on value types

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

namespace System.Runtime.Serialization
{
    public struct SerializationEntry
    {
        internal SerializationEntry(string entryName, object? entryValue, Type entryType)
        {
            Name = entryName;
            Value = entryValue;
            ObjectType = entryType;
        }

        public string Name { get; }
        public Type ObjectType { get; }

        public object? Value { get; }
    }

    public sealed class SerializationInfoEnumerator : IEnumerator
    {
        private bool _current;
        private int _currentItem;
        private readonly object?[] _data;
        private readonly string[] _members;
        private readonly int _numItems;
        private readonly Type[] _types;

        internal SerializationInfoEnumerator(string[] members, object?[] info, Type[] types, int numItems)
        {
            Debug.Assert(numItems >= 0, "[SerializationInfoEnumerator.ctor]numItems>=0");
            Debug.Assert(members.Length >= numItems, "[SerializationInfoEnumerator.ctor]members.Length>=numItems");
            Debug.Assert(info.Length >= numItems, "[SerializationInfoEnumerator.ctor]info.Length>=numItems");
            Debug.Assert(types.Length >= numItems, "[SerializationInfoEnumerator.ctor]types.Length>=numItems");

            _members = members;
            _data = info;
            _types = types;

            //The MoveNext semantic is much easier if we enforce that [0..m_numItems] are valid entries
            //in the enumerator, hence we subtract 1.
            _numItems = numItems - 1;
            _currentItem = -1;
            _current = false;
        }

        public SerializationEntry Current
        {
            get
            {
                if (!_current)
                {
                    throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                }
                return new SerializationEntry(_members[_currentItem], _data[_currentItem], _types[_currentItem]);
            }
        }

        public string Name
        {
            get
            {
                if (!_current)
                {
                    throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                }
                return _members[_currentItem];
            }
        }

        public Type ObjectType
        {
            get
            {
                if (!_current)
                {
                    throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                }
                return _types[_currentItem];
            }
        }

        public object? Value
        {
            get
            {
                if (!_current)
                {
                    throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                }
                return _data[_currentItem];
            }
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_currentItem < _numItems)
            {
                _currentItem++;
                _current = true;
            }
            else
            {
                _current = false;
            }

            return _current;
        }

        public void Reset()
        {
            _currentItem = -1;
            _current = false;
        }
    }
}

#endif