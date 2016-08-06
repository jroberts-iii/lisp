using System.Collections.Generic;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Closure : IClosure
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
    }
}
