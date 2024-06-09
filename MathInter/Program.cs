using System.Diagnostics;
using GerSegCond.Console.Express;
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
        Main2();
        //string expression = "sen(3.5+4.2)*cos(2/(1-5))+log(10,2)"; // OK
        //string expression = "log(8,2)-2*cos(0.3)+\"JOAO\".Length"; // OK
        //string expression = "(Guilherme).Substring(0+4,3)"; // OK
        // string expression = "(\"Guilherme\"+\"Salles\").Substring(0,3)"; // OK

        // var result = new Evaluator(expression).Evaluate();
        // Console.WriteLine($"Result of expression '{expression}': {result}");

        // var basePath = "D:/gusta/Documents/Estudos/math-inter/Template";

        // var file = new FileHandler($"{basePath}/RDS.tpt");
        
        // file.EvaluateFile($"{basePath}/Output.cs");
    }

    static void Main2()
    {
        ExpressionConverter converter = new ExpressionConverter();
        //converter.parse("for(\"i\", 0, 10, \"saveFile(\"\"C:\\Users\\gustavo.rocha_nuria\\\"\"+i+\"\".txt\"\", \"\"Guilherme Salles \"\"+i)\")");
        converter.parse("saveFile(\"C:\\Users\\gustavo.rocha_nuria\\Gerador.txt\", for(\"i\", 0, 10, \"\"\"Teste => \"\"+(i+10)\"))");
        Console.WriteLine(converter.convertToRPN());
        Console.WriteLine(converter.evaluateS());
    }
}
