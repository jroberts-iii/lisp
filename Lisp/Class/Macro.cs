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

        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            var env = new Environment();

            list = list.Rest;
            foreach (var parameterSymbol in ParameterSymbols)
            {
                env.AddSymbol(parameterSymbol.Name, list.First);
                list = list.Rest;
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
