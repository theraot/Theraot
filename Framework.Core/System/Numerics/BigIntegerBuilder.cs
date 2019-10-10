#if LESSTHAN_NET40 || NETSTANDARD1_0

using Theraot.Core;

namespace System.Numerics
{
    internal struct BigIntegerBuilder
    {
        private uint[]? _bits;
        private bool _fWritable;
        private int _iuLast;

        private uint _uSmall;

        public BigIntegerBuilder(ref BigIntegerBuilder reg)
        {
            this = reg;
            if (!_fWritable)
            {
                return;
            }

            _fWritable = false;
            if (_iuLast != 0)
            {
                reg._fWritable = false;
            }
            else
            {
                _bits = null;
            }
        }

        public BigIntegerBuilder(int cuAlloc)
        {
            _iuLast = 0;
            _uSmall = 0;
            if (cuAlloc <= 1)
            {
                _bits = null;
                _fWritable = false;
            }
            else
            {
                _bits = new uint[cuAlloc];
                _fWritable = true;
            }
        }

        public BigIntegerBuilder(BigInteger bn)
        {
            _fWritable = false;
            _bits = bn.InternalBits;
            if (_bits != null)
            {
                _iuLast = _bits.Length - 1;
                _uSmall = _bits[0];
                while (_iuLast > 0 && _bits[_iuLast] == 0)
                {
                    _iuLast--;
                }
            }
            else
            {
                _iuLast = 0;
                _uSmall = NumericHelper.Abs(bn.InternalSign);
            }
        }

        public BigIntegerBuilder(BigInteger bn, ref int sign)
        {
            _fWritable = false;
            _bits = bn.InternalBits;
            var num = bn.InternalSign;
            var num1 = num >> 31;
            sign = (sign ^ num1) - num1;
            if (_bits != null)
            {
                _iuLast = _bits.Length - 1;
                _uSmall = _bits[0];
                while (_iuLast > 0 && _bits[_iuLast] == 0)
                {
                    _iuLast--;
                }
            }
            else
            {
                _iuLast = 0;
                _uSmall = (uint)((num ^ num1) - num1);
            }
        }

        public uint High => _iuLast != 0 ? _bits![_iuLast] : _uSmall;

        public int Size => _iuLast + 1;

        private int CuNonZero
        {
            get
            {
                var num = 0;
                for (var i = _iuLast; i >= 0; i--)
                {
                    if (_bits![i] != 0)
                    {
                        num++;
                    }
                }

                return num;
            }
        }

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

        public void Add(uint u)
        {
            if (_iuLast == 0)
            {
                _uSmall += u;
                if (_uSmall >= u)
                {
                    return;
                }

                SetSizeLazy(2);
                _bits![0] = _uSmall;
                _bits[1] = 1;
                return;
            }

            if (u == 0)
            {
                return;
            }

            var num2 = _bits![0] + u;
            if (num2 < u)
            {
                EnsureWritable(1);
                ApplyCarry(1);
            }
            else if (!_fWritable)
            {
                EnsureWritable();
            }

            _bits[0] = num2;
        }

        public void Add(ref BigIntegerBuilder reg)
        {
            if (reg._iuLast == 0)
            {
                Add(reg._uSmall);
                return;
            }

            if (_iuLast == 0)
            {
                var num = _uSmall;
                if (num != 0)
                {
                    Load(ref reg, 1);
                    Add(num);
                }
                else
                {
                    this = new BigIntegerBuilder(ref reg);
                }

                return;
            }

            EnsureWritable(Math.Max(_iuLast, reg._iuLast) + 1, 1);
            var num1 = reg._iuLast + 1;
            if (_iuLast < reg._iuLast)
            {
                num1 = _iuLast + 1;
                Array.Copy(reg._bits, _iuLast + 1, _bits, _iuLast + 1, reg._iuLast - _iuLast);
                _iuLast = reg._iuLast;
            }

            uint num2 = 0;
            for (var i = 0; i < num1; i++)
            {
                num2 = AddCarry(ref _bits![i], reg._bits![i], num2);
            }

            if (num2 != 0)
            {
                ApplyCarry(num1);
            }
        }

