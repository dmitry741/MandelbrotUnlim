using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LongFloatLib;

namespace LongFloatTestApp
{
    class TestLongFloat
    {
        const string cPassed = "PASSED";
        const string cFailed = "FAILED";
        int _testNumber = 1;
        int _success = 0;
        int _failed = 0;

        private void WriteResult(bool condition, int testNumber)
        {
            if (condition)
            {
                Console.WriteLine("Test {0} \t {1}", testNumber, cPassed);
                _success++;
            }
            else
            {
                Console.WriteLine("Test {0} \t {1}", testNumber, cFailed);
                _failed++;
            }
        }

        public void Run()
        {
            _testNumber = 1;
            _success = 0;
            _failed = 0;

            double[] arr = { 0.123, 456.657, 1001, -1.29, 0.06757, -0.34 };
            double d1, d2;
            LongFloat lf1, lf2;

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr.Length; j++)
                {
                    d1 = arr[i];
                    d2 = arr[j];

                    lf1 = new LongFloat(d1.ToString());
                    lf2 = new LongFloat(d2.ToString());

                    // арифметика
                    WriteResult((lf1 + lf2).ToString() == (d1 + d2).ToString(), _testNumber++);
                    WriteResult((lf2 + lf1).ToString() == (d1 + d2).ToString(), _testNumber++);
                    WriteResult((lf1 * lf2).ToString() == (d1 * d2).ToString(), _testNumber++);
                    WriteResult((lf2 * lf1).ToString() == (d2 * d1).ToString(), _testNumber++);
                    WriteResult((lf1 - lf2).ToString() == (d1 - d2).ToString(), _testNumber++);
                    WriteResult((lf2 - lf1).ToString() == (d2 - d1).ToString(), _testNumber++);
                    WriteResult((-lf1).ToString() == (-d1).ToString(), _testNumber++);
                    WriteResult((-lf2).ToString() == (-d2).ToString(), _testNumber++);

                    // сравнение
                    WriteResult((lf1 < lf2) == (d1 < d2), _testNumber++);
                    WriteResult((lf1 > lf2) == (d1 > d2), _testNumber++);
                    WriteResult((lf1 == lf2) == (d1 == d2), _testNumber++);
                    WriteResult((lf1 <= lf2) == (d1 <= d2), _testNumber++);
                    WriteResult((lf1 >= lf2) == (d1 >= d2), _testNumber++);
                }
            }

            // тест быстродействия
            LongFloat c = LongFloat.FromDouble(0.22);
            LongFloat z = LongFloat.FromDouble(0.02);

            for (int i = 0; i < 10; i++)
            {
                z = z * z + c;
            }

            stopWatch.Stop();

            // тесты завершены
            Console.WriteLine();
            Console.WriteLine(string.Format("Total tests: \t {0}", _testNumber - 1));
            Console.WriteLine(string.Format("Successful: \t {0}", _success));
            Console.WriteLine(string.Format("Failed: \t {0}", _failed));
            Console.WriteLine(string.Format("Time elapsed: \t {0} ms", stopWatch.Elapsed.Seconds * 1000 + stopWatch.Elapsed.Milliseconds));
            Console.WriteLine();
            Console.WriteLine("Press ENTER to continue");
            Console.ReadLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TestLongFloat testLongFloat = new TestLongFloat();
            testLongFloat.Run();
        }
    }
}
