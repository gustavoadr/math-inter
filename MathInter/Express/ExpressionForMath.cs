using System;
using System.Collections.Generic;
namespace GerSegCond.Console.Express;

public class ExpressionForMath : ExpressionBase
{
    public ExpressionForMath()
    {
        addFunction("log", (Stack<object> stack) => {
            double baseLog = (double)stack.Pop();
            double value = (double)stack.Pop();
            return Math.Log(value, baseLog);
        });
        addFunction("sin", (Stack<object> stack) => Math.Sin((double)stack.Pop()));
        addFunction("cos", (Stack<object> stack) => Math.Cos((double)stack.Pop()));
        addFunction("tan", (Stack<object> stack) => Math.Tan((double)stack.Pop()));
        addVar("PI", Math.PI);
    }
}