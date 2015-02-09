using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public partial class WeakNeedle<T> : INeedle<T>, IEquatable<WeakNeedle<T>>
        where T : class
    {
        private readonly int _hashCode;
        private readonly bool _trackResurrection;
        private GCHandle _handle;
        private int _managedDisposal;

        public WeakNeedle()
            : this(false)
        {
            //Empty
        }

        public WeakNeedle(bool trackResurrection)
        {
            _trackResurrection = trackResurrection;
            _hashCode = NeedleHelper.GetNextHashCode();
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public WeakNeedle(T target)
            : this(target, false)
        {
            //Empty
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public WeakNeedle(T target, bool trackResurrection)
        {
            _trackResurrection = trackResurrection;
            SetTargetValue(target);
            _hashCode = NeedleHelper.GetNextHashCode();
        }

        public Exception Error
        {
            get
            {
                if (_handle.IsAllocated)
                {
                    object target;
                    try
                    {
                        target = _handle.Target; //Throws InvalidOperationException
                    }
                    catch (InvalidOperationException)
                    {
                        return null;
                    }
                    if (target is ExceptionStructNeedle<T>)
                    {
                        return ((ExceptionStructNeedle<T>)target).Error;
                    }
                }
                return null;
            }
        }

        public bool IsAlive
        {
            get
            {
                if (_handle.IsAllocated)
                {
                    object target;
                    try
                    {
                        target = _handle.Target; //Throws InvalidOperationException
                    }
                    catch (InvalidOperationException)
                    {
                        return false;
                    }
                    var needle = target as INeedle<T>;
                    if (needle != null)
                    {
                        return needle.IsAlive;
                    }
                }
                return false;
            }
        }

        public bool IsFaulted
        {
            get
            {
                if (_handle.IsAllocated)
                {
                    object target;
                    try
                    {
                        target = _handle.Target; //Throws InvalidOperationException
                    }
                    catch (InvalidOperationException)
                    {
                        return false;
                    }
                    if (target is ExceptionStructNeedle<T>)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public virtual bool TrackResurrection
        {
            get
            {
                return _trackResurrection;
            }
        }

        public virtual T Value
        {
            [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
            get
            {
                if (_handle.IsAllocated)
                {
                    object target;
                    try
                    {
                        target = _handle.Target; //Throws InvalidOperationException
                    }
                    catch (InvalidOperationException)
                    {
                        return null;
                    }
                    var needle = target as INeedle<T>;
                    if (needle != null)
                    {
                        return needle.Value;
                    }
                }
                return null;
            }
            [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
            set
            {
                SetTargetValue(value);
            }
        }

        public static explicit operator T(WeakNeedle<T> needle)
        {
            return Check.NotNullArgument(needle, "needle").Value;
        }

        public static implicit operator WeakNeedle<T>(T field)
        {
            return new WeakNeedle<T>(field);
        }

        public static bool operator !=(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public sealed override bool Equals(object obj)
        {
            var _obj = obj as WeakNeedle<T>;
            if (!ReferenceEquals(null, _obj))
            {
                return EqualsExtractedExtracted(this, _obj);
            }
            else
            {
                var __obj = obj as T;
                if (__obj == null)
                {
                    return false;
                }
                else
                {
                    var target = Value;
                    return IsAlive && EqualityComparer<T>.Default.Equals(target, __obj);
                }
            }
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
            else
            {
                return "<Dead Needle>";
            }
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected void SetTargetError(Exception error)
        {
            var target = new ExceptionStructNeedle<T>(error);
            if (_status == -1 || !ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
            {
                ReleaseExtracted();
                _handle = CreateHandle(target, _trackResurrection);
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
                        }
                        catch (InvalidOperationException)
                        {
                            // Empty
                        }
                    }
                    _handle = CreateHandle(target, _trackResurrection);
                    if (oldHandle.IsAllocated)
                    {
                        oldHandle.Free();
                        try
                        {
                            oldHandle.Free();
                        }
                        catch (InvalidOperationException)
                        {
                            //Empty
                        }
                    }
                }
                finally
                {
                    System.Threading.Interlocked.Decrement(ref _status);
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected void SetTargetValue(T value)
        {
            var target = new StructNeedle<T>(value);
            if (_status == -1 || !ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
            {
                ReleaseExtracted();
                _handle = CreateHandle(target, _trackResurrection);
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
                            // Empty
                        }
                    }
                    _handle = CreateHandle(target, _trackResurrection);
                    if (oldHandle.IsAllocated)
                    {
                        oldHandle.Free();
                        try
                        {
                            oldHandle.Free();
                        }
                        catch (InvalidOperationException)
                        {
                            //Empty
                        }
                    }
                }
                finally
                {
                    System.Threading.Interlocked.Decrement(ref _status);
                }
            }
        }

        private static bool EqualsExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            else
            {
                return !ReferenceEquals(right, null) && EqualsExtractedExtracted(left, right);
            }
        }

        private static bool EqualsExtractedExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            var _left = left.Value;
            if (left.IsAlive)
            {
                var _right = right.Value;
                return right.IsAlive && EqualityComparer<T>.Default.Equals(_left, _right);
            }
            else
            {
                return !right.IsAlive;
            }
        }

        private static bool NotEqualsExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            else
            {
                return ReferenceEquals(right, null) || NotEqualsExtractedExtracted(left, right);
            }
        }

        private static bool NotEqualsExtractedExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            var _left = left.Value;
            if (left.IsAlive)
            {
                var _right = right.Value;
                return !right.IsAlive || !EqualityComparer<T>.Default.Equals(_left, _right);
            }
            else
            {
                return right.IsAlive;
            }
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        private static GCHandle CreateHandle(object target, bool trackResurrection)
        {
            return GCHandle.Alloc(target, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        private void ReleaseExtracted()
        {
            if (_handle.IsAllocated)
            {
                try
                {
                    _handle.Free();
                }
                catch (InvalidOperationException)
                {
                    //Empty
                }
            }
        }

        private void ReportManagedDisposal()
        {
            Thread.VolatileWrite(ref _managedDisposal, 1);
        }
    }
}