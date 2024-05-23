namespace MathInter;

public static class Expressions
{
    public static readonly Dictionary<string, FunctionOperation> functionOperations;
    // public static readonly Dictionary<string, int> precedence;

    static Expressions()
    {
        functionOperations = new ();
        
        Add(new Add());
        Add(new Subtract());
        Add(new Multiply());
        Add(new Divide());
        Add(new Power());

        Add(new Sen());
        Add(new Cos());
        Add(new Log());
        Add(new Dot());

        // Add(new Count());
    }

    public static void Add(FunctionOperation functionOperation)
    {
        functionOperations.Add(functionOperation.GetName(), functionOperation);
    }
}