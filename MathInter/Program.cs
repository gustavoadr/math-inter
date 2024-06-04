using MathInter;
using MathInter.Modelo;

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
        //string expression = "log(8,2)-2*cos(0.3)+\"JOAO\".Length";
        string expression = "sen(3.5+4.2)*cos(2/(1-5))+log(10,2)";
        //string expression = "(\"Guilherme\"+\"Salles\").Substring(8,3)";
        var result = new NotationConverter().InfixToPostfix(expression);
        Console.WriteLine($"Result of expression '{expression}': {result}");

        var basePath = "D:/gusta/Documents/Estudos/math-inter/Template";

        var file = new FileHandler($"{basePath}/RDS.tpt");
        file.EvaluateFile($"{basePath}/Output.cs");
    }
}
