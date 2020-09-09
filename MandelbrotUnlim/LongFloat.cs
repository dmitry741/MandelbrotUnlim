using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelbrotUnlim
{
    /// <summary>
    /// Класс реализующий длинную вещественную арифметику.
    /// </summary>
    class LongFloat
    {
        int sign;
        List<int> digits;
        int exponent;

        bool IsZero => digits.Count == 1 && digits[0] == 0;

        private void InitFromString(string s)
        {
            int index;

            if (s[0] == '-')
            {
                sign = -1;
                index = 1;
            }
            else
            {
                sign = 1;
                index = 0;
            }

            exponent = s.Length - index;

            while (index < s.Length)
            {
                if (s[index] == '.')
                    exponent = sign == 1 ? index : index - 1;
                else
                    digits.Add(s[index] - '0');

                index++;
            }
        }

        private void RemoveZeroes()
        {
            int n = Math.Max(1, exponent);

            while (digits.Count > n && digits[digits.Count - 1] == 0)
                digits.RemoveAt(digits.Count - 1);

            while (digits.Count > 1 && digits[0] == 0)
            {
                digits.RemoveAt(0);
                exponent--;
            }

            while (digits.Count > n && digits[digits.Count - 1] == 0)
                digits.RemoveAt(digits.Count - 1);

            if (IsZero)
            {
                exponent = 1;
                sign = 1;
            }

            Normalize();
        }

        private void Normalize()
        {
            const int cDivDigits = 1000;
            int start = Math.Max(exponent, 0);
            int realDigits = digits.Count - start;

            if (realDigits >= cDivDigits)
            {
                int count;
                int maxCount = 0;

                int i = start;

                while (i < digits.Count)
                {
                    count = 0;

                    while (i < digits.Count && digits[i] == 9)
                    {
                        count++;
                        i++;
                    }

                    if (count > maxCount)
                        maxCount = count;

                    i++;
                }

                if (maxCount > cDivDigits * 4 / 5)
                {
                    i = digits.Count - 1;

                    do
                    {
                        count = 0;

                        while (i > 0 && digits[i] != 9)
                            i--;

                        while (i > 0 && digits[i] == 9)
                        {
                            count++;
                            i--;
                        }
                    } while (count != maxCount);

                    int startIndex = digits[0] + i + 1;
                    digits.RemoveRange(startIndex, digits.Count - startIndex + 1);
                    digits[i]++;
                }
            }
        }

        public LongFloat(string s)
        {
            InitFromString(s);
        }

        public LongFloat(double d)
        {
            InitFromString(d.ToString());
        }

        public static LongFloat FromString(string s)
        {
            return new LongFloat(s);
        }

        public static LongFloat FromDouble(double d)
        {
            return new LongFloat(d);
        }
    }
}
