using System;
using System.Collections.Generic;

namespace LongFloatLib
{
    /// <summary>
    /// Класс реализующий длинную вещественную арифметику.
    /// </summary>
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public class LongFloat
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        #region === members ===

        int _sign;
        List<int> _digits = new List<int>();
        int _exponent;

        #endregion

        #region === private ===

        private bool IsZero => _digits.Count == 1 && _digits[0] == 0;

        private static List<int> ListDeepCopy(List<int> digits)
        {
            int[] digs = new int[digits.Count];
            digits.CopyTo(digs);

            return new List<int>(digs);
        }

        private void InitFromString(string s)
        {
            var nf = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
            int index;

            if (s[0] == '-')
            {
                _sign = -1;
                index = 1;
            }
            else
            {
                _sign = 1;
                index = 0;
            }

            _exponent = s.Length - index;

            while (index < s.Length)
            {
                if (s[index] == nf.NumberDecimalSeparator[0])
                    _exponent = _sign == 1 ? index : index - 1;
                else
                    _digits.Add(s[index] - '0');

                index++;
            }
        }

        private void RemoveZeroes()
        {
            int n = Math.Max(1, _exponent);

            while (_digits.Count > n && _digits[_digits.Count - 1] == 0)
                _digits.RemoveAt(_digits.Count - 1);

            while (_digits.Count > 1 && _digits[0] == 0)
            {
                _digits.RemoveAt(0);
                _exponent--;
            }

            while (_digits.Count > n && _digits[_digits.Count - 1] == 0)
                _digits.RemoveAt(_digits.Count - 1);

            if (IsZero)
            {
                _exponent = 1;
                _sign = 1;
            }

            Normalize();
        }

        private void Normalize()
        {
            const int cDivDigits = 1000;
            int start = Math.Max(_exponent, 0);
            int realDigits = _digits.Count - start;

            if (realDigits >= cDivDigits)
            {
                int count;
                int maxCount = 0;

                int i = start;

                while (i < _digits.Count)
                {
                    count = 0;

                    while (i < _digits.Count && _digits[i] == 9)
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
                    i = _digits.Count - 1;

                    do
                    {
                        count = 0;

                        while (i > 0 && _digits[i] != 9)
                            i--;

                        while (i > 0 && _digits[i] == 9)
                        {
                            count++;
                            i--;
                        }
                    } while (count != maxCount);

                    int startIndex = _digits[0] + i + 1;
                    _digits.RemoveRange(startIndex, _digits.Count - startIndex + 1);
                    _digits[i]++;
                }
            }
        }

        #endregion

        #region === constructors ===

        public LongFloat()
        {
            _sign = 1;
            _digits = new List<int> { 0 };
            _exponent = 0;
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

        #endregion

        #region === ariphmetics ===

        public static LongFloat operator -(LongFloat a)
        {
            return new LongFloat
            {
                _digits = ListDeepCopy(a._digits),
                _exponent = a._exponent,
                _sign = -a._sign
            };
        }

        public static LongFloat operator +(LongFloat a, LongFloat b)
        {
            if (a._sign == b._sign)
            {
                int exp1 = a._exponent;
                int exp2 = b._exponent;
                int exp = Math.Max(exp1, exp2);

                List<int> d1 = new List<int>(a._digits);
                List<int> d2 = new List<int>(b._digits);

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
                    _digits = new List<int>(zeros),
                    _sign = a._sign
                };

                for (int i = 0; i < size; i++)
                    res._digits[i + 1] = d1[i] + d2[i];

                for (int i = len - 1; i > 0; i--)
                {
                    res._digits[i - 1] += res._digits[i] / 10;
                    res._digits[i] %= 10;
                }

                res._exponent = exp + 1;
                res.RemoveZeroes();

                return res;
            }

            if (a._sign == -1)
                return b - (-a);

            return a - (-b);
        }

