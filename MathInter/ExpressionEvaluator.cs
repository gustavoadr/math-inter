public class ExpressionEvaluator
{
    // Delegate for unary operations
    public delegate double UnaryOperation(double operand);

    // Delegate for binary operations
    public delegate double BinaryOperation(double operand1, double operand2);

    // Dictionary to map operators to their corresponding binary operations
    private readonly Dictionary<string, BinaryOperation> binaryOperations = new Dictionary<string, BinaryOperation>
    {
        {"+", (x, y) => x + y},
        {"-", (x, y) => x - y},
        {"*", (x, y) => x * y},
        {"/", (x, y) => x / y},
        {"^", Math.Pow}
    };

    // Dictionary to map functions to their corresponding unary operations
    private readonly Dictionary<string, UnaryOperation> unaryOperations = new Dictionary<string, UnaryOperation>
    {
        {"sin", Math.Sin},
        {"cos", Math.Cos}
    };

    // Method to evaluate a mathematical expression
    public double Evaluate(string expression)
    {
        // Parse the expression into tokens
        string[] tokens = expression.Split(' ');

        // Stack to hold operands and operators
        Stack<double> operands = new Stack<double>();
        Stack<string> operators = new Stack<string>();

        foreach (string token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                // If token is a number, push it onto the operand stack
                operands.Push(number);
            }
            else if (token == "(")
            {
                // If token is '(', push it onto the operator stack
                operators.Push("(");
            }
            else if (token == ")")
            {
                // If token is ')', evaluate the expression inside parentheses
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    EvaluateOperation(operands, operators);
                }

                // Remove '(' from the operator stack
                if (operators.Count > 0 && operators.Peek() == "(")
                {
                    operators.Pop();
                }
                else
                {
                    throw new ArgumentException("Invalid expression: mismatched parentheses");
                }
            }
            else if (binaryOperations.ContainsKey(token))
            {
                // If token is a binary operator, evaluate higher precedence operators and push the current operator onto the stack
                while (operators.Count > 0 && Precedence(token) <= Precedence(operators.Peek()))
                {
                    EvaluateOperation(operands, operators);
                }

                operators.Push(token);
            }
            else if (unaryOperations.ContainsKey(token))
            {
                // If token is a unary function, push it onto the operator stack
                operators.Push(token);
            }
            else
            {
                throw new ArgumentException($"Invalid token: {token}");
            }
        }

        // Evaluate remaining operations
        while (operators.Count > 0)
        {
            EvaluateOperation(operands, operators);
        }

        // Result should be left on the operand stack
        if (operands.Count == 1)
        {
            return operands.Pop();
        }
        else
        {
            throw new ArgumentException("Invalid expression");
        }
    }

    // Method to evaluate an operation and push the result onto the operand stack
    private void EvaluateOperation(Stack<double> operands, Stack<string> operators)
    {
        if (operands.Count < 2 || operators.Count == 0)
        {
            throw new ArgumentException("Invalid expression");
        }

        double operand2 = operands.Pop();
        double operand1 = operands.Pop();

        if (operators.Peek() == "(")
        {
            operators.Pop(); // Remove '(' from the operator stack
            return;
        }

        if (operators.Peek() == ")")
        {
            throw new ArgumentException("Invalid expression");
        }

        string op = operators.Pop();

        if (binaryOperations.ContainsKey(op))
        {
            BinaryOperation operation = binaryOperations[op];
            double result = operation(operand1, operand2);
            operands.Push(result);
        }
        else if (unaryOperations.ContainsKey(op.ToString()))
        {
            UnaryOperation operation = unaryOperations[op.ToString()];
            double result = operation(operand2); // Assuming unary functions like sin and cos operate on the second operand
            operands.Push(result);
        }
        else
        {
            throw new ArgumentException($"Invalid operator or function: {op}");
        }
    }

    // Method to determine the precedence of an operator
    private int Precedence(string op)
    {
        switch (op)
        {
            case "+":
            case "-":
                return 1;
            case "*":
            case "/":
                return 2;
            case "^":
                return 3;
            default:
                return 0;
        }
    }
}