using Lisp.Interface;

namespace Lisp.Class
{
    public class Symbol : ISymbol
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
    }
}
