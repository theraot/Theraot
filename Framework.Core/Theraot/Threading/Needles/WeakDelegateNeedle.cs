// Needed for Workaround

using System;
using System.Diagnostics;
using System.Reflection;
using Theraot.Reflection;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public sealed class WeakDelegateNeedle : WeakNeedle<Delegate>, IEquatable<Delegate>, IEquatable<WeakDelegateNeedle>
    {
        public WeakDelegateNeedle()
        {
            // Empty
        }

        public WeakDelegateNeedle(Delegate handler)
            : base(handler)
        {
            // Empty
        }

        public MethodInfo Method
        {
            get
            {
                return TryGetValue(out var value) ? value.GetMethodInfo() : null;
            }
        }

        public bool Equals(Delegate other)
        {
            var method = other.GetMethodInfo();
            return !(other is null) && Equals(method, other.Target);
        }

        public bool Equals(WeakDelegateNeedle other)
        {
            if (other is null)
            {
                return false;
            }

            bool isAlive = TryGetValue(out var value);
            bool isOtherAlive = TryGetValue(out var otherValue);

            if (!isAlive)
            {
                return !isOtherAlive;
            }

            if (!isOtherAlive)
            {
                return false;
            }

            var method = otherValue.GetMethodInfo();
            return value.GetMethodInfo().Equals(method) && value.Target != otherValue.Target;
        }

        public bool Equals(MethodInfo method, object target)
        {
            return TryGetValue(out var value) && value.DelegateEquals(method, target);
        }

        public void Invoke(object[] args)
        {
            TryInvoke(args);
        }

        public bool TryInvoke(object[] args)
        {
            if (!TryGetValue(out var value))
            {
                return false;
            }

            value.DynamicInvoke(args); // Throws TargetInvocationException
            return true;
        }

        public bool TryInvoke(object[] args, out object result)
        {
            if (TryGetValue(out var value))
            {
                result = value.DynamicInvoke(args);
                return true;
            }

            result = null;
            return false;
        }

        public bool TryInvoke<TResult>(object[] args, out TResult result)
        {
            if (TryGetValue(out var value))
            {
                result = (TResult)value.DynamicInvoke(args);
                return true;
            }

            result = default;
            return false;
        }
    }
}