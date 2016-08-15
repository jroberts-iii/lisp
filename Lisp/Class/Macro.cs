using System.Collections.Generic;
using System.IO;
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

        public ISExpression Evaluate(IEnvironment environment, IList parameters)
        {
            var env = new Environment();

            foreach (var parameterSymbol in ParameterSymbols)
            {
                env.AddSymbol(parameterSymbol.Name, parameters.First);
                parameters = parameters.Rest;
            }

            var sExpression = Evaluate(env, Body);
            return Evaluate(environment, sExpression);
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("<lambda>");
        }
    }
}
