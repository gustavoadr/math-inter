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

     public object Evaluate() //log(8,2) - 2 * cos(0.3) + "Joao".Count()
    {
        List<string> tokens = Tokenize(Expression);

        Stack<object> operands = new Stack<object>();
        Stack<string> operators = new Stack<string>();

        foreach (string token in tokens)
        {
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                operands.Push(number);
            else if (token == "(")
                operators.Push("(");
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                    EvaluateOperation(operands, operators);

                if (operators.Count > 0 && operators.Peek() == "(")
                    operators.Pop();
                else
                    throw new ArgumentException("Invalid expression: mismatched parentheses");
            }
            else if (Operations.ContainsKey(token))
            {
                while (operators.Count > 0 && Operations.ContainsKey(operators.Peek()) && Operations[token].Precedence() <= Operations[operators.Peek()].Precedence())
                    EvaluateOperation(operands, operators);

                operators.Push(token);
            }
            else
                operands.Push(token);
        }

        while (operators.Count > 0)
            EvaluateOperation(operands, operators);

        return operands.Pop();
    }

    private List<string> Tokenize(string expression)
    {
        List<string> tokens = new ();
        string currentToken = "";

        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];

            if ((c == '-' || c == '+') && (i == 0 || expression[i - 1] == '(' || Operations.ContainsKey(expression[i - 1].ToString())))
                currentToken += c;
            else if (c == '.')
            {
                if(currentToken.Length > 0 && currentToken[currentToken.Length - 1] == '"')
                {
                    do
                    {
                        currentToken += c;
                        if(++i >= expression.Length)
                            break;
                        c = expression[i];
                    }
                    while(!Operations.ContainsKey(c.ToString()));
                    
                    tokens.Add(".");
                    tokens.Add(currentToken);
                    currentToken = "";
                }
                else
                    currentToken += c;
            }
            else if (c == '(' || c == ')' || Operations.ContainsKey(c.ToString()))
            {
                if (currentToken != "")
                {
                    tokens.Add(currentToken);
                    currentToken = "";
                }
                tokens.Add(c.ToString());
            }
            else if (c == ',')
            {
                if (currentToken != "")
                {
                    tokens.Add(currentToken);
                    currentToken = "";
                }
            }
            else
                currentToken += c;
        }

        if (currentToken != "")
            tokens.Add(currentToken);

        return tokens;
    }

    private void EvaluateOperation(Stack<object> operands, Stack<string> operators)
    {
        if (operands.Count < 1 || operators.Count == 0)
            throw new ArgumentException("Invalid expression");

        string op = operators.Pop();

        if (op == "(")
            return;

        if (op == ")")
            throw new ArgumentException("Invalid expression");

        if (Operations.ContainsKey(op))
        {
            FunctionOperation operation = Operations[op];
            
            var pars = new object[operation.NParam()];
            for(int i=operation.NParam()-1;i>=0;i--)
                pars[i] = operands.Pop();

            var result = operation.Process(pars);
            operands.Push(result);
        }
        else if (Operations.ContainsKey(op[0].ToString()))
        {
            if (operands.Count < 2)
                throw new ArgumentException("Invalid expression");

            var operand2 = operands.Pop();
            
            // Check if there are unary operations pending and evaluate them first
            while (operators.Count > 0 && Operations.ContainsKey(operators.Peek()))
                EvaluateOperation(operands, operators);
            
            var operand1 = operands.Pop();
            FunctionOperation operation = Operations[op[0].ToString()];
            object[] parametros = new object[] { operand1, operand2 };
            var result = operation.Process(parametros);
            operands.Push(result);
        }
        else
            throw new ArgumentException($"Invalid operator or function: {op}");
    }
}