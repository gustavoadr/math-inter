using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
namespace GerSegCond.Console.Express;

public class ExpressionBase
{
    private Queue<string> output = new();
    private HashSet<string> functions = new();
    private Dictionary<string, FFFF> functionsD = new();
    protected Dictionary<string, object> varD = new();
    public ExpressionBase()
    {
        addFunction("[]", (Stack<object> stack) => {
            int index = (int)(double)stack.Pop();
            object obj = LimparStr(stack.Pop());
            Type type = obj.GetType();
            foreach (PropertyInfo property in type.GetProperties())
                if (property.GetIndexParameters().Length > 0)
                {
                    object value = property.GetValue(obj, new object[] { index });
                    return EnvolveStr(value);
                }
            throw new Exception("Operador [] não implementado.");
        });
    }
    private static Dictionary<string, Queue<string>> cache = new();
    public void parse(string expression)
    {
        if (cache.ContainsKey(expression))
        {
            output = new Queue<string>( cache[expression] );
            return;
        }

        output.Clear();
        Stack<string> operators = new Stack<string>();
        List<string> tokens = ajustaStrVariaveis(Tokenize(expression));
        foreach (var token in tokens)
        {
            if (double.TryParse(token, out _))
                output.Enqueue(token);
            else if (token.Length > 1 && token.First() == '"' && token.Last() == '"')
                output.Enqueue(token);
            else if (varD.ContainsKey(token))
                output.Enqueue(token);
            else if (!isSpecial(token))
            {
                if (token.First() == '.')
                {
                    addFunction(token);
                    operators.Push(token);
                }
                else
                    output.Enqueue(token);
            }
            else if (isFunction(token))
                operators.Push(token);
            else if (token == ",")
            {
                while (operators.Peek() != "(" && operators.Peek() != "[")
                    output.Enqueue(operators.Pop());
            }
            else if (IsOperator(token))
            {
                while (operators.Count > 0 && IsOperator(operators.Peek()) &&
                       GetPrecedence(token) <= GetPrecedence(operators.Peek()))
                    output.Enqueue(operators.Pop());
                operators.Push(token);
            }
            else if (token == "(" || token == "[")
                operators.Push(token);
            else if (token == ")" || token == "]")
            {
                string abertura = token == ")" ? "(" : "[";
                while (operators.Count > 0 && operators.Peek() != abertura)
                    output.Enqueue(operators.Pop());
                operators.Pop(); // Pop the '('
                if (token == "]")
                    output.Enqueue("[]");

                if (operators.Count > 0 && isFunction(operators.Peek()))
                    output.Enqueue(operators.Pop());
            }
            else if (token == ";")
                while (operators.Count > 0)
                    output.Enqueue(operators.Pop());
            else
                operators.Push(token);
        }

        while (operators.Count > 0)
            output.Enqueue(operators.Pop());

        cache.Add(expression, new Queue<string>(output));
    }
    protected virtual List<string> ajustaStrVariaveis(List<string> tokens)
    {
        for (int i = 0; i < tokens.Count; i++)
            if (tokens[i] == "=")
                tokens[i - 1] = $"\"{tokens[i - 1]}\"";
            else if (tokens[i].StartsWith('.') && (i == tokens.Count - 1 || tokens[i + 1] != "("))
            {
                tokens.Insert(i + 1, "(");
                tokens.Insert(i + 2, ")");
                i += 2;
            }
        return tokens;
    }
    private static List<string> Tokenize(string expression)
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

