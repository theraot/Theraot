// Needed for NET40

#if FAT

namespace Theraot.Core
{
    public static partial class NumericHelper
    {
        public static int UncheckedDecrement(this int value)
        {
            unchecked
            {
                value--;
                return value;
            }
        }

        public static short UncheckedDecrement(this short value)
        {
            unchecked
            {
                value--;
                return value;
            }
        }

        public static long UncheckedDecrement(this long value)
        {
            unchecked
            {
                value--;
                return value;
            }
        }

        public static int UncheckedIncrement(this int value)
        {
            unchecked
            {
                value++;
                return value;
            }
        }

        public static short UncheckedIncrement(this short value)
        {
            unchecked
            {
                value++;
                return value;
            }
        }

        public static long UncheckedIncrement(this long value)
        {
            unchecked
            {
                value++;
                return value;
            }
        }
    }
}

#endif