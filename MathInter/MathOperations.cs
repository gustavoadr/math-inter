namespace MathInter;

public class MathOperations
{
    public delegate double UnaryOperation(double operand);
    public delegate double BinaryOperation(double operand1, double operand2);

    public static double Sin(double operand)
    {
        return Math.Sin(operand);
    }

    public static double Cos(double operand)
    {
        return Math.Cos(operand);
    }

    // Binary operations
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
}

