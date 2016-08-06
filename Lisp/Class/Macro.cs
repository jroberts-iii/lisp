using System.Collections.Generic;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Macro : IMacro
    {
        public Macro(ISExpression body, IEnumerable<ISymbol> parameterSymbols)
        {
            Body = body;
            ParameterSymbols = parameterSymbols;
        }

        public ISExpression Body { get; }
        public IEnumerable<ISymbol> ParameterSymbols { get; }
    }
}
