using System.Globalization;
using System.Text;

namespace MathInter
{
    public class Evaluator
    {
        private string Expression {get; set;}
        private readonly Dictionary<string, FunctionOperation> Operations = Expressions.functionOperations;

        public Evaluator(string expression)
        {
            Expression = expression.Trim();
        }

        public object Evaluate()
        {
            var tokens = InfixToPostfix().Split(' ');
            var stack = new Stack<object>();

            foreach (var token in tokens)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                    stack.Push(number);
                else if(Operations.ContainsKey(token))
                {
                    FunctionOperation operation = Operations[token];
                    var parameters = new object[operation.NParam()];
                    
                    for(int i = operation.NParam() - 1; i >= 0; i--)
                        parameters[i] = stack.Pop();

                    var result = operation.Process(parameters);
                    stack.Push(result);
                }
                else
                    throw new InvalidOperationException($"Operador desconhecido: {token}");
            }

            if (stack.Count != 1)
                throw new InvalidOperationException("Expressão RPN inválida.");

            return stack.Pop();
        }

        public string InfixToPostfix()
        {
            List<string> tokens = Tokenize(Expression);
            Stack<string> stack = new Stack<string>();
            StringBuilder result = new StringBuilder();

            foreach (string token in tokens)
            {
                if (Operations.ContainsKey(token) && Operations[token].Precedence() == 4)
                {
                    stack.Push(token);
                }
                else if (token == ",")
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                        result.Append(stack.Pop());
                }
                else if(Operations.ContainsKey(token)) 
                {
                    while (stack.Count > 0 && Operations.ContainsKey(stack.Peek()))
                    {
                        string topOp = stack.Peek();
                        if ((Operations[token].Precedence() < 4 && Operations[token].Precedence() < Operations[topOp].Precedence()) ||
                            (Operations[token].Precedence() == 4 && Operations[token].Precedence() <= Operations[topOp].Precedence()))
                        {
                            result.Append(stack.Pop() + " ");
                        }
                        else
                            break;
                    }
                    
                    stack.Push(token);
                }
                else if (token == "(")
                {
                    stack.Push(token);
                }
                else if (token == ")")
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                        result.Append(stack.Pop() + " ");
                    
                    if(stack.Count > 0 && stack.Peek() == "(")
                        stack.Pop();
                }
                else
                    result.Append(token + " ");
            }

            while (stack.Count > 0)
                result.Append(stack.Pop() + " ");

            return result.ToString().Trim();
        }

        public static List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();
            var token = new StringBuilder();

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if (Char.IsDigit(c) || c == '"' || (c == '.' && token.Length > 0 && Char.IsDigit(token[0])))
                    token.Append(c);
                else if (Char.IsLetter(c))
                    token.Append(c);
                else
                {
                    if (token.Length > 0)
                    {
                        tokens.Add(token.ToString());
                        token.Clear();
                    }
                    tokens.Add(c.ToString());
                }
            }

            if (token.Length > 0)
                tokens.Add(token.ToString());

            return tokens;
        }
    }
}