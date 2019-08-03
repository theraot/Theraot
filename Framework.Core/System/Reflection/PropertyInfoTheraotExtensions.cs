#if LESSTHAN_NET45

#pragma warning disable CA2201 // Do not raise reserved exception types

using System.Runtime.CompilerServices;

namespace System.Reflection
{
    public static class PropertyInfoTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static object GetValue(this PropertyInfo info, object obj)
        {
            if (info == null)
            {
                throw new NullReferenceException();
            }
            return info.GetValue(obj, null);
        }
    }
}

#endif