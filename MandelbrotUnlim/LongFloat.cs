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
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    class LongFloat
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        int sign;
        List<int> digits;
        int exponent;

        bool IsZero => digits.Count == 1 && digits[0] == 0;

        private static List<int> ListDeepCopy(List<int> digits)
        {
            int[] digs = new int[digits.Count];
            digits.CopyTo(digs);

            return new List<int>(digs);
        }

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

        public static LongFloat operator -(LongFloat a) 
        {
            return new LongFloat
            {
                digits = ListDeepCopy(a.digits),
                exponent = a.exponent,
                sign = -a.sign
            };
        }

        public static LongFloat operator +(LongFloat a, LongFloat b)
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
            if (a.sign == 1 && b.sign == 1)
            {
                bool cmp = a > b;

                int exp1 = cmp ? a.exponent : b.exponent;
                int exp2 = cmp ? b.exponent : a.exponent;
                int exp = Math.Max(exp1, exp2);

                List<int> d1 = ListDeepCopy(cmp ? a.digits : b.digits);
                List<int> d2 = ListDeepCopy(cmp ? b.digits : a.digits);

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

                int[] zeros = new int[len];
                Array.Clear(zeros, 0, len);

                LongFloat res = new LongFloat
                {
                    sign = cmp ? 1 : -1,
                    digits = new List<int>(zeros)
                };

                for (int i = 0; i < size; i++)
                    res.digits[i + 1] = d1[i] - d2[i];

                for (int i = len - 1; i > 0; i--)
                {
                    if (res.digits[i] < 0)
                    {
                        res.digits[i] += 10;
                        res.digits[i - 1]--;
                    }
                }

                res.exponent = exp + 1;
                res.RemoveZeroes();

                return res;
            }

            if (a.sign == -1 && b.sign == -1)
                return (-b) - (-a);

            return a + (-b);
        }

        public static LongFloat operator *(LongFloat a, LongFloat b)
        {
            int len = a.digits.Count + b.digits.Count;
            int[] zeros = new int[len];
            Array.Clear(zeros, 0, len);

            LongFloat res = new LongFloat
            {
                sign = a.sign * b.sign,
                exponent = a.exponent + b.exponent,
                digits = new List<int>(zeros)
            };

            for (int i = 0; i < a.digits.Count; i++)
                for (int j = 0; j < b.digits.Count; j++)
                    res.digits[i + j + 1] += a.digits[i] * b.digits[j];

            for (int i = len - 1; i > 0; i--)
            {
                res.digits[i - 1] += res.digits[i] / 10;
                res.digits[i] %= 10;
            }

            res.RemoveZeroes();

            return res;
        }

        public static bool operator >(LongFloat a, LongFloat b)
        {
            if (a.sign != b.sign)
                return a.sign > b.sign;

            if (a.exponent != b.exponent)
                return (a.exponent > b.exponent) ^ (a.sign == -1);

            List<int> d1 = ListDeepCopy(a.digits);
            List<int> d2 = ListDeepCopy(b.digits);

            int size = Math.Max(d1.Count, d2.Count);

            while (d1.Count != size)
                d1.Add(0);

            while (d2.Count != size)
                d2.Add(0);

            for (int i = 0; i < size; i++)
                if (d1[i] != d2[i])
                    return (d1[i] > d2[i]) ^ (a.sign == -1);

            return false;
        }

        public static bool operator <(LongFloat a, LongFloat b)
        {
            return !(a > b || a == b);
        }

        public static bool operator ==(LongFloat a, LongFloat b)
        {
            if (a.sign != b.sign)
                return false;

            if (a.exponent != b.exponent)
                return false;

            if (a.digits.Count != b.digits.Count)
                return false;

            for (int i = 0; i < a.digits.Count; i++)
                if (a.digits[i] != b.digits[i])
                    return false;

            return true;
        }

        public static bool operator !=(LongFloat a, LongFloat b)
        {
            return !(a == b);
        }

        public static bool operator >=(LongFloat a, LongFloat b)
        {
            return a > b || a == b;
        }

        public static bool operator <=(LongFloat a, LongFloat b)
        {
            return a < b || a == b;
        }

        public override string ToString()
        {
            string result = string.Empty;

            if (sign == -1)
                result += "-";

            if (exponent > 0)
            {
                int i = 0;
                int exp = exponent;

                while (i < digits.Count && i < exp)
                    result += digits[i++];

                while (i < exp)
                {
                    result += "0";
                    i++;
                }

                if (i < digits.Count)
                {
                    result += ".";

                    while (i < digits.Count)
                        result += digits[i++];
                }
            }
            else if (exponent == 0)
            {
                result += "0.";

                for (int i = 0; i < digits.Count; i++)
                    result += digits[i];
            }
            else
            {
                result += "0.";

                for (int i = 0; i < -exponent; i++)
                    result += "0";

                for (int i = 0; i < digits.Count; i++)
                    result += digits[i];
            }

            return result;
        }
    }
}
