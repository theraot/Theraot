#if LESSTHAN_NET40 || NETSTANDARD1_0

#pragma warning disable S4143 // Collection elements should not be replaced unconditionally

using Theraot.Core;

namespace System.Numerics
{
    internal partial struct BigIntegerBuilder
    {
        public static void Gcd(ref BigIntegerBuilder reg1, ref BigIntegerBuilder reg2)
        {
            if ((reg1._iuLast <= 0 || reg1._bits![0] != 0) && (reg2._iuLast <= 0 || reg2._bits![0] != 0))
            {
                LehmerGcd(ref reg1, ref reg2);
            }
            else
            {
                var num = reg1.MakeOdd();
                var num1 = reg2.MakeOdd();
                LehmerGcd(ref reg1, ref reg2);
                var num2 = Math.Min(num, num1);
                if (num2 > 0)
                {
                    reg1.ShiftLeft(num2);
                }
            }
        }

        public static uint Mod(ref BigIntegerBuilder regNum, uint num5)
        {
            if (num5 == 1)
            {
                return 0;
            }

            if (regNum._iuLast == 0)
            {
                return regNum._uSmall % num5;
            }

            var num = (ulong)0;
            for (var i = regNum._iuLast; i >= 0; i--)
            {
                num = NumericHelper.BuildUInt64((uint)num, regNum._bits![i]);
                num %= num5;
            }

            return (uint)num;
        }

        private static uint AddCarry(ref uint u1, uint u2, uint uCarry)
        {
            var num = u1 + (ulong)u2 + uCarry;
            u1 = (uint)num;
            return (uint)(num >> 32);
        }

        private static uint AddMulCarry(ref uint uAdd, uint uMul1, uint uMul2, uint uCarry)
        {
            var num = (uMul1 * (ulong)uMul2) + uAdd + uCarry;
            uAdd = (uint)num;
            return (uint)(num >> 32);
        }

