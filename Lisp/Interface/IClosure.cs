using System.Collections.Generic;

namespace Lisp.Interface
{
    public interface IClosure : ILambda
    {
        ISExpression Body { get; }
        IEnvironment ClosureEnvironment { get; }
        IEnumerable<ISymbol> ParameterSymbols { get; }
    }
}
