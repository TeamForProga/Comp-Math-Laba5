using System.Dynamic;
using System.IO.Compression;
using System.Xml.Serialization;

namespace laba5;

static class MyMath
{
    public static double f(double x) => (2.5 * x * x - 0.1) / (Math.Log(x) + 1.0);

    public static double GetErrorByRunge(double accuracyOrder, double value1, double value2, double stepPower = 2.0)
    {  
        return Math.Abs(value1 - value2) / (Math.Pow(stepPower, accuracyOrder) - 1.0);
    }

    public enum Method 
    {
        RightRectangles,
        Trapezia,
        Simpson
    };

    private static double RightRectanglesMethod(double a, double b, int stepCount)
    {
        double stepSize = (b - a) / stepCount;
     
        double result = 0;
        for (int i = 1; i <= stepCount; i++)
        {
            result += f(a + stepSize * i);
        }
        result *= stepSize;

        return result;
    }

    static double TrapezeMethod(double a, double b, int stepCount)
    {
        double stepSize = (b - a) / stepCount;

        double result = 0;
        for (int i = 1; i < stepCount; i++)
        {
            result += f(a + stepSize * i);
        }
        result *= 2;
        result += f(a) + f(b);
        result *= stepSize / 2;
        
        return result;
    }

    static double SimpsonMethod(double a, double b, int stepCount)
    {
        double stepSize = (b - a) / stepCount;

        double result = 0;

        // Коэффициенты имеют такой вид: 1 4 2 4 2 ... 2 4 2 4 1
        // Оптимизируем число операций через группировку

        // Формула выглядит так
        // 1. сложим все f(xn) где n = 1,3,...,stepCount - 1 (нечётные индексы)
        // 2. Умножим общий результат на 2
        // 3. сложим все f(xn) где n = 2,4,...,stepCount - 2 (чётные индексы)
        // 4. Умножим общий результат на 2
        // 5. добавим f(a) (n = 0) и f(b) (n = stepCount)
        // 6. Умножим общий результат на stepSize / 3

        for (int i = 1; i < stepCount; i += 2)
        {
            result += f(a + i * stepSize);
        }
        // 2.
        result *= 2;
        
        // 3.
        for (int i = 2; i < stepCount; i += 2)
        {
            result += f(a + i * stepSize);
        }
        // 4.
        result *= 2;

        // 5.
        result += f(a);
        result += f(b);

        // 6.
        result *= stepSize / 3;

        return result;
    }



    public static double SolveIntegral(double a, double b, double eps, Method methodType)
    {    
        var solveMethod = RightRectanglesMethod;
        
        int accuracyOrder = 1;

        int stepCount = 1;

        switch (methodType)
        {
            case Method.RightRectangles:
                Console.Write("\n=== Right Rectangles ===\n");
                break;

            case Method.Trapezia:
                Console.Write("\n=== Trapezia ===\n");
                solveMethod = TrapezeMethod;
                accuracyOrder = 2;
                break;

            case Method.Simpson:
                Console.Write("\n=== Simpson ===\n");
                solveMethod = SimpsonMethod;
                accuracyOrder = 4;
                stepCount = 2;
                break;

            default:
                Console.Write("Unknown method type. Used configuration for RightRectangles\n");
                Console.Write("\n=== Right Rectangles ===\n");
                break;
        }
        
        Console.Write($"Accuracy: {eps}\n");

        double stepSize = b - a;
        int stepPower = 2;

        double error = eps;
        int iteration = 1;
        double prevRes = 0;
        while (error >= eps)
        {
            if (stepCount > int.MaxValue / 2)
            {
                Console.Write($"Error: accuracy too high or bad range. Returned result of {iteration - 1} iteration\n");
                return prevRes;
            }

            Console.Write($"\nIteration: {iteration}\n");
            Console.Write($"Step size: {stepSize}\n");
            Console.Write($"Step count: {stepCount}\n");
            
            double res = solveMethod(a, b, stepCount);

            Console.Write($"Result: {res}\n");
            
            if (iteration != 1)
            {
                error = GetErrorByRunge(accuracyOrder, prevRes, res, stepPower);
                Console.Write($"Error: {error}\n");
            }

            prevRes = res;
            
            stepCount *= stepPower;
            stepSize /= stepPower;

            ++iteration;
        }

        Console.Write($"\nFinal result: {prevRes}\n\n");
        return prevRes;
    }
}