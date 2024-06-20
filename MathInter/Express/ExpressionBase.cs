using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
namespace Agilis.CodeG;

public class ExpressionBase
{
    private Queue<string> output = new();
    private HashSet<string> functions = new();
    private Dictionary<string, FFFF> functionsD = new();
    protected Dictionary<string, object> varD = new();
    protected Dictionary<string, Queue<string>> dicFuncoesAnonimas = new();
    protected Dictionary<string, (List<string> parametros, string funcaoAnonima)> dicFuncoesScript = new();
    public ExpressionBase()
    {
        addFunction("[]", (int nParam, Stack<object> stack) => {
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
        addFunction("function", (int nParam, Stack<object> stack) => {
            string codigo = (string)LimparStr(stack.Pop());
            List<string> parametros = new List<string>();
            for (int i = 0; i < nParam - 2; i++)
                parametros.Add((string)LimparStr(stack.Pop()));
            string functionName = (string)LimparStr(stack.Pop());
            dicFuncoesScript.Add(functionName, (parametros, codigo));
            functions.Add(functionName);
            return new Stack<object>();
        });
    }
    public void parse(string expression)
    {
        List<string> tokens = ajustaStrVariaveis(tokenize(expression));
        output = new Queue<string>(parseTokens(tokens));
    }
    private Queue<string> parseTokens(List<string> tokens)
    {
        Queue<string> queue = new Queue<string>();
        Stack<string> operators = new Stack<string>();
        for (int i = 0; i < tokens.Count; ++i)
        {
            string token = tokens[i];
            if (double.TryParse(token, out _))
                queue.Enqueue(token);
            else if (token.Length > 1 && token.First() == '"' && token.Last() == '"')
                queue.Enqueue(token);
            else if (varD.ContainsKey(token))
                queue.Enqueue(token);
            else if (!isSpecial(token))
            {
                if (token.First() == '.')
                    operators.Push(token);
                else
                    queue.Enqueue(token);
            }
            else if (isFunction(token))
                operators.Push(token);
            else if (token == "{")
            {
                List<string> subTokens = new List<string>();
                int contador = 0;
                while (true)
                {
                    token = tokens[++i];
                    if (token == "{")
                        ++contador;
                    else if (token == "}")
                    {
                        if (contador == 0)
                            break;
                        else
                            --contador;
                    }
                    subTokens.Add(token);
                }
                Queue<string> q = parseTokens(subTokens);
                string name = $"dicFuncoesAnonimas{dicFuncoesAnonimas.Count}";
                queue.Enqueue($"\"{name}\"");
                dicFuncoesAnonimas.Add(name, q);
            }
            else if (token == ",")
            {
                while (operators.Peek() != "(" && operators.Peek() != "[")
                    queue.Enqueue(operators.Pop());
            }
            else if (IsOperator(token))
            {
                while (operators.Count > 0 && IsOperator(operators.Peek()) &&
                       GetPrecedence(token) <= GetPrecedence(operators.Peek()))
                    queue.Enqueue(operators.Pop());
                operators.Push(token);
            }
            else if (token == "(" || token == "[")
                operators.Push(token);
            else if (token == ")" || token == "]")
            {
                string abertura = token == ")" ? "(" : "[";
                while (operators.Count > 0 && operators.Peek() != abertura)
                    queue.Enqueue(operators.Pop());
                operators.Pop(); // Pop the '('
                if (token == "]")
                    queue.Enqueue("[]");

                if (operators.Count > 0 && isFunction(operators.Peek()))
                    queue.Enqueue(operators.Pop());
            }
            else if (token == ";")
                while (operators.Count > 0)
                    queue.Enqueue(operators.Pop());
            else
                operators.Push(token);
        }

        while (operators.Count > 0)
            queue.Enqueue(operators.Pop());
        return queue;
    }
    protected virtual List<string> ajustaStrVariaveis(List<string> tokens)
    {
        for (int i = 0; i < tokens.Count; i++)
            if (tokens[i] == "=")
                tokens[i - 1] = $"\"{tokens[i - 1]}\"";
        return tokens;
    }
    private List<string> tokenize(string expression)
    {
        List<string> tokens = new List<string>();
        Stack<int> contadorParametros = new Stack<int>();
        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];
            if ((char.IsDigit(c)) ||
                (c == '.' && char.IsDigit(expression[i + 1])) ||
                (c == '-' && (i == 0 || tokens.Last() == "," || tokens.Last() == ";") && (char.IsDigit(expression[i + 1]) || expression[i + 1] == '.'))
               )
            {
                StringBuilder number = new StringBuilder(c.ToString());
                while (i + 1 < expression.Length && (char.IsDigit(c = expression[i + 1]) || c == '.'))
                {
                    number.Append(c);
                    ++i;
                }
                tokens.Add(number.ToString());
            }
            else if (c == ' ')
                continue;
            else if (c == ',' || c == '(' || c == ')' || c == ';' || c == '[' || c == ']' || c == '{' || c == '}')
            {
                if (c == '(' || c == '[')
                {
                    if (tokens.Count > 0 && (isFunction(tokens.Last()) || c == '['))
                        contadorParametros.Push(0);
                    else
                        contadorParametros.Push(-1);
                }
                else if (c == ',')
                    contadorParametros.Push(contadorParametros.Pop() + 1);
                else if (c == ')' || c == ']')
                {
                    int v = contadorParametros.Pop();
                    if (v != -1) // é função
                    {
                        if (tokens.Last() != "(" && tokens.Last() != "]")
                        {
                            tokens.Add(",");
                            ++v;
                        }
                        tokens.Add(v.ToString());
                    }
                }
                AddToken(tokens, c.ToString());
            }
            else if (c == '"')
            {
                StringBuilder str = new StringBuilder(c.ToString());
                while (i + 1 < expression.Length)
                {
                    char cc = expression[i + 1];
                    if (cc == '"' && i + 2 < expression.Length && expression[i + 2] == '"')
                        ++i;
                    else if (cc == '"')
                        break;
                    str.Append(cc);
                    ++i;
                }
                tokens.Add(str.Append(expression[++i]).ToString());
            }
            else if (IsOperator(c))
            {
                string v = c.ToString();
                while (IsOperator(c = expression[i + 1]))
                {
                    v += c.ToString();
                    ++i;
                }
                AddToken(tokens, v);
            }
            else
            {
                StringBuilder func = new StringBuilder(c.ToString());
                while (i + 1 < expression.Length && ValidoParaNomeDeFuncao(expression[i + 1]))
                    func.Append(expression[++i]);
                AddToken(tokens, func.ToString());
                if (func[0] == '.')
                    addFunction(func.ToString());
            }
        }
        AddToken(tokens);
        return tokens;
    }
    private void AddToken(List<string> tokens, string token = null)
    {
        if (tokens.Count > 1 && tokens.Last() == "(" && tokens[tokens.Count - 2] == "function")
        {
            addFunction(token);
            tokens.Add($"\"{token}\"");
            return;
        }
        else if (token != "(" && tokens.Count > 0 && isFunction(tokens.Last()))
        // tem que verificar também se não é digito que foi escrito como .123
        {
            tokens.Add("(");
            tokens.Add("0");
            tokens.Add(")");
        }
        if (token != null)
            tokens.Add(token);
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
    private static HashSet<char> operatorsList = new HashSet<char> { '>', '<', '=', '+', '-', '&', '|', '!', '*', '/', '^', '!' };
    private static bool IsOperator(char token)
    {
        return operatorsList.Contains(token);
    }
    private List<string> specials = new List<string>() { "(", ")", ",", "\"", ";", "[", "]", "{", "}" };
    private bool isSpecial(string token)
    {
        return IsOperator(token) || isFunction(token) || specials.Contains(token);
    }
    private static bool IsOperator(string token)
    {
        for (int i = 0; i < token.Length; ++i)
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
        return evaluate(new Queue<string>(this.output));
    }
    protected Stack<object> evaluate(Queue<string> p_queue)
    {
        Queue<string> queue = new Queue<string>(p_queue);
        Stack<object> stack = new Stack<object>();
        while (queue.Count > 0)
        {
            string token = queue.Dequeue();
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
                if (result != null)
                    stack.Push(result);
            }
            else if (isFunction(token))
            {
                int nParam = (int)(double)stack.Pop();
                if (dicFuncoesScript.ContainsKey(token))
                    EmpilharPilhaEmOutra(stack, funcaoScript(token, stack));
                else if (functionsD.ContainsKey(token))
                    EmpilharPilhaEmOutra(stack, functionsD[token](nParam, stack));
                else
                    EmpilharPilhaEmOutra(stack, S(Reflaction)(token, nParam, stack));
            }
            else
                stack.Push(token);
        }
        return stack;
    }
    public delegate object F(string token, int nParam, Stack<object> stack);
    public delegate object FF(int nParam, Stack<object> stack);
    public delegate Stack<object> FFF(string token, int nParam, Stack<object> stack);
    public delegate Stack<object> FFFF(int nParam, Stack<object> stack);
    private static FFF S(F f)
    {
        return (string token, int nParam, Stack<object> stack) => {
            object x = f(token, nParam, stack);
            Stack<object> ret = new();
            stack.Push(x);
            return ret;
        };
    }
    private static FFFF S(FF f)
    {
        return (int nParam, Stack<object> stack) => {
            object x = f(nParam, stack);
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
    private Stack<object> funcaoScript(string token, Stack<object> stack)
    {
        var f = dicFuncoesScript[token];
        Queue<string> queue = dicFuncoesAnonimas[f.funcaoAnonima];
        List<object> valoresAntigos = new List<object>();
        // setando as variáveis parametro e salvando as antigas para voltar com o valor
        for (int i = 0; i < f.parametros.Count; ++i)
        {
            string k = f.parametros[i];
            if (varD.ContainsKey(k))
                valoresAntigos.Add(varD[k]);
            else
                valoresAntigos.Add(null);
            addVar(k, stack.Pop());
        }
        Stack<object> temp = evaluate(queue);
        // voltando com o valor antigo das variáveis
        for (int i = 0; i < f.parametros.Count; ++i)
        {
            object v = valoresAntigos[i];
            string k = f.parametros[i];
            if (v==null)
                varD.Remove(k);
            else
                varD[k] = v;
        }
        return temp;
    }
    private static object Reflaction(string token, int nParam, Stack<object> stack)
    {
        List<Type> parametros = new List<Type>();
        List<object> valores = new List<object>();
        for (int i=0; i<nParam; ++i)
        {
            object p = InteiroSePossivel(LimparStr(stack.Pop()));
            parametros.Insert(0, p.GetType());
            valores.Insert(0, p);
        }
        object obj = InteiroSePossivel(LimparStr(stack.Pop()));
        Type type = obj.GetType();
        token = token.Substring(1); // tira o ponto
        if (nParam == 0)
        {
            FieldInfo campo = type.GetField(token);
            if (campo != null)
                return EnvolveStr(campo.GetValue(obj));
            PropertyInfo propriedade = type.GetProperty(token);
            if (propriedade != null)
                return EnvolveStr(propriedade.GetValue(obj));
        }

        MethodInfo method = type.GetMethod(token, parametros.ToArray());
        if (method != null)
            return EnvolveStr(method.Invoke(obj, valores.ToArray()));

        throw new Exception($"Function {token} not found.");
    }
    private static object InteiroSePossivel(object v)
    {
        if (v is double)
            return (int)(double)v;
        return v;
    }
}