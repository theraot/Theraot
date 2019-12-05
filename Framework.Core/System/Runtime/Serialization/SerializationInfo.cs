#if LESSTHAN_NETSTANDARD13
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Theraot;
using Theraot.Reflection;

namespace System.Runtime.Serialization
{
    /// <summary>The structure for holding all of the data needed for object serialization and deserialization.</summary>
    public sealed class SerializationInfo
    {
        private const int _defaultSize = 4;
        private readonly IFormatterConverter _converter;

        private readonly Dictionary<string, int> _nameToIndex;

        // Even though we have a dictionary, we're still keeping all the arrays around for back-compat.
        // Otherwise we may run into potentially breaking behaviors like GetEnumerator() not returning entries in the same order they were added.
        private string[] _names;

        private string _rootTypeAssemblyName;
        private string _rootTypeName;
        private Type[] _types;
        private object?[] _values;

        [CLSCompliant(false)]
        public SerializationInfo(Type type, IFormatterConverter converter)
        {
            ObjectType = type ?? throw new ArgumentNullException(nameof(type));
            _rootTypeName = type.FullName;
            _rootTypeAssemblyName = type.GetTypeInfo().Module.Assembly.FullName;

            _names = new string[_defaultSize];
            _values = new object[_defaultSize];
            _types = new Type[_defaultSize];

            _nameToIndex = new Dictionary<string, int>();

            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        [CLSCompliant(false)]
        public SerializationInfo(Type type, IFormatterConverter converter, bool requireSameTokenInPartialTrust)
            : this(type, converter)
        {
            // requireSameTokenInPartialTrust is a vacuous parameter in a platform that does not support partial trust.
            No.Op(requireSameTokenInPartialTrust);
        }

        public string AssemblyName
        {
            get => _rootTypeAssemblyName;
            set
            {
                _rootTypeAssemblyName = value ?? throw new ArgumentNullException(nameof(value));
                IsAssemblyNameSetExplicit = true;
            }
        }

        public string FullTypeName
        {
            get => _rootTypeName;
            set
            {
                _rootTypeName = value ?? throw new ArgumentNullException(nameof(value));
                IsFullTypeNameSetExplicit = true;
            }
        }

        public bool IsAssemblyNameSetExplicit { get; private set; }

        public bool IsFullTypeNameSetExplicit { get; private set; }

        public int MemberCount { get; private set; }

        public Type ObjectType { get; private set; }

        public void AddValue(string name, object? value, Type type)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            AddValueInternal(name, value, type);
        }

        public void AddValue(string name, object value)
        {
            if (value == null)
            {
                AddValue(name, null, typeof(object));
            }
            else
            {
                AddValue(name, value, value.GetType());
            }
        }

        public void AddValue(string name, bool value)
        {
            AddValue(name, value, typeof(bool));
        }

        public void AddValue(string name, char value)
        {
            AddValue(name, value, typeof(char));
        }

        [CLSCompliant(false)]
        public void AddValue(string name, sbyte value)
        {
            AddValue(name, value, typeof(sbyte));
        }

        public void AddValue(string name, byte value)
        {
            AddValue(name, value, typeof(byte));
        }

        public void AddValue(string name, short value)
        {
            AddValue(name, value, typeof(short));
        }

        [CLSCompliant(false)]
        public void AddValue(string name, ushort value)
        {
            AddValue(name, value, typeof(ushort));
        }

        public void AddValue(string name, int value)
        {
            AddValue(name, value, typeof(int));
        }

        [CLSCompliant(false)]
        public void AddValue(string name, uint value)
        {
            AddValue(name, value, typeof(uint));
        }

        public void AddValue(string name, long value)
        {
            AddValue(name, value, typeof(long));
        }

        [CLSCompliant(false)]
        public void AddValue(string name, ulong value)
        {
            AddValue(name, value, typeof(ulong));
        }

        public void AddValue(string name, float value)
        {
            AddValue(name, value, typeof(float));
        }

        public void AddValue(string name, double value)
        {
            AddValue(name, value, typeof(double));
        }

        public void AddValue(string name, decimal value)
        {
            AddValue(name, value, typeof(decimal));
        }

        public void AddValue(string name, DateTime value)
        {
            AddValue(name, value, typeof(DateTime));
        }

        public bool GetBoolean(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(bool)) ? (bool)value : _converter.ToBoolean(value);
        }

