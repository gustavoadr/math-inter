using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
namespace Agilis.CodeG;

public class Teste
{
    private const string 
        fileName = "CodeG\\Template.txt", 
        outputDir = "C:\\Projetos\\Teste", 
        inputDir = "C:\\Projetos\\agilis-util\\Agilis";
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Testes
    public static void Run()
    {
        // O que pode ser gerado
        //      ORM
        //      Gerador UML tradicional
        //      Arquitetura de rede/infra
        //      Documentação
        //      Orçamento
        //      Um relatório de um sistema qualquer
        //      Rede Neural
        //      Tela
        //      Mapeamento para Barramento

        // Operações básicas
        Exec("1", "1");
        Exec("-1", "-1");
        Exec("-.13", "-0,13");
        Exec(".12", "0,12");
        Exec("1+2", "3");
        Exec("1+2", "3");
        Exec("7*(1+2)", "21");
        Exec("(1+(1+1))*7", "21");
        // Funções e variáveis
        Exec("PI", $"{Math.PI}");
        Exec("sin(0)", "0");
        Exec("sin(PI/2)", "1");
        // Funções com mais de um retorno
        Exec("f(5, 3)", "-5\r\n3");
        Exec("f((1+1)*2, (1+(2))*(1+3))", "-4\r\n12");
        // Mais de uma operação em uma expressão
        Exec("1;-.2", "1\r\n-0,2");
        // Booleano e outros operadores
        Exec("1==2", "False");
        Exec("1>2", "False");
        Exec("1<2", "True");
        Exec("1!=2", "True");
        Exec("1>=2", "False");
        Exec("1<=2", "True");
        Exec("(1+2)==(5-2);", "True");
        Exec("(1+2)==(5-2); (2==2)&&(1==2); (1==2)||(2==2);", "True\r\nFalse\r\nTrue");
        // String
        Exec("\"oi\"", "oi");
        // Reflaction
        Exec("\"oi\".Length", "2");
        Exec("\"oi\"[0]", "o");
        Exec("\"oii\".Substring(1)", "ii");
        Exec("MODEL.titulo", null, null, new Viagem());
        Exec("MODEL.passeios", null, null, new Viagem());
        Exec("MODEL.passeios.Count", "10", null, new Viagem());
        Exec("MODEL.passeios().Count()", "10", null, new Viagem());
        Exec("MODEL.passeios[2].titulo[0]", "T", null, new Viagem());
        // Variavel
        Exec("i=1+2; i; \"i\"", "3\r\ni");
        Exec("i=1; i=i+1; i; \"i\"", "2\r\ni");
        Exec("i=1; i=\"i\"+i; i", "i1");
        // For
        Exec("for(i,0,3,{i})", "0\r\n1\r\n2");
        Exec("for(i,0+1-1,1+2*2-2,{i})", "0\r\n1\r\n2");
        Exec("a=0;b=3;for(i,a,b,{i})", "0\r\n1\r\n2");
        Exec("a=0;b=3;for(i,a,b,{((i+1));(i)})", "1\r\n0\r\n2\r\n1\r\n3\r\n2");
        // If
        Exec("i=\"a\";if(1==1, {i}, {i+1})", "a");
        Exec("i=\"a\";if(1!=1, {i}, {i+1})", "a1");
        // replace => troca o conteúdo dentro de uma string se existir variável com mesmo nome
        Exec("a=123;replace(\"casa\")", "c123s123");
        Exec("a=123;s=\"bbb\";replace(\"casa\")", "c123bbb123");
        Exec("for(i,0,2,{replace(\"Guilherme\")})", "Gu0lherme\r\nGu1lherme");
        // save
        if (Directory.Exists(outputDir))
            Directory.Delete(outputDir, true);
        Exec($"save(\"{outputDir}\\{fileName}\", {{{"1"}}})", "1", fileName);
        Exec($"Template=\"MinhaClasse\";save(\"{outputDir}\\\"+replace(\"{fileName}\"), {{for(i,0,4,{{i+1}})}})", "1234", "CodeG\\MinhaClasse.txt");
        // function
        Exec($"function(soma, \"a\", \"b\", {{a+b}}); soma(2,3);", "5");
        Exec($"function(primeiroEsoma, \"a\", \"b\", {{a;a+b}}); primeiroEsoma(2,3);", "2\r\n5");
        Exec("function(soma, \"a\", \"b\", {a+b;});soma(1, soma(1, 2));", "4");

        // simples
        GerarTemplate("CodeG\\Template01.txt", "1");
        // trocando o nome do arquivo
        GerarTemplate("CodeG\\Template02.txt", "Template=Macarrao", "CodeG\\Macarrao02.txt");
        // for e if
        GerarTemplate("CodeG\\Template03.txt", "\tSalles\r\n\tSalles\r\n\tGuilherme\r\n\tSalles\r\n\tSalles\r\n\tSalles\r\n");
        // expressao no meio do arquivo
        GerarTemplate("CodeG\\Template04.txt", "\tSalles abcdefghijklmnopqrztuvxz[0]=a\r\n\tSalles abcdefghijklmnopqrztuvxz[1]=b\r\n\tGuilherme abcdefghijklmnopqrztuvxz[2]=c\r\n\tSalles abcdefghijklmnopqrztuvxz[3]=d\r\n\tSalles abcdefghijklmnopqrztuvxz[4]=e\r\n\tSalles abcdefghijklmnopqrztuvxz[5]=f\r\n");
        // gerando arquivos dentro do loop
        GerarTemplate("CodeG\\Template05.txt", "\t2: Guilherme\r\n", "CodeG\\Template05-2.txt");
        // keep simples e keep variando o nome
        GerarTemplate("CodeG\\Template06.txt");
        GerarTemplate("CodeG\\Template07.txt");
        // gerando com modelo
        GerarTemplate("CodeG\\Template08.txt", null, null, new Viagem());
        GerarTemplate("CodeG\\Template09.txt", null, null, new Viagem());
        // gerando com função
        GerarTemplate("CodeG\\Template10.txt", "3");
        GerarTemplate("CodeG\\Template11.txt", "c");
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// Simulação de um modelo
    /// 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private static Random r = new Random();
    private static int g_Index = 0;
    public class Passeios
    {
        public int duracao = r.Next(0, 100);
        public string titulo = $"Tour - {++g_Index}";
        public DateTime saida = DateTime.Now.AddDays(r.Next(0, 100)).AddMonths(r.Next(0, 100)).AddHours(r.Next(0, 100));
    }
    public class Viagem
    {
        public List<Passeios> passeios = new List<Passeios>();
        public string titulo = $"Meus passeios preferidos - {++g_Index}";
        public Viagem()
        {
            for(int i = 0; i < 10; i++)
                passeios.Add(new Passeios());
            var r = passeios.Count;
        }
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// Metodos para chamadas de execução e verificação
    /// 
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    private static (double x, double y) ReflexaoR2(double x, double y)
    {
        return (-x, y);
    }
    private static Stack<object> ReflexaoR2(int nParam, Stack<object> stack)
    {
        double y = (double)stack.Pop();
        double x = (double)stack.Pop();
        var refletido = ReflexaoR2(x, y);
        Stack<object> ret = new Stack<object>();
        ret.Push(refletido.x);
        ret.Push(refletido.y);
        return ret;
    }
    private static void Exec(string expression, string resposta = null, string arquivo = null, object obj = null)
    {
        ExpressionBase ex = new ExpressionCodeGenerator();
        ex.addFunction("sin", (int nParam, Stack<object> stack) => System.Math.Sin((double)stack.Pop()));
        ex.addFunction("f", ReflexaoR2);
        ex.addVar("PI", Math.PI);
        if (obj != null) 
            ex.addVar("MODEL", obj);
        Debug.WriteLine(expression);
        ex.parse(expression);
        Debug.WriteLine(ex.convertToRPN());
        string v = ex.evaluateS("\r\n");
        Debug.WriteLine(v);
        if (arquivo != null)
        {
            v = File.ReadAllText($"{outputDir}\\{arquivo}");
            Debug.WriteLine(v);
        }
        if (resposta!=null && v != resposta)
            throw new Exception("Erro");
    }
    private static void GerarTemplate(string fn, string resultadoEsperado = null, string fnGerado = null, object MODEL = null)
    {
        if (fnGerado == null)
            fnGerado = fn;
        CodeGenerator t = new CodeGenerator(inputDir, outputDir);
        var v = t.generate(fn, MODEL);
        if (fnGerado != null && resultadoEsperado != null)
        {
            string vv = File.ReadAllText($"{outputDir}\\{fnGerado}");
            if (vv != resultadoEsperado)
                throw new Exception("Erro");
        }
    }
}