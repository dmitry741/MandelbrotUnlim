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

        public LongFloat()
        {
	        sign = 1;
            digits = new List<int> { 0 };
	        exponent = 1;
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

        public static LongFloat operator-(LongFloat a) 
        {
            LongFloat result = new LongFloat();

            int[] digs = new int[a.digits.Count];
            a.digits.CopyTo(digs);

            result.digits = new List<int>(digs);
            result.exponent = a.exponent;
            result.sign = -a.sign;

	        return result;
        }

        public static LongFloat operator+(LongFloat a, LongFloat b)
        {
            if (a.sign == b.sign)
            {
                int exp1 = a.exponent;
                int exp2 = b.exponent;
                int exp = Math.Max(exp1, exp2);

                List<int> d1 = new List<int>(a.digits);
                List<int> d2 = new List<int>(b.digits);

                while (exp1 != exp)
                {
                    d1.Insert(0, 0);
                    exp1++;
                }

                while (exp2 != exp)
                {
                    d2.Insert(0, 0);
                    exp2++;
                }

                int size = Math.Max(d1.Count, d2.Count);

                while (d1.Count != size)
                    d1.Add(0);

                while (d2.Count != size)
                    d2.Add(0);

                int len = 1 + size;

                LongFloat res = new LongFloat();                

                int[] zeros = new int[len];
                Array.Clear(zeros, 0, len);

                res.digits = new List<int>(zeros);
                res.sign = a.sign;

                for (int i = 0; i < size; i++)
                    res.digits[i + 1] = d1[i] + d2[i];

                for (int i = len - 1; i > 0; i--)
                {
                    res.digits[i - 1] += res.digits[i] / 10;
                    res.digits[i] %= 10;
                }

                res.exponent = exp + 1;
                res.RemoveZeroes();

                return res;
            }

            if (a.sign == -1)
                return b - (-a);

            return a - (-b);
        }

        public static LongFloat operator -(LongFloat a, LongFloat b)
        {
            /*if (a.sign == 1 && b.sign == 1)
            {
                bool cmp = *this > x;

                long exp1 = cmp ? a.exponent : b.exponent;
                long exp2 = cmp ? b.exponent : a.exponent;
                long exp = Math.Max(exp1, exp2);

                vector<int> d1(cmp? digits : x.digits);
                vector<int> d2(cmp? x.digits : digits);

                while (exp1 != exp)
                {
                    d1.insert(d1.begin(), 0);
                    exp1++;
                }

                while (exp2 != exp)
                {
                    d2.insert(d2.begin(), 0);
                    exp2++;
                }

                int size = max(d1.size(), d2.size());

                while (d1.size() != size)
                    d1.push_back(0);

                while (d2.size() != size)
                    d2.push_back(0);

                int len = 1 + size;

                LongFloat res = new LongFloat();

                res.sign = cmp ? 1 : -1;
                res.digits = vector<int>(len, 0);

                for (size_t i = 0; i < size; i++)
                    res.digits[i + 1] = d1[i] - d2[i];

                for (size_t i = len - 1; i > 0; i--)
                {
                    if (res.digits[i] < 0)
                    {
                        res.digits[i] += 10;
                        res.digits[i - 1]--;
                    }
                }

                res.exponent = exp + 1;
                res.removeZeroes();

                return res;
            }

            if (sign == -1 && x.sign == -1)
                return (-x) - (-(*this));

            return *this + (-x);*/

            return null;
        }
    }
}
