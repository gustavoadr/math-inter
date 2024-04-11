using System.ComponentModel;

namespace MathInter;

public static class MathExpressions
{
    public static readonly Dictionary<string, MathOperations.BinaryOperation> binaryOperations = new Dictionary<string, MathOperations.BinaryOperation>
    {
        {"+", MathOperations.Add},
        {"-", MathOperations.Subtract},
        {"*", MathOperations.Multiply},
        {"/", MathOperations.Divide},
        {"^", MathOperations.Power},
    };

    public static readonly Dictionary<string, FunctionOperation> functionOperations;

    static MathExpressions()
    {
        functionOperations = new ();
        
        Add(new Sen());
        Add(new Cos());
    }

    public static void Add(FunctionOperation functionOperation)
    {
        functionOperations.Add(functionOperation.GetName(), functionOperation);
    }

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