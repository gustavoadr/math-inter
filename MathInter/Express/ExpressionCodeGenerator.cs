using System;
using System.IO;
using System.Collections.Generic;
namespace Agilis.CodeG;
public class ExpressionCodeGenerator : ExpressionBase
{
    public ExpressionCodeGenerator(ExpressionCodeGenerator context) : this()
    {
        foreach (var kv in context.varD)
            addVar(kv.Key, kv.Value);
    }
    public ExpressionCodeGenerator()
    {
        addFunction("if", (int nParam, Stack<object> stack) => {
            string cElse = (string)LimparStr(stack.Pop());
            string cTrue = (string)LimparStr(stack.Pop());
            bool comparador = (bool)stack.Pop();
            Queue<string> queue = dicFuncoesAnonimas[comparador ? cTrue : cElse];
            return evaluate(queue);
        });
        addFunction("for", (int nParam, Stack<object> stack) => {
            string codigo = (string)LimparStr(stack.Pop());
            double ultimo = Convert.ToDouble(stack.Pop());
            double primeiro = (double)stack.Pop();
            string variavel = (string)LimparStr(stack.Pop());
            Stack<object> ret = new();
            Queue<string> queue = dicFuncoesAnonimas[codigo];
            for (double i = primeiro; i < ultimo-0.0001; ++i)
            {
                addVar(variavel, i);
                Stack<object> temp = evaluate(queue);
                EmpilharPilhaEmOutra(ret, temp);
            }
            return ret;
        });
        addFunction("save", (int nParam, Stack<object> stack) => {
            string codigo = (string)LimparStr(stack.Pop());
            string fileName = (string)LimparStr(stack.Pop());
            string directoryPath = Path.GetDirectoryName(fileName);
            Directory.CreateDirectory(directoryPath);
            Queue<string> queue = dicFuncoesAnonimas[codigo];
            Stack<object> revert = InvertePilhaLimpandoStr(evaluate(queue));
            using StreamWriter sr = new StreamWriter(fileName, false);
            while (revert.Count > 0)
                sr.Write(revert.Pop().ToString().Replace("TRIM_TAB", "\t").Replace("TRIM_NL", "\r\n"));
            return new Stack<object>();
        });
        addFunction("replace", (int nParam, Stack<object> stack) => {
            string value = (string)stack.Pop();
            foreach (var kv in varD)
                value = value.Replace(kv.Key, kv.Value.ToString());
            return value;
        });
        addFunction("keep", (int nParam, Stack<object> stack) => {
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
            Queue<string> queue = dicFuncoesAnonimas[codigo];
            ret.Push($"\"<#keep({keepName})#>\"");
            EmpilharPilhaEmOutra(ret, evaluate(queue));
            ret.Push($"\"<#/#>\"");
            return ret;
        });
        addVar("TRIM_NL", "\r\n");
        addVar("TRIM_TAB", "\t");
    }
}