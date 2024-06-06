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
            
            try
            {
                return Convert.ToDouble(parametros[0]) + Convert.ToDouble(parametros[1]);
            }
            catch
            {
                var par1 = parametros[0].ToString().Replace("\"", "");
                var par2 = parametros[1].ToString().Replace("\"", "");
                return '"' + par1 + par2 + '"';
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

            double number = (double)parametros[0];
            double baseValue = (double)parametros[1];

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
        public object Process(object[] parameters)
        {
            if (parameters.Length != 2)
                throw new ArgumentException("A operação com pontos requer ao menos dois parametros.");
            
            Type type = null;
            if(parameters[0].ToString()[0] == '"')
            {
                parameters[0] = parameters[0].ToString().Replace("\"", "");
                type = typeof(string);
            }

            string methodName;
            object[] methodArgs = null;

            if(parameters[1].ToString().Contains('('))
            {
                var split = parameters[1].ToString().Split('(');
                methodName = split[0];
                
                split[1] = split[1].Substring(0, split[1].Length - 1);

                var args = split[1].Split(',');
                methodArgs = new object[args.Length];
                int i = 0;

                foreach(var arg in args)
                {
                    var eval = new Evaluator(arg);
                    object aux;

                    if(eval.Any())
                        aux = eval.Evaluate();
                    else
                        aux = arg;
                    
                    methodArgs[i++] = Int32.TryParse(arg, out var result) ? result : arg;
                }
            }
            else
                methodName = parameters[1].ToString();

            if(methodArgs == null)
            {
                var property = type.GetProperty(methodName);
                if(property != null)
                    return property.GetValue(parameters[0]);
            }
            else
            {
                Type[] argTypes = new Type[methodArgs.Length];
                int i = 0;
                foreach(var arg in methodArgs)
                    argTypes[i++] = arg.GetType();

                var method = type.GetMethod(methodName, argTypes);
                
                if(method != null)
                    return method.Invoke(parameters[0], methodArgs);
            }

            throw new ArgumentException("O objeto, propriedade ou método não foi identificado.");
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