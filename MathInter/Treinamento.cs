using System;
using System.Collections.Generic;

public class MathExpressionInterpreter
{
    public static double Interpret(string expression)
    {
        try
        {
            return EvaluateExpression(ParseExpression(expression));
        }
        catch (Exception ex)
        {
            // Se ocorrer um erro durante a interpretação, imprima o erro e retorne NaN (Not a Number).
            Console.WriteLine("Erro durante a interpretação: " + ex.Message);
            return double.NaN;
        }
    }

    private static Queue<string> ParseExpression(string expression)
    {
        var outputQueue = new Queue<string>();
        var operatorStack = new Stack<char>();

        foreach (char token in expression)
        {
            if (char.IsWhiteSpace(token))
                continue;

            if (char.IsDigit(token) || token == '.')
            {
                // Se o token for um dígito ou um ponto decimal, adicioná-lo à fila de saída.
                int startIndex = expression.IndexOf(token);
                int endIndex = startIndex;

                while (endIndex < expression.Length && (char.IsDigit(expression[endIndex]) || expression[endIndex] == '.'))
                    endIndex++;

                outputQueue.Enqueue(expression.Substring(startIndex, endIndex - startIndex));
            }
            else if (token == '(')
            {
                // Se o token for um parêntese de abertura, empilhá-lo.
                operatorStack.Push(token);
            }
            else if (token == ')')
            {
                // Se o token for um parêntese de fechamento, desempilhar os operadores até encontrar o parêntese de abertura correspondente.
                while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                    outputQueue.Enqueue(operatorStack.Pop().ToString());

                if (operatorStack.Count == 0)
                    throw new ArgumentException("Expressão inválida: parênteses não correspondentes.");

                operatorStack.Pop(); // Remover o parêntese de abertura da pilha.
            }
            else if (IsOperator(token))
            {
                // Se o token for um operador, desempilhar operadores de maior ou igual precedência até encontrar um operador com menor precedência ou parênteses.
                while (operatorStack.Count > 0 && operatorStack.Peek() != '(' && Precedence(operatorStack.Peek()) >= Precedence(token))
                    outputQueue.Enqueue(operatorStack.Pop().ToString());

                operatorStack.Push(token); // Empilhar o operador atual.
            }
            else
            {
                throw new ArgumentException("Token inválido: " + token);
            }
        }

        // Desempilhar os operadores restantes.
        while (operatorStack.Count > 0)
        {
            if (operatorStack.Peek() == '(')
                throw new ArgumentException("Expressão inválida: parênteses não correspondentes.");

            outputQueue.Enqueue(operatorStack.Pop().ToString());
        }

        return outputQueue;
    }

    private static double EvaluateExpression(Queue<string> expression)
    {
        var operandStack = new Stack<double>();

        while (expression.Count > 0)
        {
            string token = expression.Dequeue();

            if (double.TryParse(token, out double number))
            {
                // Se o token for um número, empilhá-lo.
                operandStack.Push(number);
            }
            else if (IsOperator(token[0]))
            {
                // Se o token for um operador, aplicá-lo aos operandos do topo da pilha.
                if (operandStack.Count < 2)
                    throw new ArgumentException("Expressão inválida: operadores insuficientes.");

                double operand2 = operandStack.Pop();
                double operand1 = operandStack.Pop();
                double result = ApplyOperator(operand1, operand2, token[0]);
                operandStack.Push(result);
            }
            else
            {
                throw new ArgumentException("Token inválido: " + token);
            }
        }

        if (operandStack.Count != 1)
            throw new ArgumentException("Expressão inválida: operandos restantes.");

        return operandStack.Pop();
    }

    private static bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/';
    }

    private static int Precedence(char op)
    {
        switch (op)
        {
            case '+':
            case '-':
                return 1;
            case '*':
            case '/':
                return 2;
            default:
                return 0;
        }
    }

    private static double ApplyOperator(double operand1, double operand2, char op)
    {
        switch (op)
        {
            case '+':
                return operand1 + operand2;
            case '-':
                return operand1 - operand2;
            case '*':
                return operand1 * operand2;
            case '/':
                if (operand2 == 0)
                    throw new DivideByZeroException("Divisão por zero.");
                return operand1 / operand2;
            default:
                throw new ArgumentException("Operador inválido: " + op);
        }
    }

    static void Main(string[] args)
    {
        string expression = "2 + 3 * 5"; // Expressão de exemplo
        double result = Interpret(expression);
		
		//criar uma classe com as funções chamada Expression
        var x = new Expression("pow(sen(1+2), 0.5)");
		
        //adiciona chaves de expressão
		x.Add("sen", Seno);
		x.Add("pow", Pow);
		double y = x.Evaluate();
		
        Console.WriteLine($"Resultado da expressão {expression}: {result}");
    }
	
    //define a execução das chamadas
	public static double Seno(double teta)
	{
		return Math.Sin(teta);	
	}
	
	public static double Pow(double x, double y)
	{
		return Math.Pow(x, y);	
	}
}