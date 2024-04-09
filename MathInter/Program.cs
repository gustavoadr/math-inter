using MathInter;

public class Program
{
    static void Main()
    {
        string expression = "sin(-0.5)-2*cos(0.3)";
        var result = new MathEvaluator(expression).Evaluate();
        Console.WriteLine($"Result of expression '{expression}': {result}");
    }
}