namespace MathInter;

public static class MathExpressions
{
    public static readonly Dictionary<char, MathOperations.BinaryOperation> binaryOperations = new Dictionary<char, MathOperations.BinaryOperation>
    {
        {'+', MathOperations.Add},
        {'-', MathOperations.Subtract},
        {'*', MathOperations.Multiply},
        {'/', MathOperations.Divide},
        {'^', MathOperations.Power}
    };

    public static readonly Dictionary<string, MathOperations.UnaryOperation> unaryOperations = new Dictionary<string, MathOperations.UnaryOperation>
    {
        {"sin", MathOperations.Sin},
        {"cos", MathOperations.Cos}
    };

    public static int Precedence(char op)
    {
        return op switch
        {
            '+' or '-' => 1,
            '*' or '/' => 2,
            '^' => 3,
            _ => 0,
        };

    }
}