using MathInter;

public class Program
{
    static void Main()
    {
        string expression = "log(8,2)-2*cos(0.3)+\"JOAO\".Length";

        //add soma de strings
        // expression = "\"Guilherme\"+\"Salles\"";

        // //add substring off sum string
        // expression = "(\"Guilherme\"+\"Salles\").SubString(1,3)";
        
        // (x^2 + y^2 + z^2) - Pitagoresco
        // MathExpressions.Add("x", 3);
        // expression = "Pitagoresco(3, 4, x)";

        // //Add colchetes [ ]
        // MathExpressions.Add("x", [1, 2, 3]);
        // expression = "x.Count()"; // return 3
        // expression = "x[1]"; //return 2

        var result = new Evaluator(expression).Evaluate();
        Console.WriteLine($"Result of expression '{expression}': {result}");
    }
}
