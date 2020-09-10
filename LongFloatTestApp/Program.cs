using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LongFloatLib;

namespace LongFloatTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int testNumber = 1;

            // 1
            double d1 = 0.123;
            double d2 = 4.56;

            LongFloat lf1 = new LongFloat(d1.ToString());
            LongFloat lf2 = new LongFloat(d2.ToString());

            LongFloat lfSum = lf1 + lf2;

            Console.WriteLine("Test {0} \t {1}", testNumber++, (lfSum.ToString() == (d1 + d2).ToString()) ? "Passed" : "Failed");

            // тесты завершены
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }
    }
}
