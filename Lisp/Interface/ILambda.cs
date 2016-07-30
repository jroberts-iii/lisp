using System.Collections.Generic;

namespace Lisp.Interface
{
    public interface ILambda : IAtom
    {
        IList Body { get; }
        IEnvironment ClosurEnvironment { get; }
        IEnumerable<ISymbol> ParameterSymbols { get; }
    }
}
