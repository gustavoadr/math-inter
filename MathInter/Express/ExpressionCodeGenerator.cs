using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
namespace GerSegCond.Console.Express;

public class ExpressionCodeGenerator : ExpressionBase
{
    public ExpressionCodeGenerator(ExpressionCodeGenerator context) : this()
    {
        foreach (var kv in context.varD)
            addVar(kv.Key, kv.Value);
    }
    public ExpressionCodeGenerator()
    {
        addFunction("if", (Stack<object> stack) => {
            string cElse = (string)LimparStr(stack.Pop());
            string cTrue = (string)LimparStr(stack.Pop());
            bool comparador = (bool)stack.Pop();
            ExpressionCodeGenerator ex = new(this);
            ex.parse(comparador ? cTrue : cElse);
            return ex.evaluate();
        });
        addFunction("for", (Stack<object> stack) => {
            string codigo = (string)LimparStr(stack.Pop());
            double ultimo = Convert.ToDouble(stack.Pop());
            double primeiro = (double)stack.Pop();
            string variavel = (string)LimparStr(stack.Pop());
            Stack<object> ret = new();
            ExpressionCodeGenerator ex = new(this);
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
            string codigo = (string)LimparStr(stack.Pop());
            string fileName = (string)LimparStr(stack.Pop());
            string directoryPath = Path.GetDirectoryName(fileName);
            Directory.CreateDirectory(directoryPath);

            ExpressionCodeGenerator ex = new(this);
            ex.parse(codigo);
            Stack<object> revert = InvertePilhaLimpandoStr(ex.evaluate());
            using StreamWriter sr = new StreamWriter(fileName, false);
            while (revert.Count > 0)
                sr.Write(revert.Pop().ToString().Replace("TRIM_TAB", "\t").Replace("TRIM_NL", "\r\n"));
            return new Stack<object>();
        });
        addFunction("replace", (Stack<object> stack) => {
            string value = (string)stack.Pop();
            foreach (var kv in varD)
                value = value.Replace(kv.Key, kv.Value.ToString());
            return value;
        });
        addFunction("keep", (Stack<object> stack) => {
            string codigo = (string)LimparStr(stack.Pop());
            string fileName = (string)LimparStr(stack.Pop());
            string keepName = (string)LimparStr(stack.Pop());
            Stack<object> ret = new();
            if (File.Exists(fileName))
            {
                string total = File.ReadAllText(fileName);
                int i = total.IndexOf($"<#keep({keepName})#>");
                if (i != -1)
                {
                    int ii = total.IndexOf($"<#/#>", i);
                    string conteudo = total.Substring(i, ii-i+5);
                    ret.Push(conteudo);
                    return ret;
                }
            }
            ExpressionCodeGenerator ex = new(this);
            ex.parse(codigo);
            ret.Push($"\"<#keep({keepName})#>\"");
            EmpilharPilhaEmOutra(ret, ex.evaluate());
            ret.Push($"\"<#/#>\"");
            return ret;
        });
        addVar("TRIM_NL", "\r\n");
        addVar("TRIM_TAB", "\t");
    }
    private static Dictionary<string, HashSet<int>> comandosParaStrVariaveis = new Dictionary<string, HashSet<int>>();
    protected override List<string> ajustaStrVariaveis(List<string> tokens)
    {
        if (comandosParaStrVariaveis.Count == 0)
        {
            comandosParaStrVariaveis["for"] = new HashSet<int>() { 0, 3 };
            comandosParaStrVariaveis["if"] = new HashSet<int>() { 1, 2 };
            comandosParaStrVariaveis["saveFile"] = new HashSet<int>() { 1 };
            comandosParaStrVariaveis["keep"] = new HashSet<int>() { 2 };
        }

        List<string> ajustaStr = new List<string>();
        string comando = null;
        int cParenteses = 0, cParametro = 0;
        StringBuilder parametro = new StringBuilder();
        HashSet<int> parametrosStr = null;
        bool tokenViraString = false;
        foreach (string token in tokens) 
        {
            if (comando == null && comandosParaStrVariaveis.ContainsKey(token))
            {
                comando = token;
                parametro.Clear();
                cParenteses = 0;
                cParametro = 0;
                parametrosStr = comandosParaStrVariaveis[token];
                if (tokenViraString = parametrosStr.Contains(cParametro))
                    parametro.Append("\"");
                ajustaStr.Add(token);
            }
            else if (comando == null) 
            {
                ajustaStr.Add(token);
            }
            else if (token == ",")
            {
                if (cParenteses == 1)
                {
                    if (tokenViraString)
                    {
                        parametro.Append("\"");
                        ajustaStr.Add(parametro.ToString());
                        parametro.Clear();
                    }
                    ajustaStr.Add(token);
                    ++cParametro;
                    if (tokenViraString = parametrosStr.Contains(cParametro))
                        parametro.Append("\"");
                }
                else if (tokenViraString)
                    parametro.Append(token);
                else
                    ajustaStr.Add(token);
            }
            else if (token == "(")
            {
                ++cParenteses;
                if (cParenteses == 1)
                    ajustaStr.Add(token);
                else if (tokenViraString)
                    parametro.Append(token);
                else
                    ajustaStr.Add(token);
            }
            else if (token == ")")
            {
                --cParenteses;
                if (cParenteses > 0)
                {
                    if (tokenViraString)
                        parametro.Append(token);
                    else
                        ajustaStr.Add(token);
                }
                else
                {
                    if (tokenViraString)
                    {
                        parametro.Append("\"");
                        ajustaStr.Add(parametro.ToString());
                    }
                    ajustaStr.Add(token);
                    comando = null;
                }
            }
            else if (tokenViraString)
                parametro.Append(token);
            else 
                ajustaStr.Add(token);
        }
        return base.ajustaStrVariaveis(ajustaStr);
    }

}