// AtomicBoolean.cs
//
// Copyright (c) 2008 Jérémie "Garuma" Laval
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Runtime.CompilerServices;

namespace System.Threading
{
    public struct AtomicBooleanValue
    {
        private const int INT_Set = 1;
        private const int INT_UnSet = 0;
        private int _flag;

        public bool Value
        {
            get
            {
                return _flag == INT_Set;
            }
            set
            {
                Exchange(value);
            }
        }

        public static explicit operator bool(AtomicBooleanValue value)
        {
            return value.Value;
        }

        public static AtomicBooleanValue FromValue(bool value)
        {
            return new AtomicBooleanValue { Value = value };
        }

        public static implicit operator AtomicBooleanValue(bool value)
        {
            return FromValue(value);
        }

        public bool CompareAndExchange(bool expected, bool newVal)
        {
            int newTemp = newVal ? INT_Set : INT_UnSet;
            int expectedTemp = expected ? INT_Set : INT_UnSet;

            return Interlocked.CompareExchange(ref _flag, newTemp, expectedTemp) == expectedTemp;
        }

        public bool Equals(AtomicBooleanValue obj)
        {
            return _flag == obj._flag;
        }

        public override bool Equals(object obj)
        {
            return obj is AtomicBooleanValue && Equals((AtomicBooleanValue)obj);
        }

        public bool Exchange(bool newVal)
        {
            int newTemp = newVal ? INT_Set : INT_UnSet;
            return Interlocked.Exchange(ref _flag, newTemp) == INT_Set;
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        public bool TryRelaxedSet()
        {
            return _flag == INT_UnSet && !Exchange(true);
        }

        public bool TrySet()
        {
            return !Exchange(true);
        }
    }
    
}