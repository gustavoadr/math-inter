using MathInter;

public class Program
{
    // todo: expression = "\"Guilherme\"+\"Salles\"";
    // todo: expression = "(\"Guilherme\"+\"Salles\").SubString(1,3)";
    
    // todo: (x^2 + y^2 + z^2) - Pitagoresco
    // todo: MathExpressions.Add("x", 3);
    // todo: expression = "Pitagoresco(3, 4, x)";

    // todo: MathExpressions.Add("x", [1, 2, 3]);
    // todo: expression = "x.Count()"; // return 3
    // todo: expression = "x[1]"; //return 2
    
    static void Main()
    {
        // string expression = "log(8,2)-2*cos(0.3)+\"JOAO\".Length";
        // var result = new Evaluator(expression).Evaluate();
        // Console.WriteLine($"Result of expression '{expression}': {result}");

        var basePath = "D:/gusta/Documents/Estudos/math-inter/Template";
        
        var file = new FileHandler($"{basePath}/RDS.tpt");
        file.EvaluateFile($"{basePath}/Output.cs");
    }
}
