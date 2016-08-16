using System.Collections.Generic;

namespace Lisp.Interface
{
    public interface IMacro : ILambda
    {
        ISExpression Body { get; }
        IEnumerable<ISymbol> ParameterSymbols { get; }
    }
}
