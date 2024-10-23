
using System;

namespace BiomeExtractorsMod.Common
{
    public struct Fraction(int num, int den = 1) : IComparable, IComparable<Fraction>
    {
        public static readonly Fraction Zero = new();
        public static readonly Fraction One = new(1);

        public Fraction() : this(0) { }
        private readonly double Value => (double)this;
        public int Num = num;
        public int Den = den;
        public readonly Fraction ToCanonical()
        {
            int n = Num;
            int d = Den;
            while (n > 0 && d > 0)
            {
                if(n>d) n%=d;
                else d%=n;
            }
            int gcd = n | d;
            int num = Num / gcd;
            int den = Den / gcd;
            if (num < 0 && den < 0) { num = -num; den = -den; }
            return new Fraction(num, den);
        }

        public static explicit operator float(Fraction a) => a.Den == 0 ? float.NaN : (float)a.Num / (float)a.Den;
        public static explicit operator double(Fraction a) => a.Den == 0 ? double.NaN : (double)a.Num / (double)a.Den;
        public static explicit operator decimal(Fraction a) => a.Den == 0 ? decimal.MaxValue : (decimal)a.Num / (decimal)a.Den;
        public static Fraction operator +(Fraction a) => a;
        public static Fraction operator -(Fraction a) => new(-a.Num, a.Den);
        public static Fraction operator +(Fraction a, Fraction b)
        {
            int da = a.Den;
            int db = b.Den;
            while (da > 0 && db > 0)
            {
                if (da > db) da %= db;
                else db %= da;
            }
            int gcd = da | db;
            int mcm = a.Den * b.Den / gcd;
            int num = (b.Num * a.Den + a.Num * b.Den) / gcd;
            return new Fraction(num, mcm);
        }
        public static Fraction operator -(Fraction a, Fraction b) => a+(-b);
        public static Fraction operator +(Fraction a, int b) => a + new Fraction(b);
        public static Fraction operator -(Fraction a, int b) => a+(-b);
        public static Fraction operator ++(Fraction a) => a+1;
        public static Fraction operator --(Fraction a) => a-1;
        public static Fraction operator *(Fraction a, Fraction b) => new(a.Num * b.Num, a.Den * b.Den);
        public static Fraction operator /(Fraction a, Fraction b) => new(a.Num * b.Den, a.Den * b.Num);
        public static Fraction operator *(Fraction a, int b) => a * new Fraction(b);
        public static Fraction operator /(Fraction a, int b) => a * new Fraction(1, b);
        public static Fraction operator /(int a, Fraction b) => new(a * b.Den, b.Num);
        public static Fraction operator ^(Fraction a, int b) => new(a.Num ^ b, a.Den ^ b);
        public static bool operator ==(Fraction a, Fraction b) => (double)a == (double)b;
        public static bool operator ==(Fraction a, int b) => (double)a == b;
        public static bool operator !=(Fraction a, Fraction b) => (double)a != (double)b;
        public static bool operator !=(Fraction a, int b) => (double)a != b;
        public static bool operator >(Fraction a, Fraction b) => (double)a > (double)b;
        public static bool operator >(Fraction a, int b) => (double)a > b;
        public static bool operator <(Fraction a, Fraction b) => (double)a < (double)b;
        public static bool operator <(Fraction a, int b) => (double)a < b;
        public static bool operator >=(Fraction a, Fraction b) => (double)a >= (double)b;
        public static bool operator >=(Fraction a, int b) => (double)a >= b;
        public static bool operator <=(Fraction a, Fraction b) => (double)a <= (double)b;
        public static bool operator <=(Fraction a, int b) => (double)a <= b;
        public override readonly bool Equals(object obj)
        {
            if (obj is Fraction frc) return this == frc;
            return Value.Equals(obj);
        }
        public override readonly int GetHashCode() => Value.GetHashCode();

        public override readonly string ToString() => Den == 1 ? Num.ToString() : $"{Num}/{Den}";

        public readonly int CompareTo(object? obj)
        {
            if (obj is Fraction other) return CompareTo(other);
            return Value.CompareTo(obj);
        }
        public readonly int CompareTo(Fraction other)
        {
            if (this > other) return 1;
            if (this < other) return -1;
            return 0;
        }
    }
}
