using System.Globalization;

namespace MathInter;

public class MathEvaluator
{
    private string Expression {get; set;}
    private readonly Dictionary<string, FunctionOperation> Operations = MathExpressions.functionOperations;

    public MathEvaluator(string expression)
    {
        Expression = expression.Trim();
    }

    public object Evaluate(string expression)
    {
        // Resolver expressão matemática
        List<double> numbers = new List<double>();
        List<char> ops = new List<char>();

        string token = "";

        for (int i = 0; i < expression.Length; i++)
        {
            if (Operations.Contains(token))
            {
                numbers.Add(double.Parse(num));
                num = "";
                ops.Add(expression[i]);
            }
            else if (expression[i] == '(')
            {
                int j = i + 1;
                int openParenthesis = 1;
                while (openParenthesis > 0)
                {
                    if (expression[j] == '(') openParenthesis++;
                    else if (expression[j] == ')') openParenthesis--;
                    j++;
                }
                numbers.Add(EvaluateExpressionRecursively(expression.Substring(i + 1, j - i - 2)));
                i = j - 1;
            }
            else
            {
                num += expression[i];
            }
        }
        numbers.Add(double.Parse(num));

        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] == '*')
            {
                numbers[i] *= numbers[i + 1];
                numbers.RemoveAt(i + 1);
                ops.RemoveAt(i);
                i--;
            }
            else if (ops[i] == '/')
            {
                numbers[i] /= numbers[i + 1];
                numbers.RemoveAt(i + 1);
                ops.RemoveAt(i);
                i--;
            }
        }

        double result = numbers[0];
        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] == '+') result += numbers[i + 1];
            else if (ops[i] == '-') result -= numbers[i + 1];
        }
        return result;
    }
}