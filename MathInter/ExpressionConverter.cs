using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
namespace GerSegCond.Console.Express;

public class ExpressionConverter
{
    private Queue<string> output = new();
    private HashSet<string> functions = new();
    private Dictionary<string, FFFF> functionsD = new();
    private Dictionary<string, object> varD = new();
    public ExpressionConverter()
    {
        addFunction("log", (Stack<object> stack) => {
            double baseLog = (double)stack.Pop();
            double value = (double)stack.Pop();
            return Math.Log(value, baseLog);
        });
        addFunction("sin", (Stack<object> stack) => Math.Sin((double)stack.Pop()));
        addFunction("cos", (Stack<object> stack) => Math.Cos((double)stack.Pop()));
        addFunction("tan", (Stack<object> stack) => Math.Tan((double)stack.Pop()));
        addFunction("if", (Stack<object> stack) => {
            object fValue = stack.Pop();
            object tValue = stack.Pop();
            double comparador = (double)stack.Pop();
            return comparador == 0 ? fValue : tValue;
        });
        addFunction("for", (Stack<object> stack) => {
            string codigo = (string)stack.Pop();
            double ultimo = (double)stack.Pop();
            double primeiro = (double)stack.Pop();
            string variavel = (string)stack.Pop();
            Stack<object> ret = new();
            ExpressionConverter ex = new();
            ex.addVar(variavel, 0);
            ex.parse(codigo);
            for (double i = primeiro; i < ultimo-0.0001; ++i)
            {
                ex.addVar(variavel, i);
                Stack<object> temp = ex.evaluate();
                EmpilharPilhaEmOutra(ret, temp);
            }
            return ret;
        });
        addFunction("saveFile", (Stack<object> stack) => {
            Stack<object> revert = new Stack<object>();
            while(stack.Count>1)
                revert.Push(stack.Pop());
            string fileName = (string)stack.Pop();
            using StreamWriter sr = new StreamWriter(fileName, false);
            while (revert.Count > 0)
                sr.Write(revert.Pop().ToString());
            return new Stack<object>();
        });
    }
    public void parse(string expression)
    {
        output.Clear();
        Stack<string> operators = new Stack<string>();

        string[] tokens = Tokenize(expression);
        foreach (var token in tokens)
        {
            if (double.TryParse(token, out _))
            {
                output.Enqueue(token);
                continue;
            }
            else if (token.Length > 1 && token.First() == '"' && token.Last() == '"')
            {
                output.Enqueue(token.Substring(1, token.Length - 2));
                continue;
            }
            else if (varD.ContainsKey(token))
            {
                output.Enqueue(token);
                continue;
            }
            else if (!isSpecial(token) && token.First() == '.')
                addFunction(token);

            if (isFunction(token))
                operators.Push(token);
            else if (token == ",")
            {
                while (operators.Peek() != "(")
                    output.Enqueue(operators.Pop());
            }
            else if (IsOperator(token))
            {
                while (operators.Count > 0 && IsOperator(operators.Peek()) &&
                       GetPrecedence(token) <= GetPrecedence(operators.Peek()))
                    output.Enqueue(operators.Pop());
                operators.Push(token);
            }
            else if (token == "(")
                operators.Push(token);
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                    output.Enqueue(operators.Pop());
                operators.Pop(); // Pop the '('

                if (operators.Count > 0 && isFunction(operators.Peek()))
                    output.Enqueue(operators.Pop());
            }
            else
                operators.Push(token);
        }

