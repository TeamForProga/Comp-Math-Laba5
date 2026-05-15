using System;

namespace laba5;

static class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Write("Введите точность epsilon: ");
            double eps = Convert.ToDouble(Console.ReadLine());
            Console.Write("Введите границу a: ");
            double a = Convert.ToDouble(Console.ReadLine());
            Console.Write("Введите границу b: ");
            double b = Convert.ToDouble(Console.ReadLine());
            if (a == b)
            {
                Console.WriteLine("Границы не могут быть равны\n");
                continue;
            }
            if (a <= 0 || b <= 0)
            {
                Console.WriteLine($"Функция не существует на отрицательных границах\n");
            }
            MyMath.SolveIntegral(a, b, eps, MyMath.Method.RightRectangles);
            MyMath.SolveIntegral(a, b, eps, MyMath.Method.Trapezia);
            MyMath.SolveIntegral(a, b, eps, MyMath.Method.Simpson);
        }
    }
}
