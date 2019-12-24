#if LESSTHAN_NET40 || NETSTANDARD1_0

using Theraot.Core;

namespace System.Numerics
{
    internal partial struct BigIntegerBuilder
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