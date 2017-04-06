#if NET20 || NET30 || NET35

using System.Text;

namespace System.Numerics
{
    internal class BigNumberBuffer
    {
        public StringBuilder Digits;

        public bool Negative
        {
            get; set;
        }

        public int Scale
        {
            get; set;
        }

        public static BigNumberBuffer Create()
        {
            return new BigNumberBuffer
            {
                Digits = new StringBuilder()
            };
        }
    }
}

#endif