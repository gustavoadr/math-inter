using MathInter;

public class Program
{
    static void Main()
    {
        string expression = "log(8,2)-2*cos(0.3)+\"JOAO\".Count()";

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

        var result = new MathEvaluator(expression).Evaluate();
        Console.WriteLine($"Result of expression '{expression}': {result}");
    }

    //blablabla <%="EXPRESSION"%> -result: blablabla EXPRESSION

    //blablabla <%="CASA"%> -result: blablabla CASA
    //blablabla <%=LOG(8,2)%> -result: blablabla 3
    
    //DEFINE COMODOS{[
    //     "QUARTO":{"TAMANHO":30}
    //     "QUARTO":{"TAMANHO":30}
    //     "QUARTO":{"TAMANHO":40}
    //     "QUARTO":{"TAMANHO":50}
    // ]}

    //blablabla <%=MODELO.QUARTOS[1].TAMANHO%> -result: blablabla 30 M^2
    //<%for(x, MODELO.QUARTOS)%> blablabla <%=x.TAMANHO%> <%\for%> 
    //saída:
        //blablabla 30 - (Quarto 1)
        //blablabla 40 - (Quarto 2)
        //blablabla 50 - (Quarto 3)
        //blablabla 60 - (Quarto 4)

}