        public static LongFloat operator -(LongFloat a, LongFloat b)
        {
            if (a._sign == 1 && b._sign == 1)
            {
                bool cmp = a > b;

                int exp1 = cmp ? a._exponent : b._exponent;
                int exp2 = cmp ? b._exponent : a._exponent;
                int exp = Math.Max(exp1, exp2);

                List<int> d1 = ListDeepCopy(cmp ? a._digits : b._digits);
                List<int> d2 = ListDeepCopy(cmp ? b._digits : a._digits);

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
                    _sign = cmp ? 1 : -1,
                    _digits = new List<int>(zeros)
                };

                for (int i = 0; i < size; i++)
                    res._digits[i + 1] = d1[i] - d2[i];

                for (int i = len - 1; i > 0; i--)
                {
                    if (res._digits[i] < 0)
                    {
                        res._digits[i] += 10;
                        res._digits[i - 1]--;
                    }
                }

                res._exponent = exp + 1;
                res.RemoveZeroes();

                return res;
            }

            if (a._sign == -1 && b._sign == -1)
                return (-b) - (-a);

            return a + (-b);
        }

        public static LongFloat operator *(LongFloat a, LongFloat b)
        {
            int len = a._digits.Count + b._digits.Count;
            int[] zeros = new int[len];
            Array.Clear(zeros, 0, len);

            LongFloat res = new LongFloat
            {
                _sign = a._sign * b._sign,
                _exponent = a._exponent + b._exponent,
                _digits = new List<int>(zeros)
            };

            for (int i = 0; i < a._digits.Count; i++)
                for (int j = 0; j < b._digits.Count; j++)
                    res._digits[i + j + 1] += a._digits[i] * b._digits[j];

            for (int i = len - 1; i > 0; i--)
            {
                res._digits[i - 1] += res._digits[i] / 10;
                res._digits[i] %= 10;
            }

            res.RemoveZeroes();

            return res;
        }

        #endregion

        #region === comparing ===

        public static bool operator >(LongFloat a, LongFloat b)
        {
            if (a._sign != b._sign)
                return a._sign > b._sign;

            if (a._exponent != b._exponent)
                return (a._exponent > b._exponent) ^ (a._sign == -1);

            List<int> d1 = ListDeepCopy(a._digits);
            List<int> d2 = ListDeepCopy(b._digits);

            int size = Math.Max(d1.Count, d2.Count);

            while (d1.Count != size)
                d1.Add(0);

            while (d2.Count != size)
                d2.Add(0);

            for (int i = 0; i < size; i++)
                if (d1[i] != d2[i])
                    return (d1[i] > d2[i]) ^ (a._sign == -1);

            return false;
        }

        public static bool operator <(LongFloat a, LongFloat b)
        {
            return !(a > b || a == b);
        }

        public static bool operator ==(LongFloat a, LongFloat b)
        {
            if (a._sign != b._sign)
                return false;

            if (a._exponent != b._exponent)
                return false;

            if (a._digits.Count != b._digits.Count)
                return false;

            for (int i = 0; i < a._digits.Count; i++)
                if (a._digits[i] != b._digits[i])
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

        #endregion

        public override string ToString()
        {
            var nf = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
            string result = string.Empty;

            if (_sign == -1)
                result += "-";

            if (_exponent > 0)
            {
                int i = 0;
                int exp = _exponent;

                while (i < _digits.Count && i < exp)
                    result += _digits[i++];

                while (i < exp)
                {
                    result += "0";
                    i++;
                }

                if (i < _digits.Count)
                {
                    result += nf.NumberDecimalSeparator[0];

                    while (i < _digits.Count)
                        result += _digits[i++];
                }
            }
            else if (_exponent == 0)
            {
                result += "0" + nf.NumberDecimalSeparator[0];

                for (int i = 0; i < _digits.Count; i++)
                    result += _digits[i];
            }
            else
            {
                result += "0" + nf.NumberDecimalSeparator[0];

                for (int i = 0; i < -_exponent; i++)
                    result += "0";

                for (int i = 0; i < _digits.Count; i++)
                    result += _digits[i];
            }

            return result;
        }
    }
}
