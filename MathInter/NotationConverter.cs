using System.Text;

namespace MathInter
{
    public class NotationConverter
    {
        private readonly Dictionary<string, FunctionOperation> Operations = Expressions.functionOperations;

        public string InfixToPostfix(string expression)
        {
            List<string> tokens = Tokenize(expression);
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

                if (Char.IsDigit(c) || (c == '.' && token.Length > 0 && Char.IsDigit(token[0])))
                {
                    token.Append(c);
                }
                else if (Char.IsLetter(c))
                {
                    token.Append(c);
                }
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