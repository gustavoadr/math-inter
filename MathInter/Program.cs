public class Program
{
    static void Main()
    {
        string expression = "sin(0.5)-2*cos(0.3)"; // Example expression
        var evaluator = new ExpressionEvaluator(expression);
        var result = evaluator.Evaluate();
        Console.WriteLine($"Result of expression '{expression}': {result}");
    }
}