// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace System.Numerics
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct DoubleUlong
    {
        [FieldOffset(0)]
        public double Dbl;

        [FieldOffset(0)]
        public ulong Uu;
    }

    internal static class NumericsHelpers
    {
        private const int _uintBitCount = 32;

        public static void GetDoubleParts(double dbl, out int sign, out int exp, out ulong man, out bool fFinite)
        {
            Contract.Ensures(Contract.ValueAtReturn(out sign) == +1 || Contract.ValueAtReturn(out sign) == -1);

            DoubleUlong du;
            du.Uu = 0;
            du.Dbl = dbl;

            sign = 1 - ((int)(du.Uu >> 62) & 2);
            man = du.Uu & 0x000FFFFFFFFFFFFF;
            exp = (int)(du.Uu >> 52) & 0x7FF;
            if (exp == 0)
            {
                // Denormalized number.
                fFinite = true;
                if (man != 0)
                {
                    exp = -1074;
                }
            }
            else if (exp == 0x7FF)
            {
                // NaN or Inifite.
                fFinite = false;
                exp = int.MaxValue;
            }
            else
            {
                fFinite = true;
                man |= 0x0010000000000000;
                exp -= 1075;
            }
        }

        public static double GetDoubleFromParts(int sign, int exp, ulong man)
        {
            DoubleUlong du;
            du.Dbl = 0;

            if (man == 0)
            {
                du.Uu = 0;
            }
            else
            {
                // Normalize so that 0x0010 0000 0000 0000 is the highest bit set.
                var cbitShift = CbitHighZero(man) - 11;
                if (cbitShift < 0)
                {
                    man >>= -cbitShift;
                }
                else
                {
                    man <<= cbitShift;
                }

                exp -= cbitShift;
                Debug.Assert((man & 0xFFF0000000000000) == 0x0010000000000000);

                // Move the point to just behind the leading 1: 0x001.0 0000 0000 0000
                // (52 bits) and skew the exponent (by 0x3FF == 1023).
                exp += 1075;

                if (exp >= 0x7FF)
                {
                    // Infinity.
                    du.Uu = 0x7FF0000000000000;
                }
                else if (exp <= 0)
                {
                    // Denormalized.
                    exp--;
                    if (exp < -52)
                    {
                        // Underflow to zero.
                        du.Uu = 0;
                    }
                    else
                    {
                        du.Uu = man >> -exp;
                        Debug.Assert(du.Uu != 0);
                    }
                }
                else
                {
                    // Mask off the implicit high bit.
                    du.Uu = (man & 0x000FFFFFFFFFFFFF) | ((ulong)exp << 52);
                }
            }

            if (sign < 0)
            {
                du.Uu |= 0x8000000000000000;
            }

            return du.Dbl;
        }

        // Do an in-place two's complement. "Dangerous" because it causes
        // a mutation and needs to be used with care for immutable types.
        public static void DangerousMakeTwosComplement(uint[] d)
        {
            if (d != null && d.Length > 0)
            {
                d[0] = ~d[0] + 1;

                var i = 1;
                // first do complement and +1 as long as carry is needed
                for (; d[i - 1] == 0 && i < d.Length; i++)
                {
                    d[i] = ~d[i] + 1;
                }
                // now ones complement is sufficient
                for (; i < d.Length; i++)
                {
                    d[i] = ~d[i];
                }
            }
        }

        public static ulong MakeUlong(uint uHi, uint uLo)
        {
            return ((ulong)uHi << _uintBitCount) | uLo;
        }

        public static uint Abs(int a)
        {
            var mask = (uint)(a >> 31);
            return ((uint)a ^ mask) - mask;
        }

        public static uint CombineHash(uint u1, uint u2)
        {
            return ((u1 << 7) | (u1 >> 25)) ^ u2;
        }

        public static int CombineHash(int n1, int n2)
        {
            return (int)CombineHash((uint)n1, (uint)n2);
        }

        public static int CbitHighZero(uint u)
        {
            if (u == 0)
            {
                return 32;
            }

            var cbit = 0;
            if ((u & 0xFFFF0000) == 0)
            {
                cbit += 16;
                u <<= 16;
            }
            if ((u & 0xFF000000) == 0)
            {
                cbit += 8;
                u <<= 8;
            }
            if ((u & 0xF0000000) == 0)
            {
                cbit += 4;
                u <<= 4;
            }
            if ((u & 0xC0000000) == 0)
            {
                cbit += 2;
                u <<= 2;
            }
            if ((u & 0x80000000) == 0)
            {
                cbit += 1;
            }

            return cbit;
        }

        public static int CbitHighZero(ulong uu)
        {
            if ((uu & 0xFFFFFFFF00000000) == 0)
            {
                return 32 + CbitHighZero((uint)uu);
            }

            return CbitHighZero((uint)(uu >> 32));
        }
    }
}