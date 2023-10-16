using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_sandbox
{
    enum X
    {
        A, B, C
    }
    class C1
    {
        int a;
        public C1(int a)
        {
            this.a = a;
        }

        public static int operator +(C1 a, C1 b) => a.a + b.a;
        public static int operator +(C1 a, int b) => a.a + b;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Hello, world!\n");
            /* using 宣言 c# 8.0 より使用可能
            var numbers = new List<int>();
            using (StreamReader reader = File.OpenText("numbers.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (int.TryParse(line, out int number))
                    {
                        numbers.Add(number);
                    }
                }
            }
            */

            X a= X.A;
            Console.WriteLine(a);
            C1 b = new C1(1);
            C1 c = new C1(2);
            Console.WriteLine(b);
            Console.WriteLine(c);
            Console.WriteLine(b+c);
            Console.WriteLine(b+10);

            int[] d = { 1, 2, 3 };
            ref int f1(int[] x)
            {
                return ref x[1];
            }
            f1(d) = 5;
            foreach(var x in d)
            {
                Console.WriteLine(x);
            }
        }
    }


}