        public int CbitLowZero()
        {
            if (_iuLast == 0)
            {
                if ((_uSmall & 1) != 0 || _uSmall == 0)
                {
                    return 0;
                }

                return NumericHelper.CbitLowZero(_uSmall);
            }

            var num = 0;
            while (_bits![num] == 0)
            {
                num++;
            }

            var num1 = NumericHelper.CbitLowZero(_bits[num]);
            return num1 + (num * 32);
        }

        public void Div(ref BigIntegerBuilder regDen)
        {
            if (regDen._iuLast == 0)
            {
                DivMod(regDen._uSmall);
                return;
            }

            if (_iuLast == 0)
            {
                _uSmall = 0;
                return;
            }

            var bigIntegerBuilder = new BigIntegerBuilder();
            ModDivCore(ref this, ref regDen, true, ref bigIntegerBuilder);
            NumericHelper.Swap(ref this, ref bigIntegerBuilder);
        }

        public uint DivMod(uint num5)
        {
            if (num5 == 1)
            {
                return 0;
            }

            if (_iuLast == 0)
            {
                var num = _uSmall;
                _uSmall = num / num5;
                return num % num5;
            }

            EnsureWritable();
            var num1 = (ulong)0;
            for (var i = _iuLast; i >= 0; i--)
            {
                num1 = NumericHelper.BuildUInt64((uint)num1, _bits![i]);
                _bits[i] = (uint)(num1 / num5);
                num1 %= num5;
            }

            Trim();
            return (uint)num1;
        }

        public void EnsureWritable(int cu, int cuExtra)
        {
            if (_fWritable && _bits!.Length >= cu)
            {
                return;
            }

            var numArray = new uint[cu + cuExtra];
            if (_iuLast > 0)
            {
                if (_iuLast >= cu)
                {
                    _iuLast = cu - 1;
                }

                Array.Copy(_bits, numArray, _iuLast + 1);
            }

            _bits = numArray;
            _fWritable = true;
        }

        public void EnsureWritable(int cuExtra)
        {
            if (_fWritable)
            {
                return;
            }

            var numArray = new uint[_iuLast + 1 + cuExtra];
            Array.Copy(_bits, numArray, _iuLast + 1);
            _bits = numArray;
            _fWritable = true;
        }

        public void EnsureWritable()
        {
            EnsureWritable(0);
        }

        public void GetApproxParts(out int exp, out ulong man)
        {
            if (_iuLast == 0)
            {
                man = _uSmall;
                exp = 0;
                return;
            }

            var num = _iuLast - 1;
            man = NumericHelper.BuildUInt64(_bits![num + 1], _bits[num]);
            exp = num * 32;
            if (num <= 0)
            {
                return;
            }

            var num1 = NumericHelper.CbitHighZero(_bits[num + 1]);
            if (num1 <= 0)
            {
                return;
            }

            man = (man << num1) | (_bits[num - 1] >> (32 - num1));
            exp -= num1;
        }

        public BigInteger GetInteger(int sign)
        {
            GetIntegerParts(sign, out sign, out var numArray);
            return new BigInteger(sign, numArray);
        }

        public void Load(ref BigIntegerBuilder reg)
        {
            Load(ref reg, 0);
        }

        public void Load(ref BigIntegerBuilder reg, int cuExtra)
        {
            if (reg._iuLast != 0)
            {
                if (!_fWritable || _bits!.Length <= reg._iuLast)
                {
                    _bits = new uint[reg._iuLast + 1 + cuExtra];
                    _fWritable = true;
                }

                _iuLast = reg._iuLast;
                Array.Copy(reg._bits, _bits, _iuLast + 1);
            }
            else
            {
                _uSmall = reg._uSmall;
                _iuLast = 0;
            }
        }

