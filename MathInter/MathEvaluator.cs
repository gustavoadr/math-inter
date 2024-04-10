using System.Globalization;
using static MathInter.MathOperations;

namespace MathInter;

public class MathEvaluator
{
    private string Expression {get; set;}
    private readonly Dictionary<char, BinaryOperation> binaryOperations = MathExpressions.binaryOperations;
    private readonly Dictionary<string, UnaryOperation> unaryOperations = MathExpressions.unaryOperations;

    public MathEvaluator(string expression)
    {
        Expression = expression;
    }
    
    // Method to evaluate a mathematical expression
    public double Evaluate()
    {
        // Parse the expression into tokens
        List<string> tokens = Tokenize(Expression);

        // Stack to hold operands and operators
        Stack<double> operands = new Stack<double>();
        Stack<string> operators = new Stack<string>();

        foreach (string token in tokens)
        {
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
            {
                operands.Push(number);
            }
            else if (token == "(")
            {
                operators.Push("(");
            }
            else if (token == ")")
            {
                // If token is ')', evaluate the expression inside parentheses
                while (operators.Count > 0 && operators.Peek() != "(")
                    EvaluateOperation(operands, operators);

                // Remove '(' from the operator stack
                if (operators.Count > 0 && operators.Peek() == "(")
                    operators.Pop();
                else
                    throw new ArgumentException("Invalid expression: mismatched parentheses");
            }
            else if (binaryOperations.ContainsKey(token[0]))
            {
                // If token is a binary operator, evaluate higher precedence operators and push the current operator onto the stack
                while (operators.Count > 0 && MathExpressions.Precedence(token[0]) <= MathExpressions.Precedence(operators.Peek()[0]))
                    EvaluateOperation(operands, operators);

                operators.Push(token);
            }
            else if (unaryOperations.ContainsKey(token))
            {
                operators.Push(token);
            }
            else
                throw new ArgumentException($"Invalid token: {token}");
        }

        while (operators.Count > 0)
            EvaluateOperation(operands, operators);

        if (operands.Count == 1)
            return operands.Pop();
        else
            throw new ArgumentException("Invalid expression");
    }

    private List<string> Tokenize(string expression)
    {
        List<string> tokens = new List<string>();
        string currentToken = "";

        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];

            if ((c == '-' || c == '+') && (i == 0 || expression[i - 1] == '(' || binaryOperations.ContainsKey(expression[i - 1])))
            {
                currentToken += c;
            }
            else if (c == '(' || c == ')' || binaryOperations.ContainsKey(c))
            {
                if (currentToken != "")
                {
                    tokens.Add(currentToken);
                    currentToken = "";
                }
                tokens.Add(c.ToString());
            }
            else if (char.IsWhiteSpace(c))
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

    private void EvaluateOperation(Stack<double> operands, Stack<string> operators)
    {
        if (operands.Count < 1 || operators.Count == 0)
            throw new ArgumentException("Invalid expression");

        string op = operators.Pop();

        if (op == "(")
            return;

        if (op == ")")
            throw new ArgumentException("Invalid expression");

        if (unaryOperations.ContainsKey(op))
        {
            UnaryOperation operation = unaryOperations[op];
            var result = operation(operands.Pop());
            operands.Push(result);
        }
        else if (binaryOperations.ContainsKey(op[0]))
        {
            if (operands.Count < 2)
                throw new ArgumentException("Invalid expression");

            var operand2 = operands.Pop();
            
            // Check if there are unary operations pending and evaluate them first
            while (operators.Count > 0 && unaryOperations.ContainsKey(operators.Peek()))
                EvaluateOperation(operands, operators);
            
            var operand1 = operands.Pop();
            BinaryOperation operation = binaryOperations[op[0]];
            var result = operation(operand1, operand2);
            operands.Push(result);
        }
        else
            throw new ArgumentException($"Invalid operator or function: {op}");
    }
}
