using System;

namespace laba5;

static class Program
{
    static void Main()
    {
        Console.Write("Введите точность epsilon: ");
	double eps = Convert.ToDouble(Console.ReadLine());
        Console.Write("Введите левую границу a: ");
	double a = Convert.ToDouble(Console.ReadLine());
        Console.Write("Введите правую границу b: ");
	double b = Convert.ToDouble(Console.ReadLine());

        MyMath.SolveIntegral(a, b, eps, MyMath.Method.RightRectangles);      
        MyMath.SolveIntegral(a, b, eps, MyMath.Method.Trapezia);      
        MyMath.SolveIntegral(a, b, eps, MyMath.Method.Simpson);      
    }
}