        public int MakeOdd()
        {
            var num = CbitLowZero();
            if (num > 0)
            {
                ShiftRight(num);
            }

            return num;
        }

        public void Mod(ref BigIntegerBuilder regDen)
        {
            if (regDen._iuLast == 0)
            {
                Set(Mod(ref this, regDen._uSmall));
                return;
            }

            if (_iuLast == 0)
            {
                return;
            }

            var bigIntegerBuilder = new BigIntegerBuilder();
            ModDivCore(ref this, ref regDen, false, ref bigIntegerBuilder);
        }

        public void ModDiv(ref BigIntegerBuilder regDen, ref BigIntegerBuilder regQuo)
        {
            if (regDen._iuLast == 0)
            {
                regQuo.Set(DivMod(regDen._uSmall));
                NumericHelper.Swap(ref this, ref regQuo);
                return;
            }

            if (_iuLast == 0)
            {
                return;
            }

            ModDivCore(ref this, ref regDen, true, ref regQuo);
        }

        public void Mul(uint u)
        {
            switch (u)
            {
                case 0:
                    Set(0);
                    return;

                case 1:
                    return;

                default:
                    break;
            }

            if (_iuLast == 0)
            {
                Set(_uSmall * (ulong)u);
                return;
            }

            EnsureWritable(1);
            uint num = 0;
            for (var i = 0; i <= _iuLast; i++)
            {
                num = MulCarry(ref _bits![i], u, num);
            }

            if (num == 0)
            {
                return;
            }

            SetSizeKeep(_iuLast + 2, 0);
            _bits![_iuLast] = num;
        }

        public void Mul(ref BigIntegerBuilder regMul)
        {
            if (regMul._iuLast == 0)
            {
                Mul(regMul._uSmall);
            }
            else if (_iuLast != 0)
            {
                var num = _iuLast + 1;
                SetSizeKeep(num + regMul._iuLast, 1);
                var num1 = num;
                while (true)
                {
                    var num2 = num1 - 1;
                    num1 = num2;
                    if (num2 < 0)
                    {
                        break;
                    }

                    var num3 = _bits![num1];
                    _bits[num1] = 0;
                    uint num4 = 0;
                    for (var i = 0; i <= regMul._iuLast; i++)
                    {
                        num4 = AddMulCarry(ref _bits[num1 + i], regMul._bits![i], num3, num4);
                    }

                    if (num4 == 0)
                    {
                        continue;
                    }

                    for (var j = num1 + regMul._iuLast + 1; num4 != 0 && j <= _iuLast; j++)
                    {
                        num4 = AddCarry(ref _bits[j], 0, num4);
                    }

                    if (num4 == 0)
                    {
                        continue;
                    }

                    SetSizeKeep(_iuLast + 2, 0);
                    _bits[_iuLast] = num4;
                }
            }
            else
            {
                var num5 = _uSmall;
                if (num5 == 1)
                {
                    this = new BigIntegerBuilder(ref regMul);
                }
                else if (num5 != 0)
                {
                    Load(ref regMul, 1);
                    Mul(num5);
                }
            }
        }

