#if LESSTHAN_NET45

#pragma warning disable CA2201 // Do not raise reserved exception types

using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace System.Reflection
{
    public static class MethodInfoTheraotExtensions
    {
        /// <summary>
        ///     Creates a closed delegate for the given (dynamic)method.
        /// </summary>
        /// <param name="methodInfo">The MethodInfo for the target method.</param>
        /// <param name="delegateType">Delegate type with a matching signature.</param>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType)
        {
            if (methodInfo == null)
            {
                throw new NullReferenceException();
            }

            if (methodInfo is DynamicMethod dynamicMethod)
            {
                return dynamicMethod.CreateDelegate(delegateType);
            }

            return Delegate.CreateDelegate(delegateType, methodInfo);
        }

        /// <summary>
        ///     Creates a closed delegate for the given (dynamic)method.
        /// </summary>
        /// <param name="methodInfo">The MethodInfo for the target method.</param>
        /// <param name="delegateType">Delegate type with a matching signature.</param>
        /// <param name="target">The object to which the delegate is bound, or null to treat method as static.</param>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType, object target)
        {
            if (methodInfo == null)
            {
                throw new NullReferenceException();
            }

            if (methodInfo is DynamicMethod dynamicMethod)
            {
                return dynamicMethod.CreateDelegate(delegateType, target);
            }

            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }
    }
}

#endif