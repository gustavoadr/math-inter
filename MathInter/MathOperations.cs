namespace MathInter;

public class MathOperations
{
    public delegate double BinaryOperation(double operand1, double operand2);

    public static double Sin(double operand)
    {
        return Math.Sin(operand);
    }

    public static double Cos(double operand)
    {
        return Math.Cos(operand);
    }

    public static double Add(double operand1, double operand2)
    {
        return operand1 + operand2;
    }

    public static double Subtract(double operand1, double operand2)
    {
        return operand1 - operand2;
    }

    public static double Multiply(double operand1, double operand2)
    {
        return operand1 * operand2;
    }

    public static double Divide(double operand1, double operand2)
    {
        return operand1 / operand2;
    }

    public static double Power(double baseValue, double exponent)
    {
        return Math.Pow(baseValue, exponent);
    }

    public static double Log(double value, double baseValue = 10)
    {
        return Math.Log(value, baseValue);
    }
}

