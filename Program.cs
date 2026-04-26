using System;

namespace laba5;

static class Program
{
    static void Main()
    {
        double a = 3;
        double b = 13;

        MyMath.SolveIntegral(a, b, 1, MyMath.Method.RightRectangles);      
        MyMath.SolveIntegral(a, b, 1, MyMath.Method.Trapezia);      
        MyMath.SolveIntegral(a, b, 1, MyMath.Method.Simpson);      
    }
}