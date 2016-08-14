using System.Collections.Generic;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Macro : SExpression, IMacro
    {
        public Macro(ISExpression body, IEnumerable<ISymbol> parameterSymbols)
        {
            Body = body;
            ParameterSymbols = parameterSymbols;
        }

        public ISExpression Body { get; }
        public IEnumerable<ISymbol> ParameterSymbols { get; }

        public override ISExpression InvokeNativeMethod(IList list, IEnvironment environment)
        {
            var env = new Environment();

            var parameterList = list.Rest;
            foreach (var parameterSymbol in ParameterSymbols)
            {
                env.AddSymbol(parameterSymbol.Name, parameterList.First);
                parameterList = parameterList.Rest;
            }

            var sExpression = Evaluator.Evaluate(Body, env);
            return Evaluator.Evaluate(sExpression, environment);
        }
    }
}
