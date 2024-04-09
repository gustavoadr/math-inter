using MathInter;

public class Program
{
    static void Main()
    {
        string expression = "cos(-3.14)"; // Example expression
        var evaluator = new MathEvaluator(expression);
        var result = evaluator.Evaluate();
        Console.WriteLine($"Result of expression '{expression}': {result}");
    }
}