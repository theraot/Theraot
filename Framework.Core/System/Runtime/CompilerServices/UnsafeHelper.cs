#if LESSTHAN_NET45

using System.Reflection.Emit;
using Theraot.Collections.ThreadSafe;

namespace System.Runtime.CompilerServices
{
    internal static partial class UnsafeHelper
    {
        private static readonly CacheDict<Type, Delegate> _addByteOffset = new(CreateByteOffsetDelegate, 256);

        private delegate ref T AddByteOffsetDelegate<T>(ref T source, IntPtr byteOffset);

        public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
        {
            return ref ((AddByteOffsetDelegate<T>)_addByteOffset[typeof(T)])(ref source, byteOffset);
        }

        private static Delegate CreateByteOffsetDelegate(Type type)
        {
            var refT = type.MakeByRefType();
            Type[] methodArgs = { refT, typeof(IntPtr) };
            var method = new DynamicMethod(nameof(AddByteOffset), refT, methodArgs);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(AddByteOffsetDelegate<>).MakeGenericType(type));
        }
    }

    internal static partial class UnsafeHelper
    {
        private static readonly CacheDict<Type, Delegate> _asPointer = new(CreateAsPointerDelegate, 256);

        private unsafe delegate void* AsPointerDelegate<T>(ref T source);

        public static unsafe void* AsPointer<T>(ref T source)
        {
            return ((AsPointerDelegate<T>)_asPointer[typeof(T)])(ref source);
        }

        private static Delegate CreateAsPointerDelegate(Type type)
        {
            var refT = type.MakeByRefType();
            Type[] methodArgs = { refT };
            var method = new DynamicMethod(nameof(AsPointer), typeof(void*), methodArgs);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Conv_U);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(AsPointerDelegate<>).MakeGenericType(type));
        }
    }
}

#endif