        public byte GetByte(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(byte)) ? (byte)value : _converter.ToByte(value);
        }

        public char GetChar(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(char)) ? (char)value : _converter.ToChar(value);
        }

        public DateTime GetDateTime(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(DateTime)) ? (DateTime)value : _converter.ToDateTime(value);
        }

        public decimal GetDecimal(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(decimal)) ? (decimal)value : _converter.ToDecimal(value);
        }

        public double GetDouble(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(double)) ? (double)value : _converter.ToDouble(value);
        }

        public SerializationInfoEnumerator GetEnumerator() => new SerializationInfoEnumerator(_names, _values, _types, MemberCount);

        public short GetInt16(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(short)) ? (short)value : _converter.ToInt16(value);
        }

        public int GetInt32(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(int)) ? (int)value : _converter.ToInt32(value);
        }

        public long GetInt64(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(long)) ? (long)value : _converter.ToInt64(value);
        }

        [CLSCompliant(false)]
        public sbyte GetSByte(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(sbyte)) ? (sbyte)value : _converter.ToSByte(value);
        }

        public float GetSingle(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(float)) ? (float)value : _converter.ToSingle(value);
        }

        public string? GetString(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(string)) ? null : _converter.ToString(value);
        }

        [CLSCompliant(false)]
        public ushort GetUInt16(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(ushort)) ? (ushort)value : _converter.ToUInt16(value);
        }

        [CLSCompliant(false)]
        public uint GetUInt32(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(uint)) ? (uint)value : _converter.ToUInt32(value);
        }

        [CLSCompliant(false)]
        public ulong GetUInt64(string name)
        {
            var value = GetElement(name, out var foundType);
            return value == null ? default : ReferenceEquals(foundType, typeof(ulong)) ? (ulong)value : _converter.ToUInt64(value);
        }

        public object? GetValue(string name, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.GetUnderlyingSystemType() != type)
            {
                throw new ArgumentException(string.Empty, nameof(type));
            }

            var value = GetElement(name, out var foundType);

            if (ReferenceEquals(foundType, type) || type.GetTypeInfo().IsAssignableFrom(foundType.GetTypeInfo()) || value == null)
            {
                return value;
            }

            return _converter.Convert(value, type);
        }

        public void SetType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (ReferenceEquals(ObjectType, type))
            {
                return;
            }

            ObjectType = type;
            _rootTypeName = type.FullName;
            _rootTypeAssemblyName = type.GetTypeInfo().Module.Assembly.FullName;
            IsFullTypeNameSetExplicit = false;
            IsAssemblyNameSetExplicit = false;
        }

        internal void AddValueInternal(string name, object? value, Type type)
        {
            if (_nameToIndex.ContainsKey(name))
            {
                throw new SerializationException();
            }
            _nameToIndex.Add(name, MemberCount);

            // If we need to expand the arrays, do so.
            if (MemberCount >= _names.Length)
            {
                ExpandArrays();
            }

            // Add the data and then advance the counter.
            _names[MemberCount] = name;
            _values[MemberCount] = value;
            _types[MemberCount] = type;
            MemberCount++;
        }

        internal object? GetValueNoThrow(string name, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Debug.Assert(type.GetUnderlyingSystemType() == type, "[SerializationInfo.GetValue]type is not a runtime type");

            var value = GetElementNoThrow(name, out var foundType);
            if (value == null)
            {
                return null;
            }

            if (ReferenceEquals(foundType, type) || type.GetTypeInfo().IsAssignableFrom(foundType.GetTypeInfo()))
            {
                return value;
            }

            return _converter.Convert(value, type);
        }

        /// <summary>
        /// <para>
        /// Finds the value if it exists in the current data. If it does, we replace
        /// the values, if not, we append it to the end. This is useful to the
        /// ObjectManager when it's performing fixups.
        /// </para>
        /// <para>
        /// All error checking is done with asserts. Although public in coreclr,
        /// it's not exposed in a contract and is only meant to be used by corefx.
        /// </para>
        /// <para>
        /// This isn't a public API, but it gets invoked dynamically by
        /// BinaryFormatter
        /// </para>
        /// <para>
        /// This should not be used by clients: exposing out this functionality would allow children
        /// to overwrite their parent's values. It is public in order to give corefx access to it for
        /// its ObjectManager implementation, but it should not be exposed out of a contract.
        /// </para>
        /// </summary>
        /// <param name="name"> The name of the data to be updated.</param>
        /// <param name="value"> The new value.</param>
        /// <param name="type"> The type of the data being added.</param>
        internal void UpdateValue(string name, object value, Type type)
        {
            var index = FindElement(name);
            if (index < 0)
            {
                AddValueInternal(name, value, type);
            }
            else
            {
                _values[index] = value;
                _types[index] = type;
            }
        }

        private void ExpandArrays()
        {
            Debug.Assert(_names.Length == MemberCount, "[SerializationInfo.ExpandArrays]_names.Length == _count");

            var newSize = MemberCount * 2;

            // In the pathological case, we may wrap
            if (newSize < MemberCount && int.MaxValue > MemberCount)
            {
                newSize = int.MaxValue;
            }

            // Allocate more space and copy the data
            var newMembers = new string[newSize];
            var newData = new object[newSize];
            var newTypes = new Type[newSize];

            Array.Copy(_names, newMembers, MemberCount);
            Array.Copy(_values, newData, MemberCount);
            Array.Copy(_types, newTypes, MemberCount);

            // Assign the new arrays back to the member vars.
            _names = newMembers;
            _values = newData;
            _types = newTypes;
        }

        private int FindElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (_nameToIndex.TryGetValue(name, out var index))
            {
                return index;
            }
            return -1;
        }

        /// <summary>
        /// Gets the location of a particular member and then returns
        /// the value of the element at that location.  The type of the member is
        /// returned in the foundType field.
        /// </summary>
        /// <param name="name"> The name of the element to find.</param>
        /// <param name="foundType"> The type of the element associated with the given name.</param>
        /// <returns>The value of the element at the position associated with name.</returns>
        private object? GetElement(string name, out Type foundType)
        {
            var index = FindElement(name);
            if (index == -1)
            {
                throw new SerializationException();
            }

            Debug.Assert(index < _values.Length, "[SerializationInfo.GetElement]index<_values.Length");
            Debug.Assert(index < _types.Length, "[SerializationInfo.GetElement]index<_types.Length");

            foundType = _types[index];
            Debug.Assert(foundType != null, "[SerializationInfo.GetElement]foundType!=null");
            return _values[index];
        }

        private object? GetElementNoThrow(string name, out Type? foundType)
        {
            var index = FindElement(name);
            if (index == -1)
            {
                foundType = null;
                return null;
            }

            Debug.Assert(index < _values.Length, "[SerializationInfo.GetElement]index<_values.Length");
            Debug.Assert(index < _types.Length, "[SerializationInfo.GetElement]index<_types.Length");

            foundType = _types[index];
            Debug.Assert(foundType != null, "[SerializationInfo.GetElement]foundType!=null");
            return _values[index];
        }
    }
}

#endif