using System.Globalization;

namespace MathInter;

public class MathEvaluator
{
    private string Expression {get; set;}
    private readonly Dictionary<string, FunctionOperation> Operations = MathExpressions.functionOperations;

    public MathEvaluator(string expression)
    {
        Expression = expression.Replace(" ", "");
    }
    
    // Method to evaluate a mathematical expression
    public object SolveExpression()
    {
        // Pilha para armazenar os valores e operações intermediárias
        Stack<object> stack = new Stack<object>();

        // Variável para armazenar temporariamente os valores dentro de colchetes
        List<object> bracketValues = new List<object>();

        for (int i = 0; i < Expression.Length; i++)
        {
            char c = Expression[i];
            
            if (c == '(' || c == '[')
            {
                // Abre um novo escopo
                stack.Push(bracketValues);
                bracketValues = new List<object>();
            }
            else if (c == ')' || c == ']')
            {
                // Fecha o escopo atual e calcula o valor dentro dos parênteses ou colchetes
                object result = EvaluateBracket(bracketValues);
                bracketValues = (List<object>)stack.Pop();

                // Adiciona o valor calculado ao escopo anterior
                bracketValues.Add(result);
            }
            else if (char.IsLetter(c))
            {
                // Continua a construção do valor atual dentro dos parênteses ou colchetes
                bracketValues.Add(c.ToString());
            }
            else if (c == '.')
            {
                // Se for um ponto, verifica se é uma expressão de método
                string method = GetMethod(Expression, ref i);
                if (method == "Count")
                {
                    // Adiciona a operação de contagem de caracteres à lista
                    bracketValues.Add(method);
                }
                else if (method == "Substring")
                {
                    // Adiciona a operação de substring à lista
                    int startIdx = GetArgument(Expression, ref i);
                    int endIdx = GetArgument(Expression, ref i);
                    bracketValues.Add(new Tuple<string, int, int>(method, startIdx, endIdx));
                }
                else
                {
                    throw new ArgumentException($"Método '{method}' não suportado.");
                }
            }
            else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '^')
            {
                // Se for um operador, empilha os operadores de maior precedência e executa os de igual ou menor precedência
                while (stack.Count > 0 && stack.Peek() is char && Operations.ContainsKey(stack.Peek().ToString()) 
                    && Operations[stack.Peek().ToString()].Precedence() >= Operations[c.ToString()].Precedence())
                {
                    ExecuteOperation(stack);
                }

                // Empilha o operador atual
                stack.Push(c);
            }
            else
            {
                // Continua a construção do valor atual dentro dos parênteses ou colchetes
                bracketValues.Add(c);
            }
        }

        // Avalia a expressão fora dos parênteses ou colchetes
        object finalResult = EvaluateBracket(bracketValues);

        // Verifica se ainda há operações pendentes a serem executadas
        while (stack.Count > 0 && stack.Peek() is char)
        {
            ExecuteOperation(stack);
        }

        return finalResult;
    }

    private object EvaluateBracket(List<object> bracketValues)
    {
        Stack<object> stack = new Stack<object>();

        foreach (object obj in bracketValues)
        {
            if (obj is char && Operations.ContainsKey(obj.ToString()))
            {
                // Se for um operador, executa a operação correspondente
                FunctionOperation operation = Operations[obj.ToString()];
                object[] parameters = new object[operation.NParam()];
                for (int i = operation.NParam() - 1; i >= 0; i--)
                {
                    parameters[i] = stack.Pop();
                }
                stack.Push(operation.Process(parameters));
            }
            else if (obj is string)
            {
                // Se for uma string, converte para número ou mantém como string
                double result;
                if (double.TryParse((string)obj, out result))
                {
                    stack.Push(result);
                }
                else
                {
                    stack.Push(obj);
                }
            }
            else if (obj is Tuple<string, int, int>)
            {
                // Se for uma tupla representando uma expressão de substring, executa a operação correspondente
                var tuple = (Tuple<string, int, int>)obj;
                if (tuple.Item1 == "Substring")
                {
                    string str = (string)stack.Pop();
                    int endIdx = tuple.Item3;
                    int startIdx = tuple.Item2;
                    stack.Push(str.Substring(startIdx, endIdx - startIdx));
                }
            }
            else
            {
                // Se for um número, adiciona à pilha
                stack.Push(obj);
            }
        }

        // O resultado final deve estar no topo da pilha
        return stack.Pop();
    }

    private void ExecuteOperation(Stack<object> stack)
    {
        char operation = (char)stack.Pop();
        FunctionOperation funcOp = Operations[operation.ToString()];
        object[] parameters = new object[funcOp.NParam()];
        for (int i = funcOp.NParam() - 1; i >= 0; i--)
        {
            parameters[i] = stack.Pop();
        }
        stack.Push(funcOp.Process(parameters));
    }

    private string GetMethod(string expression, ref int index)
    {
        int startIdx = index - 1;
        while (startIdx >= 0 && char.IsLetter(expression[startIdx]))
        {
            startIdx--;
        }

        int endIdx = index + 1;
        while (endIdx < expression.Length && char.IsLetter(expression[endIdx]))
        {
            endIdx++;
        }

        string method = expression.Substring(startIdx + 1, endIdx - startIdx - 1);
        index = endIdx - 1;
        return method;
    }

    private int GetArgument(string expression, ref int index)
    {
        int startIdx = index + 1;
        while (startIdx < expression.Length && !char.IsDigit(expression[startIdx]))
        {
            startIdx++;
        }

        int endIdx = startIdx + 1;
        while (endIdx < expression.Length && char.IsDigit(expression[endIdx]))
        {
            endIdx++;
        }

        int argument = int.Parse(expression.Substring(startIdx, endIdx - startIdx));
        index = endIdx - 1;
        return argument;
    }
}