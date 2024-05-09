namespace MathInter
{
    public interface FunctionOperation
    {
        public object Process(object[] parametros);
        public int NParam();
        public int Precedence();
        public string GetName();
    }

    public class Add : FunctionOperation
    {
        public object Process(object[] parametros)
        {
            if (parametros.Length != 2)
                throw new ArgumentException("A operação de adição requer exatamente dois parâmetros.");

            if (parametros[0] is string || parametros[1] is string)
            {
                // Se pelo menos um dos parâmetros for uma string, realiza a concatenação de strings
                return parametros[0].ToString() + parametros[1].ToString();
            }
            else if (parametros[0] is double && parametros[1] is double)
            {
                // Se ambos os parâmetros forem números, realiza a adição
                return (double)parametros[0] + (double)parametros[1];
            }
            else
            {
                throw new ArgumentException("Pelo menos um dos parâmetros deve ser do tipo string para realizar a concatenação, ou ambos devem ser do tipo double para realizar a adição.");
            }
        }

        public int NParam()
        {
            return 2;
        }

        public int Precedence()
        {
            return 1;
        }

        public string GetName()
        {
            return "+";
        }
    }

    public class Subtract : FunctionOperation
    {
        public object Process(object[] parametros)
        {
            if (parametros.Length != 2)
                throw new ArgumentException("A operação de subtração requer exatamente dois parâmetros.");

            if (!(parametros[0] is double) || !(parametros[1] is double))
                throw new ArgumentException("Ambos os parâmetros devem ser do tipo double para a operação de subtração.");

            return (double)parametros[0] - (double)parametros[1];
        }

        public int NParam()
        {
            return 2;
        }

        public int Precedence()
        {
            return 1;
        }

        public string GetName()
        {
            return "-";
        }
    }

    public class Multiply : FunctionOperation
    {
        public object Process(object[] parametros)
        {
            if (parametros.Length != 2)
                throw new ArgumentException("A operação de multiplicação requer exatamente dois parâmetros.");

            if (!(parametros[0] is double) || !(parametros[1] is double))
                throw new ArgumentException("Ambos os parâmetros devem ser do tipo double para a operação de multiplicação.");

            return (double)parametros[0] * (double)parametros[1];
        }

        public int NParam()
        {
            return 2;
        }

        public int Precedence()
        {
            return 2;
        }

        public string GetName()
        {
            return "*";
        }
    }

    public class Divide : FunctionOperation
    {
        public object Process(object[] parametros)
        {
            if (parametros.Length != 2)
                throw new ArgumentException("A operação de divisão requer exatamente dois parâmetros.");

            if (!(parametros[0] is double) || !(parametros[1] is double))
                throw new ArgumentException("Ambos os parâmetros devem ser do tipo double para a operação de divisão.");

            if ((double)parametros[1] == 0)
                throw new DivideByZeroException("Divisão por zero não é permitida.");

            return (double)parametros[0] / (double)parametros[1];
        }

        public int NParam()
        {
            return 2;
        }

        public int Precedence()
        {
            return 2;
        }

        public string GetName()
        {
            return "/";
        }
    }

    public class Power : FunctionOperation
    {
        public object Process(object[] parametros)
        {
            if (parametros.Length != 2)
                throw new ArgumentException("A operação de potência requer exatamente dois parâmetros.");

            if (!(parametros[0] is double) || !(parametros[1] is double))
                throw new ArgumentException("Ambos os parâmetros devem ser do tipo double para a operação de potência.");

            return Math.Pow((double)parametros[0], (double)parametros[1]);
        }

        public int NParam()
        {
            return 2;
        }

        public int Precedence()
        {
            return 3;
        }

        public string GetName()
        {
            return "^";
        }
    }

    public class Sen : FunctionOperation
    {
        public object Process(object[] parametros)
        {
            if (parametros.Length != 1)
                throw new ArgumentException("A operação seno requer exatamente um parâmetro.");

            if (!(parametros[0] is double))
                throw new ArgumentException("O parâmetro deve ser do tipo double para a operação seno.");

            return Math.Sin((double)parametros[0]);
        }

        public int NParam()
        {
            return 1;
        }

        public int Precedence()
        {
            return 4;
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
            if (parametros.Length != 1)
                throw new ArgumentException("A operação cosseno requer exatamente um parâmetro.");

            if (!(parametros[0] is double))
                throw new ArgumentException("O parâmetro deve ser do tipo double para a operação cosseno.");

            return Math.Cos((double)parametros[0]);
        }

        public int NParam()
        {
            return 1;
        }

        public int Precedence()
        {
            return 4;
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
            if (parametros.Length != 2)
                throw new ArgumentException("A operação de logaritmo requer exatamente dois parâmetros.");

            if (!(parametros[0] is double) || !(parametros[1] is double))
                throw new ArgumentException("Ambos os parâmetros devem ser do tipo double para a operação de logaritmo.");

            double baseValue = (double)parametros[0];
            double number = (double)parametros[1];

            if (baseValue <= 0 || baseValue == 1 || number <= 0)
                throw new ArgumentException("O logaritmo só é definido para números positivos e a base do logaritmo deve ser diferente de 1.");

            return Math.Log(number, baseValue);
        }

        public int NParam()
        {
            return 2;
        }

        public int Precedence()
        {
            return 4;
        }

        public string GetName()
        {
            return "log";
        }
    }

    public class Dot : FunctionOperation
    {
        public object Process(object[] parametros)
        {
            if (parametros.Length != 2)
                throw new ArgumentException("A operação com pontos requer exatamento dois parametros.");

            if (!(parametros[0] is double))
                throw new ArgumentException("O parâmetro deve ser do tipo double para a operação cosseno.");

            return Math.Cos((double)parametros[0]);
        }

        public int NParam()
        {
            return 2;
        }

        public int Precedence()
        {
            return 5;
        }

        public string GetName()
        {
            return ".";
        }
    }
}