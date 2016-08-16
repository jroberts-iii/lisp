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

        public ISExpression Evaluate(IEnvironment environment, IList list)
        {
            var env = new Environment(ClosureEnvironment);

            list = list.Rest;
            foreach (var parameterSymbol in ParameterSymbols)
            {
                env.AddSymbol(parameterSymbol.Name, Evaluate(environment, list.First));
                list = list.Rest;
            }

            return Evaluate(env, Body);
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write("<closure>");
        }
    }
}
