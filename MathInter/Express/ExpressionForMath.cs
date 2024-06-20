using System;
using System.Collections.Generic;
namespace Agilis.CodeG;

public class ExpressionForMath : ExpressionBase
{
    public ExpressionForMath()
    {
        addFunction("log", (int nParam, Stack<object> stack) => {
            double baseLog = (double)stack.Pop();
            double value = (double)stack.Pop();
            return Math.Log(value, baseLog);
        });
        addFunction("sin", (int nParam, Stack<object> stack) => Math.Sin((double)stack.Pop()));
        addFunction("cos", (int nParam, Stack<object> stack) => Math.Cos((double)stack.Pop()));
        addFunction("tan", (int nParam, Stack<object> stack) => Math.Tan((double)stack.Pop()));
        addVar("PI", Math.PI);
    }
}