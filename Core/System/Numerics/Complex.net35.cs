#if NET35

// Complex.cs: Complex number support

// Author:
//   Miguel de Icaza (miguel@gnome.org)
//   Marek Safar (marek.safar@gmail.com)
//   Jb Evain (jbevain@novell.com)

// Copyright 2009, 2010 Novell, Inc.

// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:

// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Numerics
{
    public struct Complex : IEquatable<Complex>, IFormattable
    {
        public static readonly Complex ImaginaryOne = new Complex(0, 1);
        public static readonly Complex One = new Complex(1, 0);
        public static readonly Complex Zero = new Complex(0, 0);
        private readonly double _imaginary;
        private readonly double _real;

        public Complex(double real, double imaginary)
        {
            _imaginary = imaginary;
            _real = real;
        }

        public double Imaginary
        {
            get
            {
                return _imaginary;
            }
        }

        public double Magnitude
        {
            get
            {
                return Math.Sqrt((_imaginary * _imaginary) + (_real * _real));
            }
        }

        public double Phase
        {
            get
            {
                return Math.Atan2(_imaginary, _real);
            }
        }

        public double Real
        {
            get
            {
                return _real;
            }
        }

        private double MagnitudeSquared
        {
            get
            {
                return (_imaginary * _imaginary) + (_real * _real);
            }
        }

        public static double Abs(Complex value)
        {
            return Math.Sqrt((value._imaginary * value._imaginary) + (value._real * value._real));
        }

        public static Complex Acos(Complex value)
        {
            return -ImaginaryOne * Log(value + (ImaginaryOne * Sqrt(One - (value * value))));
        }

        public static Complex Add(Complex left, Complex right)
        {
            return new Complex
                (
                    left._real + right._real,
                    left._imaginary + right._imaginary
                );
        }

        public static Complex Asin(Complex value)
        {
            return -ImaginaryOne * Log((ImaginaryOne * value) + Sqrt(One - (value * value)));
        }

        public static Complex Atan(Complex value)
        {
            return (ImaginaryOne / new Complex(2, 0)) * (Log(One - (ImaginaryOne * value)) - Log(One + (ImaginaryOne * value)));
        }

        public static Complex Conjugate(Complex value)
        {
            return new Complex(value._real, -value._imaginary);
        }

        public static Complex Cos(Complex value)
        {
            return new Complex
                (
                    Math.Cos(value._real) * Math.Cosh(value._imaginary),
                    -Math.Sin(value._real) * Math.Sinh(value._imaginary)
                );
        }

        public static Complex Cosh(Complex value)
        {
            return new Complex
                (
                    Math.Cosh(value._real) * Math.Cos(value._imaginary),
                    -Math.Sinh(value._real) * Math.Sin(value._imaginary)
                );
        }

        public static Complex Divide(Complex dividend, Complex divisor)
        {
            double divisorMagnitudeSquared = divisor.MagnitudeSquared;
            return new Complex
                (
                    ((dividend._real * divisor._real) + (dividend._imaginary * divisor._imaginary)) / divisorMagnitudeSquared,
                    ((dividend._imaginary * divisor._real) - (dividend._real * divisor._imaginary)) / divisorMagnitudeSquared
                );
        }

        public static Complex Exp(Complex value)
        {
            var expReal = Math.Exp(value._real);
            return new Complex
                (
                    expReal * Math.Cos(value._imaginary),
                    expReal * Math.Sin(value._imaginary)
                );
        }

        public static explicit operator Complex(decimal value)
        {
            return new Complex((double)value, 0);
        }

        public static explicit operator Complex(BigInteger value)
        {
            return new Complex((double)value, 0);
        }

        public static Complex FromPolarCoordinates(double magnitude, double phase)
        {
            return new Complex
                (
                    magnitude * Math.Cos(phase),
                    magnitude * Math.Sin(phase)
                );
        }

        public static implicit operator Complex(byte value)
        {
            return new Complex(value, 0);
        }

        public static implicit operator Complex(double value)
        {
            return new Complex(value, 0);
        }

        public static implicit operator Complex(short value)
        {
            return new Complex(value, 0);
        }

        public static implicit operator Complex(int value)
        {
            return new Complex(value, 0);
        }

        public static implicit operator Complex(long value)
        {
            return new Complex(value, 0);
        }

        [CLSCompliant(false)]
        public static implicit operator Complex(sbyte value)
        {
            return new Complex(value, 0);
        }

        public static implicit operator Complex(float value)
        {
            return new Complex(value, 0);
        }

        [CLSCompliant(false)]
        public static implicit operator Complex(ushort value)
        {
            return new Complex(value, 0);
        }

        [CLSCompliant(false)]
        public static implicit operator Complex(uint value)
        {
            return new Complex(value, 0);
        }

        [CLSCompliant(false)]
        public static implicit operator Complex(ulong value)
        {
            return new Complex(value, 0);
        }

        public static Complex Log(Complex value)
        {
            return new Complex
                (
                    Math.Log(Abs(value)),
                    value.Phase
                );
        }

        public static Complex Log(Complex value, double baseValue)
        {
            return Log(value) / Log(new Complex(baseValue, 0));
        }

        public static Complex Log10(Complex value)
        {
            return Log(value, 10);
        }

        public static Complex Multiply(Complex left, Complex right)
        {
            return new Complex
                (
                    (left._real * right._real) - (left._imaginary * right._imaginary),
                    (left._real * right._imaginary) + (left._imaginary * right._real)
                );
        }

        public static Complex Negate(Complex value)
        {
            return -value;
        }

        public static Complex operator -(Complex left, Complex right)
        {
            return new Complex
                (
                    left._real - right._real,
                    left._imaginary - right._imaginary
                );
        }

        public static Complex operator -(Complex value)
        {
            return new Complex(-value._real, -value._imaginary);
        }

        public static bool operator !=(Complex left, Complex right)
        {
            return !left._real.Equals(right._real) || !left._imaginary.Equals(right._imaginary);
        }

        public static Complex operator *(Complex left, Complex right)
        {
            return new Complex
                (
                    (left._real * right._real) - (left._imaginary * right._imaginary),
                    (left._real * right._imaginary) + (left._imaginary * right._real)
                );
        }

        public static Complex operator /(Complex left, Complex right)
        {
            double rsri = (right._real * right._real) + (right._imaginary * right._imaginary);
            return new Complex
                (
                    ((left._real * right._real) + (left._imaginary * right._imaginary)) / rsri,
                    ((left._imaginary * right._real) - (left._real * right._imaginary)) / rsri
                );
        }

        public static Complex operator +(Complex left, Complex right)
        {
            return new Complex
                (
                    left._real + right._real,
                    left._imaginary + right._imaginary
                );
        }

        public static bool operator ==(Complex left, Complex right)
        {
            return left._real.Equals(right._real) && left._imaginary.Equals(right._imaginary);
        }

        public static Complex Pow(Complex value, double power)
        {
            return Pow(value, new Complex(power, 0));
        }

        public static Complex Pow(Complex value, Complex power)
        {
            return Exp(Log(value) * power);
        }

        public static Complex Reciprocal(Complex value)
        {
            if (value == Zero)
            {
                return value;
            }
            else
            {
                return One / value;
            }
        }

        public static Complex Sin(Complex value)
        {
            return new Complex
                (
                    Math.Sin(value._real) * Math.Cosh(value._imaginary),
                    Math.Cos(value._real) * Math.Sinh(value._imaginary)
                );
        }

        public static Complex Sinh(Complex value)
        {
            return new Complex
                (
                    Math.Sinh(value._real) * Math.Cos(value._imaginary),
                    Math.Cosh(value._real) * Math.Sin(value._imaginary)
                );
        }

        public static Complex Sqrt(Complex value)
        {
            return FromPolarCoordinates(Math.Sqrt(value.Magnitude), value.Phase / 2);
        }

        public static Complex Subtract(Complex left, Complex right)
        {
            return new Complex
                (
                    left._real - right._real,
                    left._imaginary - right._imaginary
                );
        }

        public static Complex Tan(Complex value)
        {
            return Sin(value) / Cos(value);
        }

        public static Complex Tanh(Complex value)
        {
            return Sinh(value) / Cosh(value);
        }

        public bool Equals(Complex value)
        {
            return _real.Equals(value._real) && _imaginary.Equals(value._imaginary);
        }

        public override bool Equals(object obj)
        {
            if (obj is Complex)
            {
                var other = (Complex)obj;
                return _real.Equals(other._real) && _imaginary.Equals(other._imaginary);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _real.GetHashCode() ^ _imaginary.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", _real, _imaginary);
        }

        public string ToString(IFormatProvider provider)
        {
            return string.Format(provider, "({0}, {1})", _real, _imaginary);
        }

        public string ToString(string format)
        {
            return string.Format("({0}, {1})", _real.ToString(format), _imaginary.ToString(format));
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format("({0}, {1})", _real.ToString(format, provider), _imaginary.ToString(format, provider));
        }
    }
}

#endif