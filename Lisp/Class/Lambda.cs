using System.Collections.Generic;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Lambda : ILambda
    {
        public Lambda(IList body, IEnvironment closurEnvironment, IEnumerable<ISymbol> parameterSymbols)
        {
            Body = body;
            ClosurEnvironment = closurEnvironment;
            ParameterSymbols = parameterSymbols;
        }

        public IList Body { get; }
        public IEnvironment ClosurEnvironment { get; }
        public IEnumerable<ISymbol> ParameterSymbols { get; }
    }
}
