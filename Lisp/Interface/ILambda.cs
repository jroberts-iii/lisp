using System.Collections.Generic;

namespace Lisp.Interface
{
    public interface ILambda : IAtom
    {
        ISExpression Body { get; }
        IEnumerable<ISymbol> ParameterSymbols { get; }
    }
}