        while (operators.Count > 0)
            output.Enqueue(operators.Pop());
    }

    private static string[] Tokenize(string expression)
    {
        List<string> tokens = new List<string>();
        string number = "";
        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];

            if (char.IsDigit(c) || (c == '.' && !string.IsNullOrEmpty(number) ))
                number += c;
            else
            {
                if (number != "")
                {
                    tokens.Add(number);
                    number = "";
                }

                if (c == ' ')
                    continue;

                if (c == ',' || c == '(' || c == ')')
                    tokens.Add(c.ToString());
                else if (c == '"')
                {
                    string func = c.ToString();
                    while (i + 1 < expression.Length)
                    {
                        char cc = expression[i + 1];
                        if (cc == '"' && i + 2 < expression.Length && expression[i + 2] == '"')
                            ++i;
                        else if (cc == '"')
                            break;
                        func += cc;
                        ++i;
                    }
                    tokens.Add(func+expression[++i].ToString());
                }
                else if (IsOperator(c))
                    tokens.Add(c.ToString());
                else
                {
                    string func = c.ToString();
                    while (i + 1 < expression.Length && char.IsLetter(expression[i + 1]))
                        func += expression[++i];
                    tokens.Add(func);
                }
            }
        }

        if (number != "")
            tokens.Add(number);

        return tokens.ToArray();
    }
    private static Dictionary<char, int> operators = new Dictionary<char, int> {
        { '+', 3 }, { '-', 3 },
        { '*', 4 }, { '/', 4 },
        { '^', 5 }
    };
    private static bool IsOperator(char token)
    {
        return operators.ContainsKey(token);
    }
    private bool isSpecial(string token)
    {
        return IsOperator(token) || isFunction(token) || token == "(" || token == ")" || token == "," || token == "\"";
    }
    private static bool IsOperator(string token)
    {
        return token.Length==1 && IsOperator(token[0]);
    }
    private bool isFunction(string token)
    {
        return functions.Contains(token) || functionsD.ContainsKey(token);
    }
    private void addFunction(string token)
    {
        functions.Add(token);
    }
    public void addFunction(string token, FF f)
    {
        functionsD.Add(token, S(f));
    }
    public void addFunction(string token, FFFF f)
    {
        functionsD.Add(token, f);
    }
    public void addVar(string token, object value)
    {
        varD[token] = value;
    }
    private static int GetPrecedence(string token)
    {
        if (IsOperator(token))
            return operators[token[0]];
        return 10;
    }
    public string convertToRPN()
    {
        return string.Join(", ", output);
    }
    public string evaluateS()
    {
        Stack<object> s = InvertePilha(evaluate());
        StringBuilder sb = new StringBuilder();
        while (s.Count > 0)
            sb.Append($"{s.Pop()}, ");
        if (sb.Length > 1)
            sb.Length -= 2;
        return sb.ToString();
    }
    public Stack<object> evaluate()
    {
        Queue<string> output = new Queue<string>(this.output);
        Stack<object> stack = new Stack<object>();
        while (output.Count > 0)
        {
            string token = output.Dequeue();
            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
                stack.Push(number);
            else if (varD.ContainsKey(token))
                stack.Push(varD[token]);
            else if (IsOperator(token))
            {
                object rightOperand = stack.Pop();
                object leftOperand = stack.Pop();
                object result = 0;

                if (rightOperand is double && leftOperand is double)
                    switch (token)
                    {
                        case "+":
                            result = (double)leftOperand + (double)rightOperand;
                            break;
                        case "-":
                            result = (double)leftOperand - (double)rightOperand;
                            break;
                        case "*":
                            result = (double)leftOperand * (double)rightOperand;
                            break;
                        case "/":
                            result = (double)leftOperand / (double)rightOperand;
                            break;
                        case "^":
                            result = Math.Pow((double)leftOperand, (double)rightOperand);
                            break;
                    }
                else
                    switch (token)
                    {
                        case "+":
                            result = leftOperand.ToString() + rightOperand.ToString();
                            break;
                        case "-":
                            result = leftOperand.ToString().Replace(rightOperand.ToString(), "");
                            break;
                            // TODO: fazer a multiplicação, colocando a string n vezes na pilha
                            // TODO: fazer a divisão, partindo a string e a colocando n vezes na pilha
                    }

                stack.Push(result);
            }
            else if (isFunction(token))
            {
                if (functionsD.ContainsKey(token))
                    EmpilharPilhaEmOutra(stack, functionsD[token](stack));
                else
                    EmpilharPilhaEmOutra(stack, Reflaction(token, stack));
            }
            else
                stack.Push(token);
        }
        return stack;
    }
    public delegate object F(string token, Stack<object> stack);
    public delegate object FF(Stack<object> stack);
    public delegate Stack<object> FFF(string token, Stack<object> stack);
    public delegate Stack<object> FFFF(Stack<object> stack);
    private static FFF S(F f)
    {
        return (string token, Stack<object> stack) => {
            object x = f(token, stack);
            Stack<object> ret = new();
            stack.Push(x);
            return ret;
        };
    }
    private static FFFF S(FF f)
    {
        return stack => {
            object x = f(stack);
            Stack<object> ret = new();
            stack.Push(x);
            return ret;
        };
    }
    private static void EmpilharPilhaEmOutra(Stack<object> x1, Stack<object> x2)
    {
        Stack<object> temp = InvertePilha(x2);
        while (temp.Count > 0)
            x1.Push(temp.Pop());
    }
    private static Stack<object> InvertePilha(Stack<object> x2)
    {
        Stack<object> temp = new Stack<object>();
        while (x2.Count > 0)
            temp.Push(x2.Pop());
        return temp;
    }
    private static Stack<object> Reflaction(string token, Stack<object> stack)
    {
        // Procura no stack uma classe que possua a função, pega o número de argumentos e faz a chamada
        throw new Exception($"Function {token} not found.");
    }
}