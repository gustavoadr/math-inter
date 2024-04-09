public class Program
{
    static void Main()
    {
        ExpressionEvaluator evaluator = new ExpressionEvaluator();
        string expression = "sin(0.5)-2*cos(0.3)"; // Example expression
        double result = evaluator.Evaluate(expression);
        Console.WriteLine($"Result of expression '{expression}': {result}");
    }
}