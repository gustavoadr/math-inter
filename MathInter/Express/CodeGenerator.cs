using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
namespace Agilis.CodeG;

internal class CodeGenerator
{
    private string start = "<#", end = "#>", close = "<#/#>";
    private string inputDir, outputDir;
    public CodeGenerator(string inputDir, string outputDir)
    { 
        this.inputDir = inputDir; 
        this.outputDir = outputDir; 
    }
    public void changeTags(string start, string end, string close)
    {
        this.start = start;
        this.end = end;
        this.close = close;
    }
    // A morfologia para parse é
    // 1. string que termina em <#, que é o início de um comando
    // 2. string que inicia em <#, que vai representar um comando e pode:
    //    2.1. ser um comando fechado tipo <#set(i,4)#>
    //    2.2. ser um comando aberto, ou seja, que tem um conteúdo e que termina com <#/#>
    //    2.3. ser um if, que tem um <#else#>
    public string parseFile(string fileName)
    {
        return parse(File.ReadAllText($"{inputDir}\\{fileName}"), fileName);
    }
    public string parse(string conteudo, string fileName)
    {
        List<string> tokens = Tokenize(conteudo.Replace("\r", "").Replace("\n", "").Replace("\t", ""));
        string ret = Tokenize(tokens, outputDir, fileName);
        Debug.WriteLine(ret);
        return ret;
    }
    public Stack<object> generate(string fileName, object MODEL = null)
    {
        string expression = parseFile(fileName);
        ExpressionCodeGenerator ecg = new ExpressionCodeGenerator();
        if (MODEL != null)
            ecg.addVar("MODEL", MODEL);
        ecg.parse(expression);
        Debug.WriteLine(ecg.convertToRPN());
        return ecg.evaluate();
    }
    ////////////////////////////////////////////////////////////////////
    /// Primeiro Tokenize
    private string conteudo;
    private int pos = 0;
    private List<string> Tokenize(string p_conteudo)
    {
        conteudo = p_conteudo;
        List<string> ret = new();
        pos = 0;
        StringBuilder token = new();
        while (pos < conteudo.Length)
        {
            if (!eh(start))
                token.Append(conteudo[pos++]);
            else
            {
                if (token.Length > 0)
                    ret.Add(token.ToString());
                token.Clear().Append(start);
                pos += start.Length;
                while (!eh(end))
                    token.Append(conteudo[pos++]);
                token.Append(end);
                pos += end.Length;
                ret.Add(token.ToString());
                token.Clear();
            }
        }
        if (token.Length > 0)
            ret.Add(token.ToString());
        return ret;
    }
    private bool eh(string val)
    {
        int tamanhoRestante = conteudo.Length - pos;
        if (tamanhoRestante < val.Length)
            return false;
        for (int i = 0; i < val.Length; i++)
            if (conteudo[pos + i] != val[i])
                return false;
        return true;
    }
    ////////////////////////////////////////////////////////////////////
    /// Segundo Tokenize
    private List<string> tokens;
    private string Tokenize(List<string> p_tokens, string outputDir, string fileName)
    {
        tokens = p_tokens;
        StringBuilder ret = new();
        Stack<string> stack = new Stack<string>();
        foreach (var token in tokens)
        {
            if (ehString(token)) // todo: parse em "
                ret.Append($"\"{token.Replace("\"", "\"\"")}\";");
            else if (ehFor(token))
            {
                int i = token.LastIndexOf(')');
                ret.Append($"{token.Substring(2, i - 2)}, {{");
                stack.Push("for");
            }
            else if (ehKeep(token))
            {
                int i = token.LastIndexOf(')');
                ret.Append($"{token.Substring(2, i - 2)}, \"{outputDir}\\\"+replace(\"{fileName}\"), {{");
                stack.Push("keep");
            }
            else if (ehSave(token))
            {
                ret.Append($"save(\"{outputDir}\\\"+replace(\"{fileName}\"), {{");
                stack.Push("save");
            }
            else if (ehIf(token))
            {
                int i = token.LastIndexOf(')');
                ret.Append($"{token.Substring(2, i - 2)}, {{");
                stack.Push("if");
            }
            else if (ehElse(token))
            {
                ret.Append("}, {");
                stack.Push("else");
            }
            else if (ehPrint(token))
            {
                string v = token.Substring(3, token.Length-5);
                ret.Append($"{v};");
            }
            else if (ehSet(token)) // tem que ser o último a ser avaliado
            {
                string v = token.Substring(2, token.Length - 4);
                int i = v.IndexOf('=');
                string v1 = v.Substring(0, i);
                string v2 = v.Substring(i+1);
                ret.Append($"{v1}={v2};");
            }
            else if (ehFunction(token))
            {
                int i = token.IndexOf("function ")+9;
                int ii = token.IndexOf('(');
                int iii = token.LastIndexOf(')');
                string fName = token.Substring(i, ii-i);
                string[] parametros = token.Substring(ii+1, iii-ii-1).Split(',');
                ret.Append("function(").Append(fName);
                foreach(string s in parametros) 
                    ret.Append($", \"{s.Trim()}\""); 
                ret.Append(", {");
                stack.Push("function");
            }
            else if (ehFechamento(token))
            {
                string comando = stack.Pop();
                if (comando == "for" || comando == "save" || comando == "keep" || comando == "function")
                {
                    ret.Append("});");
                }
                else if (comando == "if")
                {
                    ret.Append("}, {});");
                }
                else if (comando == "else")
                {
                    stack.Pop();
                    ret.Append("});");
                }
            }
        }
        return ret.ToString();
    }
    private bool ehString(string val)
    {
        return !val.StartsWith(start);
    }
    private static bool ehFor(string val)
    {
        return val.Contains("for");
    }
    private static bool ehFunction(string val)
    {
        return val.Contains("function");
    }
    private static bool ehKeep(string val)
    {
        return val.Contains("keep");
    }
    private static bool ehIf(string val)
    {
        return val.Contains("if");
    }
    private static bool ehElse(string val)
    {
        return val.Contains("else");
    }
    private static bool ehSave(string val)
    {
        return val.Contains("save");
    }
    private bool ehPrint(string val)
    {
        return val.StartsWith($"{start}=");
    }
    private static bool ehSet(string val)
    {
        return val.Contains("=");
    }
    private bool ehFechamento(string val) 
    {
        return val == close;
    }
}
