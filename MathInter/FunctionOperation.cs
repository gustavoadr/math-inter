using System.Diagnostics;

namespace MathInter
{
    public interface FunctionOperation
    {
        public object Process(object[] parametros);
        public int NParam();
        public string GetName();
    }

    public class Sen : FunctionOperation
    {
        public object Process(object[] parametros)
        {
            return Math.Sin(double.Parse(parametros[0].ToString()));
        }

        public int NParam()
        {
            return 1;
        }

        public string GetName()
        {
            return "sen";
        }
    }

    public class Cos : FunctionOperation
    {
        public object Process(object[] parametros)
        {
            return Math.Cos(double.Parse(parametros[0].ToString()));
        }

        public int NParam()
        {
            return 1;
        }

        public string GetName()
        {
            return "cos";
        }
    }

    public class Log : FunctionOperation
    {
        public object Process(object[] parametros)
        {
            return Math.Log(double.Parse(parametros[0].ToString()), double.Parse(parametros[1].ToString()));
        }

        public int NParam()
        {
            return 2;
        }

        public string GetName()
        {
            return "log";
        }
    }
}