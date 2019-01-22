#if LESSTHAN_NET40

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
        public static explicit operator Complex(decimal value)
        {
            return new Complex((double)value, 0);
        }

        public static explicit operator Complex(BigInteger value)
        {
            return new Complex((double)value, 0);
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

        public static readonly Complex ImaginaryOne = new Complex(0, 1);
        public static readonly Complex One = new Complex(1, 0);
        public static readonly Complex Zero = new Complex(0, 0);

        public Complex(double real, double imaginary)
        {
            Imaginary = imaginary;
            Real = real;
        }

        public double Imaginary { get; }

        public double Magnitude => Math.Sqrt((Imaginary * Imaginary) + (Real * Real));

        public double Phase => Math.Atan2(Imaginary, Real);

        public double Real { get; }

        private double MagnitudeSquared => (Imaginary * Imaginary) + (Real * Real);

        public static Complex operator -(Complex left, Complex right)
        {
            return new Complex
                (
                    left.Real - right.Real,
                    left.Imaginary - right.Imaginary
                );
        }

        public static Complex operator -(Complex value)
        {
            return new Complex(-value.Real, -value.Imaginary);
        }

        public static bool operator !=(Complex left, Complex right)
        {
            return !left.Real.Equals(right.Real) || !left.Imaginary.Equals(right.Imaginary);
        }

        public static Complex operator *(Complex left, Complex right)
        {
            return new Complex
                (
                    (left.Real * right.Real) - (left.Imaginary * right.Imaginary),
                    (left.Real * right.Imaginary) + (left.Imaginary * right.Real)
                );
        }

        public static Complex operator /(Complex left, Complex right)
        {
            var d = (right.Real * right.Real) + (right.Imaginary * right.Imaginary);
            return new Complex
                (
                    ((left.Real * right.Real) + (left.Imaginary * right.Imaginary)) / d,
                    ((left.Imaginary * right.Real) - (left.Real * right.Imaginary)) / d
                );
        }

        public static Complex operator +(Complex left, Complex right)
        {
            return new Complex
                (
                    left.Real + right.Real,
                    left.Imaginary + right.Imaginary
                );
        }

        public static bool operator ==(Complex left, Complex right)
        {
            return left.Real.Equals(right.Real) && left.Imaginary.Equals(right.Imaginary);
        }

        public static double Abs(Complex value)
        {
            return Math.Sqrt((value.Imaginary * value.Imaginary) + (value.Real * value.Real));
        }

        public static Complex Acos(Complex value)
        {
            return -ImaginaryOne * Log(value + (ImaginaryOne * Sqrt(One - (value * value))));
        }

        public static Complex Add(Complex left, Complex right)
        {
            return new Complex
                (
                    left.Real + right.Real,
                    left.Imaginary + right.Imaginary
                );
        }

        public static Complex Asin(Complex value)
        {
            return -ImaginaryOne * Log((ImaginaryOne * value) + Sqrt(One - (value * value)));
        }

        public static Complex Atan(Complex value)
        {
            return ImaginaryOne / new Complex(2, 0) * (Log(One - (ImaginaryOne * value)) - Log(One + (ImaginaryOne * value)));
        }

        public static Complex Conjugate(Complex value)
        {
            return new Complex(value.Real, -value.Imaginary);
        }

        public static Complex Cos(Complex value)
        {
            return new Complex
                (
                    Math.Cos(value.Real) * Math.Cosh(value.Imaginary),
                    -Math.Sin(value.Real) * Math.Sinh(value.Imaginary)
                );
        }

        public static Complex Cosh(Complex value)
        {
            return new Complex
                (
                    Math.Cosh(value.Real) * Math.Cos(value.Imaginary),
                    -Math.Sinh(value.Real) * Math.Sin(value.Imaginary)
                );
        }

        public static Complex Divide(Complex dividend, Complex divisor)
        {
            var divisorMagnitudeSquared = divisor.MagnitudeSquared;
            return new Complex
                (
                    ((dividend.Real * divisor.Real) + (dividend.Imaginary * divisor.Imaginary)) / divisorMagnitudeSquared,
                    ((dividend.Imaginary * divisor.Real) - (dividend.Real * divisor.Imaginary)) / divisorMagnitudeSquared
                );
        }

        public static Complex Exp(Complex value)
        {
            var expReal = Math.Exp(value.Real);
            return new Complex
                (
                    expReal * Math.Cos(value.Imaginary),
                    expReal * Math.Sin(value.Imaginary)
                );
        }

        public static Complex FromPolarCoordinates(double magnitude, double phase)
        {
            return new Complex
                (
                    magnitude * Math.Cos(phase),
                    magnitude * Math.Sin(phase)
                );
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
                    (left.Real * right.Real) - (left.Imaginary * right.Imaginary),
                    (left.Real * right.Imaginary) + (left.Imaginary * right.Real)
                );
        }

        public static Complex Negate(Complex value)
        {
            return -value;
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
            return value == Zero ? value : One / value;
        }

        public static Complex Sin(Complex value)
        {
            return new Complex
                (
                    Math.Sin(value.Real) * Math.Cosh(value.Imaginary),
                    Math.Cos(value.Real) * Math.Sinh(value.Imaginary)
                );
        }

        public static Complex Sinh(Complex value)
        {
            return new Complex
                (
                    Math.Sinh(value.Real) * Math.Cos(value.Imaginary),
                    Math.Cosh(value.Real) * Math.Sin(value.Imaginary)
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
                    left.Real - right.Real,
                    left.Imaginary - right.Imaginary
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

        public bool Equals(Complex other)
        {
            return Real.Equals(other.Real) && Imaginary.Equals(other.Imaginary);
        }

        public override bool Equals(object obj)
        {
            if (obj is Complex other)
            {
                return Real.Equals(other.Real) && Imaginary.Equals(other.Imaginary);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Real.GetHashCode() ^ Imaginary.GetHashCode();
        }

        public override string ToString()
        {
            return $"({Real}, {Imaginary})";
        }

        public string ToString(IFormatProvider provider)
        {
            return string.Format(provider, "({0}, {1})", Real, Imaginary);
        }

        public string ToString(string format)
        {
#pragma warning disable CA1305 // Specify IFormatProvider
            return $"({Real.ToString(format)}, {Imaginary.ToString(format)})";
#pragma warning restore CA1305 // Specify IFormatProvider
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"({Real.ToString(format, formatProvider)}, {Imaginary.ToString(format, formatProvider)})";
        }
    }
}

#endif