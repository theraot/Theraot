// Needed for Workaround

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable ConstantNullCoalescingCondition

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public class WeakNeedle<T> : IEquatable<WeakNeedle<T>>, IRecyclable, ICacheNeedle<T>
        where T : class
    {
        private readonly int _hashCode;

        private readonly bool _trackResurrection;

        private WeakReference<T>? _handle;

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

        public WeakNeedle(T? target)
            : this(target, false)
        {
            // Empty
        }

        public WeakNeedle(T? target, bool trackResurrection)
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

        public Exception? Exception { get; private set; }

        public bool IsAlive => Exception == null && _handle?.TryGetTarget(out _) == true;

        bool IPromise.IsCompleted => true;
        public bool IsFaulted => Exception != null;

        public virtual bool TrackResurrection => _trackResurrection;

        public virtual T Value
        {
            get
            {
                if (Exception == null && _handle != null && _handle.TryGetTarget(out var target))
                {
                    return target;
                }

                throw new InvalidOperationException();
            }

            set => SetTargetValue(value);
        }

        public static bool operator !=(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            if (left is null)
            {
                return !(right is null);
            }

            return right is null || !EqualsExtractedExtracted(left, right);
        }

        public static bool operator ==(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            if (left is null)
            {
                return right is null;
            }

            return !(right is null) && EqualsExtractedExtracted(left, right);
        }

        public sealed override bool Equals(object? obj)
        {
            switch (obj)
            {
                case WeakNeedle<T> needle:
                    return EqualsExtractedExtracted(this, needle);

                case T value when TryGetValue(out var target):
                    return EqualityComparer<T?>.Default.Equals(target, value);

                default:
                    return false;
            }
        }

        public bool Equals(WeakNeedle<T> other)
        {
            return !(other is null) && EqualsExtractedExtracted(this, other);
        }

        public void Free()
        {
            SetTargetValue(null);
        }

        public sealed override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            if (Exception != null)
            {
                return $"<Faulted: {Exception}>";
            }

            return _handle != null && _handle.TryGetTarget(out var target) ? target.ToString() ?? "<?>" : "<Dead Needle>";
        }

        public virtual bool TryGetValue(out T value)
        {
            if (Exception == null && _handle != null && _handle.TryGetTarget(out var found))
            {
                value = found;
                return true;
            }
            value = default!;
            return false;
        }

        protected void SetTargetError(Exception error)
        {
            Exception = error;
            _handle = null;
        }

        protected void SetTargetValue(T? value)
        {
            if (value == null)
            {
                _handle = null;
            }
            else
            {
                if (_handle == null)
                {
                    _handle = new WeakReference<T>(value, _trackResurrection);
                }
                else
                {
                    _handle.SetTarget(value);
                }
            }

            Exception = null;
        }

        private static bool EqualsExtractedExtracted(WeakNeedle<T> left, WeakNeedle<T> right)
        {
            var leftException = left.Exception;
            var rightException = right.Exception;
            if (left.Exception != null || right.Exception != null)
            {
                return EqualityComparer<Exception?>.Default.Equals(leftException, rightException);
            }

            if (left.TryGetValue(out var leftValue) && right.TryGetValue(out var rightValue))
            {
                return EqualityComparer<T?>.Default.Equals(leftValue, rightValue);
            }

            return false;
        }
    }
}