#if FAT

using NUnit.Framework;
using System;
using System.Globalization;
using Theraot.Collections.Specialized;
using Theraot.Core;

namespace Tests.Theraot.Core
{
    public class CloneHelperTests
    {
        [Test]
        public void CloneArray()
        {
            var array = new[] { 1, 2, 3 };
            var clone = CloneHelper<int[]>.GetCloner().Clone(array); // Cloner
            Assert.AreEqual(array, clone);
            Assert.IsFalse(ReferenceEquals(array, clone));
        }

        [Test]
        public void CloneBasicTypes()
        {
            Assert.AreEqual('a', CloneHelper<char>.GetCloner().Clone('a'));

            Assert.AreEqual(false, CloneHelper<bool>.GetCloner().Clone(false));

            Assert.AreEqual(123, CloneHelper<byte>.GetCloner().Clone(123));
            Assert.AreEqual(1234, CloneHelper<ushort>.GetCloner().Clone(1234));
            Assert.AreEqual(123456, CloneHelper<uint>.GetCloner().Clone(123456));
            Assert.AreEqual(12345678901, CloneHelper<ulong>.GetCloner().Clone(12345678901));

            Assert.AreEqual(-123, CloneHelper<sbyte>.GetCloner().Clone(-123));
            Assert.AreEqual(-1234, CloneHelper<short>.GetCloner().Clone(-1234));
            Assert.AreEqual(-123456, CloneHelper<int>.GetCloner().Clone(-123456));
            Assert.AreEqual(-12345678901, CloneHelper<long>.GetCloner().Clone(-12345678901));

            Assert.AreEqual(decimal.Parse("12345678901234567890123456789"), CloneHelper<decimal>.GetCloner().Clone(decimal.Parse("12345678901234567890123456789")));

            Assert.AreEqual(0.5F, CloneHelper<double>.GetCloner().Clone(0.5f));
            Assert.AreEqual(0.5, CloneHelper<double>.GetCloner().Clone(0.5));
        }

        [Test]
        public void CloneCloneable()
        {
            var cloneable = new Cloneable(16);
            var clone = CloneHelper<Cloneable>.GetCloner().Clone(cloneable); // Cloner
            Assert.AreEqual(cloneable, clone);
            Assert.IsFalse(ReferenceEquals(cloneable, clone));
        }

        [Test]
        public void CloneConstructer()
        {
            var constructer = new Constructer(16);
            var clone = CloneHelper<Constructer>.GetCloner().Clone(constructer); // ConstructorCloner
            Assert.AreEqual(constructer, clone);
            Assert.IsFalse(ReferenceEquals(constructer, clone));
        }

        [Test]
        public void CloneDate()
        {
            var date = DateTime.Now;
            var clone = CloneHelper<DateTime>.GetCloner().Clone(date); // StructCloner
            Assert.AreEqual(date, clone);
        }

        [Test]
        public void CloneDeconstructer()
        {
            var deconstructer = new Deconstructer(16);
            var clone = CloneHelper<Deconstructer>.GetCloner().Clone(deconstructer); // DeconstructCloner
            Assert.AreEqual(deconstructer, clone);
            Assert.IsFalse(ReferenceEquals(deconstructer, clone));
        }

        [Test]
        public void CloneFlagArray()
        {
            var array = new FlagArray(16, true);
            var clone = CloneHelper<FlagArray>.GetCloner().Clone(array); // GenericCloner
            Assert.AreEqual(array, clone);
            Assert.IsFalse(ReferenceEquals(array, clone));
        }

        [Test]
        public void CloneGenericCloneable()
        {
            var cloneable = new GenericCloneable(16);
            var clone = CloneHelper<GenericCloneable>.GetCloner().Clone(cloneable); // GenericCloner
            Assert.AreEqual(cloneable, clone);
            Assert.IsFalse(ReferenceEquals(cloneable, clone));
        }

        [Test]
        public void CloneMockerA()
        {
            var mocker = new MockerB(16);
            var clone = CloneHelper<MockerB>.GetCloner().Clone(mocker); // MockCloner
            Assert.AreEqual(mocker, clone);
            Assert.IsFalse(ReferenceEquals(mocker, clone));
        }

        [Test]
        public void CloneMockerB()
        {
            var mocker = new MockerA(16);
            var clone = CloneHelper<MockerA>.GetCloner().Clone(mocker); // MockCloner
            Assert.AreEqual(mocker, clone);
            Assert.IsFalse(ReferenceEquals(mocker, clone));
        }

        [Test]
        public void CloneObject()
        {
            var obj = new object();
            var clone = CloneHelper<object>.GetCloner().Clone(obj); // SerializerCloner
            Assert.IsInstanceOf<object>(obj);
            Assert.IsInstanceOf<object>(clone);
            // There is only reference comparison, and the clone is a differente reference
            Assert.IsFalse(ReferenceEquals(obj, clone));
        }

        [Test]
        public void CloneSerializable()
        {
            var serializable = new Serializable(16);
            Console.WriteLine(CloneHelper<Serializable>.GetCloner());
            var clone = CloneHelper<Serializable>.GetCloner().Clone(serializable); // SerializerCloner
            Assert.AreEqual(serializable, clone);
            Assert.IsFalse(ReferenceEquals(serializable, clone));
        }

