#if LESSTHAN_NET40 || NETSTANDARD1_0

using System.Text;

namespace System.Numerics
{
    internal sealed class BigNumberBuffer
    {
        public readonly StringBuilder Digits;

        private BigNumberBuffer(StringBuilder digits)
        {
            Digits = digits;
        }

        public bool Negative { get; set; }

        public int Scale { get; set; }

        public static BigNumberBuffer Create()
        {
            return new BigNumberBuffer(new StringBuilder());
        }
    }
}

#endif