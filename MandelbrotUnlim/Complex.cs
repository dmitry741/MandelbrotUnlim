using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LongFloatLib;

namespace MandelbrotUnlim
{
    /// <summary>
    /// Класс комплексных чисел
    /// </summary>
    public class Complex
    {
        public Complex(LongFloat re, LongFloat im)
        {
            Re = re;
            Im = im;
        }

        /// <summary>
        /// Реальная часть комплексного числа.
        /// </summary>
        public LongFloat Re { get; private set; }

        /// <summary>
        /// Мнимая часть комплекстного числа.
        /// </summary>
        public LongFloat Im { get; private set; }

        /// <summary>
        /// Квадрат модуля комплексного числа.
        /// </summary>
        public LongFloat ModuleInSquare => Re * Re + Im * Im;

        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Re + b.Re, a.Im + b.Im);
        }

        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Re - b.Re, a.Im - b.Im);
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Re * b.Re - a.Im * b.Im, a.Im * b.Re + a.Re * b.Im);
        }

        public static Complex operator ^(Complex a, int power)
        {
            LongFloat one = LongFloat.FromDouble(1);
            LongFloat zero = LongFloat.FromDouble(0);

            Complex result = new Complex(one, zero);

            for (int i = 0; i < power; i++)
            {
                result *= a;
            }

            return result;
        }
    }
}