        [Test]
        public void CloneString()
        {
            var str = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            var clone = CloneHelper<string>.GetCloner().Clone(str); // Cloner
            Assert.AreEqual(str, clone);
            // String.Clone returns the same instance
            Assert.IsTrue(ReferenceEquals(str, clone));
        }

        [Test]
        public void UnableToCloneOther()
        {
            Assert.IsNull(CloneHelper<Other>.GetCloner());
        }

        private class Cloneable : IComparable<Cloneable>, IEquatable<Cloneable>, ICloneable
        {
            private readonly int _value;

            public Cloneable(int value)
            {
                _value = value;
            }

            object ICloneable.Clone()
            {
                return new Cloneable(_value);
            }

            public int CompareTo(Cloneable other)
            {
                if (ReferenceEquals(this, other))
                {
                    return 0;
                }
                if (ReferenceEquals(null, other))
                {
                    return 1;
                }
                return _value.CompareTo(other._value);
            }

            public bool Equals(Cloneable other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _value == other._value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((Cloneable)obj);
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }

        private class Constructer : IComparable<Constructer>, IEquatable<Constructer>
        {
            private readonly int _value;

            public Constructer(int value)
            {
                _value = value;
            }

            public Constructer(Constructer prototype)
            {
                _value = prototype._value;
            }

            public int CompareTo(Constructer other)
            {
                if (ReferenceEquals(this, other))
                {
                    return 0;
                }
                if (ReferenceEquals(null, other))
                {
                    return 1;
                }
                return _value.CompareTo(other._value);
            }

            public bool Equals(Constructer other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _value == other._value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((Constructer)obj);
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }

        private class Deconstructer : IComparable<Deconstructer>, IEquatable<Deconstructer>
        {
            private readonly int _value;

            public Deconstructer(int value)
            {
                _value = value;
            }

            public int CompareTo(Deconstructer other)
            {
                if (ReferenceEquals(this, other))
                {
                    return 0;
                }
                if (ReferenceEquals(null, other))
                {
                    return 1;
                }
                return _value.CompareTo(other._value);
            }

            public void Deconstruct(out int value)
            {
                value = _value;
            }

            public bool Equals(Deconstructer other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _value == other._value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((Deconstructer)obj);
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }

        private class GenericCloneable : IComparable<GenericCloneable>, IEquatable<GenericCloneable>, ICloneable<GenericCloneable>
        {
            private readonly int _value;

            public GenericCloneable(int value)
            {
                _value = value;
            }

            public object Clone()
            {
                return ((ICloneable<GenericCloneable>)this).Clone();
            }

            GenericCloneable ICloneable<GenericCloneable>.Clone()
            {
                return new GenericCloneable(_value);
            }

            public int CompareTo(GenericCloneable other)
            {
                if (ReferenceEquals(this, other))
                {
                    return 0;
                }
                if (ReferenceEquals(null, other))
                {
                    return 1;
                }
                return _value.CompareTo(other._value);
            }

            public bool Equals(GenericCloneable other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _value == other._value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((GenericCloneable)obj);
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }

        private class MockerA : IComparable<MockerA>, IEquatable<MockerA>
        {
            private readonly int _value;

            public MockerA(int value)
            {
                _value = value;
            }

            public MockerA Clone()
            {
                return new MockerA(_value);
            }

            public int CompareTo(MockerA other)
            {
                if (ReferenceEquals(this, other))
                {
                    return 0;
                }
                if (ReferenceEquals(null, other))
                {
                    return 1;
                }
                return _value.CompareTo(other._value);
            }

            public bool Equals(MockerA other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _value == other._value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((MockerA)obj);
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }

        private class MockerB : IComparable<MockerB>, IEquatable<MockerB>
        {
            private readonly int _value;

            public MockerB(int value)
            {
                _value = value;
            }

            public object Clone()
            {
                return new MockerB(_value);
            }

            public int CompareTo(MockerB other)
            {
                if (ReferenceEquals(this, other))
                {
                    return 0;
                }
                if (ReferenceEquals(null, other))
                {
                    return 1;
                }
                return _value.CompareTo(other._value);
            }

            public bool Equals(MockerB other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _value == other._value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((MockerB)obj);
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }

        private class Other : IComparable<Other>, IEquatable<Other>
        {
            private readonly int _value;

            public Other(int value)
            {
                _value = value;
            }

            public int CompareTo(Other other)
            {
                if (ReferenceEquals(this, other))
                {
                    return 0;
                }
                if (ReferenceEquals(null, other))
                {
                    return 1;
                }
                return _value.CompareTo(other._value);
            }

            public bool Equals(Other other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _value == other._value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((Other)obj);
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }

        [Serializable]
        private class Serializable : IComparable<Serializable>, IEquatable<Serializable>
        {
            private readonly int _value;

            public Serializable(int value)
            {
                _value = value;
            }

            public int CompareTo(Serializable other)
            {
                if (ReferenceEquals(this, other))
                {
                    return 0;
                }
                if (ReferenceEquals(null, other))
                {
                    return 1;
                }
                return _value.CompareTo(other._value);
            }

            public bool Equals(Serializable other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _value == other._value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((Serializable)obj);
            }

            public override int GetHashCode()
            {
                return _value;
            }
        }
    }
}

#endif