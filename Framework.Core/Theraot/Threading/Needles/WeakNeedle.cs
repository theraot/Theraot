// Needed for Workaround

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public partial class WeakNeedle<T> : IEquatable<WeakNeedle<T>>, IRecyclableNeedle<T>, ICacheNeedle<T>
        where T : class
    {
        private readonly int _hashCode;
        private readonly bool _trackResurrection;
        private volatile bool _faultExpected;
        private GCHandle _handle;
        private int _managedDisposal;

        public WeakNeedle()
            : this(false)
        {
            // Empty
        }

        public WeakNeedle(bool trackResurrection)
        {
            _trackResurrection = trackResurrection;
            _hashCode = base.GetHashCode();
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public WeakNeedle(T target)
            : this(target, false)
        {
            // Empty
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public WeakNeedle(T target, bool trackResurrection)
        {
            if (target == null)
            {
                _hashCode = base.GetHashCode();
            }
            else
            {
                SetTargetValue(target);
                _hashCode = target.GetHashCode();
            }
            _trackResurrection = trackResurrection;
        }

        public Exception Exception
        {
            get
            {
                if (ReadTarget(out object target))
                {
                    if (target is Exception exception && _faultExpected)
                    {
                        return exception;
                    }
                }
                return null;
            }
        }

        bool IPromise.IsCanceled
        {
            get { return false; }
        }

        bool IPromise.IsCompleted
        {
            get { return true; }
        }

        public bool IsAlive
        {
            get
            {
                if (ReadTarget(out object target))
                {
                    if (target is T && !_faultExpected)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsFaulted
        {
            get
            {
                if (ReadTarget(out object target))
                {
                    if (target is Exception && _faultExpected)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public virtual bool TrackResurrection
        {
            get { return _trackResurrection; }
        }

        public virtual T Value
        {
            [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
            get
            {
                if (ReadTarget(out object target))
                {
                    if (target is T inner && !_faultExpected)
                    {
                        return inner;
                    }
                }
                return null;
            }

            [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
            set { SetTargetValue(value); }
        }

        public static explicit operator T(WeakNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }
            return needle.Value;
        }

        public static implicit operator WeakNeedle<T>(T field)
        {
            return new WeakNeedle<T>(field);
        }

        public static bool operator !=(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            return ReferenceEquals(right, null) || NotEqualsExtractedExtracted(left, right);
        }

        public static bool operator ==(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return !ReferenceEquals(right, null) && EqualsExtractedExtracted(left, right);
        }

        public sealed override bool Equals(object obj)
        {
            var needle = obj as WeakNeedle<T>;
            if (needle != null)
            {
                return EqualsExtractedExtracted(this, needle);
            }
            if (obj is T value)
            {
                var target = Value;
                return IsAlive && EqualityComparer<T>.Default.Equals(target, value);
            }
            return false;
        }

        public bool Equals(WeakNeedle<T> other)
        {
            return !ReferenceEquals(other, null) && EqualsExtractedExtracted(this, other);
        }

        public void Free()
        {
            Dispose();
        }

        public sealed override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var target = Value;
            if (IsAlive)
            {
                return target.ToString();
            }
            return "<Dead Needle>";
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public virtual bool TryGetValue(out T value)
        {
            value = null;
            if (ReadTarget(out object target))
            {
                if (target is T inner)
                {
                    value = inner;
                    return true;
                }
            }
            return false;
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected void SetTargetError(Exception error)
        {
            _faultExpected = true;
            WriteTarget(error);
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected void SetTargetValue(T value)
        {
            _faultExpected = false;
            WriteTarget(value);
        }

        private static bool EqualsExtractedExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            var leftValue = left.Value;
            if (left.IsAlive)
            {
                var rightValue = right.Value;
                return right.IsAlive && EqualityComparer<T>.Default.Equals(leftValue, rightValue);
            }
            return !right.IsAlive;
        }

        private static bool NotEqualsExtractedExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            var leftValue = left.Value;
            if (left.IsAlive)
            {
                var rightValue = right.Value;
                return !right.IsAlive || !EqualityComparer<T>.Default.Equals(leftValue, rightValue);
            }
            return right.IsAlive;
        }

        private bool ReadTarget(out object target)
        {
            target = null;
            if (_handle.IsAllocated)
            {
                try
                {
                    target = _handle.Target; // Throws InvalidOperationException
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        private void ReleaseExtracted()
        {
            if (_handle.IsAllocated)
            {
                try
                {
                    _handle.Free(); // Throws InvalidOperationException
                }
                catch (InvalidOperationException)
                {
                    // Empty
                }
            }
        }

        private void ReportManagedDisposal()
        {
            Volatile.Write(ref _managedDisposal, 1);
        }

        private void WriteTarget(object target)
        {
            if (_disposeStatus == -1 || !ThreadingHelper.SpinWaitRelativeSet(ref _disposeStatus, 1, -1))
            {
                ReleaseExtracted();
                _handle = GCHandle.Alloc(target, _trackResurrection ? GCHandleType.Weak : GCHandleType.WeakTrackResurrection);
                if (Interlocked.CompareExchange(ref _managedDisposal, 0, 1) == 1)
                {
                    GC.ReRegisterForFinalize(this);
                }
                UnDispose();
            }
            else
            {
                try
                {
                    var oldHandle = _handle;
                    if (oldHandle.IsAllocated)
                    {
                        try
                        {
                            oldHandle.Target = target;
                            return;
                        }
                        catch (InvalidOperationException)
                        {
                            _handle = GCHandle.Alloc(target, _trackResurrection ? GCHandleType.Weak : GCHandleType.WeakTrackResurrection);
                        }
                    }
                    else
                    {
                        _handle = GCHandle.Alloc(target, _trackResurrection ? GCHandleType.Weak : GCHandleType.WeakTrackResurrection);
                    }
                    if (oldHandle.IsAllocated)
                    {
                        oldHandle.Free();
                        try
                        {
                            oldHandle.Free();
                        }
                        catch (InvalidOperationException)
                        {
                            // Empty
                        }
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref _disposeStatus);
                }
            }
        }
    }
}