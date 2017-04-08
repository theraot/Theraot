// CancellationToken.cs
//
// Authors:
//       Jérémie "Garuma" Laval <jeremie.laval@gmail.com>
//       Marek Safar (marek.safar@gmail.com)
//       Alfonso J. Ramos (theraot@gmail.com)
//
// Copyright (c) 2009 Jérémie "Garuma" Laval
// Copyright 2011 Xamarin, Inc (http://www.xamarin.com)
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

using System.Diagnostics;
using Theraot.Core;

namespace System.Threading
{
    [DebuggerDisplay("IsCancellationRequested = {IsCancellationRequested}")]
    public struct CancellationToken
    {
        private readonly CancellationTokenSource _source;

        public CancellationToken(bool canceled)
            : this(canceled ? CancellationTokenSource.CanceledSource : null)
        {
            //Empty
        }

        internal CancellationToken(CancellationTokenSource source)
        {
            _source = source;
        }

        public static CancellationToken None
        {
            get
            {
                // simply return new struct value, it's the fastest option
                // and we don't have to bother with resetting source
                return new CancellationToken();
            }
        }

        public bool CanBeCanceled
        {
            get { return _source != null; }
        }

        public bool IsCancellationRequested
        {
            get
            {
                return _source == null ? false : Source.IsCancellationRequested;
            }
        }

        public WaitHandle WaitHandle
        {
            get { return Source.WaitHandle; }
        }

        private CancellationTokenSource Source
        {
            get { return _source ?? CancellationTokenSource.NoneSource; }
        }

        public static bool operator !=(CancellationToken left, CancellationToken right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(CancellationToken left, CancellationToken right)
        {
            return left.Equals(right);
        }

        public bool Equals(CancellationToken other)
        {
            return Source == other.Source;
        }

        public override bool Equals(object obj)
        {
            return (obj is CancellationToken) && Equals((CancellationToken)obj);
        }

        public override int GetHashCode()
        {
            return Source.GetHashCode();
        }

        public CancellationTokenRegistration Register(Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            return Register(callback, false);
        }

        public CancellationTokenRegistration Register(Action callback, bool useSynchronizationContext)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            if (_source == null)
            {
                return new CancellationTokenRegistration();
            }
            return _source.Register(callback, useSynchronizationContext);
        }

        public CancellationTokenRegistration Register(Action<object> callback, object state)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            return Register(() => callback(state), false);
        }

        public CancellationTokenRegistration Register(Action<object> callback, object state, bool useSynchronizationContext)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            return Register(() => callback(state), useSynchronizationContext);
        }

        public void ThrowIfCancellationRequested()
        {
            if (!ReferenceEquals(_source, null) && _source.IsCancellationRequested)
            {
                throw new NewOperationCanceledException(this);
            }
        }
    }
}

#endif