// CancellationTokenRegistration.cs
//
// Authors:
//       Jérémie "Garuma" Laval <jeremie.laval@gmail.com>
//       Alfonso J. Ramos <theraot@gmail.com>
//
// Copyright (c) 2009 Jérémie "Garuma" Laval
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

#if NET20 || NET30 || NET35

namespace System.Threading
{
    public struct CancellationTokenRegistration : IDisposable, IEquatable<CancellationTokenRegistration>
    {
        private readonly int _id;
        private CancellationTokenSource _source;

        internal CancellationTokenRegistration(int id, CancellationTokenSource source)
        {
            _id = id;
            _source = source;
        }

        public static bool operator !=(CancellationTokenRegistration left, CancellationTokenRegistration right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(CancellationTokenRegistration left, CancellationTokenRegistration right)
        {
            return left.Equals(right);
        }

        public void Dispose()
        {
            var source = _source;
            if (!ReferenceEquals(source, null))
            {
                if (source.RemoveCallback(this))
                {
                    _source = null;
                }
            }
        }

        public bool Equals(CancellationTokenRegistration other)
        {
            return _id == other._id && _source == other._source;
        }

        public override bool Equals(object obj)
        {
            return (obj is CancellationTokenRegistration registration) && Equals(registration);
        }

        public override int GetHashCode()
        {
            return _id;
        }

        internal bool Equals(int id, CancellationTokenSource source)
        {
            return _id == id && _source == source;
        }
    }
}

#endif