                if (c == ',' || c == '(' || c == ')' || c == ';' || c == '[' || c == ']')
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
                    tokens.Add(func + expression[++i].ToString());
                }
                else if (IsOperator(c))
                {
                    string v = c.ToString();
                    while (IsOperator(c = expression[i + 1]))
                    {
                        v += c.ToString();
                        ++i;
                    }
                    tokens.Add(v);
                }
                else
                {
                    string func = c.ToString();
                    while (i + 1 < expression.Length && ValidoParaNomeDeFuncao(expression[i + 1]))
                        func += expression[++i];
                    tokens.Add(func);
                }
            }
        }

        if (number != "")
            tokens.Add(number);

        return tokens;
    }
    private static bool ValidoParaNomeDeFuncao(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }
    private static Dictionary<string, int> operators = new Dictionary<string, int> {
        { "=", 1 },
        { ">", 2 }, { "<", 2 }, { ">=", 2 }, { "<=", 2 }, { "==", 2 },{ "!=", 2 },
        { "+", 3 }, { "-", 3 }, { "&&", 3 }, { "||", 3 },
        { "*", 4 }, { "/", 4 },
        { "^", 5 }
    };
    private static HashSet<char> operatorsList = new HashSet<char> {'>', '<', '=', '+', '-', '&', '|', '!', '*', '/', '^', '!' };
    private static bool IsOperator(char token)
    {
        return operatorsList.Contains(token);
    }
    private List<string> specials = new List<string>() { "(", ")", ",", "\"", ";", "[", "]" };
    private bool isSpecial(string token)
    {
        return IsOperator(token) || isFunction(token) || specials.Contains(token);
    }
    private static bool IsOperator(string token)
    {
        for(int i=0; i<token.Length; ++i)
            if (!IsOperator(token[i]))
                return false;
        return true;
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
        functionsD[token] = S(f);
    }
    public void addFunction(string token, FFFF f)
    {
        functionsD[token] = f;
    }
    public void addVar(string token, object value)
    {
        varD[token] = value;
    }
    private static int GetPrecedence(string token)
    {
        if (IsOperator(token))
            return operators[token];
        return 10;
    }
    public string convertToRPN()
    {
        return string.Join(" ", output);
    }
    public string evaluateS(string separador = ", ")
    {
        Stack<object> s = InvertePilhaLimpandoStr(evaluate());
        return string.Join(separador, s);
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
                object rightOperand = LimparStr(stack.Pop());
                object leftOperand = LimparStr(stack.Pop());
                object result = null;

                if (token == "=")
                    addVar((string)leftOperand, rightOperand);
                else if (rightOperand is double && leftOperand is double)
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
                        case ">":
                            result = ((double)leftOperand) > ((double)rightOperand);
                            break;
                        case "<":
                            result = ((double)leftOperand) < ((double)rightOperand);
                            break;
                        case "==":
                            result = ((double)leftOperand) == ((double)rightOperand);
                            break;
                        case ">=":
                            result = ((double)leftOperand) >= ((double)rightOperand);
                            break;
                        case "<=":
                            result = ((double)leftOperand) <= ((double)rightOperand);
                            break;
                        case "!=":
                            result = ((double)leftOperand) != ((double)rightOperand);
                            break;
                    }
                else if (rightOperand is bool && leftOperand is bool)
                    switch (token)
                    {
                        case "&&":
                            result = (bool)leftOperand && (bool)rightOperand;
                            break;
                        case "||":
                            result = (bool)leftOperand || (bool)rightOperand;
                            break;
                        case "==":
                            result = (bool)leftOperand == (bool)rightOperand;
                            break;
                        case "!=":
                            result = (bool)leftOperand != (bool)rightOperand;
                            break;
                    }
                else
                {
                    switch (token)
                    {
                        case "+":
                            result = leftOperand.ToString() + rightOperand.ToString();
                            break;
                        case "-":
                            result = leftOperand.ToString().Replace(rightOperand.ToString(), "");
                            break;
                        case "==":
                            result = leftOperand.Equals(rightOperand);
                            break;
                        case "!=":
                            result = !leftOperand.Equals(rightOperand);
                            break;
                            // TODO: fazer a multiplicação, colocando a string n vezes na pilha
                            // TODO: fazer a divisão, partindo a string e a colocando n vezes na pilha
                            // TODO: fazer >, < ?
                    }
                    result = EnvolveStr(result);
                }
                if (result!=null)
                    stack.Push(result);
            }
            else if (isFunction(token))
            {
                if (functionsD.ContainsKey(token))
                    EmpilharPilhaEmOutra(stack, functionsD[token](stack));
                else
                    EmpilharPilhaEmOutra(stack, S(Reflaction)(token, stack));
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
    protected static void EmpilharPilhaEmOutra(Stack<object> x1, Stack<object> x2)
    {
        Stack<object> temp = InvertePilha(x2);
        while (temp.Count > 0)
            x1.Push(temp.Pop());
    }
    protected static Stack<object> InvertePilha(Stack<object> x2)
    {
        Stack<object> temp = new Stack<object>();
        while (x2.Count > 0)
            temp.Push(x2.Pop());
        return temp;
    }
    protected static Stack<object> InvertePilhaLimpandoStr(Stack<object> x2)
    {
        Stack<object> temp = new Stack<object>();
        while (x2.Count > 0)
            temp.Push(LimparStr(x2.Pop()));
        return temp;
    }
    protected static object LimparStr(object x) 
    {
        if (x is string)
        {
            string s = (string)x;
            if (s.First() == '"' && s.Last() == '"')
                return s.Substring(1, s.Length - 2);
        }
        return x;
    }
    protected static object EnvolveStr(object x)
    {
        if (x is string)
            return $"\"{x}\"";
        return x;
    }

    private static object Reflaction(string token, Stack<object> stack)
    {
        token = token.Substring(1); // tira o ponto
        object obj = InteiroSePossivel(LimparStr(stack.Pop()));
        Type type = obj.GetType();
        FieldInfo campo = type.GetField(token);
        if (campo != null) 
            return EnvolveStr(campo.GetValue(obj));
        PropertyInfo propriedade = type.GetProperty(token);
        if (propriedade  != null)
            return EnvolveStr(propriedade.GetValue(obj));
        List<Type> parametros = new List<Type>();
        List<object> valores = new List<object>();
        while (true)
        {
            MethodInfo method = type.GetMethod(token, parametros.ToArray());
            if (method != null)
                return EnvolveStr(method.Invoke(obj, valores.ToArray()));
            parametros.Insert(0, obj.GetType());
            valores.Insert(0, obj);
            if (stack.Count == 0)
                break;
            obj = InteiroSePossivel(LimparStr(stack.Pop()));
            type = obj.GetType();
        }
        throw new Exception($"Function {token} not found.");
    }
    private static object InteiroSePossivel(object v)
    {
        if (v is double)
            return (int)(double)v;
        return v;
    }
}