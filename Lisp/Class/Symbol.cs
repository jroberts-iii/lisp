using System.IO;
using Lisp.Exception;
using Lisp.Interface;

namespace Lisp.Class
{
    public class Symbol : SExpression, ISymbol
    {
        public Symbol(string name) : this(name, string.Empty)
        {
        }

        public Symbol(string name, string ns)
        {
            Name = name;
            Namespace = ns;
        }

        public string FullName => string.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}.{Name}";
        public string Name { get; }
        public string Namespace { get; }

        public override ISExpression Evaluate(IEnvironment environment)
        {
            ISExpression sExpression;
            if (environment.TryGetSymbol(FullName, out sExpression))
            {
                return sExpression;
            }

            throw new LispException($"Undefined symbol {FullName}.");
        }

        public override void Write(TextWriter textWriter)
        {
            textWriter.Write(FullName);
        }
    }
}
