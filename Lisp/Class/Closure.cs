using System.Collections.Generic;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Closure : SExpression, IClosure
    {
        public Closure(ISExpression body, IEnvironment closureEnvironment, IEnumerable<ISymbol> parameterSymbols)
        {
            Body = body;
            ClosureEnvironment = closureEnvironment;
            ParameterSymbols = parameterSymbols;
        }

        public ISExpression Body { get; }
        public IEnvironment ClosureEnvironment { get; }
        public IEnumerable<ISymbol> ParameterSymbols { get; }

        public override ISExpression InvokeNativeMethod(IList list, IEnvironment environment)
        {
            var env = new Environment(ClosureEnvironment);

            list = list.Rest;
            foreach (var parameterSymbol in ParameterSymbols)
            {
                env.AddSymbol(parameterSymbol.Name, Evaluator.Evaluate(list.First, environment));
                list = list.Rest;
            }

            return Evaluator.Evaluate(Body, env);
        }
    }
}
