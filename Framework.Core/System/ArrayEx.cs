using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13

using Theraot.Collections.ThreadSafe;

#endif

namespace System
{
#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13

    public static partial class ArrayEx
    {
        private static readonly CacheDict<Type, Array> _emptyArrays = new CacheDict<Type, Array>(256);

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static T[] Empty<T>()
        {
            var type = typeof(T);
            if (type == typeof(Type))
            {
                return (T[])(object)TypeEx.EmptyTypes;
            }

            if (_emptyArrays.TryGetValue(type, out var array))
            {
                return (T[])array;
            }

            var result = new T[0];
            _emptyArrays[type] = result;
            return result;
        }
    }

#else

    public static partial class ArrayEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static T[] Empty<T>()
        {
            return Array.Empty<T>();
        }
    }

#endif

    public static partial class ArrayEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ReadOnlyCollection<T> AsReadOnly<T>(T[] array)
        {
#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            return new ReadOnlyCollection<T>(array);
#else
            return Array.AsReadOnly(array);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Converter<TInput, TOutput> converter)
        {
#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            var newArray = new TOutput[array.Length];
            for (var index = 0; index < array.Length; index++)
            {
                newArray[index] = converter(array[index]);
            }

            return newArray;
#else
            return Array.ConvertAll(array, converter);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Copy(Array sourceArray, Array destinationArray, long length)
        {
#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
            if (length > int.MaxValue || length < int.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Arrays larger than 2GB are not supported.");
            }

            Array.Copy(sourceArray, destinationArray, (int)length);
#else
            Array.Copy(sourceArray, destinationArray, length);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Copy(Array sourceArray, long sourceIndex, Array destinationArray, long destinationIndex, long length)
        {
#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
            if (sourceIndex > int.MaxValue || sourceIndex < int.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceIndex), "Arrays larger than 2GB are not supported.");
            }

            if (destinationIndex > int.MaxValue || destinationIndex < int.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(destinationIndex), "Arrays larger than 2GB are not supported.");
            }

            if (length > int.MaxValue || length < int.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Arrays larger than 2GB are not supported.");
            }

            Array.Copy(sourceArray, (int)sourceIndex, destinationArray, (int)destinationIndex, (int)length);
#else
            Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Fill<T>(T[] array, T value)
        {
#if TARGETS_NET || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            for (int index = 0; index < array.Length; index++)
            {
                array[index] = value;
            }
#else
            Array.Fill(array, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void ForEach<T>(T[] array, Action<T> action)
        {
#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in array)
            {
                action(item);
            }
#else
            Array.ForEach(array, action);
#endif
        }
    }
}