        public void Mul(ref BigIntegerBuilder reg1, ref BigIntegerBuilder reg2)
        {
            if (reg1._iuLast == 0)
            {
                if (reg2._iuLast != 0)
                {
                    Load(ref reg2, 1);
                    Mul(reg1._uSmall);
                }
                else
                {
                    Set(reg1._uSmall * (ulong)reg2._uSmall);
                }
            }
            else if (reg2._iuLast != 0)
            {
                SetSizeClear(reg1._iuLast + reg2._iuLast + 2);
                uint[] numArray;
                uint[] numArray1;
                int num;
                int num1;
                if (reg1.CuNonZero > reg2.CuNonZero)
                {
                    numArray = reg2._bits!;
                    num = reg2._iuLast + 1;
                    numArray1 = reg1._bits!;
                    num1 = reg1._iuLast + 1;
                }
                else
                {
                    numArray = reg1._bits!;
                    num = reg1._iuLast + 1;
                    numArray1 = reg2._bits!;
                    num1 = reg2._iuLast + 1;
                }

                for (var i = 0; i < num; i++)
                {
                    var num2 = numArray[i];
                    if (num2 == 0)
                    {
                        continue;
                    }

                    uint num3 = 0;
                    var num4 = i;
                    var num5 = 0;
                    while (num5 < num1)
                    {
                        num3 = AddMulCarry(ref _bits![num4], num2, numArray1[num5], num3);
                        num5++;
                        num4++;
                    }

                    while (num3 != 0)
                    {
                        var num6 = num4;
                        num4 = num6 + 1;
                        num3 = AddCarry(ref _bits![num6], 0, num3);
                    }
                }

                Trim();
            }
            else
            {
                Load(ref reg1, 1);
                Mul(reg2._uSmall);
            }
        }

        public void Set(uint u)
        {
            _uSmall = u;
            _iuLast = 0;
        }

        public void Set(ulong uu)
        {
            var hi = NumericHelper.GetHi(uu);
            if (hi != 0)
            {
                SetSizeLazy(2);
                _bits![0] = (uint)uu;
                _bits[1] = hi;
            }
            else
            {
                _uSmall = NumericHelper.GetLo(uu);
                _iuLast = 0;
            }
        }

        public void ShiftLeft(int cbit)
        {
            if (cbit <= 0)
            {
                if (cbit < 0)
                {
                    ShiftRight(-cbit);
                }

                return;
            }

            ShiftLeft(cbit / 32, cbit % 32);
        }

        public void ShiftLeft(int cuShift, int cbitShift)
        {
            var num = _iuLast + cuShift;
            uint high = 0;
            if (cbitShift > 0)
            {
                high = High >> ((32 - cbitShift) & 31);
                if (high != 0)
                {
                    num++;
                }
            }

            if (num == 0)
            {
                _uSmall <<= cbitShift & 31;
                return;
            }

            var numArray = _bits;
            var flag = cuShift > 0;
            if (!_fWritable || _bits!.Length <= num)
            {
                _bits = new uint[num + 1];
                _fWritable = true;
                flag = false;
            }

            if (_iuLast == 0)
            {
                if (high != 0)
                {
                    _bits[cuShift + 1] = high;
                }

                _bits[cuShift] = _uSmall << (cbitShift & 31);
            }
            else if (cbitShift != 0)
            {
                var num1 = _iuLast;
                var num2 = _iuLast + cuShift;
                if (num2 < num)
                {
                    _bits[num] = high;
                }

                while (num1 > 0)
                {
                    _bits[num2] = (numArray![num1] << (cbitShift & 31)) | (numArray[num1 - 1] >> ((32 - cbitShift) & 31));
                    num1--;
                    num2--;
                }

                _bits[cuShift] = numArray![0] << (cbitShift & 31);
            }
            else
            {
                Array.Copy(numArray, 0, _bits, cuShift, _iuLast + 1);
            }

            _iuLast = num;
            if (flag)
            {
                Array.Clear(_bits, 0, cuShift);
            }
        }

        public void ShiftRight(int cbit)
        {
            if (cbit <= 0)
            {
                if (cbit < 0)
                {
                    ShiftLeft(-cbit);
                }

                return;
            }

            ShiftRight(cbit / 32, cbit % 32);
        }

