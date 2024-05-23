namespace MathInter
{
    public class FileHandler
    {
        private string FilePath { get; set; }

        public FileHandler(string templatePath)
        {
            FilePath = templatePath;
        }

        public void EvaluateFile(string outputPath)
        {
            try
            {
                if (!File.Exists(FilePath))
                    throw new FileNotFoundException($"The file at path {FilePath} does not exist.");

                List<string> fileLines = File.ReadLines(FilePath).ToList();

                FileHandler output = new FileHandler(outputPath);
                
                if(!File.Exists(outputPath))
                    output.CreateFile();

                foreach(var line in fileLines)
                {
                    var result = FindTaggedElement(line);
                    output.AppendToFile(result.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static string FindTaggedElement(string line)
        {
            if(!line.Contains("<%"))
                return line;

            var solvedExpression = string.Empty;
            var expression = string.Empty;
            
            var isTagged = false;
            var directReference = false;

            for(int i=0; i<line.Count();i++)
            {
                if(line[i] == '<' &&  line[i+1] == '%')
                {
                    isTagged = true;
                    ++i;
                    
                    if(line[i+1] == '=')
                    {
                        ++i;
                        directReference = true;
                    }
                }
                else if(line[i] == '%' && line[i+1] == '>')
                {
                    //todo: avaliar expressão direta ou expressão regular C# (<%=AwsAccessKeyId%> ou <%for...%>...<%/>)
                    if(!string.IsNullOrWhiteSpace(expression))
                        solvedExpression += new Evaluator(expression).Evaluate().ToString();
                    
                    directReference = false;
                    expression = string.Empty;
                    isTagged = false;
                    ++i;
                }
                else if(isTagged)
                    expression += line[i];
                else
                    solvedExpression += line[i];
            }
            return solvedExpression;
        }

        private void CreateFile()
        {
            try
            {
                FileStream fs = File.Create(FilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void AppendToFile(string content)
        {
            try
            {
                File.AppendAllText(FilePath, content + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}