        private static void LehmerGcd(ref BigIntegerBuilder reg1, ref BigIntegerBuilder reg2)
        {
            var num = 1;
            while (true)
            {
                var num1 = reg1._iuLast + 1;
                var num2 = reg2._iuLast + 1;
                if (num1 < num2)
                {
                    NumericHelper.Swap(ref reg1, ref reg2);
                    NumericHelper.Swap(ref num1, ref num2);
                }

                if (num2 == 1)
                {
                    if (num1 == 1)
                    {
                        reg1._uSmall = NumericHelper.GCD(reg1._uSmall, reg2._uSmall);
                    }
                    else if (reg2._uSmall != 0)
                    {
                        reg1.Set(NumericHelper.GCD(Mod(ref reg1, reg2._uSmall), reg2._uSmall));
                    }

                    return;
                }

                if (num1 == 2)
                {
                    break;
                }

                if (num2 > num1 - 2)
                {
                    var high2 = reg1.GetHigh2(num1);
                    var high21 = reg2.GetHigh2(num1);
                    var num3 = NumericHelper.CbitHighZero(high2 | high21);
                    if (num3 > 0)
                    {
                        high2 = (high2 << (num3 & 63)) | (reg1._bits![num1 - 3] >> ((32 - num3) & 31));
                        high21 = (high21 << (num3 & 63)) | (reg2._bits![num1 - 3] >> ((32 - num3) & 31));
                    }

                    if (high2 < high21)
                    {
                        NumericHelper.Swap(ref high2, ref high21);
                        NumericHelper.Swap(ref reg1, ref reg2);
                    }

                    if (high2 == ulong.MaxValue || high21 == ulong.MaxValue)
                    {
                        high2 >>= 1;
                        high21 >>= 1;
                    }

                    if (high2 == high21)
                    {
                        reg1.Sub(ref num, ref reg2);
                    }
                    else if (NumericHelper.GetHi(high21) != 0)
                    {
                        uint num4 = 1;
                        uint num5 = 0;
                        uint num6 = 0;
                        uint num7 = 1;
                        while (true)
                        {
                            uint num8 = 1;
                            var num9 = high2 - high21;
                            while (num9 >= high21 && num8 < 32)
                            {
                                num9 -= high21;
                                num8++;
                            }

                            if (num9 >= high21)
                            {
                                var num10 = high2 / high21;
                                if (num10 <= uint.MaxValue)
                                {
                                    num8 = (uint)num10;
                                    num9 = high2 - (num8 * high21);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            var num11 = num4 + (num8 * (ulong)num6);
                            var num12 = num5 + (num8 * (ulong)num7);
                            if (num11 > 2147483647 || num12 > 2147483647)
                            {
                                break;
                            }

                            if (num9 < num12 || num9 + num11 > high21 - num6)
                            {
                                break;
                            }

                            num4 = (uint)num11;
                            num5 = (uint)num12;
                            high2 = num9;
                            if (high2 > num5)
                            {
                                num8 = 1;
                                num9 = high21 - high2;
                                while (num9 >= high2 && num8 < 32)
                                {
                                    num9 -= high2;
                                    num8++;
                                }

                                if (num9 >= high2)
                                {
                                    var num13 = high21 / high2;
                                    if (num13 <= uint.MaxValue)
                                    {
                                        num8 = (uint)num13;
                                        num9 = high21 - (num8 * high2);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                num11 = num7 + (num8 * (ulong)num5);
                                num12 = num6 + (num8 * (ulong)num4);
                                if (num11 > 2147483647 || num12 > 2147483647)
                                {
                                    break;
                                }

                                if (num9 < num12 || num9 + num11 > high2 - num5)
                                {
                                    break;
                                }

                                num7 = (uint)num11;
                                num6 = (uint)num12;
                                high21 = num9;
                                if (high21 <= num6)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (num5 != 0)
                        {
                            reg1.SetSizeKeep(num2, 0);
                            reg2.SetSizeKeep(num2, 0);
                            var num14 = 0;
                            var num15 = 0;
                            for (var i = 0; i < num2; i++)
                            {
                                var num16 = reg1._bits![i];
                                var num17 = reg2._bits![i];
                                var num18 = ((long)num16 * num4) - ((long)num17 * num5) + num14;
                                var num19 = ((long)num17 * num7) - ((long)num16 * num6) + num15;
                                num14 = (int)(num18 >> 32);
                                num15 = (int)(num19 >> 32);
                                reg1._bits[i] = (uint)num18;
                                reg2._bits[i] = (uint)num19;
                            }

                            reg1.Trim();
                            reg2.Trim();
                        }
                        else if (high2 / 2 < high21)
                        {
                            reg1.Sub(ref num, ref reg2);
                        }
                        else
                        {
                            reg1.Mod(ref reg2);
                        }
                    }
                    else
                    {
                        reg1.Mod(ref reg2);
                    }
                }
                else
                {
                    reg1.Mod(ref reg2);
                }
            }

            reg1.Set(NumericHelper.GCD(reg1.GetHigh2(2), reg2.GetHigh2(2)));
        }

        private static void ModDivCore(ref BigIntegerBuilder regNum, ref BigIntegerBuilder regDen, bool fQuo, ref BigIntegerBuilder regQuo)
        {
            regQuo.Set(0);
            if (regNum._iuLast < regDen._iuLast)
            {
                return;
            }

            var num1 = regDen._iuLast + 1;
            var num2 = regNum._iuLast - regDen._iuLast;
            var num3 = num2;
            var num4 = regNum._iuLast;
            while (true)
            {
                if (num4 < num2)
                {
                    num3++;
                    break;
                }

                if (regDen._bits![num4 - num2] == regNum._bits![num4])
                {
                    num4--;
                }
                else
                {
                    if (regDen._bits[num4 - num2] < regNum._bits[num4])
                    {
                        num3++;
                    }

                    break;
                }
            }

            if (num3 == 0)
            {
                return;
            }

            if (fQuo)
            {
                regQuo.SetSizeLazy(num3);
            }

            var num5 = regDen._bits![num1 - 1];
            var num6 = regDen._bits[num1 - 2];
            var num7 = NumericHelper.CbitHighZero(num5);
            var num8 = 32 - num7;
            if (num7 > 0)
            {
                num5 = (num5 << (num7 & 31)) | (num6 >> (num8 & 31));
                num6 <<= num7 & 31;
                if (num1 > 2)
                {
                    num6 |= regDen._bits[num1 - 3] >> (num8 & 31);
                }
            }

            regNum.EnsureWritable();
            var num9 = num3;
            while (true)
            {
                var num10 = num9 - 1;
                num9 = num10;
                if (num10 < 0)
                {
                    break;
                }

                var num11 = num9 + num1 > regNum._iuLast ? 0 : regNum._bits![num9 + num1];
                var num12 = NumericHelper.BuildUInt64(num11, regNum._bits![num9 + num1 - 1]);
                var num13 = regNum._bits[num9 + num1 - 2];
                if (num7 > 0)
                {
                    num12 = (num12 << (num7 & 63)) | (num13 >> (num8 & 31));
                    num13 <<= num7 & 31;
                    if (num9 + num1 >= 3)
                    {
                        num13 |= regNum._bits[num9 + num1 - 3] >> (num8 & 31);
                    }
                }

                var num14 = num12 / num5;
                var num15 = (ulong)(uint)(num12 % num5);
                if (num14 > uint.MaxValue)
                {
                    num15 += num5 * (num14 - uint.MaxValue);
                    num14 = uint.MaxValue;
                }

                while (num15 <= uint.MaxValue && num14 * num6 > NumericHelper.BuildUInt64((uint)num15, num13))
                {
                    num14--;
                    num15 += num5;
                }

                if (num14 > 0)
                {
                    var num16 = (ulong)0;
                    for (var i = 0; i < num1; i++)
                    {
                        num16 += regDen._bits[i] * num14;
                        var num17 = (uint)num16;
                        num16 >>= 32;
                        if (regNum._bits[num9 + i] < num17)
                        {
                            num16++;
                        }

                        regNum._bits[num9 + i] -= num17;
                    }

                    if (num11 < num16)
                    {
                        uint num18 = 0;
                        for (var j = 0; j < num1; j++)
                        {
                            num18 = AddCarry(ref regNum._bits[num9 + j], regDen._bits[j], num18);
                        }

                        num14--;
                    }

                    regNum._iuLast = num9 + num1 - 1;
                }

                if (!fQuo)
                {
                    continue;
                }

                if (num3 != 1)
                {
                    regQuo._bits![num9] = (uint)num14;
                }
                else
                {
                    regQuo._uSmall = (uint)num14;
                }
            }

            regNum._iuLast = num1 - 1;
            regNum.Trim();
        }

        private static uint MulCarry(ref uint u1, uint u2, uint uCarry)
        {
            var num = (u1 * (ulong)u2) + uCarry;
            u1 = (uint)num;
            return (uint)(num >> 32);
        }

        private static uint SubBorrow(ref uint u1, uint u2, uint uBorrow)
        {
            var num = u1 - (ulong)u2 - uBorrow;
            u1 = (uint)num;
            return (uint)-(int)(num >> 32);
        }

        private static uint SubRevBorrow(ref uint u1, uint u2, uint uBorrow)
        {
            var num = u2 - (ulong)u1 - uBorrow;
            u1 = (uint)num;
            return (uint)-(int)(num >> 32);
        }
    }
}

#endif