        public void ShiftRight(int cuShift, int cbitShift)
        {
            if ((cuShift | cbitShift) == 0)
            {
                return;
            }

            if (cuShift > _iuLast)
            {
                Set(0);
                return;
            }

            if (_iuLast == 0)
            {
                _uSmall >>= cbitShift & 31;
                return;
            }

            var numArray = _bits;
            var num = _iuLast + 1;
            _iuLast -= cuShift;
            if (_iuLast != 0)
            {
                if (!_fWritable)
                {
                    _bits = new uint[_iuLast + 1];
                    _fWritable = true;
                }

                if (cbitShift <= 0)
                {
                    Array.Copy(numArray, cuShift, _bits, 0, _iuLast + 1);
                }
                else
                {
                    var num1 = cuShift + 1;
                    var num2 = 0;
                    while (num1 < num)
                    {
                        _bits![num2] = (numArray![num1 - 1] >> (cbitShift & 31)) | (numArray[num1] << ((32 - cbitShift) & 31));
                        num1++;
                        num2++;
                    }

                    _bits![_iuLast] = numArray![num - 1] >> (cbitShift & 31);
                    Trim();
                }
            }
            else
            {
                _uSmall = numArray![cuShift] >> (cbitShift & 31);
            }
        }

        public void Sub(ref int sign, uint u)
        {
            if (_iuLast == 0)
            {
                if (u > _uSmall)
                {
                    _uSmall = u - _uSmall;
                    sign = -sign;
                }
                else
                {
                    _uSmall -= u;
                }

                return;
            }

            if (u == 0)
            {
                return;
            }

            EnsureWritable();
            var num = _bits![0];
            _bits[0] = num - u;
            if (num >= u)
            {
                return;
            }

            ApplyBorrow(1);
            Trim();
        }

        public void Sub(ref int sign, ref BigIntegerBuilder reg)
        {
            if (reg._iuLast == 0)
            {
                Sub(ref sign, reg._uSmall);
                return;
            }

            if (_iuLast == 0)
            {
                var num = _uSmall;
                if (num != 0)
                {
                    Load(ref reg);
                    Sub(ref sign, num);
                }
                else
                {
                    this = new BigIntegerBuilder(ref reg);
                }

                sign = -sign;
                return;
            }

            if (_iuLast < reg._iuLast)
            {
                SubRev(ref reg);
                sign = -sign;
                return;
            }

            var num1 = reg._iuLast + 1;
            if (_iuLast == reg._iuLast)
            {
                _iuLast = BigInteger.GetDiffLength(_bits!, reg._bits!, _iuLast + 1) - 1;
                if (_iuLast < 0)
                {
                    _iuLast = 0;
                    _uSmall = 0;
                    return;
                }

                var num2 = _bits![_iuLast];
                var num3 = reg._bits![_iuLast];
                if (_iuLast == 0)
                {
                    if (num2 >= num3)
                    {
                        _uSmall = num2 - num3;
                    }
                    else
                    {
                        _uSmall = num3 - num2;
                        sign = -sign;
                    }

                    return;
                }

                if (num2 < num3)
                {
                    reg._iuLast = _iuLast;
                    SubRev(ref reg);
                    reg._iuLast = num1 - 1;
                    sign = -sign;
                    return;
                }

                num1 = _iuLast + 1;
            }

            EnsureWritable();
            uint num4 = 0;
            for (var i = 0; i < num1; i++)
            {
                num4 = SubBorrow(ref _bits![i], reg._bits![i], num4);
            }

            if (num4 != 0)
            {
                ApplyBorrow(num1);
            }

            Trim();
        }

