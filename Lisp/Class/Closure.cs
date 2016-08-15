using System.Collections.Generic;
using System.IO;
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

        public ISExpression Evaluate(IEnvironment environment, IList parameters)
        {
            var env = new Environment(ClosureEnvironment);

            foreach (var parameterSymbol in ParameterSymbols)
            {
                env.AddSymbol(parameterSymbol.Name, Evaluate(environment, parameters.First));
                parameters = parameters.Rest;
            }

            return Evaluate(env, Body);
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("<lambda>");
        }
    }
}
