#if LESSTHAN_NETSTANDARD13

#pragma warning disable CA2201 // Do not raise reserved exception types

using System.Runtime.CompilerServices;

namespace System.Reflection
{
    public static class PropertyInfoExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo? GetGetMethod(this PropertyInfo property)
        {
            if (property == null)
            {
                throw new NullReferenceException(nameof(property));
            }

            var result = property.GetMethod;
            return result?.IsPublic != true ? null : result;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo? GetGetMethod(this PropertyInfo property, bool nonPublic)
        {
            if (property == null)
            {
                throw new NullReferenceException(nameof(property));
            }

            var result = property.GetMethod;
            if (result == null || (!result.IsPublic && !nonPublic))
            {
                return null;
            }
            return result;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo? GetSetMethod(this PropertyInfo property)
        {
            if (property == null)
            {
                throw new NullReferenceException(nameof(property));
            }

            var result = property.SetMethod;
            return result?.IsPublic != true ? null : result;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo? GetSetMethod(this PropertyInfo property, bool nonPublic)
        {
            if (property == null)
            {
                throw new NullReferenceException(nameof(property));
            }

            var result = property.SetMethod;
            if (result == null || (!result.IsPublic && !nonPublic))
            {
                return null;
            }
            return result;
        }
    }
}

#endif