        internal void GetIntegerParts(int signSrc, out int sign, out uint[]? bits)
        {
            if (_iuLast == 0)
            {
                if (_uSmall <= 2147483647)
                {
                    sign = (int)(signSrc * _uSmall);
                    bits = null;
                    return;
                }

                if (_bits == null)
                {
                    _bits = new[] { _uSmall };
                }
                else if (_fWritable)
                {
                    _bits[0] = _uSmall;
                }
                else if (_bits[0] != _uSmall)
                {
                    _bits = new[] { _uSmall };
                }
            }

            sign = signSrc;
            var length = _bits!.Length - _iuLast - 1;
            if (length <= 1)
            {
                if (length == 0 || _bits[_iuLast + 1] == 0)
                {
                    _fWritable = false;
                    bits = _bits;
                    return;
                }

                if (_fWritable)
                {
                    _bits[_iuLast + 1] = 0;
                    _fWritable = false;
                    bits = _bits;
                    return;
                }
            }

            bits = _bits;
            Array.Resize(ref bits, _iuLast + 1);
            if (!_fWritable)
            {
                _bits = bits;
            }
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

        private void ApplyBorrow(int iuMin)
        {
            for (var i = iuMin; i <= _iuLast; i++)
            {
                var num = _bits![i];
                var num1 = num;
                _bits[i] = num - 1;
                if (num1 > 0)
                {
                    return;
                }
            }
        }

        private void ApplyCarry(int iu)
        {
            while (true)
            {
                if (iu <= _iuLast)
                {
                    var num = _bits![iu] + 1;
                    var num1 = num;
                    _bits[iu] = num;
                    if (num1 <= 0)
                    {
                        iu++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (_iuLast + 1 == _bits!.Length)
                    {
                        Array.Resize(ref _bits, _iuLast + 2);
                    }

                    _iuLast++;
                    _bits[_iuLast] = 1;
                    break;
                }
            }
        }

        private ulong GetHigh2(int cu)
        {
            if (cu - 1 <= _iuLast)
            {
                return NumericHelper.BuildUInt64(_bits![cu - 1], _bits[cu - 2]);
            }

            return cu - 2 != _iuLast ? 0 : _bits![cu - 2];
        }

        private void SetSizeClear(int cu)
        {
            if (cu <= 1)
            {
                _iuLast = 0;
                _uSmall = 0;
                return;
            }

            if (!_fWritable || _bits!.Length < cu)
            {
                _bits = new uint[cu];
                _fWritable = true;
            }
            else
            {
                Array.Clear(_bits, 0, cu);
            }

            _iuLast = cu - 1;
        }

        private void SetSizeKeep(int cu, int cuExtra)
        {
            if (cu <= 1)
            {
                if (_iuLast > 0)
                {
                    _uSmall = _bits![0];
                }

                _iuLast = 0;
                return;
            }

            if (!_fWritable || _bits!.Length < cu)
            {
                var numArray = new uint[cu + cuExtra];
                if (_iuLast != 0)
                {
                    Array.Copy(_bits, numArray, Math.Min(cu, _iuLast + 1));
                }
                else
                {
                    numArray[0] = _uSmall;
                }

                _bits = numArray;
                _fWritable = true;
            }
            else if (_iuLast + 1 < cu)
            {
                Array.Clear(_bits, _iuLast + 1, cu - _iuLast - 1);
                if (_iuLast == 0)
                {
                    _bits[0] = _uSmall;
                }
            }

            _iuLast = cu - 1;
        }

        private void SetSizeLazy(int cu)
        {
            if (cu <= 1)
            {
                _iuLast = 0;
                return;
            }

            if (!_fWritable || _bits!.Length < cu)
            {
                _bits = new uint[cu];
                _fWritable = true;
            }

            _iuLast = cu - 1;
        }

        private void SubRev(ref BigIntegerBuilder reg)
        {
            EnsureWritable(reg._iuLast + 1, 0);
            var num = _iuLast + 1;
            if (_iuLast < reg._iuLast)
            {
                Array.Copy(reg._bits, _iuLast + 1, _bits, _iuLast + 1, reg._iuLast - _iuLast);
                _iuLast = reg._iuLast;
            }

            uint num1 = 0;
            for (var i = 0; i < num; i++)
            {
                num1 = SubRevBorrow(ref _bits![i], reg._bits![i], num1);
            }

            if (num1 != 0)
            {
                ApplyBorrow(num);
            }

            Trim();
        }

        private void Trim()
        {
            if (_iuLast <= 0 || _bits![_iuLast] != 0)
            {
                return;
            }

            _uSmall = _bits[0];
            do
            {
                _iuLast--;
            } while (_iuLast > 0 && _bits[_iuLast] == 0);
        }
    }
}

#endif