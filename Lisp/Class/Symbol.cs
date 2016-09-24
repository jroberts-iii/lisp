using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Symbol : Identifier, ISymbol
    {
        public Symbol(string name) : base(name, string.Empty)
        {
        }

        public Symbol(string name, string @namespace) : base(name, @namespace)
        {
        }

        public override ISExpression Evaluate(IEnvironment environment)
        {
            ISExpression sExpression;
            if (environment.TryGetSymbol(FullName, out sExpression))
            {
                return sExpression;
            }

            throw new LispException($"Undefined symbol {FullName}.");
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
