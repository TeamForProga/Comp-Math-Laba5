using System.Dynamic;
using System.IO.Compression;
using System.Xml.Serialization;

namespace laba5;

static class MyMath
{
    public static double f(double x) => (2.5 * x * x - 0.1) / (Math.Log(x) + 1.0);

    /// <summary>
    /// Оценка погрешности по правилу Рунге
    /// </summary>
    /// Формула Рунге: δ = |S_h/2 - S_h| / (k^p - 1)
    /// где p - порядок точности, k - кратность уменьшения шага
    /// </remarks>
    public static double GetErrorByRunge(double accuracyOrder, double value1, double value2, double stepPower = 2.0)
    {
        return Math.Abs(value1 - value2) / (Math.Pow(stepPower, accuracyOrder) - 1.0);
    }

    /// <summary>
    /// Перечисление доступных методов численного интегрирования
    /// </summary>
    public enum Method
    {
        RightRectangles,   
        Trapezia,          
        Simpson               
    };

    /// <summary>
    /// Метод правых прямоугольников
    /// </summary>
    /// <remarks>
    /// Формула: ∫f(x)dx ≈ h * Σ f(x_i), где i = 1..n
    /// Порядок точности: O(h) - первый
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

    /// <summary>
    /// Метод трапеций
    /// </summary>
    /// <remarks>
    /// Формула: ∫f(x)dx ≈ h/2 * [f(a) + 2*Σf(x_i) + f(b)], i = 1..n-1
    /// Порядок точности: второй
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

    /// <summary>
    /// Метод Симпсона (парабол)
    /// </summary>
    /// Порядок точности: четвёртый
    static double SimpsonMethod(double a, double b, int stepCount)
    {
        double stepSize = (b - a) / stepCount;
        double sumOdd = 0;   
        double sumEven = 0;  

        for (int i = 1; i < stepCount; i++)
        {
            if (i % 2 != 0)
                sumOdd += f(a + i * stepSize);   // Нечёт
            else
                sumEven += f(a + i * stepSize);  // Чёт
        }

        // Формула Симпсона: f(a) + f(b) + 4*sumOdd + 2*sumEven
        double result = f(a) + f(b) + 4 * sumOdd + 2 * sumEven;

        // Умножаем на h/3
        return result * (stepSize / 3);
    }

    /// <summary>
    /// Основная функция решения интеграла с автоматическим подбором шага
    /// </summary>
    public static double SolveIntegral(double a, double b, double eps, Method methodType)
    {
        var solveMethod = RightRectanglesMethod;

        // Порядок точности метода
        int accuracyOrder = 1;

        // Начальное количество разбиений
        int stepCount = 1;

        switch (methodType)
        {
            case Method.RightRectangles:
                Console.Write("\n=== Right Rectangles ===\n");
                // accuracyOrder = 1 (уже установлен)
                // stepCount = 1 (уже установлен)
                break;

            case Method.Trapezia:
                Console.Write("\n=== Trapezia ===\n");
                solveMethod = TrapezeMethod;
                accuracyOrder = 2;   // Метод трапеций - 2-й порядок точности
                break;

            case Method.Simpson:
                Console.Write("\n=== Simpson ===\n");
                solveMethod = SimpsonMethod;
                accuracyOrder = 4;   // Метод Симпсона - 4-й порядок точности
                stepCount = 2;       // Симпсону нужно чётное число разбиений, начинаем с 2
                break;

            default:
                Console.Write("Unknown method type. Used configuration for RightRectangles\n");
                Console.Write("\n=== Right Rectangles ===\n");
                break;
        }

        Console.Write($"Accuracy: {eps}\n");

        double stepSize = b - a;     // Текущий шаг интегрирования
        int stepPower = 2;           // Во сколько раз уменьшаем шаг (удваиваем разбиения)

        double error = eps;          // Текущая оценка погрешности
        int iteration = 1;           // Номер итерации
        double prevRes = 0;          // Значение интеграла на предыдущей итерации (с шагом h)

        // Основной цикл
        while (error >= eps)
        {
            // Защита от бесконечного цикла при слишком маленькой точности
            if (stepCount > int.MaxValue / 32)
            {
                Console.Write($"Error: accuracy too high or bad range. Returned result of {iteration - 1} iteration\n");
                Console.WriteLine("Введите Enter чтобы продолжить");
                Console.ReadLine();
                return prevRes;
            }

            Console.Write($"\nIteration: {iteration}\n");
            Console.Write($"Step size: {stepSize}\n");
            Console.Write($"Step count: {stepCount}\n");

            // Вычисляем интеграл с текущим шагом
            double res = solveMethod(a, b, stepCount);

            Console.Write($"Result: {res}\n");

            // Начиная со 2-й итерации, можем оценить погрешность
            if (iteration != 1)
            {
                // Правило Рунге: сравниваем результат с шагом h (prevRes) и h/2 (res)
                error = GetErrorByRunge(accuracyOrder, prevRes, res, stepPower);
                Console.Write($"Error: {error}\n");
            }

            prevRes = res;

            // Уменьшаем шаг в 2 раза (удваиваем количество разбиений)
            stepCount *= stepPower;
            stepSize /= stepPower;

            ++iteration;
        }

        Console.Write($"\nFinal result: {prevRes}\n\n");
        return prevRes;
    }
}