using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
namespace GerSegCond.Console.Express;

internal class Teste
{
    private const string 
        fileName = "Express\\Template.txt", 
        outputDir = "C:\\Projetos\\Teste", 
        inputDir = "C:\\Projetos\\GroupSeguros\\GerSegCond\\Fontes\\GerSegCond.Console";
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
        //Math("-1", "-1");
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
        Exec("1;2", "1\r\n2");
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
        // Variavel
        Exec("i=1+2; i; \"i\"", "3\r\ni");
        Exec("i=1; i=i+1; i; \"i\"", "2\r\ni");
        Exec("i=1; i=\"i\"+i; i", "i1");
        // For
        Exec("for(i,0,3,i)", "0\r\n1\r\n2");
        Exec("for(i,0+1-1,1+2*2-2,i)", "0\r\n1\r\n2");
        Exec("a=0;b=3;for(i,a,b,i)", "0\r\n1\r\n2");
        Exec("a=0;b=3;for(i,a,b,((i+1));(i))", "1\r\n0\r\n2\r\n1\r\n3\r\n2");
        // If
        Exec("i=\"a\";if(1==1, i, i+1)", "a");
        Exec("i=\"a\";if(1!=1, i, i+1)", "a1");
        // replace => troca o conteúdo dentro de uma string se existir variável com mesmo nome
        Exec($"a=123;replace(\"casa\")", "c123s123");
        Exec($"a=123;s=\"bbb\";replace(\"casa\")", "c123bbb123");
        Exec($"for(i,0,2,replace(\"Guilherme\"))", "Gu0lherme\r\nGu1lherme");
        // saveFile
        if (Directory.Exists(outputDir))
            Directory.Delete(outputDir, true);
        Exec($"saveFile(\"{outputDir}\\{fileName}\", 1)", "1", fileName);
        Exec($"saveFile(\"{outputDir}\\{fileName}\", \"1\")", "1", fileName);
        Exec($"Template=\"MinhaClasse\";saveFile(\"{outputDir}\\\"+replace(\"{fileName}\"), for(i,0,4,i+1))", "1234", "Express\\MinhaClasse.txt");

        // simples
        GerarTemplate("Express\\Template1.txt", "1");
        // trocando o nome do arquivo
        GerarTemplate("Express\\Template2.txt", "Template=Macarrao", "Express\\Macarrao2.txt");
        // for e if
        GerarTemplate("Express\\Template3.txt", "\tSalles\r\n\tSalles\r\n\tGuilherme\r\n\tSalles\r\n\tSalles\r\n\tSalles\r\n");
        // expressao no meio do arquivo
        GerarTemplate("Express\\Template4.txt");
        // gerando arquivos dentro do loop
        GerarTemplate("Express\\Template5.txt");
        // keep simples e keep variando o nome
        GerarTemplate("Express\\Template6.txt");
        GerarTemplate("Express\\Template7.txt");
        // gerando com modelo
        GerarTemplate("Express\\Template8.txt", null, null, new Viagem());
        GerarTemplate("Express\\Template9.txt", null, null, new Viagem());

        //ajustar operação com valor negativo -1 (tokenize)
        //ajustar reflection (primeira funcao que bate) (tokenize, antes do param coloca o num de argumento) (toString de data com formato, template 9)
        //reflection (testes linha 62, 63) ajustar local
        //diferenciar variavel local de variavel global
        //implementar função (declarar a função e executar a função declarada)
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
    private static Stack<object> ReflexaoR2(Stack<object> stack)
    {
        double y = (double)stack.Pop();
        double x = (double)stack.Pop();
        var refletido = ReflexaoR2(x, y);
        Stack<object> ret = new Stack<object>();
        ret.Push(refletido.x);
        ret.Push(refletido.y);
        return ret;
    }
    private static void Exec(string expression, string resposta = null, string arquivo = null, object MODEL = null)
    {
        ExpressionBase ex = new ExpressionCodeGenerator();
        ex.addFunction("sin", (Stack<object> stack) => System.Math.Sin((double)stack.Pop()));
        ex.addFunction("f", ReflexaoR2);
        ex.addVar("PI", Math.PI);
        if (MODEL != null) 
            ex.addVar("MODEL", MODEL);
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